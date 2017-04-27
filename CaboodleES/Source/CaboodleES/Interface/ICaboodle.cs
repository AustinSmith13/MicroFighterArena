using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CaboodleES
{
    public interface ICaboodle
    {
        Manager.EntityManager GetEntityManager();
        Manager.SystemManager GetSystemsManager();
        void Union(ICaboodle caboodle);
        void Clear();
    }
}
