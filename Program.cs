using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (onelyOnce && matches.Count > 1) { throw new ArgumentException("Map contains more than one tile of this type but onely one is asked to be find!")}
            if (matches.Count > 0) { return matches; }
            else { throw new ArgumentException("Tile not found!"); }
        }

        public static List<Step> SolveMaze(char[,] map, char barier, char free, char begening, char end, bool multipleEndsPossible)
        {
            List<Step> way = new List<Step>();
            List<List<int[]>> tiles = new List<List<int[]>>();
            CheckMap(map, barier, free, begening, end, multipleEndsPossible);
            tiles.Add(new List<int[]>{FindTiles(map, begening, true).ToArray()[0]};
            List<int[]> ends = FindTiles(map, end, !multipleEndsPossible);
            bool found = false;
            List<int[]> curentList = new List<int[]>(); //in the end this is list of found endpoints
            int[] curentCoord;

            while (!found)
            {
                curentList.Clear();
                foreach (int[] start in tiles[tiles.Count-1])
                {
                    try
                    {
                        if
                        (
                            map[start[0] + 1, start[1]] == free &&
                            !tiles[tiles.Count - 1].Any(item => item == new int[] { start[0] + 1, start[1] })
                        ) { curentList.Add(new int[] { start[0] + 1, start[1] }); }
                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if
                        (
                            map[start[0] - 1, start[1]] == free &&
                            !tiles[tiles.Count - 1].Any(item => item == new int[] { start[0] - 1, start[1] })
                        ) { curentList.Add(new int[] { start[0] - 1, start[1] }); }
                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if
                        (
                            map[start[0], start[1] + 1] == free &&
                            !tiles[tiles.Count - 1].Any(item => item == new int[] { start[0], start[1] + 1 })
                        ) { curentList.Add(new int[] { start[0], start[1] + 1 }); }
                    }
                    catch (IndexOutOfRangeException) { }
                    try
                    {
                        if
                        (
                            map[start[0], start[1] - 1] == free &&
                            !tiles[tiles.Count - 1].Any(item => item == new int[] { start[0], start[1] - 1 })
                        ) { curentList.Add(new int[] { start[0], start[1] - 1 }); }
                    }
                    catch (IndexOutOfRangeException) { }
                }
                tiles.Add(curentList);
                if (curentList.Intersect(ends).Count() > 1) 
                { 
                    found = true;
                    curentCoord = curentList.Intersect(ends).ToList()[0]; //tohle by se mělo upravit, aby tam nebylo to .ToList()
                }
            }

            for (int i = tiles.Count-1; i >= 0; i--)
            {
                //tenhle loop pojede "pozpátku" skrz tiles(postupný list těch průstupných míst) 
                //a u každé "vrstvy" vyhledá tile, v dochozí vzdálenosti z curentCoord
                //a přidá směr z této do CurentCoord do way a přiřadí tento tile do CurentCoord
            }
            

            way.Reverse();
            return way;
        }
    }

}

/*
Příklad: Vytvořte program, který pro robota, 
který umí udělat 1 krok vlevo, vpravo, nahoru nebo dolů, 
vytvoří posloupnost kroků tak, že se robot dostane z libovolného výchozího místa 
do libovolného konečného místa. 
Robot prochází dvoudimenzionálním prostorem s bariérami, který modelujte jako 
dvoudimenzionální pole celých čísel, kde 0 značí volné pole a 1 značí obsazené pole:

Příklad prostoru:
Z 0 0 0 0 0 0 0 0
0 1 1 1 1 1 0 0 0
1 0 0 0 0 0 0 1 1
0 0 0 0 1 0 0 1 1
1 1 1 1 1 1 0 1 1
1 1 1 1 1 0 0 0 0
0 0 K 0 0 0 0 0 0

Vypište instrukce robotu tak, aby se dostal z místa Z do místa K.
Zakreslete postup robota do prostoru.

 */