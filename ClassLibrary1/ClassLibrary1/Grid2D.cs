using System;

namespace ClassLibrary
{
    [Serializable]
    public struct Grid2D
    {
        public float StepX { get; set; }
        public int NumOfNodesX { get; set; }
        public float StepY { get; set; }
        public int NumOfNodesY { get; set; }

        public Grid2D(float x1 = 10, float y1 = 10, int x2 = 3, int y2 = 3)
        {
            StepX = x1;
            StepY = y1;
            NumOfNodesX = x2;
            NumOfNodesY = y2;
        }

        public Grid2D(Grid2D grid)
        {
            StepX = grid.StepX;
            StepY = grid.StepY;
            NumOfNodesX = grid.NumOfNodesX;
            NumOfNodesY = grid.NumOfNodesY;
        }

        public override string ToString()
        {
            return "Step X: " + StepX.ToString() + ",\n" +
                   "Number of nodes X: " + NumOfNodesX.ToString() + ",\n" +
                   "Step Y: " + StepY.ToString() + ",\n" +
                   "Number of nodes Y: " + NumOfNodesY.ToString();
        }

        public string ToString(string format)
        {
            return "Step X: " + StepX.ToString(format) + ",\n" +
                   "Number of nodes X: " + NumOfNodesX.ToString() + ",\n" +
                   "Step Y: " + StepY.ToString(format) + ",\n" +
                   "Number of nodes Y: " + NumOfNodesY.ToString();
        }

    }
}
