using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace max_flow_ford_fulkerson
{
    class Program
    {
        static void Main(string[] args)
        {
            (int, int) sizeGraph = (0, 0);
            List<(int, int, int)> edgesList = Read("test.txt", ref sizeGraph);
            int[,] matrix = AdjMatrix(sizeGraph, edgesList);
            (int, int[,]) result = Fulkerson(matrix, sizeGraph);
            ShowResults(result.Item1, result.Item2, matrix);
        }
        static List<(int, int, int)> Read(string path, ref (int, int) sizeGraph)
        { 
            List<(int, int, int)> list = new List<(int, int, int)>();
            StreamReader read = new StreamReader(path);
            string[] size = read.ReadLine()?.Split(' ');
            if (size != null)
            {
                sizeGraph = (Convert.ToInt32(size[0]), Convert.ToInt32(size[1]));
                for (int i = 0; i < sizeGraph.Item2; ++i)
                {
                    size = read.ReadLine()?.Split(' ');
                    if (size != null)
                        list.Add((Convert.ToInt32(size[0]), Convert.ToInt32(size[1]), Convert.ToInt32(size[2])));
                }
            }
            return list;
        }
        static int[,] AdjMatrix((int, int) sizeMatrix, List<(int, int, int)> edgeList)
        {
            int[,] matrixA = new Int32[sizeMatrix.Item1, sizeMatrix.Item1];
            foreach (var item in edgeList)
                matrixA[item.Item1 - 1, item.Item2 - 1] = item.Item3;
            return matrixA;
        }
        static void ShowResults(int max, int[,]  newMatrix, int[,] matrix)
        {
            Console.WriteLine($"Максимальный поток {max}");
            Console.WriteLine("Поток по каждому ребру");
            for(var i = 0; i < matrix.GetLength(0); ++i)
                for(var j = 0; j < matrix.GetLength(0); ++j)
                    if (matrix[i, j] != 0 && SearchFlowForShow(newMatrix, i, j, matrix) > 0)
                        Console.WriteLine($"из {i + 1} в {j + 1} = {SearchFlowForShow(newMatrix, i, j, matrix)}");
        }
        static double SearchFlowForShow(int[,] newMatrix, int i, int j, int[,] matrix)
        {
            if (newMatrix[i, j] < 0)
                return matrix[i, j];
            return matrix[i, j] - newMatrix[i, j];
        }
        static (int, int) IstokStok(int[,] matrix, (int, int) sizeG)
        {
            int[] degreeEntry = new int[sizeG.Item1];
            int[] degreeExit = new int[sizeG.Item1];
            (int, int) istokstok = (0, 0);
            for(var i = 0; i < sizeG.Item1; ++i)
                for(var j = 0; j < sizeG.Item1; ++j)
                {
                    if (matrix[i, j] != 0)
                    {
                        ++degreeEntry[j];
                        ++degreeExit[i];
                    }
                    if (degreeEntry[j] == 0)
                        istokstok = (j, istokstok.Item2);
                    else if (degreeEntry[i] == 0)
                        istokstok = (i, istokstok.Item2);
                    if (degreeExit[i] == 0)
                        istokstok = (istokstok.Item1, i);
                    else if (degreeExit[j] == 0)
                        istokstok = (istokstok.Item1, j);
                    
                }
            return istokstok;
        }
        static (int, int[,]) Fulkerson(int[,] matrix, (int, int) sizeG)
        {
            (int, int) istokStok = IstokStok(matrix, sizeG);
            var parent = Enumerable.Repeat(-1, matrix.Length).ToArray();
            var changeAdjM = matrix.Clone() as int[,];
            var max = 0;
            
            while(BFS(changeAdjM, istokStok , ref parent))
            {  
                int temp = int.MaxValue;
                int start = istokStok.Item2;

                while(start != istokStok.Item1)
                {
                    temp = Math.Min(temp, changeAdjM[parent[start], start]);
                    start = parent[start];
                }

                max += temp;
                int v = istokStok.Item2;

                while(v != istokStok.Item1)
                {
                    var u = parent[v];
                    changeAdjM[u, v] -= temp;
                    changeAdjM[v, u] += temp;
                    v = parent[v];
                }
            }
            return (max, changeAdjM);
        }
        static bool BFS(int[,] matrix, (int, int) istokstok, ref int[] parent)
        {
            var visited = Enumerable.Repeat(false, matrix.Length).ToArray();
            var queue = new Queue<int>();
            visited[istokstok.Item1] = true; queue.Enqueue(istokstok.Item1);
            while(queue.Count != 0)
            {
                var u = queue.Dequeue();
                for(var v = 0; v < matrix.GetLength(0); ++v)
                    if (!visited[v] && matrix[u, v] > 0)
                    {
                        queue.Enqueue(v);
                        visited[v] = true;
                        parent[v] = u;
                    }
            }
            return (visited[istokstok.Item2]);
        }
    }
}