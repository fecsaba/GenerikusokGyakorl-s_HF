using System;
using System.Collections.Generic;
using System.Linq;


namespace GenerikusokGyakorlás
{
    public class Win32
    {
        [System.Runtime.InteropServices.DllImport("KERNEL32")]
        public static extern bool QueryPerformanceCounter(ref ulong lpPerformanceCount);
        [System.Runtime.InteropServices.DllImport("KERNEL32")]
        public static extern bool QueryPerformanceFrequency(ref ulong lpFrequency);
    }
    class Program

        // Kíváncsiságból lemértem a futásidőket. A leírás szerint valami frekvenciaszámlálót kérdez le,
        // annak különbségét íratom ki. Hogy ez milyen egység és hogy jó-e, arról Lacit kifaggatjuk
        // Egyébként a mérési eredmények valóban a hagyományos eljárások alkalmazását indokolják
    {
        static ulong readTime()
        {
            ulong readedTime = 0;
            Win32.QueryPerformanceCounter(ref readedTime);
            return readedTime;

        }

        static void timeCount(string cim,ulong begin, ulong end)
        {
            ulong freq = 1;
            Win32.QueryPerformanceFrequency(ref freq);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{cim} Futásidő: " + (end - begin) + " egység");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine();
        }
        static void kiir(string cim, int[] rl)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(cim);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(String.Join(", ",rl));
            Console.WriteLine();
        }
        static int myInput(string szöveg, int min, int max)
        {
            Console.Write($"{szöveg}[{min}-{max}]: ");
            string inp = Console.ReadLine();
            int visszatér = 0;
            try
            {
                visszatér = int.Parse(inp);
            }
            catch (Exception)
            {
                throw new Exception("Rossz input!");
            }
            if (visszatér < min) throw new Exception("Túl kicsi!");
            if (visszatér > max) throw new Exception("Túl nagy!");
            return visszatér;
        }
        static void Main(string[] args)
        {
            // 1. adatok bekérése  
            //Kérjünk be egy min értéket 10-100
            
            //Kérjünk be egy max értéket 20-200
            
            //Kérjünk be a db értéket 5-80
           
            //Ha konveriós hiba, 
            //ha értéktartomány hiba,
            //ha min<=max, 
            //akkor jelezzük a hiba okát
            //és kérjük újra az adatokat

            bool rosszInput;
            int min = int.MinValue;
            int max = int.MinValue;
            int db = int.MinValue;
            do
            {
                rosszInput = false;
                try
                {
                    if (min == int.MinValue) min = myInput("Min=", 10, 100);
                    if (max == int.MinValue) max = myInput("Max=", 20, 200);
                    if (db == int.MinValue) db = myInput("db=", 5, 80);
                    if (min >= max)
                    {
                        min = int.MinValue;
                        max = int.MinValue;
                        throw new Exception("min>=max");
                    }
                    if (db > max - min)
                    {
                        db = int.MinValue;
                        throw new Exception("db > max-min");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    rosszInput = true;
                }
            } while (rosszInput);



            //2. Halmaz feltöltése "db" darab véletlen számokkal min-max között
            HashSet<int> h = new HashSet<int>();
            Random r = new Random();
            while (h.Count<db)
            {
                h.Add(r.Next(min, max + 1));
            }
            kiir("A halmaz", h.ToArray());
            //3. Másoljuk át a halamz elemeit egy listába
            ulong begin = readTime();
            List<int> lista = h.ToList();
            ulong end = readTime();
            timeCount("Másoljuk át a halamz elemeit egy listába generikusan", begin, end);

            //HF: a fent sor foreach és for ciklussal
            begin = readTime();
            int index = 0;
            foreach (int i in h)
            {
                lista[index] = i;
                index++;
                // Console.Write($"{i}, ");
            }
            

            end = readTime();
            timeCount("Másoljuk át a halamz elemeit egy listába foreach-el:", begin, end);
            kiir("A lista:", lista.ToArray());
            //Console.WriteLine();
            int[] a = new int[h.Count];
            a = h.ToArray();
            begin = readTime();
            for (int i = 0; i < h.Count; i++)
            {
                lista[i] = a[i];
            }
            end = readTime();
            timeCount("Másoljuk át a halamz elemeit egy listába for ciklussal:", begin, end);
            //4. Rendezzük a lista elemei csökkenő rendben
            begin = readTime();
            var rendezettLista = lista.OrderByDescending(x => x).ToList();
            end = readTime();
            timeCount("Rendezzük a lista elemei csökkenő rendben generikussal:", begin, end);
            //HF: a kííráshoz készítsünk saját metódust! 
            // Használd a Join() metódust! pl.: "34, 25, 13, 10" 
            // Írjad ki a rendezett lista elemeit!
            var s=0;
            begin = readTime();
            for (int i = 0; i < lista.Count-1; i++)
            {
                for (int j = i+1; j < lista.Count; j++)
                {
                    if (lista[i]>lista[j])
                    {
                        s = lista[i];
                        lista[i] = lista[j];
                        lista[j] = s;
                    }
                }
            }
            end = readTime();
            timeCount("Rendezzük a lista elemei csökkenő rendben buborékkal:", begin, end);
            kiir("A rendezett lista buborékkal", rendezettLista.ToArray());
            //5. Töröljük a listából a páros értékű elemeket
            //Linq:
            begin = readTime();
            var párosNélkül = rendezettLista.RemoveAll(x => x % 2 == 0).ToString();
            end = readTime();
            timeCount("Töröljük a listából a páros értékű elemeket generikussal:", begin, end);
            //HF normál (Linq-nélkül) megoldás, Remove() metódusal
            begin = readTime();
            for (int i = 0; i < rendezettLista.Count; i++)
            {
                if (rendezettLista[i] % 2 == 0)
                {
                    rendezettLista.RemoveAt(i);
                }
            }
            end = readTime();
            timeCount("Töröljük a listából a páros értékű elemeket for ciklussal:", begin, end);
            kiir("Páros nélküli rendezett lista",rendezettLista.ToArray());
            Console.ReadKey();
        }
    }
}
