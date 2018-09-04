using Base;
using Data;
using System;
using System.Threading;


namespace App
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine($"error : process start need outer parameter");
                Console.ReadKey();
                return;
            }

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            int bigAreaId = 0;
            try
            {
                bigAreaId = int.Parse(args[0]);
            }
            catch
            {
                Console.WriteLine($"error : process parameter {args[0]}.");
                return;
            }

            int curAppid = 0;
            try
            {
                curAppid = int.Parse(args[1]);
            }
            catch
            {
                Console.WriteLine($"error : process parameter {args[1]}.");
                return;
            }

            //bool isAdd = false;
            //try
            //{
            //    isAdd = int.Parse(args[2]) == 0 ? false : true;
            //}
            //catch
            //{
            //    Console.WriteLine($"error : process parameter {args[2]}.");
            //    return;
            //}
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);
            Game.Instance.Init(curAppid, bigAreaId);
            Console.Title = $"{Game.Instance.AppType.ToString()}({curAppid})";

            Console.WriteLine($"main thread id {Thread.CurrentThread.ManagedThreadId}.");

            while (Game.Instance.State != GameState.End)
            {
                try
                {
                    Thread.Sleep(1);
                    Game.Instance.Update();
                    OneThreadSynchronizationContext.Instance.Update();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            Console.ReadLine();
        }
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is System.Exception)
            {
                Exception ex = (System.Exception)e.ExceptionObject;
                Console.WriteLine(ex.ToString());
            }
        }
    }
}