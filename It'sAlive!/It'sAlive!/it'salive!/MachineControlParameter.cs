using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace It_sAlive_
{
    class MachineControlParameter
    {
        public string name;
        public int max;
        public int min;
        public float value;
        public int type;

        public MachineControlParameter(string name, int max, int min,int initial, int appearanceType)
        {
            this.name = name;
            this.max = max;
            this.min = min;
            this.value = initial;
            this.type = appearanceType;
        }

    }
}
