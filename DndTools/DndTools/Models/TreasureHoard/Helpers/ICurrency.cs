using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DndTools.Models.TreasureHoard.Helpers
{
    public interface ICurrency
    {
        public int Value { get; set; }

        public abstract float ConvertToGpValue();
    }
}
