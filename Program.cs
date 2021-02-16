using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinding
{
    public enum Step { Up, Down, Left, Right };
    
    class Program
    {
        static void Main(string[] args)
        {
        }

        public static char[,] LoadMap(string stringMap)
        {
            string[] rows = stringMap.Split('\n');
            char[,] final = new char[rows.Length, rows[0].Length];
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].Length != rows[0].Length) { throw new ArgumentException("Map must be a rectangle!"); }
                final[i] = rows[i].ToCharArray();
            }
            return final;
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