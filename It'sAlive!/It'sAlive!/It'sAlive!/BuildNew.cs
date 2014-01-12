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
    class Build
    {
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

        
        //  menu
        public bool menu = false;

        public Texture2D arrow;
        public Texture2D highArrow;

        public bool full = false;
        public int scrollPos = 0;
        public int buildScreenLength = 8;

        public Vector2 leftArrowPos = new Vector2(70, 760);
        public Vector2 rightArrowPos = new Vector2(920, 720);
        
        private Texture2D dummyTexture;
        private Color boxColour = Color.Black;
        private Rectangle buildRectangle = new Rectangle(0, 0, 1980, 70);
        public Vector2 buildPos = new Vector2(0, 1010);

        public bool menuMouseover = false;


        public FloorObject menuHighlightObject = null;
        private List<FloorObject> remove = new List<FloorObject> { };

        // set initial variables on frames

        public List<FloorObject> buildList = new List<FloorObject>();

        public Build(Vector2 iconPosition, Texture2D iconTex, Texture2D highlightTex, Texture2D clickTex, Texture2D buildArrow, Texture2D highBuildArrow, GraphicsDevice graphicsDevice)
        {          
            this.tex = iconTex;
            this.hTex = highlightTex;
            this.cTex = clickTex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.width = (int)(tex.Width * scale);
            this.height = (int)(tex.Height * scale);

            this.position = iconPosition;
             this.arrow = buildArrow;
            this.highArrow = highBuildArrow;

            this.dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            this.dummyTexture.SetData(new Color[] { Color.Gray });
        }

        // add object to build list
        public void Add(FloorObject item)
        {
            buildList.Add(item);
            item.onBuildList = true;

            int rank = buildList.IndexOf(item);

            item.iconPosition = new Vector2(120 + 80 * rank, 1015);

            if (rank > buildScreenLength)
            {
                item.onBuildList = false;
            }         
        }

        // Build object - setup to remove object from build list on next update


         public void BuildThis(List<FloorObject> floorObjectList, FloorObject item, ReachableArea reachable)
        {
            remove.Add(item);                   // add item to list of items to remove from build list
            floorObjectList.Add(item);          // add to list of objects on floor
            reachable.Update(floorObjectList);  // update reachable area to account for this
            menu = false;                       // close menu
        }

        public void ScrollLeft()
        {
            if (scrollPos > 0)
            {
                scrollPos -= 1;

                foreach (FloorObject curitem in buildList)
                {
                    if (buildList.IndexOf(curitem) >= scrollPos && buildList.IndexOf(curitem) < (scrollPos + buildScreenLength))
                    {
                        curitem.onBuildList = true;
                    }

                    else
                    {
                        curitem.onBuildList = false;
                    }

                    curitem.iconPosition = curitem.iconPosition + new Vector2(80, 0);
                }
            }
        }

        public void ScrollRight()
        {
            if (scrollPos < (buildList.Count - 1))
            {
                scrollPos += 1;

                foreach (FloorObject curitem in buildList)
                {
                    if (buildList.IndexOf(curitem) >= scrollPos && buildList.IndexOf(curitem) < (scrollPos + buildScreenLength))
                    {
                        curitem.onBuildList = true;
                    }

                    else
                    {
                        curitem.onBuildList = false;
                    }

                    curitem.iconPosition = curitem.iconPosition - new Vector2(80, 0);
                }
            }
        }


        public void Update(Cursor cursor, GameTime gameTime, NumericalCounter money)
        {
            // clicking on build icon
            
            if (cursor.position.X >= position.X && cursor.position.X < (position.X + width) && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + height))
            {
                if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                {
                    if (cursor.click == false)
                    {
                        menu = true;
                        clickOn = true;

                        cursor.click = true;
                    }

                    
                }
            }

            // turn click anim on for a bit
            if (clickOn == true)
            {
                clickCount += gameTime.ElapsedGameTime.Milliseconds;

                if (clickCount > 100)
                {
                    clickCount = 0;
                    clickOn = false;
                }
            }


            // turm the menu off
             if (menuMouseover == false && (cursor.position.X >= position.X && cursor.position.X < (position.X + width) && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + height)) == false)
                {
                    if (cursor.position.Y < buildPos.Y)
                    {
                        if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (menu == true)
                            {
                                menu = false;
                            }
                                                        
                        }
                    }
                }

            // remove objects that have been built

             if (remove.Count > 0)
             {
                 buildList.Remove(remove[0]);
                 money.value -= remove[0].cost;


                 foreach (FloorObject curitem in buildList)
                 {
                     int rank = buildList.IndexOf(curitem);
                     curitem.iconPosition = new Vector2(120 + 80 * rank, 1015);

                     if (rank > buildScreenLength)
                     {
                         curitem.onBuildList = false;
                     }

                     else
                     {
                         curitem.onBuildList = true;
                     }
                 }

                 remove = new List<FloorObject> { };
             }

        }

               
        public void Render(SpriteBatch sbatch,Cursor cursor,SpriteFont font,NumericalCounter money, List<FloorObject> floorObjectList, ReachableArea reachable, Scientist scientist, Assistant assistant)
        {
            // draw icon - highlighted

            if (cursor.position.X >= position.X && cursor.position.X < (position.X + width) 
                && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + height)
                && clickOn == false)
            {
                sbatch.Draw(hTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero,scale, SpriteEffects.None, layer);
            }

            // draw icon - clicked
            else if (clickOn == true)
            {
                sbatch.Draw(cTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
            }

            // draw icon - unhighlighted
            else
            {
                sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
            }

            // menu
            if (menu == true)
            {
                // arrows
                if (buildList.Count > buildScreenLength)
                {
                    full = true;


                    if (scrollPos == (buildList.Count - buildScreenLength))
                    {
                        if (cursor.position.X >= leftArrowPos.X && cursor.position.X <= leftArrowPos.X + arrow.Width
                                    && cursor.position.Y >= leftArrowPos.Y - arrow.Height && cursor.position.Y <= leftArrowPos.Y)
                        {

                            sbatch.Draw(highArrow, leftArrowPos, null, Color.White, -(float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }

                        else
                        {
                            sbatch.Draw(arrow, leftArrowPos, null, Color.White, -(float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }
                    }

                    if (scrollPos == 0)
                    {

                        if (cursor.position.X >= rightArrowPos.X - arrow.Width && cursor.position.X <= rightArrowPos.X 
                                    && cursor.position.Y >= rightArrowPos.Y && cursor.position.Y <= rightArrowPos.Y + arrow.Height)
                        {
                            sbatch.Draw(highArrow, rightArrowPos, null, Color.White, (float)Math.PI/2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }

                        else
                        {
                            sbatch.Draw(arrow, rightArrowPos, null, Color.White, (float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }
                    }

                    if (scrollPos < (buildList.Count - buildScreenLength) && scrollPos > 0)
                    {
                        if (cursor.position.X >= leftArrowPos.X && cursor.position.X <= leftArrowPos.X + arrow.Width
                                    && cursor.position.Y >= leftArrowPos.Y - arrow.Height && cursor.position.Y <= leftArrowPos.Y)
                        {

                            sbatch.Draw(highArrow, leftArrowPos, null, Color.White, -(float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }

                        else
                        {
                            sbatch.Draw(arrow, leftArrowPos, null, Color.White, -(float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }



                        if (cursor.position.X >= rightArrowPos.X - arrow.Width && cursor.position.X <= rightArrowPos.X
                                    && cursor.position.Y >= rightArrowPos.Y && cursor.position.Y <= rightArrowPos.Y + arrow.Height)
                        {
                            sbatch.Draw(highArrow, rightArrowPos, null, Color.White, (float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }

                        else
                        {
                            sbatch.Draw(arrow, rightArrowPos, null, Color.White, (float)Math.PI / 2.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 0.1f);
                        }
                    }

                }


                else
                {
                    full = false;
                }

                // render background

                sbatch.Draw(dummyTexture, buildPos, buildRectangle, boxColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.17f);

                // render items

                for (int x = scrollPos; x < (scrollPos + buildScreenLength) && x < buildList.Count; x++)
                {
                    FloorObject curitem = buildList[x];

                    curitem.IconRender(sbatch,this);
                }

            }

                //// remove anything that has been built from build list, build, update its position
                //if (remove != null)
                //{
                //    floorObjectList.Add(remove);        // add to floor objects (build)
                //    Remove(remove, money);              // remove from build list
                //    reachable.Update(floorObjectList);  // update reachable squares of grid, update character's paths
                //    scientist.path.Update(reachable);   // update path from updated reachable
                //    assistant.path.Update(reachable);   // update path from updated reachable
                //    remove = null;
                //}

                // set to close menu if clicking is done outside...
               
                
            }




            
        }
    }


