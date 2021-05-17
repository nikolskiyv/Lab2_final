using System;
using System.Numerics;

namespace ClassLibrary
{
    [Serializable]
    public abstract class V5Data
    {
        public string Info { get; set; }
        public DateTime Time { get; set; }

        public V5Data(string I = "default", DateTime T = default)
        {
            Info = I;
            Time = T;
        }

        public abstract Vector2[] NearEqual(float eps);

        public abstract string ToLongString();

        public override string ToString()
        {
            return Info + ", " + Time.ToString() + "\n";
        }

        public abstract string ToLongString(string format);
    }
}