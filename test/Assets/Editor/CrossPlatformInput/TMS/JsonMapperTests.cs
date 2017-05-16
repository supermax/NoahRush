using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;
using TMS.Common.Serialization.Json;
using Osge.Axion.State.JsonData;
using System.Collections.Generic;

[TestFixture]
public class JsonMapperTests {

	[Test]
    public void JsonMapperTests_ListOfIntLists_Test()
	{
        //[[38401,10],[84302,20]]
        var ary = new List<List<int>>();
        ary.Add(new List<int>{38401,10});
        ary.Add(new List<int>{84302,20});
        var json = JsonMapper.Default.ToJson(ary);
        Debug.Log("JSON: " + json);
        Assert.NotNull(json);

        var obj = JsonMapper.Default.ToObject(json);
        Assert.NotNull(obj);

        ary = JsonMapper.Default.ToObject<List<List<int>>>(json);
        Assert.NotNull(ary);       
	}
}
