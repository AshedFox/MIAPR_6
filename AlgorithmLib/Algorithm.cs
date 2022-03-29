using System.Diagnostics;

namespace AlgorithmLib;

public class Algorithm
{
    private Table _table = null!;
    private List<Move> _moves = null!;
    private readonly double _min = 0;
    private readonly double _max = 100;
    private readonly string _availableNames = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private byte _currentName;
    private AlgorithmType _currentType;

    public Move Process(int numOfObjects, AlgorithmType type)
    {
        _currentType = type;
        _moves = new List<Move>();
        _table = new Table(numOfObjects)
        {
            Cells = GenerateDistances(numOfObjects)
        };

        for (var k = 0; k < numOfObjects; k++)
        {
            _table.Headers[k] = $"x[{k}]";
        }

        PrintTable();

        while (_table.Cells.GetLength(0) > 1)
        {
            var (i, j) = Find();
            Unite(i, j);
            PrintTable();
        }

        return _moves.Last();
    }

    private void PrintTable()
    {
        Debug.WriteLine("");
        
        Debug.Write($"| {string.Empty, 5} |");
        for (var i = 0; i < _table.Cells.GetLength(0); i++)
        {
            Debug.Write($"| {_table.Headers[i], 5} |");
        }
        Debug.WriteLine("");
        
        for (var i = 0; i < _table.Cells.GetLength(0); i++)
        {
            Debug.Write($"| {_table.Headers[i], 5} |");
            for (var j = 0; j < _table.Cells.GetLength(1); j++)
            {
                Debug.Write($"| {_table.Cells[i, j]:00.00} |");
            }
            Debug.WriteLine("");
        }
    }
    
    private double[,] GenerateDistances(int numOfObjects)
    {
        var random = new Random();
        var cells = new double[numOfObjects, numOfObjects];

        for (var i = 0; i < cells.GetLength(0); i++)
        {
            for (var j = i + 1; j < cells.GetLength(1); j++)
            {
                cells[i, j] = random.GenerateDouble(_min, _max);
            }
        }
        
        for (var i = cells.GetLength(0) - 1; i > 0; i--)
        {
            for (var j = 0; j < i; j++)
            {
                cells[i, j] = cells[j, i];
            }
        }

        return cells;
    }

    private (int i, int j) Find()
    {
        if (_currentType == AlgorithmType.Min)
        {
            return FindMin();
        } 
        else
        {
            return FindMax();
        }
    }
    
    private (int i, int j) FindMax()
    {
        var (maxI, maxJ, maxValue) = (0, 0, double.MinValue);

        for (var i = 0; i < _table.Cells.GetLength(0); i++)
        {
            for (var j = i + 1; j < _table.Cells.GetLength(1); j++)
            {
                if (_table.Cells[i, j] > maxValue)
                {
                    (maxI, maxJ, maxValue) = (i, j, _table.Cells[i, j]);
                }
            }
        }

        return (maxI, maxJ);
    }
    
    private (int i, int j) FindMin()
    {
        var (minI, minJ, minValue) = (0, 0, double.MaxValue);

        for (var i = 0; i < _table.Cells.GetLength(0); i++)
        {
            for (var j = i + 1; j < _table.Cells.GetLength(1); j++)
            {
                if (_table.Cells[i, j] < minValue)
                {
                    (minI, minJ, minValue) = (i, j, _table.Cells[i, j]);
                }
            }
        }

        return (minI, minJ);
    }

    private void Unite(int x, int y)
    {
        var move = new Move()
        {
            NewValue = (_availableNames[_currentName++].ToString(), _table.Cells[x, y]),
            OldValues = new List<(string name, double val)>
            {
                (_table.Headers[x], _currentType == AlgorithmType.Max ? _max : _min),
                (_table.Headers[y], _currentType == AlgorithmType.Max ? _max : _min)
            }
        };

        //var participants = _moves.FindAll(m => move.OldValues.Exists(ov => ov.name == m.NewValue.name));
        
        var oldMoves = _moves.FindAll(m => move.OldValues.Exists(ov => ov.name == m.NewValue.name));
        move.OldMoves = oldMoves;
        
        if (oldMoves.Count == 2)
        {
            move.OldValues = oldMoves.Select(m => m.NewValue).ToList();
        } 
        else if (oldMoves.Count == 1)
        {
            move.OldValues = oldMoves.Select(m => m.NewValue).ToList();

            move.OldValues.Add(oldMoves[0].NewValue.name == _table.Headers[x]
                ? (_table.Headers[y], _currentType == AlgorithmType.Max ? _max : _min)
                : (_table.Headers[x], _currentType == AlgorithmType.Max ? _max : _min));
        }

        _moves.Add(move);
        
        var newLength = _table.Cells.GetLength(0) - 1;
        var newTable = new Table(newLength);

        var (newI, newJ) = (0, 0);

        for (var i = 0; i < _table.Cells.GetLength(0); i++)
        {
            var used = true;
            newJ = 0;
            for (var j = 0; j < _table.Cells.GetLength(1); j++)
            {
                if (i == x || i == y)
                {
                    used = false;
                    continue;
                }
                else if (i == j)
                {
                    newTable.Headers[newI] = _table.Headers[i];
                    newJ++;
                    continue;
                }
                else if (j == x || j == y)
                {
                    continue;
                }

                newTable.Cells[newI, newJ++] = _table.Cells[i, j];
                newTable.Headers[newI] = _table.Headers[i];
            }

            if (used)
            {
                newI++;
            }
        }

        var newRow = new double[newLength];
        var rowIndex = 0;

        for (var j = 0; j < _table.Cells.GetLength(1); j++)
        {
            if (j == y || j == x)
            {
                continue;
            }

            if (_currentType == AlgorithmType.Min)
            {
                newRow[rowIndex++] = Math.Min(_table.Cells[x, j], _table.Cells[y, j]);
            } else if (_currentType == AlgorithmType.Max)
            {
                newRow[rowIndex++] = Math.Max(_table.Cells[x, j], _table.Cells[y, j]);
            }
        }

        for (var i = 0; i < newLength; i++)
        {
            newTable.Cells[i, newLength - 1] = newRow[i];
            newTable.Cells[newLength - 1, i] = newRow[i];
        }

        newTable.Headers[newLength - 1] = move.NewValue.name;

        _table = newTable;
    }
}