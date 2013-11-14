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
    class Graveyard
    {
        public Vector2 position;
        public float layer;
        private Texture2D tex;
        private Rectangle rect;

        // icon
        public Vector2 iposition;
        private Texture2D itex;
        private Texture2D htex;
        private Texture2D ctex;
        private Rectangle irect;
        private bool clickOn = false;
        private int clickCount = 0;
        public float ilayer = 0.1f;
        private int iwidth;
        private int iheight;
        private float scale;
        private bool doable = false;
        private bool high = false;


        public Graveyard(Vector2 position, float drawLayer,Vector2 iconPosition, Texture2D tex, Texture2D digtex, Texture2D digHightex, Texture2D digClicktex, GraphicsDevice graphicsDevice, float scale)
        {
            this.position = position;
            this.iposition = iconPosition;
            this.layer = drawLayer;
            this.tex = tex;
            this.itex = digtex;
            this.htex = digHightex;
            this.ctex = digClicktex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.irect.Width = itex.Width;
            this.irect.Height = itex.Height;
            this.scale = scale;
            this.iwidth = (int)(itex.Width * scale);
            this.iheight = (int)(itex.Height * scale);

        }

        public void Update(GameTime gameTime, Cursor cursor, List<FloorObject> floorObjectList, FloorObject table, Assistant assistant, Corpse corpse, List<MiniProgressBar> bars)
        {
            if (floorObjectList.Contains(table) && assistant.digging == false && corpse.visible == false && assistant.outside == false && assistant.corpseCarrying == false)
            {
                doable = true;
            }

            else
            {
                doable = false;
            }

            // clicking on build icon


            if (cursor.position.X >= iposition.X && cursor.position.X < (iposition.X + iwidth) && cursor.position.Y >= iposition.Y && cursor.position.Y < (iposition.Y + iheight))
            {
                high = true;

                if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                {
                    if (cursor.click == false)
                    {
                        if (doable == true)
                        {
                            assistant.DigUpCorpse(corpse);
                            clickOn = true;
                        }


                    }

                    cursor.click = true;
                }
            }

            else
            {
                high = false;
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

        public void Render(SpriteBatch sbatch, SpriteFont font)
        {

            // draw graveyard
            sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);

            // draw icon

            if (high == true && clickOn == false && doable == true)
            {
                sbatch.Draw(htex, iposition, irect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, ilayer);
            }

           // draw icon - clicked
            else if (clickOn == true)
            {
                sbatch.Draw(ctex, iposition, irect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, ilayer);
            }

            // draw icon - unhighlighted
            else
            {
                sbatch.Draw(itex, iposition, irect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, ilayer);
            }


        }
    }
}

