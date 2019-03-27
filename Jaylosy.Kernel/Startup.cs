using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jaylosy.Kernel
{
    public abstract class MainStartup : IStartup
    {
        public abstract void ExecuteCmd(ICommand command, params string[] args);

        public virtual void Run(params string[] args)
        {

        }
    }

    public interface IStartup
    {
        /// <summary>
        /// 程序运行
        /// </summary>
        /// <param name="args"></param>
        void Run(params string[] args);
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        void ExecuteCmd(ICommand command, params string[] args);
    }

    public interface ICommand
    {
    }
    
}
