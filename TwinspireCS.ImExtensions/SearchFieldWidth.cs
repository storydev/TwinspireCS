using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwinspireCS.ImExtensions
{
    public class SearchFieldWidth : Attribute
    {

        public float Width;

        public SearchFieldWidth(float width)
        {
            Width = width;
        }

    }
}
