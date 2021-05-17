using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using System.Runtime.Serialization;

namespace ClassLibrary
{
    [Serializable]
    public class V5DataOnGrid : V5Data, IEnumerable<DataItem>, ISerializable
    {
        public Grid2D Grid { get; set; }
        public Vector2[,] Vector { get; set; }

        public V5DataOnGrid(string x1, DateTime x2, Grid2D grid) : base(x1, x2)
        {
            Grid = grid;
            Vector = new Vector2[Grid.NumOfNodesX, Grid.NumOfNodesY];
        }

        public V5DataOnGrid(string filename)
        {
            StreamReader sr = null;

            try
            {
                sr = new StreamReader(filename);

                Info = sr.ReadLine();

                Time = DateTime.Parse(sr.ReadLine());

                Grid2D grid = new Grid2D
                {
                    NumOfNodesX = int.Parse(sr.ReadLine()),
                    StepX = float.Parse(sr.ReadLine()),
                    NumOfNodesY = int.Parse(sr.ReadLine()),
                    StepY = float.Parse(sr.ReadLine()),
                };
                Grid = grid;
                Vector = new Vector2[Grid.NumOfNodesX, Grid.NumOfNodesY];

                for (int i = 0; i < Grid.NumOfNodesX; i++)
                {
                    for (int j = 0; j < Grid.NumOfNodesY; j++)
                    {

                        string[] data = sr.ReadLine().Split(' ');

                        Vector[i, j] = new Vector2(
                             (float)Convert.ToDouble(data[0]),
                             (float)Convert.ToDouble(data[1]));
                    }
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine("Filename is empty string");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File is not found");
            }
            catch (DirectoryNotFoundException)
            {
                Console.WriteLine("Directory is not found");
            }
            catch (IOException)
            {
                Console.WriteLine("Unacceptable filename");
            }
            catch (FormatException)
            {
                Console.WriteLine("String could not be parsed");
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sr != null)
                    sr.Dispose();
            }
        }

        public void InitRandom(float MinValue, float MaxValue)
        {
            Random rand = new Random();
            float x, y;
            for (int i = 0; i < Grid.NumOfNodesX; i++)
                for (int j = 0; j < Grid.NumOfNodesY; j++)
                {
                    x = (float)rand.NextDouble();
                    y = (float)rand.NextDouble();
                    x = MinValue * x + MaxValue * (1 - x);
                    y = MinValue * y + MaxValue * (1 - y);
                    Vector[i, j] = new Vector2(x, y);
                }
        }

        public static explicit operator V5DataCollection(V5DataOnGrid Data)
        {
            V5DataCollection Col = new V5DataCollection(Data.Info, Data.Time);
            Vector2 key, value;
            for (int i = 0; i < Data.Grid.NumOfNodesX; i++)
                for (int j = 0; j < Data.Grid.NumOfNodesY; j++)
                {
                    key = new Vector2(i * Data.Grid.StepX, j * Data.Grid.StepY);
                    value = new Vector2(Data.Vector[i, j].X, Data.Vector[i, j].Y);
                    Col.Elements.Add(key, value);
                }
            return Col;
        }

        public override Vector2[] NearEqual(float eps)
        {
            List<Vector2> v = new List<Vector2>();
            for (int i = 0; i < Grid.NumOfNodesX; i++)
                for (int j = 0; j < Grid.NumOfNodesY; j++)
                    if (Math.Abs(Vector[i, j].X - Vector[i, j].Y) <= eps)
                        v.Add(Vector[i, j]);
            Vector2[] res = v.ToArray();
            return res;
        }

        public override string ToLongString()
        {

            string ResultString = "V5DataOnGrid\n";
            ResultString += Info + " " + Time.ToString() + " " + Grid.ToString() + "\n";
            for (int i = 0; i < Grid.NumOfNodesX; i++)
                for (int j = 0; j < Grid.NumOfNodesY; j++)
                {
                    ResultString += "[" + (i * Grid.StepX).ToString() + ", " +
                                 (j * Grid.StepY).ToString() + "] (" +
                                 Vector[i, j].X + ", " + Vector[i, j].Y + ")\n";
                }
            ResultString += "\n";
            return ResultString;
        }

        public override string ToString()
        {
            string str = "V5DataOnGrid\n";
            str += Info + " " + Time.ToString() + " " + Grid.ToString() + "\n";
            return str;

        }

        public override string ToLongString(string format)
        {
            string ResultString = "V5DataOnGrid\n";
            ResultString += Info + " " + Time.ToString() + " " + Grid.ToString(format) + "\n";
            for (int i = 0; i < Grid.NumOfNodesX; i++)
                for (int j = 0; j < Grid.NumOfNodesY; j++)
                {
                    ResultString += "[" + (i * Grid.StepX).ToString(format) + ", " +
                                 (j * Grid.StepY).ToString(format) + "] (" +
                                 Vector[i, j].X.ToString(format) + ", " +
                                 Vector[i, j].Y.ToString(format) + ")\n";
                }
            ResultString += "\n";
            return ResultString;
        }

        IEnumerator<DataItem> IEnumerable<DataItem>.GetEnumerator()
        {
            List<DataItem> Items = new List<DataItem>();
            Vector2 Vector = new Vector2(0, 0);
            DataItem Item = new DataItem(Vector, Vector);
            for (int i = 0; i < Grid.NumOfNodesX; i++)
                for (int j = 0; j < Grid.NumOfNodesX; j++)
                {
                    Vector.X = i * Grid.StepX;
                    Vector.Y = j * Grid.StepY;
                    Item.Coordinate = Vector;
                    Item.Value = this.Vector[i, j];
                    Items.Add(Item);
                }
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            List<DataItem> Items = new List<DataItem>();
            Vector2 Vector = new Vector2(0, 0);
            DataItem Item = new DataItem(Vector, Vector);
            for (int i = 0; i < Grid.NumOfNodesX; i++)
                for (int j = 0; j < Grid.NumOfNodesX; j++)
                {
                    Vector.X = i * Grid.StepX;
                    Vector.Y = j * Grid.StepY;
                    Item.Coordinate = Vector;
                    Item.Value = this.Vector[i, j];
                    Items.Add(Item);
                }
            return Items.GetEnumerator();
        }

        public void GetObjectData(SerializationInfo Info, StreamingContext Context)
        {
            float[] ValueX = new float[Grid.NumOfNodesX];
            float[] ValueY = new float[Grid.NumOfNodesY];
            for (int i = 0; i < ValueX.Length; i++)
                for (int j = 0; j < ValueY.Length; j++)
                {
                    ValueX[i] = Vector[i, j].X;
                    ValueY[j] = Vector[i, j].Y;
                }
            Info.AddValue("Grid", Grid);
            Info.AddValue("ValueX", ValueX);
            Info.AddValue("ValueY", ValueY);
            Info.AddValue("Info", base.Info);
            Info.AddValue("Time", Time);
        }

        public V5DataOnGrid(SerializationInfo Info, StreamingContext Context):
                    base((string)Info.GetValue("Info", typeof(string)),
                       (DateTime)Info.GetValue("Time", typeof(DateTime)))
        {
            Grid = (Grid2D)Info.GetValue("Grid", typeof(Grid2D));
            Vector = new Vector2[Grid.NumOfNodesX, Grid.NumOfNodesY];
            float[] ValueX = (float[])Info.GetValue("ValueX", typeof(float[]));
            float[] ValueY = (float[])Info.GetValue("ValueY", typeof(float[]));
            for (int i = 0; i < ValueX.Length; i++)
                for (int j = 0; j < ValueY.Length; j++)
                {
                    Vector[i, j].X = ValueX[i];
                    Vector[i, j].Y = ValueY[j];
                }
        }
    }
}
