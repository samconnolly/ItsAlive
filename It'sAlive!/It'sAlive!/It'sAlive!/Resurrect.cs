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
    class Resurrect
    {
        // icon
        public Vector2 position;
        public float layer = 0.1f;
        public Texture2D tex;
        private Texture2D hTex;
        private Rectangle rect;

        // bools
        public bool doable = false;
        public bool fail = false;

        private int counter;

        public Resurrect(Vector2 iconPosition, Texture2D iconTex, Texture2D highlightTex, GraphicsDevice graphicsDevice)
        {
            this.position = iconPosition;

            this.tex = iconTex;
            this.hTex = highlightTex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;

        }


        public void Animate(Corpse corpse, NumericalCounter humanity, NumericalCounter longevity, NumericalCounter research,Random random)
        {
            fail = false;

            double rnd = random.NextDouble();            // produce a random number
            float chance = 1.0f * (research.value/50.0f)* (corpse.rot/3.0f) * (float) rnd; // calculate a resurrection chance based on amount of research done and corpse freshness
            
            // resurrect if successful
            if (chance > 0.9f && longevity.value > 0)
            {

                double rnd1 = random.NextDouble(); // create a set of random numbers
                double rnd2 = random.NextDouble();
                double rnd3 = random.NextDouble();
                longevity.value = (float)((rnd1 + rnd2 + rnd3) / 3.0 + 0.5) * longevity.value; // create a pseudo-normal random value between 0.5 and 1.5 and multiply by the longevity

                double rnd4 = random.NextDouble(); // create a set of random numbers
                double rnd5 = random.NextDouble();
                double rnd6 = random.NextDouble();
                humanity.value = (float)((rnd4 + rnd5 + rnd6) / 3.0 + 0.5) * humanity.value; // create a pseudo-normal random value between 0.5 and 1.5 and multiply by the humanity

                corpse.alive = true;
            }

            // else it fails...
            else
            {
                corpse.visible = false;
                fail = true;
            }

            
        }


                    

        public void Update(Corpse corpse,NumericalCounter lifeForce, NumericalCounter humanity, NumericalCounter longevity, FloorObject conductor,GameTime gameTime)
        {
            if (lifeForce.value == 100 && conductor.on == true && corpse.visible == true && corpse.alive == false)
            {
                doable = true;
            }

            else
            {
                doable = false;
            }


        }

        public void Render(SpriteBatch sbatch, Cursor cursor)
        {
            // icon

            if (cursor.position.X >= position.X && cursor.position.X < (position.X + tex.Width) 
                && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + tex.Height)
                && doable == true)
            {
                sbatch.Draw(hTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            }

            else
            {
                sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            }
 
            }
        }
    
}

