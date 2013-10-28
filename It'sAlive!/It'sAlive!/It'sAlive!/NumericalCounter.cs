using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace It_sAlive_
{
    class NumericalCounter
    {
        public Vector2 position;
        public int max;
        public int init;

        public float value;
        public float valueChange;
        public float timeInterval;
        private float elapsed = 0;
        public string name;
        private SpriteFont font;
        private bool show;
        private bool visible;

        private Color gcolour;

        private string gtext;
        private string text;
        private int goffset;

        public NumericalCounter(string name, Vector2 position, int maxValue, int initialValue, int valueChange,float timeInterval, SpriteFont font, Color growthColour, bool growthShow = false, bool visible = true)
        {
  
            this.position = position;

            this.name = name;
            this.font = font;

            this.max = maxValue;
            this.init = initialValue;

            this.value = initialValue;
            this.valueChange = (float) valueChange;
            this.timeInterval = timeInterval;

            this.show = growthShow;
            this.visible = visible;
            this.gcolour = growthColour;
            

        }


        public void Update(GameTime gameTime)
        {

            elapsed += (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (elapsed >= timeInterval)
            {
                elapsed = 0;
                value += valueChange;
            }

            if (value < 0)
            {
                value = 0;

            }

            if (max != 0)
            {
                if (value > max)
                {
                    value = max;
                }
            }

        }

        public void Render(SpriteBatch sbatch)
        {
            if (visible == true)
            {
                int svalue = (int)value;
                text = name + ": " + svalue.ToString();

                goffset = (int) font.MeasureString(text).X;

            

                if (show == true)
                {
                    if (valueChange >= 0)
                    {
                        gtext = " +" + valueChange.ToString();
                    }

                    else
                    {
                        gtext = " " + valueChange.ToString();
                    }
                }

                sbatch.DrawString(font,text, position, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);

                if (show == true)
                {
                    sbatch.DrawString(font, gtext, position + new Vector2(goffset,0), gcolour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                }
            }
        }

    }
}
