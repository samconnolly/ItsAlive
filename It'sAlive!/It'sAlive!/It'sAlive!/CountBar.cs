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
    class CountBar
    {
        private Texture2D dummyTexture;
        public Vector2 position;
        public int max;
        public int init;

        private Color barColour;
        private Color boxColour;

        private Rectangle box;
        private Rectangle bar;
        private int fullHeight;

        public float value;



        public CountBar(GraphicsDevice graphicsDevice,Vector2 position,int width, int height, int maxValue, int initialValue, Color barColour, Color boxColour)
        {
            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Gray });

            box = new Rectangle();
            bar = new Rectangle();

            this.position = position;

            this.max = maxValue;
            this.init = initialValue;

            this.barColour = barColour;
            this.boxColour = boxColour;

            this.box.Width = width;
            this.box.Height = height;

            this.fullHeight = (int)(height - 10);
            this.value = initialValue;

            this.bar.Width = (int)(width - 10);
            this.bar.Height = (int)(fullHeight * (value / (float)max));

            
        }


        public void Update(GameTime gameTime)
        {
            value -= (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            if (value < 0)
            {
                value = 0;

            }

                if (value > max)
                {
                    value = max;
                }

            bar.Height = (int)(fullHeight * (value / (float)max));

        }

        public void Render(SpriteBatch sbatch)
        {
            // outline
            sbatch.Draw(dummyTexture, position, box, boxColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.105f);

            // bar
            sbatch.Draw(dummyTexture, position + new Vector2(5,5 + (fullHeight - bar.Height)), bar, barColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);

        }

    }
}
