using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

namespace ClassLibrary
{
    [Serializable]
    public class V5DataCollection : V5Data, IEnumerable<DataItem>, ISerializable
    {
        public Dictionary<Vector2, Vector2> Elements { get; set; }
        public List<DataItem> DataItems { get; set; }

        public V5DataCollection(string x1, DateTime x2) : base(x1, x2)
        {
            Elements = new Dictionary<Vector2, Vector2>();
            DataItems = new List<DataItem>();
        }

        public void InitRandom(int nItems, float xmax, float ymax, float minValue, float maxValue)
        {
            Random rand = new Random();

            float k1, k2, k3, k4, x, y, x_data, y_data;
            Vector2 key, value;

            for (int i = 0; i < nItems; i++)
            {
                k1 = (float)rand.NextDouble();
                k2 = (float)rand.NextDouble();
                k3 = (float)rand.NextDouble();
                k4 = (float)rand.NextDouble();
                x_data = minValue * k1 + maxValue * (1 - k1);
                y_data = minValue * k2 + maxValue * (1 - k2);
                x = xmax * k3;
                y = ymax * k4;

                key = new Vector2(x, y);
                value = new Vector2(x_data, y_data);

                Elements.Add(key, value);
            }
        }

        public override Vector2[] NearEqual(float eps)
        {
            List<Vector2> list = new List<Vector2>();
            foreach (KeyValuePair<Vector2, Vector2> kvp in Elements)
            {
                Vector2 theElement = kvp.Value;
                if (Math.Abs(theElement.X - theElement.Y) <= eps)
                    list.Add(theElement);

            }
            Vector2[] array = list.ToArray();
            return array;
        }

        public override string ToString()
        {
            string str = "V5DataCollection ";
            str += Info + " " + Time.ToString() + "\nNum of elements: " + Elements.Count + "\n";
            return str;
        }

        public override string ToLongString()
        {
            string str = "V5DataCollection ";
            str += Info + " " + Time.ToString() + "\nNum of elements: " + Elements.Count + "\n";
            foreach (KeyValuePair<Vector2, Vector2> kvp in Elements)
            {
                str += kvp.Key + " " + kvp.Value + "\n";
            }
            return str;
        }

        public override string ToLongString(string format)
        {
            string str = "V5DataCollection ";
            str += Info + " " + Time.ToString(format) + "\nNum of elements: " + Elements.Count + "\n";
            foreach (KeyValuePair<Vector2, Vector2> kvp in Elements)
            {
                str += kvp.Key + " " + kvp.Value.ToString(format) + "\n";
            }
            return str;
        }

        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            foreach (KeyValuePair<Vector2, Vector2> key in Elements)
            {
                DataItem Item = new DataItem(key.Key, key.Value);
                yield return Item;
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (KeyValuePair<Vector2, Vector2> key in Elements)
            {
                DataItem Item = new DataItem(key.Key, key.Value);
                yield return Item;
            }
        }

        public void GetObjectData(SerializationInfo Info, StreamingContext Context)
        {
            float[] CoordinateX = new float[Elements.Count];
            float[] CoordinateY = new float[Elements.Count];
            float[] ValueX = new float[Elements.Count];
            float[] ValueY = new float[Elements.Count];
            int Counter = 0;
            foreach (KeyValuePair<Vector2, Vector2> pair in Elements)
            {
                CoordinateX[Counter] = pair.Key.X;
                CoordinateY[Counter] = pair.Key.Y;
                ValueX[Counter] = pair.Value.X;
                ValueY[Counter] = pair.Value.Y;
                Counter++;
            }
            Info.AddValue("CoordinateX", CoordinateX);
            Info.AddValue("CoordinateY", CoordinateY);
            Info.AddValue("ValueX", ValueX);
            Info.AddValue("ValueY", ValueY);
            Info.AddValue("Info", base.Info);
            Info.AddValue("Time", Time);
        }

        public V5DataCollection(SerializationInfo Info, StreamingContext Context) :
                         base((string)Info.GetValue("Info", typeof(string)),
                    (DateTime)Info.GetValue("Time", typeof(DateTime)))
        {
            float[] CoordinateX = (float[])Info.GetValue("CoordinateX", typeof(float[]));
            float[] CoordinateY = (float[])Info.GetValue("CoordinateY", typeof(float[]));
            float[] ValueX = (float[])Info.GetValue("ValueX", typeof(float[]));
            float[] ValueY = (float[])Info.GetValue("ValueY", typeof(float[]));
            Elements = new Dictionary<Vector2, Vector2>();
            for (int j = 0; j < CoordinateX.Length; j++)
            {
                Elements.Add(new Vector2(CoordinateX[j], CoordinateY[j]), new Vector2(ValueX[j], ValueY[j]));
            }
        }

    }

}