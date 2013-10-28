using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace It_sAlive_
{
    class MenuAction
    {
        public string name;
        public bool scientist;
        public float money;
        public float madness;
        public float research;
        public float lifeForce;
        public float longevity;
        public float humanity;
        public float moneyM;
        public float madnessM;
        public float researchM;
        public float lifeForceM;
        public float longevityM;
        public float humanityM;
        public int time;
        public bool remain;
        public bool done = false;
        public bool on = false;
        public bool off = false;
        public List <NumericalCounter> dependent;
        private float multiplier;
        public int count;

        public MenuAction(string name, bool scientistJob, int timeTaken, float researchUp, float madnessUp, float moneyChange, float lifeForceUp, float longevityUp, float humanityUp, bool remain, bool turnOn = false, bool turnOff = false, List<NumericalCounter> dependentVariable = null,
                             float researchUpMultiplier = 0, float madnessUpMultiplier = 0, float moneyChangeMultiplier = 0, float lifeForceUpMultiplier = 0, float longevityUpMultiplier = 0, float humanityUpMultiplier = 0)
        {
            this.name = name;
            this.money = moneyChange;
            this.madness = madnessUp;
            this.research = researchUp;
            this.lifeForce = lifeForceUp;
            this.longevity = longevityUp;
            this.humanity = humanityUp;
            this.moneyM = moneyChangeMultiplier;
            this.madnessM = madnessUpMultiplier;
            this.researchM = researchUpMultiplier;
            this.lifeForceM = lifeForceUpMultiplier;
            this.longevityM = longevityUpMultiplier;
            this.humanityM = humanityUpMultiplier;
            this.time = timeTaken;
            this.remain = remain;
            this.dependent = dependentVariable;
            this.scientist = scientistJob;
            this.on = turnOn;
            this.off = turnOff;
        
        }

        

        public void Complete(NumericalCounter researchCounter, NumericalCounter madnessCounter, NumericalCounter moneyCounter, NumericalCounter longevityCounter, NumericalCounter lifeforceCounter, NumericalCounter humanityCounter, FloorObject machine)
        {
            count += 1;

                       
            
            researchCounter.value += research;
            madnessCounter.value += madness;
            moneyCounter.value += money;
            lifeforceCounter.value += lifeForce;
            humanityCounter.value += humanity;
            longevityCounter.value += longevity;
            

            if (dependent != null)
            {
                multiplier = 0;

                foreach (NumericalCounter counter in dependent)
                {
                    multiplier += counter.value;
                } 

                researchCounter.value += researchM * multiplier;
                madnessCounter.value += madnessM * multiplier;
                moneyCounter.value += moneyM * multiplier;
                lifeforceCounter.value += lifeForceM * multiplier;
                humanityCounter.value += humanityM * multiplier;
                longevityCounter.value += longevityM * multiplier;
            }

            if (on == true)
            {
                machine.on = true;
            }

            if (off == true)
            {
                machine.on = false;
            }
        }
    }
}
