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
    class positionTextBlob
    {

        public Texture2D charTex;
        private Rectangle rect;

        public int width = 0;
        public int height = 0;
      

        public Vector2 position = Vector2.Zero;
        public Vector2 offset;

        public Vector2 gridPosition = Vector2.Zero;

        private bool keydown;

        public float scale = 1.0f;

        // The public instance of the object

        public positionTextBlob(Texture2D texture, Vector2 startPosition)
        {

            this.charTex = texture;
            this.gridPosition = startPosition;

            // Create rectangle for animation

            rect.X = 0;
            rect.Y = 0;
            rect.Width = texture.Width;
            rect.Height = texture.Height;

            offset = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);


        }


        public void Update(GameTime gametime, Grid grid)
        {

            KeyboardState kstate = Keyboard.GetState();

            if (keydown == false)
            {
                if (kstate.IsKeyDown(Keys.Left))
                {
                    if (gridPosition.X > 1)
                    {
                        gridPosition.X -= 1;
                    }

                    keydown = true;

                }

                if (kstate.IsKeyDown(Keys.Right))
                {
                    if (gridPosition.X < grid.columns)
                    {
                        gridPosition.X += 1;
                    }

                    keydown = true;

                }

                if (kstate.IsKeyDown(Keys.Down))
                {
                    if (gridPosition.Y > 1)
                    {
                        gridPosition.Y -= 1;
                    }

                    keydown = true;

                }

                if (kstate.IsKeyDown(Keys.Up))
                {
                    if (gridPosition.Y < grid.rows)
                    {
                        gridPosition.Y += 1;
                    }

                    keydown = true;

                }
            }

            if (kstate.IsKeyUp(Keys.Up) && kstate.IsKeyUp(Keys.Down) && kstate.IsKeyUp(Keys.Left) && kstate.IsKeyUp(Keys.Right))
            {
                keydown = false;
            }

            

            position = grid.CartesianCoords(gridPosition);

            

        }

        public void Render(SpriteBatch sbatch)
        {
            // scaling from perspective grid class!

            sbatch.Draw(charTex, position - offset, rect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, 0.3f);


        }
    }


}


