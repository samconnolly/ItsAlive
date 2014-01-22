using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace It_sAlive_
{
    class MachineDependentParameter
    {
        public string name;
        public int max;
        public int min;
        public float value;
        public MachineControlParameter dependent;
        public int type;

        // should eventually have some kind of dependence function on the dependent param (or more than one!) - delta function??
        public MachineDependentParameter(string name, int min, int max, int initial, MachineControlParameter dependent, int appearanceType)
        {
            this.name = name;
            this.max = max;
            this.min = min;
            this.value = initial;
            this.dependent = dependent;
            this.type = appearanceType;
        }

        public void Update()
        {
            value = dependent.value;
        }

    }
}
