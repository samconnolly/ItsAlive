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
        private Func<double, double> func;

        // should eventually have some kind of dependence function on the dependent param (or more than one!) - delta function??
        public MachineDependentParameter(string name, int min, int max, int initial, MachineControlParameter dependent, Func<double, double> function, int appearanceType)
        {
            this.name = name;
            this.max = max;
            this.min = min;
            this.value = initial;
            this.dependent = dependent;
            this.type = appearanceType;
            this.func = function;
        }

        public void Update()
        {
            value = (float) func(dependent.value);
        }

    }
}
