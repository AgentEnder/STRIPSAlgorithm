using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace STRIPSAlgorithm
{
    class FileParser
    {
        char[,] startState;
        char[,] goalState;
        int w, h;

        public FileParser(string path)
        {
            Queue<string> lines = new Queue<string>(File.ReadAllLines(path));
            h = lines.Count / 2;
            w = lines.Peek().Split().Count();
            startState = new char[W, H];
            goalState = new char[W, H];
            string[] line;
            int y = 0;
            while ((line = lines.Dequeue().Split()).Count() > 1)
            {
                for (int x = 0; x < W; x++)
                {
                    startState[x, y] = char.Parse(line[x]);
                }
                y++;
            }
            y = 0;
            while (lines.Count() > 0 && (line = lines.Dequeue().Split()).Count() > 0)
            {
                for (int x = 0; x < W; x++)
                {
                    char c = goalState[x, y] = char.Parse(line[x]);
                    if (c!='0')
                    {
                        Program.usedChars.Add(c);
                    }
                }
                y++;
            }
        }

        public char[,] StartState { get => startState; }
        public char[,] GoalState { get => goalState; }
        public int W { get => w; }
        public int H { get => h; }
    }
}
