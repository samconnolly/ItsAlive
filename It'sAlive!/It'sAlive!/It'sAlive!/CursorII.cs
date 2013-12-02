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
    class Cursor
    {
        private Texture2D cursorTex;
        private Texture2D dummyTexture;
        private Rectangle rect;
        private Rectangle menuRectangle;


        public bool click = false;
        public bool menu = false;
        public bool graveMenu = false;
        public bool corpseMenu = false;
        public bool mouseOver = false;
        public bool menuMouseover = false;
        public bool buildIconMouseover = false;
        public bool graveMouseOver = false;
        public bool corpseMouseover = false;
        public Color highlightColour = Color.Lime;
                
        public FloorObject menuObject;
        public MenuAction menuHighlightAction = null;
        public MiniProgressBar menuProgBar = null;

        private Vector2 textOffset = new Vector2(25, 0);

        public Vector2 position = Vector2.Zero;
        public string text = "none";

        public MouseState mouseState;

        public Cursor(Texture2D texture,GraphicsDevice graphicsDevice)
        {
            this.cursorTex = texture;

            rect.Width  = texture.Width;
            rect.Height = texture.Height;
            rect.X = 0;
            rect.Y = 0;

            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Gray });
            
            menuRectangle.Width = 0;
            menuRectangle.Height = 0;
            menuRectangle.X = 0;
            menuRectangle.Y = 0;

            

        }

        public void Update(GraphicsDeviceManager graphics, float xOffset, float yOffset, float wxOffset, float wyOffset)
        {
            mouseState = Mouse.GetState();

            // Put the cursor where the mouse is constantly

            if (graphics.IsFullScreen == true)
            {
                position.X = (mouseState.X / xOffset);
                position.Y = (mouseState.Y / yOffset);
            }


            if (graphics.IsFullScreen == false)
            {
                position.X = (mouseState.X / wxOffset);
                position.Y = (mouseState.Y / wyOffset);
            }

            // set mouseover false for this pass
            buildIconMouseover = false;

            // turn off click flag when no longer clicking

            if (mouseState.LeftButton == ButtonState.Released && mouseState.RightButton == ButtonState.Released && click == true)
            {
                click = false;
            }

        }

        //===========================================================================================================================

        public void Render(GraphicsDevice graphicsDevice, SpriteBatch sbatch, SpriteFont font, Build build, Graveyard graveyard, Corpse corpse)
        {
            sbatch.Draw(cursorTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

           

            


            // Mouse-over text

            if (mouseOver == true && menu == false && corpseMouseover == false)
            {
                //sbatch.DrawString(font, text, position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
                //sbatch.DrawString(font, menu.ToString() + ' ' + click.ToString()+ ' ' + mouseOver.ToString(), position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
                sbatch.DrawString(font, menuObject.name, position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

            

            if (graveMouseOver == true && menu == false)
            {                
                sbatch.DrawString(font, "Graveyard", position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

            if (corpseMouseover == true && menu == false)
            {
                sbatch.DrawString(font, "Corpse: rot = " + corpse.rot.ToString(), position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

            if (buildIconMouseover == true && menu == false && build.menu == true)
            {   
                // tooltip
                sbatch.DrawString(font, menuObject.name + " : " + menuObject.cost.ToString(), position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);

                // highlight box


            }

        }
    }
}
