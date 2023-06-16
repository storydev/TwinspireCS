using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS
{
    public interface IGameState
    {

        object GetData();
        void SaveData(string initialPath);
        void LoadData(string initialPath, int fileIndex);

    }
}
