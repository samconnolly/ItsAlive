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
        public bool assistant;
        public List<float> attributes;
        public List<List<Tuple<NumericalCounter, int>>> multipliers;
        public int time;
        public bool remain;
        public bool done = false;
        public bool on = false;
        public bool off = false;
        private float multiplier;
        public int count;

        public MenuAction(string name, bool scientistJob,bool assistantJob, int timeTaken, 
                            float researchUp, List<Tuple<NumericalCounter,int>> researchMultiplier,
                            float madnessUp, List<Tuple<NumericalCounter,int>> madnessMultiplier,
                            float moneyChange, List<Tuple<NumericalCounter,int>> moneyMultiplier,
                            float lifeForceUp, List<Tuple<NumericalCounter,int>> lifeforceupMultiplier,
                            float longevityUp, List<Tuple<NumericalCounter,int>> longevityMultiplier,
                            float humanityUp, List<Tuple<NumericalCounter,int>> humanityMultiplier,
                            bool remain, bool turnOn = false, bool turnOff = false)
        {
            this.name = name;
            this.attributes = new List<float> { moneyChange, madnessUp, researchUp, lifeForceUp, longevityUp, humanityUp };
                     
            this.multipliers = new List<List<Tuple<NumericalCounter, int>>>{ moneyMultiplier,madnessMultiplier,researchMultiplier,
                                            lifeforceupMultiplier,longevityMultiplier,humanityMultiplier};
            this.time = timeTaken;
            this.remain = remain;
            this.scientist = scientistJob;
            this.assistant = assistantJob;
            this.on = turnOn;
            this.off = turnOff;
        
        }

        

        public void Complete(NumericalCounter researchCounter, NumericalCounter madnessCounter, NumericalCounter moneyCounter, NumericalCounter longevityCounter, 
                                NumericalCounter lifeforceCounter, NumericalCounter humanityCounter, FloorObject machine)
        {
            count += 1;

            List<NumericalCounter> counters = new List<NumericalCounter>{moneyCounter, madnessCounter, researchCounter, 
                                                                             lifeforceCounter,longevityCounter, humanityCounter};

            for (int x = 0; x < attributes.Count; x++)
            {
                if (multipliers[x][0].Item1 == null)
                {
                    multiplier = 1;
                }
 

                else
                {
                    multiplier = (float)Math.Pow(multipliers[x][0].Item1.value, multipliers[x][0].Item2);

                    if (multipliers[x][1].Item1 != null)
                    {
                        multiplier += (float)Math.Pow(multipliers[x][1].Item1.value, multipliers[x][1].Item2);
                    }

                }

                counters[x].value += attributes[x] * multiplier;

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
