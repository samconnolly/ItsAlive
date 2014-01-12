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
    class MiniProgressBar
    {
        private Texture2D dummyTexture;
        public Vector2 position;
        
        public int init;

        private Color barColour;
        private Color boxColour;

        private Rectangle box;
        private Rectangle bar;
        private int fullWidth;

        public float value;
        public int width = 50;
        public int height = 10;

        private float elapsed = 0;
        private float timeInterval = 100;

        private bool mouseover;
        private string barText;

        public MenuAction action;
        public FloorObject floorObject;
        

        public MiniProgressBar(GraphicsDevice graphicsDevice, Vector2 position, MenuAction menuAction, FloorObject machine)
        {
            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Gray });

            box = new Rectangle();
            bar = new Rectangle();

            this.position = position;
               
            this.barColour = Color.LimeGreen;
            this.boxColour = Color.Gray;

            this.box.Width = width;
            this.box.Height = height;

            this.fullWidth = (int)(width - 4);
            
            this.bar.Height = (int)(height - 4);
            this.bar.Width = (int)(fullWidth * (value / (float)init));

            this.action = menuAction;
            this.init = action.time;
            this.value = 0;

            this.floorObject = machine;

            action.done = false;

        }


        public void Update(GameTime gameTime, Cursor cursor)
        {
            mouseover = false;

            if (cursor.menu == false)
            {
                // objects
                if (cursor.position.X >= position.X && cursor.position.X <= (position.X + width)
                        && cursor.position.Y >= position.Y && cursor.position.Y <= (position.Y + height)
                            && cursor.mouseOver == false)
                {
                    mouseover = true; // add object mouseover text
                    cursor.menuProgBar = this;
                }

            }

            // change bar length according to vlue - decrease value over time

            elapsed += (float)gameTime.ElapsedGameTime.Milliseconds;

            if (elapsed >= timeInterval)
            {
                elapsed = 0;
                value += timeInterval/1000.0f;
            }

            if (value > init)
            {
                value = init;
                action.done = true;

            }

           
            bar.Width = (int)(fullWidth * (value / (float)init));
            barText = value.ToString("#.#") + "/" + init.ToString("#.#");
        }

        public void Render(SpriteBatch sbatch, SpriteFont font)
        {
            // text
            if (mouseover == true)
            {                
                sbatch.DrawString(font, barText, position + new Vector2(0,-25), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

            // outline
            sbatch.Draw(dummyTexture, position, box, boxColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.115f);

            // bar
            sbatch.Draw(dummyTexture, position + new Vector2(2, 2), bar, barColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.11f);

        }

    }
}
