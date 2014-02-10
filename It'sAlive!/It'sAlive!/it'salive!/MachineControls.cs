using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace It_sAlive_
{
    // This'll be totally great! A dropdown menu from above with machine-dependent controls! Lovely!

    class MachineControls
    {
        private Vector2 position = Vector2.Zero;

        private Tuple<List<Knob>,List<Slider>,List<Gauge>,List<SlideGauge>> controls;

        private bool comingDown = false;
        private bool goingUp = false;
        private bool down = false;

        private Texture2D knobTex;
        private Texture2D gaugeTex;
        private Texture2D sliderTex;
        private Texture2D slideGaugeTex;
        private Texture2D gaugeHandTex;
        private Texture2D sliderKnobTex;
        private Texture2D slideGaugeHandTex;

        private int rounds = 0;
        private int rectangles = 0;

        private float drop = 0;
        private float totalDrop = 0;
        private int fall = 380;
        private Vector2 offScreenPos = new Vector2(320, -300);

        private List<Vector2> roundPositions = new List<Vector2> { };
        private List<Vector2> rectanglePositions = new List<Vector2> { };

        public MachineControls(Texture2D knobTex, Texture2D gaugeTex, Texture2D gaugeHandTex, Texture2D sliderTex, Texture2D sliderKnobTex, Texture2D slideGaugeTex, Texture2D slideGaugeHandTex)
        {
            this.knobTex = knobTex;
            this.gaugeTex = gaugeTex;
            this.gaugeHandTex = gaugeHandTex;
            this.sliderTex = sliderTex;
            this.sliderKnobTex = sliderKnobTex;
            this.slideGaugeTex = slideGaugeTex;
            this.slideGaugeHandTex = slideGaugeHandTex;

        }

        // eventually link this to each floorobject's lists of controls!
        public void OpenMenu(List<MachineControlParameter> newControls, List<MachineDependentParameter> newDisplays)
        {
            // determine number of each type of control, work out some positions

            // count them...
            rounds = 0;
            rectangles = 0;         

            foreach (MachineControlParameter control in newControls)
            {
                if (control.type == 1) { rounds += 1; }
                if (control.type == 2) { rectangles += 1; }
            }

            foreach (MachineDependentParameter display in newDisplays)
            {
                if (display.type == 1) { rounds += 1; }
                if (display.type == 2) { rectangles += 1; }
            }

            // ...then position them...

            // rounds.. (next to one another)
            Vector2 thisPos = offScreenPos;

            for (int i = 0; i < rounds; i++)
            {
                roundPositions.Add(thisPos);
                thisPos += new Vector2(250, 0);
            }

            // rectangles.. (sets of two above one another)
            int stack = 0;

            for (int i = 0; i < rectangles; i++)
            {
                rectanglePositions.Add(thisPos);

                if (stack == 0)
                {
                    thisPos += new Vector2(0, 150);
                    stack = 1;
                }

                else if (stack == 1)
                {
                    thisPos += new Vector2(250, -150);
                    stack = 0;
                }
            }

            // fill control arrays
            controls = new System.Tuple<System.Collections.Generic.List<Knob>,System.Collections.Generic.List<Slider>,System.Collections.Generic.List<Gauge>,System.Collections.Generic.List<SlideGauge>>(
                            new List<Knob>{},new List<Slider>{},new List<Gauge>{},new List<SlideGauge>{});

            int roundPos = 0;
            int rectanglePos = 0;

            foreach (MachineControlParameter control in newControls)
            {
                if (control.type == 1)
                {
                    controls.Item1.Add(new Knob(roundPositions[roundPos],0.3f,knobTex,control));
                    roundPos += 1;
                }

                else if (control.type == 2)
                {
                    controls.Item2.Add(new Slider(rectanglePositions[rectanglePos], 0.3f, sliderTex, sliderKnobTex,control, 0.9f));
                    rectanglePos += 1;
                }
            }

            foreach (MachineDependentParameter display in newDisplays)
            {
                if (display.type == 1)
                {
                    controls.Item3.Add(new Gauge(roundPositions[roundPos], 0.3f, gaugeTex, gaugeHandTex, display, true, 100));
                    roundPos += 1;
                }

                else if (display.type == 2)
                {
                    controls.Item4.Add(new SlideGauge(rectanglePositions[rectanglePos], 0.3f, slideGaugeTex, slideGaugeHandTex, display, 0.9f, true, 100));
                    rectanglePos += 1;
                }
            }

            comingDown = true;

            //down = true;
        }

        public void CloseMenu()
        {
            down = false;
            goingUp = true;
        }

        public void Update(Cursor cursor, Random random, GameTime gameTime)
        {
            // update all the positions if coming down....

            if (comingDown == true)
            {
                drop = gameTime.ElapsedGameTime.Milliseconds / 1.0f;

                if (drop + totalDrop > fall)
                {
                    drop = fall - totalDrop;
                }

                totalDrop += drop;
                
                foreach (Knob knob in controls.Item1)
                {
                    knob.position.Y += drop;
                }
                foreach (Slider slider in controls.Item2)
                {
                    slider.position.Y += drop;
                    slider.knobPosition.Y += drop;
                }
                foreach (Gauge gauge in controls.Item3)
                {
                    gauge.position.Y += drop;
                    gauge.handPosition.Y += drop;
                }
                foreach (SlideGauge slideGauge in controls.Item4)
                {
                    slideGauge.position.Y += drop;
                    slideGauge.handPosition.Y += drop;
                }


                if (totalDrop >= fall)
                {
                    totalDrop = 0;
                    drop = 0;
                    comingDown = false;
                    down = true;
                }
            }

            if (goingUp == true)
            {
                drop = gameTime.ElapsedGameTime.Milliseconds / 1.0f;

                if (drop + totalDrop > fall)
                {
                    drop = fall - totalDrop;
                }

                totalDrop += drop;

                foreach (Knob knob in controls.Item1)
                {
                    knob.position.Y -= drop;
                }
                foreach (Slider slider in controls.Item2)
                {
                    slider.position.Y -= drop;
                    slider.knobPosition.Y -= drop;
                }
                foreach (Gauge gauge in controls.Item3)
                {
                    gauge.position.Y -= drop;
                    gauge.handPosition.Y -= drop;
                }
                foreach (SlideGauge slideGauge in controls.Item4)
                {
                    slideGauge.position.Y -= drop;
                    slideGauge.handPosition.Y -= drop;
                }


                if (totalDrop >= fall)
                {
                    totalDrop = 0;
                    drop = 0;
                    goingUp = false;

                }
            }

            if (down == true || comingDown == true || goingUp == true)
            {
                foreach (Knob knob in controls.Item1)
                {
                    knob.Update(cursor);
                }
                foreach (Slider slider in controls.Item2)
                {
                    slider.Update(cursor);
                }
                foreach (Gauge gauge in controls.Item3)
                {
                    gauge.Update(random,gameTime);
                }
                foreach (SlideGauge slideGauge in controls.Item4)
                {
                    slideGauge.Update(random,gameTime);
                }
            }

        }

        public void Render(SpriteBatch spriteBatch, SpriteFont font)
        {
            if (down == true || comingDown == true || goingUp == true)
            {
                foreach (Knob knob in controls.Item1)
                {
                    knob.Render(spriteBatch, font);
                }
                foreach (Slider slider in controls.Item2)
                {
                    slider.Render(spriteBatch, font);
                }
                foreach (Gauge gauge in controls.Item3)
                {
                    gauge.Render(spriteBatch, font);
                }
                foreach (SlideGauge slideGauge in controls.Item4)
                {
                    slideGauge.Render(spriteBatch, font);
                }
            }

        }
    }
}
