#region Usings

using System.Diagnostics;

#endregion

namespace Unity.IO.Compression
{
	// Strictly speaking this class is not a HuffmanTree, this class is 
	// a lookup table combined with a HuffmanTree. The idea is to speed up
	// the lookup for short symbols (they should appear more frequently ideally.)
	// However we don't want to create a huge table since it might take longer to 
	// build the table than decoding (Deflate usually generates new tables frequently.)  
	// 
	// Jean-loup Gailly and Mark Adler gave a very good explanation about this.
	// The full text (algorithm.txt) can be found inside 
	// ftp://ftp.uu.net/pub/archiving/zip/zlib/zlib.zip.
	//
	// Following paper explains decoding in details:
	//   Hirschberg and Lelewer, "Efficient decoding of prefix codes,"
	//   Comm. ACM, 33,4, April 1990, pp. 449-459.
	//

	internal class HuffmanTree
	{
		internal const int MaxLiteralTreeElements = 288;
		internal const int MaxDistTreeElements = 32;
		internal const int EndOfBlockCode = 256;
		internal const int NumberOfCodeLengthTreeElements = 19;

		// huffman tree for static block
#if DEBUG
		private uint[] codeArrayDebug;
#endif
		private readonly byte[] codeLengthArray;
		private short[] left;
		private short[] right;
		private short[] table;

		private readonly int tableBits;

		private readonly int tableMask;

		static HuffmanTree()
		{
			// construct the static literal tree and distance tree
			StaticLiteralLengthTree = new HuffmanTree(GetStaticLiteralTreeLength());
			StaticDistanceTree = new HuffmanTree(GetStaticDistanceTreeLength());
		}

		public HuffmanTree(byte[] codeLengths)
		{
			Debug.Assert(codeLengths.Length == MaxLiteralTreeElements
			             || codeLengths.Length == MaxDistTreeElements
			             || codeLengths.Length == NumberOfCodeLengthTreeElements,
				"we only expect three kinds of Length here");
			codeLengthArray = codeLengths;

			if (codeLengthArray.Length == MaxLiteralTreeElements) tableBits = 9;
			else tableBits = 7;
			tableMask = (1 << tableBits) - 1;

			CreateTable();
		}

		public static HuffmanTree StaticLiteralLengthTree { get; private set; }

		public static HuffmanTree StaticDistanceTree { get; private set; }


		// Generate the array contains huffman codes lengths for static huffman tree.
		// The data is in RFC 1951.
		private static byte[] GetStaticLiteralTreeLength()
		{
			var literalTreeLength = new byte[MaxLiteralTreeElements];
			for (var i = 0; i <= 143; i++)
				literalTreeLength[i] = 8;

			for (var i = 144; i <= 255; i++)
				literalTreeLength[i] = 9;

			for (var i = 256; i <= 279; i++)
				literalTreeLength[i] = 7;

			for (var i = 280; i <= 287; i++)
				literalTreeLength[i] = 8;

			return literalTreeLength;
		}

		private static byte[] GetStaticDistanceTreeLength()
		{
			var staticDistanceTreeLength = new byte[MaxDistTreeElements];
			for (var i = 0; i < MaxDistTreeElements; i++) staticDistanceTreeLength[i] = 5;
			return staticDistanceTreeLength;
		}


		// Calculate the huffman code for each character based on the code length for each character.
		// This algorithm is described in standard RFC 1951
		private uint[] CalculateHuffmanCode()
		{
			var bitLengthCount = new uint[17];
			foreach (int codeLength in codeLengthArray) bitLengthCount[codeLength]++;
			bitLengthCount[0] = 0; // clear count for length 0

			var nextCode = new uint[17];
			uint tempCode = 0;
			for (var bits = 1; bits <= 16; bits++)
			{
				tempCode = (tempCode + bitLengthCount[bits - 1]) << 1;
				nextCode[bits] = tempCode;
			}

			var code = new uint[MaxLiteralTreeElements];
			for (var i = 0; i < codeLengthArray.Length; i++)
			{
				int len = codeLengthArray[i];

				if (len > 0)
				{
					code[i] = FastEncoderStatics.BitReverse(nextCode[len], len);
					nextCode[len]++;
				}
			}
			return code;
		}

		private void CreateTable()
		{
			var codeArray = CalculateHuffmanCode();
			table = new short[1 << tableBits];
#if DEBUG
			codeArrayDebug = codeArray;
#endif

			// I need to find proof that left and right array will always be 
			// enough. I think they are.
			left = new short[2 * codeLengthArray.Length];
			right = new short[2 * codeLengthArray.Length];
			var avail = (short) codeLengthArray.Length;

			for (var ch = 0; ch < codeLengthArray.Length; ch++)
			{
				// length of this code
				int len = codeLengthArray[ch];
				if (len > 0)
				{
					// start value (bit reversed)
					var start = (int) codeArray[ch];

					if (len <= tableBits)
					{
						// If a particular symbol is shorter than nine bits, 
						// then that symbol's translation is duplicated
						// in all those entries that start with that symbol's bits.  
						// For example, if the symbol is four bits, then it's duplicated 
						// 32 times in a nine-bit table. If a symbol is nine bits long, 
						// it appears in the table once.
						// 
						// Make sure that in the loop below, code is always
						// less than table_size.
						//
						// On last iteration we store at array index:
						//    initial_start_at + (locs-1)*increment
						//  = initial_start_at + locs*increment - increment
						//  = initial_start_at + (1 << tableBits) - increment
						//  = initial_start_at + table_size - increment
						//
						// Therefore we must ensure:
						//     initial_start_at + table_size - increment < table_size
						// or: initial_start_at < increment
						//
						var increment = 1 << len;
						if (start >= increment) throw new InvalidDataException(SR.GetString(SR.InvalidHuffmanData));

						// Note the bits in the table are reverted.
						var locs = 1 << (tableBits - len);
						for (var j = 0; j < locs; j++)
						{
							table[start] = (short) ch;
							start += increment;
						}
					}
					else
					{
						// For any code which has length longer than num_elements,
						// build a binary tree.

						var overflowBits = len - tableBits; // the nodes we need to respent the data.
						var codeBitMask = 1 << tableBits; // mask to get current bit (the bits can't fit in the table)  

						// the left, right table is used to repesent the
						// the rest bits. When we got the first part (number bits.) and look at
						// tbe table, we will need to follow the tree to find the real character.
						// This is in place to avoid bloating the table if there are
						// a few ones with long code.
						var index = start & ((1 << tableBits) - 1);
						var array = table;

						do
						{
							var value = array[index];

							if (value == 0)
							{
								// set up next pointer if this node is not used before.
								array[index] = (short) -avail; // use next available slot.
								value = (short) -avail;
								avail++;
							}

							if (value > 0) throw new InvalidDataException(SR.GetString(SR.InvalidHuffmanData));

							Debug.Assert(value < 0, "CreateTable: Only negative numbers are used for tree pointers!");

							if ((start & codeBitMask) == 0) array = left;
							else array = right;
							index = -value; // go to next node

							codeBitMask <<= 1;
							overflowBits--;
						} while (overflowBits != 0);

						array[index] = (short) ch;
					}
				}
			}
		}

		//
		// This function will try to get enough bits from input and 
		// try to decode the bits.
		// If there are no enought bits in the input, this function will return -1.
		//
		public int GetNextSymbol(InputBuffer input)
		{
			// Try to load 16 bits into input buffer if possible and get the bitBuffer value.
			// If there aren't 16 bits available we will return all we have in the 
			// input buffer.
			var bitBuffer = input.TryLoad16Bits();
			if (input.AvailableBits == 0) return -1;

			// decode an element 
			int symbol = table[bitBuffer & tableMask];
			if (symbol < 0)
			{
				//  this will be the start of the binary tree
				// navigate the tree
				var mask = (uint) 1 << tableBits;
				do
				{
					symbol = -symbol;
					if ((bitBuffer & mask) == 0)
						symbol = left[symbol];
					else
						symbol = right[symbol];
					mask <<= 1;
				} while (symbol < 0);
			}

			int codeLength = codeLengthArray[symbol];

			// huffman code lengths must be at least 1 bit long
			if (codeLength <= 0)
				throw new InvalidDataException(SR.GetString(SR.InvalidHuffmanData));

			//
			// If this code is longer than the # bits we had in the bit buffer (i.e.
			// we read only part of the code), we can hit the entry in the table or the tree
			// for another symbol. However the length of another symbol will not match the 
			// available bits count.
			if (codeLength > input.AvailableBits)
				return -1;

			input.SkipBits(codeLength);
			return symbol;
		}
	}
}