using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PracticMethodPothential
{
    class cell
    {
        public const int M = 1000000;
        public int c;
        public char sign; //-, + 
        public bool isUsed;
        public int d;
        public int x;
        public int v;
        public int u;
        public int i;
        public int j;
        public bool isBased;
        public bool isCross; // для побудови циклу
        public cell(int _i, int _j, int cost, int x_, int v_, int u_, int d_)
        {
            i = _i;
            j = _j;
            isCross = false;
            c = cost;
            x = x_;
            v = v_;
            u = u_;
            d = d_;
            sign = '-';
            isUsed = false;
            if (x != M)
                isBased = true;
        }
        public cell()
        {
            isBased = false;
            isCross = false;
            c = 0;
            x = 0;
            v = 0;
            u = 0;
            d = 0;
            sign = '-';
            isUsed = false;
        }
    }

    class Program
    {
        static int m = 0, n = 0, res = 0;
        static List<int> a, b, u, v;
        static int[,] cost;
        static cell[,] table;
        static int M = 1000000;
        static cell begin_cycle;
        static FileStream solution = new FileStream("solution3.txt", FileMode.Open);
        static StreamWriter writer = new StreamWriter(solution);
        static FileStream task3 = new FileStream("task3.txt", FileMode.Open);
        static StreamReader reader = new StreamReader(task3);

        static void DatePreparation()
        {
            m = int.Parse(reader.ReadLine()); // кількість рядків, що відповідають виробникам    a
            n = int.Parse(reader.ReadLine()); // кількість стовпців, що відповідають споживачам  b
            a = new List<int>();
            b = new List<int>();
           
            int sum1 = 0, sum2 = 0;
            List<string> CostList = new List<string>();

            // обробка масиву а
            string str = reader.ReadLine();
            foreach (string s in str.Split(' '))
                a.Add(int.Parse(s));
            foreach (int i in a)
                sum1 += i;

            // обробка масиву b
            str = reader.ReadLine();
            foreach (string s in str.Split(' '))
                b.Add(int.Parse(s));
            foreach (int j in b)
                sum2 += j;

            for (int i = 0; i < m; ++i)
                CostList.Add(reader.ReadLine());
            reader.Close();

            if (sum1 != sum2)
            {
                if (sum1 < sum2)
                    a.Add(sum2 - sum1);
                else
                    b.Add(sum1 - sum2);
            }

            cost = new int[a.Count, b.Count];
            string[] tmp = new string[n];

            for (int i = 0; i < m; ++i)
            {
                tmp = CostList[i].Split(' ');
                for (int j = 0; j < n; ++j)
                    cost[i, j] = int.Parse(tmp[j]);
            }
            m = a.Count;
            n = b.Count;

            table = new cell[m, n];

            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                    table[i, j] = new cell(i, j, cost[i, j], M, M, M, M);
        }

        static void Nord_West_Angle()
        {
            int i;
            int count = 0;
            for (int j = 0; j < n; ++j)
            {
                i = 0;
                while (b[j] != 0 && i < m)
                {
                    if (a[i] != 0)
                    {
                        if (a[i] == b[j])
                        {
                            table[i, j].x = a[i];
                            b[j] = 0;
                            a[i] = 0;
                            ++count;
                        }
                        else if (a[i] < b[j])
                        {
                            table[i, j].x = a[i];
                            b[j] -= a[i];
                            a[i] = 0;
                            ++count;
                        }
                        else
                        {
                            table[i, j].x = b[j];
                            a[i] -= b[j];
                            b[j] = 0;
                            ++count;
                        }
                    }
                    ++i;
                }

                //закінчити цю частину
                while (count < (m + n - 1))     
                    count++;
            }
        }

        static void MinElement()
        {
            List<cell> arr = new List<cell>();

            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                    arr.Add(table[i, j]);

            arr = arr.OrderBy(e => e.c).ToList();

            int tmp_cost = 0, count = 0;

            for (int k = 0; k < m * n; ++k)
            {
                tmp_cost = Math.Min(a[arr[k].i], b[arr[k].j]);
                if (tmp_cost != 0)
                {
                    count++;
                    table[arr[k].i, arr[k].j].x = tmp_cost;
                    a[arr[k].i] -= tmp_cost;
                    b[arr[k].j] -= tmp_cost;
                }
            }
            while (count < (m + n - 1))      //дописати
            {
                for (int k = 0; k < m * n; ++k)
                {
                    if (table[arr[k].i, arr[k].j].x != M)
                    {
                        count++;
                        table[arr[k].i, arr[k].j].x = tmp_cost;
                        k = m * n;
                    }
                }
            }
        }

        static void ResetTable()
        {
            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                {
                    table[i, j].isBased = false;
                    table[i, j].isCross = false;
                    table[i, j].isUsed = false;
                    table[i, j].d = M;
                }
        }

        static void Fill_u_and_v()
        {
            Queue<int> ind_u = new Queue<int>();
            Queue<int> ind_v = new Queue<int>();
            int temp_index = 0,
                count_v = 0, count_u = 0;
            for (int k = 0; k < m; ++k)
                u[k] = M;
            for (int k = 0; k < n; ++k)
                v[k] = M;
            u[0] = 0;
            count_u++;
            ind_u.Enqueue(0);
            for (int i = 0; i < n; ++i)
            {
                if (table[0, i].x != M)
                {
                    v[i] = u[0] + cost[0, i];
                    count_v++;
                    ind_v.Enqueue(i);
                }
            }
            while (count_u < m || count_v < n)
            {
                while (ind_u.Count != 0)
                {
                    temp_index = ind_u.Dequeue();
                    for (int j = 0; j < n; ++j)
                    {
                        if (table[temp_index, j].x != M)
                        {
                            if (v[j] == M)
                            {
                                v[j] = cost[temp_index, j] + u[temp_index];
                                count_v++;
                                ind_v.Enqueue(j);
                            }
                        }
                    }
                }

                while (ind_v.Count != 0)
                {
                    temp_index = ind_v.Dequeue();
                    for (int i = 0; i < m; ++i)
                    {
                        if (table[i, temp_index].x != M)
                        {
                            if (u[i] == M)
                            {
                                u[i] = v[temp_index] - cost[i, temp_index];
                                count_u++;
                                ind_u.Enqueue(i);
                            }
                        }
                    }
                }
            }
            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                {
                    table[i, j].u = u[i];
                    table[i, j].v = v[j];
                }
        }

        static void SearchDelta(ref int temp_min, ref int temp_i, ref int temp_j)
        {
            int min_cost = M;
            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                {
                    table[i, j].d = cost[i, j] - (v[j] - u[i]);
                    if (table[i, j].d < temp_min)
                    {
                        temp_min = table[i, j].d;
                        temp_i = i; temp_j = j;
                        min_cost = table[i, j].c;
                    }
                    else if(table[i, j].d == temp_min && table[i, j].c < min_cost)
                    {
                        temp_min = table[i, j].d;
                        temp_i = i; temp_j = j;
                        min_cost = table[i, j].c;
                    }
                }
        }

        static void Iterations()
        {
            u = new List<int>();
            v = new List<int>();
            for (int k = 0; k < m; ++k)
                u.Add(M);
            for (int k = 0; k < n; ++k)
                v.Add(M);
            
            //шукаємо дельта
            int temp_min = - M, temp_i = 0, temp_j = 0;
            int count_ = 1;

            while (temp_min < 0)
            {
                writer.WriteLine();
                writer.WriteLine("***  {0}-а ітерація  ***", count_);
                temp_min = M;
                Fill_u_and_v();
                writer.Write("u: ");
                foreach (int e in u)
                    writer.Write("{0}  ", e);
                writer.WriteLine();
                writer.Write("v: ");
                foreach (int e in b)
                    writer.Write("{0}  ", e);
                writer.WriteLine();

                SearchDelta(ref temp_min, ref temp_i, ref temp_j);
                writer.WriteLine("Матриця відносних оцінок d: ");
                for (int i = 0; i < m; ++i)
                {
                    for (int j = 0; j < n; ++j)
                        writer.Write("{0}  ", table[i, j].d);
                    writer.WriteLine();
                }
               
                if (temp_min < 0)
                {
                    writer.WriteLine("Мінімальна відносна оцінка: {0}, ", temp_min);
                    writer.WriteLine("Координати клітинки, що вводиться до базисних: ({0}, {1})", temp_i, temp_j);
                    table[temp_i, temp_j].x = 0;
                    table[temp_i, temp_j].isBased = true;

                    BuildCycle(temp_i, temp_j);
                    writer.WriteLine();
                    writer.WriteLine("Новий базисний розв'язок: ");
                    FilePrintSolution();

                    ResetTable();
                }
                else
                {
                    writer.WriteLine("Усі симплекс-різниці невід'ємні");
                }
                ++count_;
            }
        }

        static void BuildCycle(int i0, int j0)
        {
            List<int> arr_v = new List<int>();
            List<int> arr_u = new List<int>();
            for (int k = 0; k < m; ++k)
                arr_u.Add(0);
            for (int k = 0; k < n; ++k)
                arr_v.Add(0);

            List<cell> before_cycle = new List<cell>();
            List<cell> cycle = new List<cell>();
            int min_value = M;   // те зачення, яке відніматимемо в циклі
            begin_cycle = new cell();
            
            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                    if (table[i, j].x != M)
                    {
                        arr_u[i] += 1;
                        arr_v[j] += 1;
                    }
            int counter = 0;
            while (counter < 3)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (arr_v[j] == 1)
                    {
                        for (int i = 0; i < m; ++i)
                        {
                            table[i, j].isCross = true;
                            if (table[i, j].x != M)
                                arr_u[i] -= 1;
                        }
                        arr_v[j] = 0;
                    }
                }
                for (int i = 0; i < m; ++i)
                {
                    if (arr_u[i] == 1)
                    {
                        for (int j = 0; j < n; ++j)
                        {
                            table[i, j].isCross = true;
                            if (table[i, j].x != M)
                                arr_v[j] -= 1;
                        }
                        arr_u[i] = 0;
                    }
                }
                counter++;
            }
            writer.WriteLine("Маска базисного розв'язку");
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (table[i, j].isCross == true)
                        writer.Write(" - ");
                    else if (table[i, j].x != M && table[i, j].isCross == false)
                    {
                        writer.Write(" * ");
                        before_cycle.Add(table[i, j]);
                    }
                    else
                        writer.Write(" M ");
                }
                writer.WriteLine();
            }
            

            begin_cycle = table[i0, j0];
            while (before_cycle.Count != 0)
            {
                for (int k = 0; k < before_cycle.Count; ++k)
                {
                    if (begin_cycle == before_cycle[k])
                    {
                        cycle.Add(begin_cycle);
                        before_cycle.RemoveAt(k);
                        for (int t = 0; t < before_cycle.Count; ++t)
                        {
                            if (before_cycle[t].i == begin_cycle.i || before_cycle[t].j == begin_cycle.j)
                            {
                                begin_cycle = before_cycle[t];
                                break;
                            }
                        }
                    }
                }
            }

            writer.WriteLine("Цикл: ");
            for (int k = 0; k < cycle.Count; ++k)
            {
                if (k % 2 == 0)
                    cycle[k].sign = '+';
                else
                {
                    if (min_value > cycle[k].x)
                        min_value = cycle[k].x;
                }
            }
            for (int k = 0; k < cycle.Count - 1; ++k)
            {
                if (k % 2 == 0)
                    writer.Write("'+'{0} -> ", cycle[k].x);
                else
                    writer.Write("'-'{0} -> ", cycle[k].x);
            }
            writer.WriteLine("'-'{0}", cycle[cycle.Count - 1].x);
            int count_unbased = 0;
            for (int k = 0; k < cycle.Count; ++k)
            {
                if (k % 2 == 0)
                    cycle[k].x += min_value;
                else
                {
                    cycle[k].x -= min_value;
                    if (cycle[k].x == 0 && count_unbased == 0)
                    {
                        cycle[k].x = M;
                        table[cycle[k].i, cycle[k].j].x = M;
                        table[cycle[k].i, cycle[k].j].isBased = false;
                        count_unbased++;
                    }
                }
            }
        }

        static void ResultCalculation()
        {
            for (int i = 0; i < m; ++i)
                for (int j = 0; j < n; ++j)
                    if (table[i, j].x != M)
                        res += (table[i, j].c * table[i, j].x);
        }

        static void FilePrintSolution()
        {
            string str = "";
            if (table != null)
            {
                for (int i = 0; i < m; ++i)
                {
                    for (int j = 0; j < n; ++j)
                    {
                        if (table[i, j].x != M)
                        {
                            str = table[i, j].x.ToString();
                            while(str.Length!=3)
                                str += ' ';
                                writer.Write(str);
                        }
                        else writer.Write("M  ");
                    }
                    writer.WriteLine();
                }
            }
        }

        static void Main()
        {
            Console.Title = "Method_Potential";
            writer.WriteLine("    Використані позначення ");
            writer.WriteLine("M - позначення небазисної клітинки");
            writer.WriteLine("'-' вказує на викреслену клітинку в масці при побудові циклу");
            writer.WriteLine("'*' вказує на клітинку, що входить в цикл");
            DatePreparation();
            writer.WriteLine("Початкові дані:");
            writer.Write("a: ( ");
            foreach (int e in a)
                writer.Write("{0}, ", e);
            writer.WriteLine(")");
            writer.Write("b: ( ");
            foreach (int e in b)
                writer.Write("{0}, ", e);
            writer.WriteLine(")");
            writer.WriteLine("Матриця транспортних вистра С:");
            for (int i = 0; i < m; ++i)
            {
                for (int j = 0; j < n; ++j)
                    writer.Write("{0}  ", table[i, j].c);
                writer.WriteLine();
            }

            MinElement();
            writer.WriteLine("Розв'язок, побудований методом мінімального елемента: ");
            FilePrintSolution();

            Iterations();
            ResultCalculation();
            writer.WriteLine();
            writer.WriteLine("Оптимальний розв'язок: ");
            FilePrintSolution();
            writer.WriteLine();
            writer.WriteLine("L(x) = {0} ", res);
            writer.Close();
            Console.ReadLine();
        }
    }
}
