using System;
using System.Numerics;
using System.Runtime.Serialization;

namespace ClassLibrary
{
    [Serializable]
    public struct DataItem:ISerializable
    {
        public Vector2 Coordinate { get; set; }
        public Vector2 Value { get; set; } 

        public DataItem(Vector2 coord, Vector2 val)
        {
            Coordinate = coord;
            Value = val;
        }

        public DataItem(V5DataOnGrid Data, int x, int y)
        {
            Coordinate  = new Vector2(Data.Grid.StepX * x, Data.Grid.StepY * y);
            Value = Data.Vector[x, y];
        }

        public override string ToString()
        {
            return Coordinate.ToString() + " " + Value.ToString() + "\n";
        }

        public string ToString(string format)
        {
            return Coordinate.ToString(format) + " " + Value.ToString(format) + "\n" 
                + "Vector Size: " + Math.Sqrt(Value.X * Value.X + Value.Y * Value.Y).ToString(format) + "\n";
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("CoordinateX", Coordinate.X);
            info.AddValue("CoordinateY", Coordinate.Y);
            info.AddValue("ValueX", Value.X);
            info.AddValue("ValueY", Value.Y);
        }

        public DataItem(SerializationInfo info, StreamingContext context)
        {
            float x = info.GetSingle("CoordinateX");
            float y = info.GetSingle("CoordinateY");
            Coordinate = new Vector2(x, y);
            x = info.GetSingle("ValueX");
            y = info.GetSingle("ValueY");
            Value = new Vector2(x, y);
        }

    }
}

