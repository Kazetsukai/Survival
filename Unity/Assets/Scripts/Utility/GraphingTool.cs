using UnityEngine;
using System.Collections.Generic;

public class GraphingTool : MonoBehaviour {

	public Color Color;
	public float WidthInSeconds = 30;

	public class DatumPacket
	{
		public float Datum { get; set; }
		public float Time { get; set; }
	}
	
	private List<DatumPacket> _data = new List<DatumPacket>();

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		
	}
	
	void OnGUI()
	{
		while (_data.Count > 0 && _data[0].Time + WidthInSeconds < Time.time)
		{
			_data.RemoveAt(0);
		}
		
		if (_data.Count == 0)
			return;
		
		var datum = _data[0];
		var startTime = Time.time;
		var startPos = new Vector2(300, 300);
		var size = 200;
		
		for (int i = 1; i < _data.Count; i++)
		{
			if (_data[i].Time - datum.Time < 0.05f)
				continue;
				
			var v1 = (datum.Time - startTime) / WidthInSeconds * size * Vector2.right + datum.Datum * -size * Vector2.up;
			var v2 = (_data[i].Time - startTime) / WidthInSeconds * size * Vector2.right + _data[i].Datum * -size * Vector2.up;
			Drawing.DrawLine (v1 + startPos, v2 + startPos, Color);
			
			datum = _data[i];
		}
		
		Drawing.DrawLine (startPos, startPos - Vector2.right * size);
	}
	
	public void LogDatum(float datum)
	{
		_data.Add(new DatumPacket() {
			Datum = datum,
			Time = Time.time
		});
	}
}
