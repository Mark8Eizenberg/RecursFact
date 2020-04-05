using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading.Tasks;

namespace FactorParallel
{
    internal class Program
    {
        private static int numThreadGlobal = 1;
        private static int factNumGlobal;
        private static readonly object thisLock = new object();
        private static BigInteger data;

        private static void Main(string[] args)
        {

            Input_factorial();
            Input_number_thread();
            BigInteger[,] interForThreads = new BigInteger[numThreadGlobal, 2];//Array for dividing task
            Stopwatch sw = new Stopwatch(); // initialize stopwatch
            sw.Start();//start stopwatch
            interForThreads = Count_Of_Thread(numThreadGlobal, factNumGlobal);//divide task
            data = Fact_Threading_Tree(interForThreads);
            sw.Stop();//end stopwatch
            Console.WriteLine("Solution for factorial: " + data);
            Console.WriteLine("Calculation time: " + sw.ElapsedMilliseconds / 1000 + " s " + sw.ElapsedMilliseconds % 1000 + " ms ");
            Console.ReadKey();
        }

        private static void Input_factorial()//Method for check input and catch exeptions
        {
            Console.WriteLine("Enter factorial and press Enter:");
            string input_count_fact = Console.ReadLine();
            try
            {
                int.TryParse(input_count_fact, out factNumGlobal);
            }
            catch (InvalidCastException e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }
        }

        private static void Input_number_thread()//Method for check input and catch exeptions
        {
            Console.WriteLine("Enter count of thread and press Enter: ");
            string input_count_thread = Console.ReadLine();
            try
            {
                int.TryParse(input_count_thread, out numThreadGlobal);
                if((numThreadGlobal == 0) || (numThreadGlobal < 0))
                {
                    Console.WriteLine("Wrong input, programm will close!");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            catch (InvalidCastException e)
            {
                if (e.Source != null)
                {
                    Console.WriteLine("IOException source: {0}", e.Source);
                }
            }
        }


        private static BigInteger Factorial(BigInteger minRang, BigInteger maxRang)//Method for calculate recursive factorial in interval [minRang,maxRang]
        {
            if ((minRang == maxRang) || (minRang > maxRang))
            {
                return minRang;
            }
            else
            {
                return maxRang * Factorial(minRang, maxRang - 1);
            }
        }

        private static BigInteger[,] Count_Of_Thread(int numThreadGlobal, int factNum)//Method for divide task
        {
            BigInteger[,] retData = new BigInteger[numThreadGlobal, 2];
            int tempFact = 0, adderForData = factNum / numThreadGlobal;
            for (int i = 0; i < numThreadGlobal; i++)
            {
                if ((tempFact + adderForData <= factNum) && (i == 0))
                {
                    retData[i, 0] = 1;
                    retData[i, 1] = tempFact + adderForData;
                }
                else if (tempFact + adderForData < factNum)
                {
                    retData[i, 0] = tempFact;
                    retData[i, 1] = tempFact + adderForData;
                }
                else if (tempFact > factNum)
                {
                    retData[i, 0] = 1;
                    retData[i, 1] = 1;
                }
                else
                {
                    retData[i, 0] = tempFact;
                    retData[i, 1] = factNum;
                }
                tempFact += adderForData + 1;
            }
            return retData;
        }
        private static Task<BigInteger> Async_Prod_Tree(BigInteger l, BigInteger r)//Delegates for create and run tasks
        { return Task<BigInteger>.Run(() => Factorial(l, r)); }

        private static BigInteger Fact_Threading_Tree(BigInteger[,] packetForCalculating)//Method for calculate factorial with async tasks
        {
            Task<BigInteger>[] tasks = new Task<BigInteger>[numThreadGlobal];
            int path = factNumGlobal / numThreadGlobal;
            for (int i = 0; i < numThreadGlobal; i++)//Starting async tasks
            {
                tasks[i] = Async_Prod_Tree(packetForCalculating[i, 0], packetForCalculating[i, 1]);
            }

            Task<BigInteger>.WaitAll(tasks);//Waiting for ending tasks
            BigInteger result = 1;
            for (int i = 0; i < numThreadGlobal; i++)
            {
                result *= tasks[i].Result;//end result
            }
            return result;
        }

    }
}
