using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppParallelDemo
{
    class ProgramDemo1
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var inputData = new List<TestInputData>();
            var loopResult = Parallel.ForEach<TestInputData>(inputData, (data, loopStatus) => {
                if (loopStatus.ShouldExitCurrentIteration)
                {
                    return;
                }

                if (sw.Elapsed.Seconds > 3)
                {
                    loopStatus.Break();
                    return;
                }
            });
        }

        class TestInputData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var sw = Stopwatch.StartNew();
            var inputData = new List<TestInputData>();
            try
            {
                var loopResult = Parallel.ForEach<TestInputData>(inputData, (data, loopStatus) => {
                    if (loopStatus.ShouldExitCurrentIteration)
                    {
                        return;
                    }

                    if (sw.Elapsed.Seconds > 3)
                    {
                        throw new TimeoutException("Parallel.ForEach istaking more than 3 seconds to complate.");
                    }
                });
            }
            catch(AggregateException ex)
            {
                foreach(Exception innerEx in ex.InnerExceptions)
                {
                    Debug.WriteLine(innerEx.ToString());
                }
            }

        }

        class TestInputData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
