﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jaylosy.Kernel.Task
{
    public interface ITaskExt
    {
        System.Threading.Tasks.Task Run(Action action);
        Task<T> Run<T>(Func<T> func);
    }
}
