﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleAppParallelDemo
{
    #region Parallel.ForEach 并行计算 
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
    #endregion
    #region 并行计算，超过3秒抛出异常
    class ProgramDemo2
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
    #endregion
    #region 并行计算，限制最大并行度
    class Program3
    {
        static void Main(string[] args)
        {
            var parallelOptions = new ParallelOptions();
            // Environment.ProcessorCount; // 逻辑内核的数量
            parallelOptions.MaxDegreeOfParallelism = 6; // 最大并行度
            var sw = Stopwatch.StartNew();
            var inputData = new List<TestInputData>();
            try
            {
                var loopResult = Parallel.ForEach<TestInputData>(inputData, parallelOptions, (data, loopStatus) => {
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
            catch (AggregateException ex)
            {
                foreach (Exception innerEx in ex.InnerExceptions)
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
    #endregion
    #region Task 并行计算
    class Program4
    {
        static void Main(string[] args)
        {
            #region Parallel.Invoke 不考虑任务结束时间(极简方式)
            Parallel.Invoke(
                () => { },
                () => { },
                () => { }
            );
            #endregion
            #region Task.WaitAll 死等所有任务结束
            var t1 = new Task(() => {
                ;
            });
            var t2 = new Task(() => {
                ;
            });
            t1.Start();
            t2.Start();
            Task.WaitAll(t1, t2);
            #endregion

            #region Task.WaitAll 指定最长等待时间
            var t3 = new Task(() => {
                ;
            });
            var t4 = new Task(() => {
                ;
            });
            t3.Start();
            t4.Start();
            if (!Task.WaitAll(new [] { t1, t2 }, 3000))
            {
                ;// 超时处理
            }
            #endregion
        }
    }
    #endregion
    #region 并行计算，限制最大并行度
    class Program5
    {
        static void Main(string[] args)
        {
            var parallelOptions = new ParallelOptions();
            // Environment.ProcessorCount; // 逻辑内核的数量
            parallelOptions.MaxDegreeOfParallelism = 6; // 最大并行度
            var sw = Stopwatch.StartNew();
            var inputData = new List<TestInputData>();
            try
            {
                var loopResult = Parallel.ForEach<TestInputData>(inputData, parallelOptions, (data, loopStatus) => {
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
            catch (AggregateException ex)
            {
                foreach (Exception innerEx in ex.InnerExceptions)
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
    #endregion
    #region Task 通过取消标记取消任务
    class Program6
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var t1 = Task.Factory.StartNew(() =>
                Func1(ct),
                ct
            );
            var t2 = Task.Factory.StartNew(() =>
                Func2(ct),
                ct
            );
            Thread.Sleep(1000);
            cts.Cancel();
            try
            {
                if (!Task.WaitAll(new[] { t1, t2 }, 3000))
                {
                    ;// 超时处理
                }
            }
            catch (AggregateException ex)
            {
                foreach (Exception innerEx in ex.InnerExceptions)
                {
                    ;
                }
                if (t1.IsCanceled)
                {
                    ;
                }
                if (t2.IsCanceled)
                {
                    ;
                }
            }
            catch(TimeoutException te)
            {
                //
            }
            catch(Exception ex)
            {
                //
            }
        }
       
        // 业务处理函数1
        static void Func1(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            for (var i = 0; i < 1000; i++)
            {
                ; // Do something.
                ct.ThrowIfCancellationRequested();
            }
        }
        // 业务处理函数2
        static void Func2(CancellationToken ct) 
        {
            var sw = Stopwatch.StartNew();
            ct.ThrowIfCancellationRequested();
            for (var i = 0; i < 100000; i++)
            {
                ; // Do something.
                ct.ThrowIfCancellationRequested();
            }
            if (sw.Elapsed.TotalSeconds > 0.5)
            {
                throw new TimeoutException("Timeout.");
            }
        }
    }
    #endregion
    #region 从任务 Task 返回值
    class Program7
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var t1 = Task.Factory.StartNew(() =>
                Func3(ct),
                ct
            );
            try
            {
                t1.Wait();
            }
            catch (AggregateException ex)
            {
                foreach (Exception innerEx in ex.InnerExceptions)
                {
                    ;
                }
                if (t1.IsCanceled)
                {
                    ;
                }
            }
            catch (Exception ex)
            {
                //
            }

            var t2 = Task.Factory.StartNew(() =>
                {
                    for(var i = 0; i < t1.Result; i++)
                    {
                        ct.ThrowIfCancellationRequested();
                    }
                },
                TaskCreationOptions.LongRunning // 任务的执行时间可能很长
            );
            Thread.Sleep(1000);
            cts.Cancel();

        }

        // 业务处理函数1
        static int Func3(CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();
            return 20;
        }
        // 业务处理函数2
        static void Func4(CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            ct.ThrowIfCancellationRequested();
            for (var i = 0; i < 100000; i++)
            {
                ; // Do something.
                ct.ThrowIfCancellationRequested();
            }
            if (sw.Elapsed.TotalSeconds > 0.5)
            {
                throw new TimeoutException("Timeout.");
            }
        }
    }
    #endregion
    #region 串联两个任务
    class Program8
    {
        static void Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            var ct = cts.Token;

            var t1 = Task.Factory.StartNew(() => { });
            var t2 = t1.ContinueWith((t) =>
            {
                ; // do something.
            });
            try
            {
                t2.Wait();
            }
            catch (Exception ex)
            {
                //
            }
        }
    }
    #endregion
    #region 并发集合 Concurrent
    class Program9
    {
        private static List<string> _keyList;
        private static void ParallelPartitionGenerateXXX()
        {
            var sw = Stopwatch.StartNew();
            lock (_keyList)
            {
                // 临界代码区，排他访问
            }
        }
        static void Main(string[] args)
        {
            _keyList = new List<string>();
            ParallelPartitionGenerateXXX();
        }
    }
    #endregion
    #region 使用并发队列 ConcurrentQueue
    class Program10
    {
        private static ConcurrentQueue<string> _keyQueue;
        private static void ParallelPartitionGenerateXXX()
        {
            var sw = Stopwatch.StartNew();
            Parallel.ForEach(new[] { "a", "b", "c" }, (c) => {
                _keyQueue.Enqueue(c);
            });
        }
        static void Main(string[] args)
        {
            _keyQueue = new ConcurrentQueue<string>();
            ;
            var tAsync = Task.Factory.StartNew(() => { ParallelPartitionGenerateXXX(); });
            string lastKey = string.Empty;
            while(tAsync.Status == TaskStatus.Running || tAsync.Status == TaskStatus.WaitingToRun)
            {
                if (_keyQueue.TryPeek(out lastKey))
                {
                    ;
                }
                else
                {
                    ; // No keys yet.
                }
            }
            tAsync.Wait();
        }
    }
    #endregion
    #region 实现并行的生产者-消费者模式 Procducer-consumer (单流水线，两阶段)
    class Program11
    {
        private static ConcurrentQueue<string> _ProcducerQueue; // 生产者队列
        private static ConcurrentQueue<string> _ConsumerQueue;  // 消费者队列
        // 生产者（多生产者）
        private static void ParallelPartitionProcducer(int maxDegree)
        {
            var sw = Stopwatch.StartNew();
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = maxDegree; // 指定最大并行度
            Parallel.ForEach(new[] { "a", "b", "c", "d", "e" },
                parallelOptions,
                (c) => {
                    _ProcducerQueue.Enqueue(c);
            });
            Debug.WriteLine($"ParallelPartitionProcducer::{sw.Elapsed.ToString()}");
        }
        // 消费者（单消费者）
        private static void ParallelPartitionConsumer(Task taskProducer)
        {
            var sw = Stopwatch.StartNew();

            while(taskProducer.Status == TaskStatus.Running 
                || taskProducer.Status == TaskStatus.WaitingToRun
                || _ProcducerQueue.Count > 0)
            {
                if (_ProcducerQueue.TryDequeue(out string result))
                {
                    var consumerItem = result; // 加工数据
                    _ConsumerQueue.Enqueue(consumerItem); // 存入消费者队列 
                }
            }
            Debug.WriteLine($"ParallelPartitionConsumer::{sw.Elapsed.ToString()}");
        }
        static void Main(string[] args)
        {
            var taskProducer = Task.Factory.StartNew(() => { // 创建并启动生产者任务
                ParallelPartitionProcducer(Environment.ProcessorCount - 1); // 最大逻辑处理器个数 Environment.ProcessorCount
            });
            var taskConsumer = Task.Factory.StartNew(() => { // 创建并启动消费者任务
                ParallelPartitionConsumer(taskProducer); 
            });
            string lastKey = string.Empty;
            while (taskConsumer.Status == TaskStatus.Running 
                || taskConsumer.Status == TaskStatus.WaitingToRun)
            {
                if (_ConsumerQueue.TryPeek(out lastKey))
                {
                    Console.WriteLine(lastKey);
                }
                else
                {
                    ; // No keys yet.
                }
            }
            Task.WaitAll(taskProducer, taskConsumer);
            Console.WriteLine("Finished.");
        }
    }
    #endregion
    #region 实现多重并行的生产者-消费者模式 Procducer-consumer (单流水线，两阶段) + Interlocked
    class Program12
    {
        private static ConcurrentQueue<string> _ProcducerQueue; // 生产者队列
        private static ConcurrentQueue<string> _ConsumerQueue;  // 消费者队列
        // 生产者（多生产者）
        private static void ParallelPartitionProcducer(int maxDegree)
        {
            var sw = Stopwatch.StartNew();
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = maxDegree; // 指定最大并行度
            Parallel.ForEach(new[] { "a", "b", "c", "d", "e" },
                parallelOptions,
                (c) => {
                    _ProcducerQueue.Enqueue(c);
                });
            Debug.WriteLine($"ParallelPartitionProcducer::{sw.Elapsed.ToString()}");
        }
        // 消费者（多消费者）
        static int taskRunning = 0; // 任务计数器
        private static void ParallelPartitionConsumer(Task taskProducer)
        {
            var sw = Stopwatch.StartNew();
            var maxTask = Environment.ProcessorCount / 2;
            var tasks = new Task[maxTask];
            for(var i = 0; i < maxTask; i++)
            {
                System.Threading.Interlocked.Increment(ref taskRunning); // 增加一个任务
                tasks[i] = Task.Factory.StartNew(() =>
                {
                    while (taskProducer.Status == TaskStatus.Running
                        || taskProducer.Status == TaskStatus.WaitingToRun
                        || _ProcducerQueue.Count > 0)
                    {
                        if (_ProcducerQueue.TryDequeue(out string result))
                        {
                            var consumerItem = result; // 加工数据
                            _ConsumerQueue.Enqueue(consumerItem); // 存入消费者队列 
                        }
                    }
                    System.Threading.Interlocked.Decrement(ref taskRunning); // 减少一个任务
                });
            }
            Task.WaitAll(tasks);
            Debug.WriteLine($"ParallelPartitionConsumer::{sw.Elapsed.ToString()}");
        }
        static void Main(string[] args)
        {
            var taskProducer = Task.Factory.StartNew(() => { // 创建并启动生产者任务
                ParallelPartitionProcducer(Environment.ProcessorCount - 1); // 最大逻辑处理器个数 Environment.ProcessorCount
            });
            var taskConsumer = Task.Factory.StartNew(() => { // 创建并启动消费者任务
                ParallelPartitionConsumer(taskProducer);
            });
            string lastKey = string.Empty;
            while (taskConsumer.Status == TaskStatus.Running
                || taskConsumer.Status == TaskStatus.WaitingToRun)
            {
                if (_ConsumerQueue.TryPeek(out lastKey))
                {
                    Console.WriteLine(lastKey);
                }
                else
                {
                    ; // No keys yet.
                }
            }
            Task.WaitAll(taskProducer, taskConsumer);
            Console.WriteLine("Finished.");
        }
    }
    #endregion
    #region 使用并发队列 ConcurrentStack
    class Program13
    {
        private static ConcurrentStack<string> _ProcducerStack; // 生产者堆栈
        private static ConcurrentStack<string> _ConsumerStack;  // 消费者堆栈
        // 生产者（多生产者）
        private static void ParallelPartitionProcducer(int maxDegree)
        {
            var sw = Stopwatch.StartNew();
            var parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = maxDegree; // 指定最大并行度
            Parallel.ForEach(new[] { "a", "b", "c", "d", "e" },
                parallelOptions,
                (c) => {
                    _ProcducerStack.Push(c);
                });
            Debug.WriteLine($"ParallelPartitionProcducer::{sw.Elapsed.ToString()}");
        }
        // 消费者（单消费者）
        private static void ParallelPartitionConsumer(Task taskProducer)
        {
            var sw = Stopwatch.StartNew();

            while (taskProducer.Status == TaskStatus.Running
                || taskProducer.Status == TaskStatus.WaitingToRun
                || !_ProcducerStack.IsEmpty)
            {
                if (_ProcducerStack.TryPop(out string result))
                {
                    var consumerItem = result; // 加工数据
                    _ConsumerStack.Push(consumerItem); // 存入消费者堆栈 
                }
            }
            Debug.WriteLine($"ParallelPartitionConsumer::{sw.Elapsed.ToString()}");
        }
        static void Main(string[] args)
        {
            var taskProducer = Task.Factory.StartNew(() => { // 创建并启动生产者任务
                ParallelPartitionProcducer(Environment.ProcessorCount - 1); // 最大逻辑处理器个数 Environment.ProcessorCount
            });
            var taskConsumer = Task.Factory.StartNew(() => { // 创建并启动消费者任务
                ParallelPartitionConsumer(taskProducer);
            });
            string lastKey = string.Empty;
            while (taskConsumer.Status == TaskStatus.Running
                || taskConsumer.Status == TaskStatus.WaitingToRun)
            {
                if (_ConsumerStack.TryPop(out lastKey))
                {
                    Console.WriteLine(lastKey);
                }
                else
                {
                    ; // No keys yet.
                }
            }
            Task.WaitAll(taskProducer, taskConsumer);
            Console.WriteLine("Finished.");
        }
    }
    #endregion
    #region 使用并发堆栈

    #endregion
    #region 将数组和不安全的集合转换为并发集合

    #endregion
    #region 使用并发的无序集合（Bag）

    #endregion
    #region 理解 IProducerConsumerCollection接口

    #endregion
    #region 理解阻塞（blocking）并发集合所提供的的限界（bounding）和阻塞能力

    #endregion
    #region 取消并发集合上的操作

    #endregion
    #region 通过很多 BlockingCollection 实例实现过滤流水线

    #endregion
}
