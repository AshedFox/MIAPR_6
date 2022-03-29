namespace AlgorithmLib;

public class Table
{
    public double[,] Cells { get; set;  }
    public string[] Headers { get; set; }

    public Table(int numOfObjects)
    {
        Cells = new double[numOfObjects, numOfObjects];
        Headers = new string[numOfObjects];
    }
}