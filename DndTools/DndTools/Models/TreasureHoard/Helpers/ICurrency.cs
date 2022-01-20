using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DndTools.Models.TreasureHoard.Helpers
{
    public interface ICurrency
    {
        public CurrencyTypeEnum Type { get; }

        public string TypeAsString { get => Type.ToString(); }

        public int Value { get; set; }

        public abstract float ConvertToGpValue();
    }
}
