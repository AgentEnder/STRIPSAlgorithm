using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace STRIPSAlgorithm
{
    class Program
    {
        struct Action
        {
            public string name;
            public List<string> pre;
            public List<string> add;
            public List<string> del;
        }

        #region factoryMethods
        static Action Stack(char x, char y)
        {
            List<string> pre = new List<string>
            {
                $"Clear({y})",
                $"Holding({x})"
            };
            List<string> add = new List<string>
            {
                $"ArmEmpty()",
                $"On({x},{y})",
                $"Clear({x})"
            };
            List<string> del = new List<string>
            {
                $"Clear({y})",
                $"Holding({x})"
            };
            return new Action { name = $"Stack({x},{y})", pre = pre, add = add, del = del };
        }

        static Action Unstack(char x, char y)
        {
            List<string> pre = new List<string>
            {
                $"ArmEmpty()",
                $"On({x},{y})",
                $"Clear({x})"
            };
            List<string> add = new List<string>
            {
                $"Holding({x})",
                $"Clear({y})"
            };
            List<string> del = new List<string>
            {
                $"On({x},{y})",
                $"Clear({x})",
                "ArmEmpty()"
            };
            return new Action { name = $"Unstack({x},{y})", pre = pre, add = add, del = del };
        }

        static Action Pickup(char x)
        {
            List<string> pre = new List<string>
            {
                $"ArmEmpty()",
                $"OnTable({x})",
                $"Clear({x})"
            };
            List<string> add = new List<string>
            {
                $"Holding({x})"
            };
            List<string> del = new List<string>
            {
                $"OnTable({x})",
                $"Clear({x})",
                $"ArmEmpty()"
            };
            return new Action { name = $"Pickup({x})", pre = pre, add = add, del = del };
        }

        static Action Putdown(char x)
        {
            List<string> add = new List<string>
            {
                $"OnTable({x})",
                $"Clear({x})",
                $"ArmEmpty()"
            };
            List<string> pre = new List<string>
            {
                $"Holding({x})"
            };
            List<string> del = new List<string>
            {
                $"Holding({x})"
            };
            return new Action { name = $"Putdown({x})", pre = pre, add = add, del = del };
        }
        #endregion

        static public List<char> usedChars = new List<char>();

        static void Main(string[] args)
        {
            string fileNum = Console.ReadLine();
            FileParser data = new FileParser($"test{fileNum}.in");
            List<string> starting = getWorldPredicates(data.StartState, data.W, data.H);
            List<string> goal = getWorldPredicates(data.GoalState, data.W, data.H);

            List<string> plan = STRIPS(starting, goal, usedChars);

            Console.WriteLine("You should: ");
            for (int i = 0; i < plan.Count(); i++)
            {
                Console.WriteLine("\t" + plan[i]);
            }
            Console.ReadKey();
        }

        static public char[,] getWorld(int w, int h)
        {
            usedChars.Clear();
            char[,] world = new char[w, h];
            for (int query = 0; query < w * h; query++)
            {
                Console.WriteLine("Lets build a world! Enter the charachter that's block would go where the ? is, or a 0 for none.");
                Console.WriteLine();
                int _x, _y;
                _x = _y = 0;
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        int idx = y * h + x;
                        if (idx < query)
                        { //Already set
                            Console.Write(world[x, y] + " ");
                        }
                        else if (idx == query)
                        {
                            _x = x;
                            _y = y;
                            Console.Write("? ");
                        }
                        else
                        {
                            Console.Write("| ");
                        }
                    }
                    Console.WriteLine();
                }
                char c = world[_x, _y] = char.Parse(Console.ReadLine());
                if (c != '0')
                {
                    if (usedChars.Contains(world[_x, _y]))
                    {
                        throw new Exception("Each block label can only be used once!");
                    }
                    else
                    {
                        usedChars.Add(world[_x, _y]);
                    }
                }
                Console.Clear();
            }
            return world;
        }

        static public List<string> getWorldPredicates(char[,] world, int w, int h)
        {
            List<string> predicates = new List<string>();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (world[x, y] != '0')
                    {
                        if (y == h - 1)
                        {
                            predicates.Add($"OnTable({world[x, y]})");
                        }
                        else
                        {
                            predicates.Add($"On({world[x, y]},{world[x, y + 1]})");
                        }
                        if (y == 0 || world[x, y - 1] == '0')
                        {
                            predicates.Add($"Clear({world[x, y]})");
                        }
                    }
                }
            }
            predicates.Add("ArmEmpty()");
            return predicates;
        }

        static public List<string> STRIPS(List<string> world, List<string> goal, List<char> blocks)
        {
            List<string> plan = new List<string>();
            Stack<string> goals = new Stack<string>(goal);

            #region ENUMERATE ALL ACTIONS
            Dictionary<string, Action> operators = new Dictionary<string, Action>();
            foreach (var block in blocks)
            {
                Action action = Pickup(block);
                operators.Add(action.name, action);
                action = Putdown(block);
                operators.Add(action.name, action);
            }
            for (int i = 0; i < blocks.Count; i++)
            {
                for (int j = 0; j < blocks.Count; j++)
                {
                    if (i != j)
                    {
                        Action action = Stack(blocks[i], blocks[j]);
                        operators.Add(action.name, action);
                        action = Unstack(blocks[i], blocks[j]);
                        operators.Add(action.name, action);
                    }
                }
            }
            #endregion
            while (goals.Count > 0)
            {
                string g = goals.Peek();
                if (operators.ContainsKey(g))
                {
                    bool valid = true;
                    operators[g].pre.FindAll(x => world.Contains(x) != true).ForEach(x =>
                       {
                           valid = false;
                           goals.Push(x);
                       });
                    if (valid)
                    {
                        plan.Add(g);
                        foreach (var predicate in operators[g].del)
                        {
                            world.Remove(predicate);
                        }
                        foreach (var predicate in operators[g].add)
                        {
                            world.Add(predicate);
                        }
                        goals.Pop();
                    }
                }
                else if (world.Contains(g))
                {
                    goals.Pop();
                }
                else
                {
                    List<Tuple<string, int>> choices = new List<Tuple<string, int>>();

                    foreach (var item in operators)
                    {

                        if (item.Value.add.Contains(g))
                        {
                            int score = 0; // lower is better;
                            foreach (var condition in item.Value.del)
                            {
                                if (goal.Contains(condition))
                                {
                                    score += 1; //BAD, we want to keep this
                                }
                                plan.FindAll((x) => operators[x].add.Contains(x)).ForEach(x => score += 5);
                            }
                            foreach (var condition in item.Value.add)
                            {
                                /*if (goal.Contains(condition))
                                {
                                    score -= 1; //GOOD, we want to get this
                                }*/
                                if (goals.Contains(condition))
                                {
                                    score -= 1; //Good we actively want this
                                }
                                if (!goal.Contains(condition)) //This isn't part of the end goal
                                {
                                    score += 1;
                                }
                            }
                            foreach (var precondition in item.Value.pre)
                            {
                                if (world.Contains(precondition))
                                {
                                    score -= 1; //GOOD its easier for us to do
                                }
                                else
                                {
                                    score += 3; //BAD, more work
                                }
                            }
                            if (goals.Contains(item.Key))
                            {
                                //score += 5;
                            }
                            choices.Add(new Tuple<string, int>(item.Key, score));
                        }
                    }
                    choices = choices.OrderBy((x) => x.Item2).ToList();
                    string choice = choices[0].Item1;
                    goals.Pop();
                    goals.Push(choice);
                    foreach (var condition in operators[choice].pre)
                    {
                        goals.Push(condition);
                    }
                }
            }
            return plan;
        }
    }
}
