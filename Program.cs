using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.IO;

namespace PathFinding
{
    public enum Step { Up, Down, Left, Right };
    
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string stringMap;
                using (StreamReader sr = new StreamReader("map.txt"))
                {
                    stringMap = sr.ReadToEnd();
                }
                char[,] map = LoadMap(stringMap);

                StringBuilder sb = new StringBuilder();
                char[,] doodleMap = map;
                int[] lastMarked = FindTiles(map, 'A', true)[0];
                List<Step> solution = SolveMaze(map, '1', '0', 'A', 'B', false);
                int pos = 0;
                foreach (Step step in solution)
                {
                    for (int i = 0; i < doodleMap.GetLength(0); i++)
                    {
                        for (int j = 0; j < doodleMap.GetLength(1); j++)
                        {
                            sb.Append(doodleMap[i,j]);
                            sb.Append(' ');
                        }
                        sb.Append("\n");
                    }
                    sb.Append("\n");

                    foreach (Step oneStep in solution)
                    {
                        sb.Append(oneStep);
                        sb.Append("\n");
                    }

                    Console.Clear();
                    Console.WriteLine(sb.ToString());
                    Thread.Sleep(500);

                    sb.Clear();
                    if (solution[pos] == Step.Down) 
                    { 
                        doodleMap[lastMarked[0] + 1, lastMarked[1]] = '■';
                        lastMarked[0]++;
                    }
                    else if (solution[pos] == Step.Up) 
                    { 
                        doodleMap[lastMarked[0] - 1, lastMarked[1]] = '■';
                        lastMarked[0]--;
                    }
                    else if (solution[pos] == Step.Right) 
                    { 
                        doodleMap[lastMarked[0], lastMarked[1] + 1] = '■';
                        lastMarked[1]++;
                    }
                    else if (solution[pos] == Step.Left) 
                    { 
                        doodleMap[lastMarked[0], lastMarked[1] - 1] = '■';
                        lastMarked[1]--;
                    }
                    pos++;
                }
            }
            catch (Exception e) { Console.WriteLine(e); }
            Console.ReadLine();
            
        }

        public static char[,] LoadMap(string stringMap)
        {
            string[] rows = stringMap.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            char[,] final = new char[rows.Length, rows[0].Length];
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Length != rows[0].Length) { throw new ArgumentException("Map must be a rectangle!"); }
                for(int f = 0; f <  rows[i].Length; f++)
                {
                    final[i,f] = rows[i][f];
                }
                
            }
            return final;
        }

        public static void CheckMap(char[,] map, char barier, char free, char begening, char end, bool multipleEndsPossible)
        {
            int begeningsOccurances = 0;
            int endsOccurances = 0;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i,j] == begening) { begeningsOccurances++; }
                    else if (map[i, j] == end) { endsOccurances++; }
                    else if (map[i,j] != barier && map[i,j] != free) { throw new ArgumentException("Map contains unexpected character!"); }
                }
            }
            if (!multipleEndsPossible && endsOccurances > 1) { throw new ArgumentException("More end points than allowed occured on given map!"); }
            if (begeningsOccurances > 1) { throw new ArgumentException("More than one start point occured on given map!"); }
            if (begeningsOccurances < 1) { throw new ArgumentException("No starting point found!"); }
            if (endsOccurances < 1) { throw new ArgumentException("No end point found!"); }
        }

        public static List<int[]> FindTiles(char[,] map, char searched, bool onelyOnce)
        {
            List<int[]> matches = new List<int[]>();
            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (map[i,j] == searched) { matches.Add(new int[] { i, j }); }
                }
            }
            if (onelyOnce && matches.Count > 1) { throw new ArgumentException("Map contains more than one tile of this type but onely one is asked to be find!"); }
            if (matches.Count > 0) { return matches; }
            else { throw new ArgumentException("Tile not found!"); }
        }

        public static bool ListOfListsContainsIntArray(List<List<int[]>> list, int[] element)
        {
            foreach (List<int[]> sublist in list)
            {
                if(ListContainsIntArray(sublist, element)) { return true; }
            }
            return false;
        }

        public static bool ListContainsIntArray(List<int[]> list, int[] element)
        {
            int pocet;
            foreach (int[] IntArr in list)
            {
                pocet = 0;
                for (int i = 0; i < IntArr.Length; i++)
                {
                    if (IntArr[i] == element[i]) { pocet++; }
                }
                if (pocet == IntArr.Length) { return true; }
            }
            return false;
        }

        public static List<int[]> SameIntArrForTwoLists(List<int[]> list1, List<int[]> list2)
        {
            List<int[]> final = new List<int[]>();
            foreach(int[] item in list1)
            {
                if (ListContainsIntArray(list2, item)) { final.Add(item); }
            }
            return final;
        }

        public static List<Step> SolveMaze(char[,] map, char barier, char free, char begening, char end, bool multipleEndsPossible)
        {
            List<Step> way = new List<Step>();
            List<List<int[]>> tiles = new List<List<int[]>>();
            CheckMap(map, barier, free, begening, end, multipleEndsPossible);
            tiles.Add(FindTiles(map, begening, true));
            List<int[]> ends = FindTiles(map, end, !multipleEndsPossible);
            bool found = false;
            List<int[]> curentList = new List<int[]>(); //in the end this is list of found endpoints
            int[] curentCoord = new int[] { 1, 1 };

            while (!found)
            {
                curentList = new List<int[]>();
                foreach (int[] start in tiles[tiles.Count - 1])
                {
                    curentCoord = new int[] { start[0] + 1, start[1] };
                    if
                    (
                        start[0] + 1 != map.GetLength(0) &&
                        (map[start[0] + 1, start[1]] == free || map[start[0] + 1, start[1]] == end) &&
                        !ListOfListsContainsIntArray(tiles, curentCoord) &&
                        !ListContainsIntArray(curentList, curentCoord)
                    ) { curentList.Add(curentCoord); }

                    curentCoord = new int[] { start[0] - 1, start[1] };
                    if
                    (
                        start[0] - 1 > -1 &&
                        (map[start[0] - 1, start[1]] == free || map[start[0] - 1, start[1]] == end) &&
                        !ListOfListsContainsIntArray(tiles, curentCoord) &&
                        !ListContainsIntArray(curentList, curentCoord)
                    ) { curentList.Add(curentCoord); }

                    curentCoord = new int[] { start[0], start[1] + 1 };
                    if
                    (
                        start[1] + 1 != map.GetLength(1) &&
                        (map[start[0], start[1] + 1] == free || map[start[0], start[1] + 1] == end) &&
                        !ListOfListsContainsIntArray(tiles, curentCoord) &&
                        !ListContainsIntArray(curentList, curentCoord)
                    ) { curentList.Add(curentCoord); }

                    curentCoord = new int[] { start[0], start[1] - 1 };
                    if
                    (
                        start[1] - 1 > -1 &&
                        (map[start[0], start[1] - 1] == free || map[start[0], start[1] - 1] == end) &&
                        !ListOfListsContainsIntArray(tiles, curentCoord) &&
                        !ListContainsIntArray(curentList, curentCoord)
                    ) { curentList.Add(curentCoord); }

                }
                tiles.Add(curentList);
                foreach (int[] endCoord in ends)
                {
                    if (ListOfListsContainsIntArray(tiles, endCoord))
                    {
                        found = true;
                        curentCoord = SameIntArrForTwoLists(curentList, ends)[0];
                    }
                }
                if (tiles.Count > map.GetLength(0) * map.GetLength(1)) { throw new ArgumentException("Unsolvable maze!"); }
            }

            for (int i = tiles.Count-2; i >= 0; i--)
            {
                foreach (int[] coord in tiles[i])
                {
                    if (coord[0] == curentCoord[0] + 1 && coord[1] == curentCoord[1]) 
                    { 
                        way.Add(Step.Up);
                        curentCoord = coord;
                        break;
                    }
                    if (coord[0] == curentCoord[0] - 1 && coord[1] == curentCoord[1])
                    {
                        way.Add(Step.Down);
                        curentCoord = coord;
                        break;
                    }
                    if (coord[1] == curentCoord[1] + 1 && coord[0] == curentCoord[0])
                    {
                        way.Add(Step.Left);
                        curentCoord = coord;
                        break;
                    }
                    if (coord[1] == curentCoord[1] - 1 && coord[0] == curentCoord[0])
                    {
                        way.Add(Step.Right);
                        curentCoord = coord;
                        break;
                    }
                }
            }
            

            way.Reverse();
            return way;
        }
    }

}
