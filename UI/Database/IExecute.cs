using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UI.Database
{
    public interface IExecute<T>
    {
        T Execute();
    }

   

    public interface IExecute<T,P>
    {
        T Execute(P param);
    }

    public interface IExecute
    {
        void Execute();
    }
}
