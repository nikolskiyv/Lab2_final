using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.ComponentModel;

[assembly: InternalsVisibleToAttribute("WpfApp1")]
namespace ClassLibrary
{
    [Serializable]
    public class V5MainCollection : IEnumerable<V5Data>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private List<V5Data> List { get; set; }  
        public bool IsChanged { get; set; }

        public V5MainCollection()
        {
            List = new List<V5Data>();
        }

        [field: NonSerialized]
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged(NotifyCollectionChangedAction ev)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public string ErrorMessage { get; set; }

        public IEnumerator<V5Data> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public int Count => List.Count; 
           
        public bool Remove(string id, DateTime date)
        {
            bool flag = false;
            for (int i = 0; i < List.Count; i++)
            {
                if (Equals(List[i].Info, id) == true
                        && List[i].Time.CompareTo(date) == 0)
                {
                    List.RemoveAt(i);
                    i--;
                    flag = true;
                    OnPropertyChanged("IsChanged");
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove);
                    OnPropertyChanged("Count");
                    OnPropertyChanged("MinAbsMainCollection");
                }
            }
            return flag;
        }

        public void AddDefaults()
        {
            Random rand = new Random();
          
            int NElem = rand.Next(3, 7), Random1, Random2, Random3, Random4;
            Grid2D Gr;
            V5DataCollection DataColl;
            V5DataOnGrid DataGrid;
            //List = new List<V5Data>();
            for (int i = 0; i < NElem; i++)
            {
                Random1 = rand.Next(0, 2);                
                if (Random1 == 0)
                {     
                    Random3 = rand.Next(1, 10);
                    Random4 = rand.Next(1, 10);
                    Gr = new Grid2D(Random3, Random3, Random4, Random4);
                    DataGrid = new V5DataOnGrid("", DateTime.Now, Gr);
                    DataGrid.InitRandom(0, 10);
                    List.Add(DataGrid);
                }
                else
                {
                    Random2 = rand.Next(1, 10);
                    DataColl = new V5DataCollection("", DateTime.Now);
                    DataColl.InitRandom(Random2, 4, 5, 1, 4);
                    List.Add(DataColl);
                }
            }
            OnCollectionChanged(NotifyCollectionChangedAction.Add);
            OnPropertyChanged("Count");
            OnPropertyChanged("MinAbsMainCollection");
            IsChanged = true;
        }

        public override string ToString()
        {
            string str = "";
            foreach (V5Data item in List)
            {
                str += item.ToString();
            }
            str += "\n\n";
            return str;
        }

        public string ToLongString(string format)
        {
            string str = "";
            foreach (V5Data item in List)
            {
                str += item.ToLongString(format);
            }
            str += "\n\n";
            return str;
        }

        public float MinVectorLengthDataCollection
        {
            get
            {
                var Query = from Elem in (from Data in List
                                          where Data is V5DataCollection
                                          select (V5DataCollection)Data)
                            from Item in Elem
                            select Item.Value.Length();
                if (Query.Count() > 0)
                    return Query.Min();
                else return 0;
            }
        }

        public float MinVectorLengthDataOnGrid
        {
            get
            {
                var Query = from Elem in (from Data in List
                                          where Data is V5DataOnGrid
                                          select (V5DataOnGrid)Data)
                            from Item in Elem
                            select Item.Value.Length();
                if (Query.Count() > 0)
                    return Query.Min();
                else return 0;
            }
        }

        public int DataOnGridCounter
        {
            get
            {
                var query = from data in List
                            where data is V5DataOnGrid
                            select (V5DataOnGrid)data;
                return query.Count();
            }
        }

        public int DataCollectionCounter
        {
            get
            {
                var Query = from Data in List
                            where Data is V5DataCollection
                            select (V5DataCollection)Data;
                return Query.Count();
            }
        }

        public string MinAbsMainCollection
        {
            get 
            {   if (Count == 0)
                {
                    return 0.ToString();
                }
                else
                {
                    if ((DataOnGridCounter != 0) && (DataCollectionCounter != 0) && (MinVectorLengthDataCollection > MinVectorLengthDataOnGrid))
                        return MinVectorLengthDataCollection.ToString("g3");
                    else if (DataCollectionCounter == 0)
                        return MinVectorLengthDataOnGrid.ToString("g3");
                    else return MinVectorLengthDataCollection.ToString("g3");
                }
            }
        }



        public float MaxDistance(Vector2 v)
        {
            float MaxDC = 0;
            float MaxDG = 0;
            var query1 = from elem in (from data in List
                                       where data is V5DataCollection
                                       select (V5DataCollection)data)
                         from item in elem
                         select Vector2.Distance(v, item.Coordinate);
            if (query1 != null)
                MaxDC = query1.Max();

            var query2 = from elem in (from data in List
                                       where data is V5DataOnGrid
                                       select (V5DataOnGrid)data)
                         from item in elem
                         select Vector2.Distance(v, item.Coordinate);
            if (query2 != null)
                MaxDG = query2.Max();

            if (MaxDC > MaxDG)
                return MaxDC;
            else
                return MaxDG;
        }

        public IEnumerable<DataItem> MaxDistanceItems(Vector2 v)
        {
            var query1 = from elem in (from data in List
                                       where data is V5DataCollection
                                       select (V5DataCollection)data)
                         from item in elem
                         where Vector2.Distance(v, item.Coordinate) == MaxDistance(v)
                         select item;

            var query2 = from elem in (from data in List
                                       where data is V5DataOnGrid
                                       select (V5DataOnGrid)data)
                         from item in elem
                         where Vector2.Distance(v, item.Coordinate) == MaxDistance(v)
                         select item;

            var query = query1.Union(query2);
            return query;
        }

        public IEnumerable<DataItem> DataItems
        {
            get
            {
                var Query1 = from Elem in (from Data in List
                                           where Data is V5DataCollection
                                           select (V5DataCollection)Data)
                             from Item in Elem
                             select Item;

                var Query2 = from Elem in (from Data in List
                                           where Data is V5DataOnGrid
                                           select (V5DataOnGrid)Data)
                             from Item in Elem
                             select Item;

                var Query = Query1.Union(Query2);

                return from Elem in Query
                       orderby Elem.Value.Length()
                       select Elem;

            }
        }

        public IEnumerable<Vector2> Points
        {
            get
            {
                IEnumerable<Vector2> Query1 = from Elem in
                                                        (from Data in List
                                                         where Data is V5DataOnGrid
                                                         select (V5DataOnGrid)Data)
                                              from Item in Elem
                                              select Item.Coordinate;
                IEnumerable<Vector2> Query2 = from Elem in
                                                        (from Data in List
                                                         where Data is V5DataCollection
                                                         select (V5DataCollection)Data)
                                              from Item in Elem
                                              select Item.Coordinate;
                var Query = from Item in Query1.Except(Query2)
                            select Item;
                return Query;
            }
        }




        public IEnumerable<V5DataCollection> getDCElems()
        {
            var query = from item in this.List
                        where item is V5DataCollection
                        select item as V5DataCollection;
            return query;
        }

        public IEnumerable<V5DataOnGrid> getDoGElems()
        {
            var query = from item in this.List
                        where item is V5DataOnGrid
                        select item as V5DataOnGrid;
            return query;
        }

        public void Save(string filename)
        {
            FileStream fs = null;
            try
            {
                if (!File.Exists(filename))
                {
                    fs = File.Create(filename);
                }
                else
                {
                    fs = File.OpenWrite(filename);
                }
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, List);
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                IsChanged = false;
                OnPropertyChanged("IsChanged");
            }
        }

        public void Load(string filename)
        {
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(filename);
                BinaryFormatter bf = new BinaryFormatter();
                var list = (List<V5Data>)bf.Deserialize(fs);
                List = list;
            }
            finally
            {
                if (fs != null)
                    fs.Close();
                IsChanged = true;
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
                OnPropertyChanged("IsChanged");
                OnPropertyChanged("Count");
                OnPropertyChanged("MinMC");
            }
        }

        public void Add(V5Data item)
        {
            try
            {
                List.Add(item);
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
                IsChanged = true;
                OnPropertyChanged("IsChanged");
                OnPropertyChanged("Count");
                OnPropertyChanged("MinMC");              
            }
            catch (Exception ex)
            {
                ErrorMessage = "Add Element: " + ex.Message;
            }
        }

        public void AddFromFile(string filename)
        {
            try
            {
                V5DataOnGrid DG = new V5DataOnGrid(filename);
                Add(DG);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddDefaultDataCollection()
        {
            V5DataCollection DC = new V5DataCollection("Default DoG", default);
            DC.InitRandom(1, 10, 10, 0, 10);
            Add(DC);
        }

        public void AddDefaultDataOnGrid()
        {
            Grid2D grid = new Grid2D();
            V5DataOnGrid DoG = new V5DataOnGrid("Default DoG", default, grid);
            DoG.InitRandom(0, 10);
            Add(DoG);
        }
    }
}