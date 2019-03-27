using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jaylosy.Kernel.Task
{
    public class TaskExt 
    {
        public static System.Threading.Tasks.Task Run(Action action)
        {
            var tcs = new TaskCompletionSource<object>();
            new Thread(() =>
            {
                action();
                tcs.SetResult(null);
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }

        public static Task<T> Run<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            new Thread(() =>
            {
                var result = func();
                tcs.SetResult(result);
            })
            { IsBackground = true }.Start();
            return tcs.Task;
        }
    }
}
