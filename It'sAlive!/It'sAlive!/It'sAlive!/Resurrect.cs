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
        // numbers
        private float raiseResearch = 300f;

        // icon
        public Vector2 position;
        public float layer = 0.1f;
        public Texture2D tex;
        private Texture2D hTex;
        private Texture2D cTex;
        private Rectangle rect;
        private bool clickOn = false;
        private int clickCount = 0;
        private float scale = 0.5f;
        private int width;
        private int height;

        // positions
        public Vector2 assPos = new Vector2(10, 20);
        public Vector2 sciPos;

        // bools
        public bool doable = false;
        public bool fail = false;

        //private int counter;

        public Resurrect(Vector2 iconPosition, Texture2D iconTex, Texture2D highlightTex,Texture2D clickTexture, GraphicsDevice graphicsDevice, FloorObject table)
        {
            this.position = iconPosition;

            this.tex = iconTex;
            this.hTex = highlightTex;
            this.cTex = clickTexture;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.width = (int) (tex.Width * scale);
            this.height = (int) (tex.Height * scale);

            this.sciPos = table.gridPosition + new Vector2(1,-1);

        }


        public void Animate(Scientist scientist, Assistant assistant)
        {
            assistant.Animate(this);
            scientist.Animate(this); 
        }

        public void Alive(Corpse corpse, NumericalCounter humanity, NumericalCounter longevity, NumericalCounter lifeForce, NumericalCounter research, NumericalCounter madness, Random random, Assistant assistant)
        {

            fail = false;
            
            double rnd = random.NextDouble();            // produce a random number
            float chance = 1.0f * (research.value/raiseResearch)* (corpse.rot/3.0f) * (float) rnd; // calculate a resurrection chance based on amount of research done and corpse freshness
            
            // resurrect if successful
            if (chance > 0.9f && longevity.value > 0 && lifeForce.value == 100)
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

                research.value += 150;
                madness.value += 20;
            }

            // else it fails...
            else
            {
                corpse.visible = false;
                fail = true;

                research.value += 50;
                madness.value += 5;
            }
            
        }


                    

        public void Update(Corpse corpse,NumericalCounter lifeForce, NumericalCounter humanity, NumericalCounter longevity, FloorObject conductor,
                                GameTime gameTime, Cursor cursor,Scientist scientist, Assistant assistant)
        {
            // check if resurrection is possible

            if (conductor.on == true && corpse.visible == true && corpse.alive == false)
            {
                doable = true;
            }

            else
            {
                doable = false;
            }

            MouseState mouseState = Mouse.GetState();

            // clicking on animate icon

            if (cursor.position.X >= position.X && cursor.position.X < (position.X + tex.Width) && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + tex.Height))
            {
                if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                {
                    if (cursor.click == false && doable == true)
                    {
                        Animate(scientist, assistant);
                        clickOn = true;
                    }

                    cursor.click = true;
                }
            }

            if (clickOn == true)
            {
                clickCount += gameTime.ElapsedGameTime.Milliseconds;

                if (clickCount > 100)
                {
                    clickCount = 0;
                    clickOn = false;
                }
            }

        }

        public void Render(SpriteBatch sbatch, Cursor cursor)
        {
            // icon

            if (cursor.position.X >= position.X && cursor.position.X < (position.X + tex.Width) 
                && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + tex.Height)
                && doable == true && clickOn == false)
            {
                sbatch.Draw(hTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
            }

            else if (clickOn == true)
            {
                sbatch.Draw(cTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
            }

            else
            {
                sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
            }
 
            }
        }
    
}

