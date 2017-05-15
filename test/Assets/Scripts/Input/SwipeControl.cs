using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeControl : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
	[SerializeField]
	private MonoBehaviour[] handlers;

	private const float DeltaDrag = 100.0f;
	private const float StartDragSector = 0.5f;
	private const float FinishDragSector = 1.0f;

	private Vector2 _origin;
	private bool _touched;
	private bool _drag;
	private bool _tap;
	private int _pointerId;

	private float _tapDelay;
	private float _deltaTapTime;

	void Awake()
	{
		_touched = false;
		_drag = false;
		_tap = false;

		_tapDelay = 0.2f;
	}

	void Update()
	{
#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.Q))
		{
			SendTouchMessage("OnLeft");
		}
		else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.E))
		{
			SendTouchMessage("OnRight");
		}

		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
		{
			SendTouchMessage("OnUp");
		}
		else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			SendTouchMessage("OnDown");
		}
#endif

		if (!_touched) return;

		if (_tap || _drag)
		{
			return;
		}

		_deltaTapTime += Time.deltaTime;

		if (_deltaTapTime >= _tapDelay)
		{
			_tap = true;
			SendTouchMessage("OnTap");
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		if (_touched) return;

		_deltaTapTime = 0.0f;
		_tap = false;
		_drag = false;
		_touched = true;
		_pointerId = data.pointerId;
		_origin = data.position;
	}

	public void OnDrag(PointerEventData data)
	{
		if (data.pointerId != _pointerId) return;
		if (_tap || _drag) return;

		Vector2 currentPosition = data.position;
		Vector2 directionRaw = currentPosition - _origin;
		Vector2 direction = directionRaw.normalized;

		var isHorizontalDirection = (StartDragSector <= Mathf.Abs(direction.x) && Mathf.Abs(direction.x) <= FinishDragSector);
		var isVerticalDirection = (StartDragSector <= Mathf.Abs(direction.y) && Mathf.Abs(direction.y) <= FinishDragSector);
		var distance = (directionRaw.sqrMagnitude > DeltaDrag);

		if (!distance) return;

		_drag = true;

		if (isHorizontalDirection)
		{
			SendTouchMessage(direction.x < 0 ? "OnLeft" : "OnRight");
		}
		else if (isVerticalDirection)
		{
			SendTouchMessage(direction.y < 0 ? "OnDown" : "OnUp");
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		if (data.pointerId != _pointerId) return;

		_touched = false;

		if (_tap)
		{
			SendTouchMessage("OnTapEnd");
		}
	}

	private void SendTouchMessage(string message)
	{
		foreach (var handler in handlers)
		{
			handler.SendMessage(message, SendMessageOptions.DontRequireReceiver);
		}
	}
}
