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
        public Rectangle menuRectangle;
        public Vector2 menuPosition;
        public bool menuMouseover = false;

        private Texture2D dummyTexture;

        public FloorObject menuHighlightObject = null;
        

        // set initial variables on frames

        public List<FloorObject> buildList = new List<FloorObject>();

        public Build(Vector2 iconPosition, Texture2D iconTex, Texture2D highlightTex, Texture2D clickTex, GraphicsDevice graphicsDevice)
        {          
            this.tex = iconTex;
            this.hTex = highlightTex;
            this.cTex = clickTex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.width = (int)(tex.Width * scale);
            this.height = (int)(tex.Height * scale);

            this.position = iconPosition;
            this.menuPosition = iconPosition + new Vector2(width, height);

            this.dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            this.dummyTexture.SetData(new Color[] { Color.Gray });
        }

        // add object to build list
        public void Add(FloorObject item)
        {
            buildList.Add(item);
            item.onBuildList = true;

            int rank = buildList.IndexOf(item);
                                   
        }

        // remove object from build list
        public void Remove(FloorObject item, NumericalCounter money)
        {
            buildList.Remove(item);
            money.value -= item.cost;

            foreach (FloorObject curitem in buildList)
            {
                int rank = buildList.IndexOf(curitem);
                                               
            }
        }

        public void Update(Cursor cursor, GameTime gameTime)
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
                                                                               
                // find longest string in menu

                int boxWidth = 0;
                
                if (menu == true)
                {
                    foreach (FloorObject machine in buildList)
                    {
                        
                        if (machine.name.Length + 5 > boxWidth)
                        {
                            boxWidth = machine.name.Length + 5; // set box width for below
                            
                        }
                    }
                }

                // draw boxes and outlines for each item, check if highlighted, clicked

                menuRectangle.Width  = boxWidth * 12 + 10;  // set width to widest text from above
                menuRectangle.Height = 30;                  // menu item height spread
                Vector2 menuItemPosition = menuPosition - new Vector2(0, menuRectangle.Height);    // position of each text menu item

                menuMouseover = false;

                MouseState mouseState = Mouse.GetState();

                FloorObject remove = null; // built items to remove from build list
                
                foreach (FloorObject machine in buildList)
                {
                    // declare colours
                    Color textColour;
                    Color boxColour;
                    Color lineColour;


                    // IF MOUSE-OVERED change colours, set to highlighted item

                    if (cursor.position.X >= menuItemPosition.X && cursor.position.X < (menuItemPosition.X + menuRectangle.Width) && 
                        cursor.position.Y >= menuItemPosition.Y && cursor.position.Y < (menuItemPosition.Y + menuRectangle.Height))
                    {
                        textColour = Color.Black;
                        boxColour  = Color.LightGray;
                        lineColour = Color.LimeGreen;

                        menuHighlightObject = machine;
                        menuMouseover = true;

                        // build item if clicked

                        if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (cursor.click == false)
                            {
                                menu = false;

                                if (money.value >= menuHighlightObject.cost)
                                {
                                    remove = menuHighlightObject;
                                }
                            }

                            cursor.click = true;

                        }                        

                    }

                    // IF NOT MOUSE-OVERED
                    else
                    {
                        // set colours to standard
                        textColour = Color.White;
                        boxColour = Color.Gray;
                        lineColour = Color.DarkGray;
                                                
                    }

                    // text

                    sbatch.DrawString(font, machine.name + ": $" + machine.cost.ToString(), menuItemPosition + new Vector2(5,0), textColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.13f);


                    // text rectangle
                    sbatch.Draw(dummyTexture, menuItemPosition , menuRectangle, boxColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.17f);

                    // top line
                    Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(menuItemPosition, (menuItemPosition + new Vector2(menuRectangle.Width, 0)));

                    float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                    float length = Vector2.Distance(line.Item1, line.Item2);

                    sbatch.Draw(dummyTexture, line.Item1 + new Vector2(0, 0), null, lineColour, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0.15f);

                    // right line
                    Tuple<Vector2, Vector2> line2 = new Tuple<Vector2, Vector2>((menuItemPosition + new Vector2(menuRectangle.Width, menuRectangle.Height)), (menuItemPosition + new Vector2(menuRectangle.Width, 0)));

                    float angle2 = (float)Math.Atan2(line2.Item2.Y - line2.Item1.Y, line2.Item2.X - line2.Item1.X);
                    float length2 = Vector2.Distance(line2.Item1, line2.Item2);

                    sbatch.Draw(dummyTexture, line2.Item1 + new Vector2(0, 0), null, lineColour, angle2, Vector2.Zero, new Vector2(length2, 3.0f), SpriteEffects.None, 0.15f);

                    // left line
                    Tuple<Vector2, Vector2> line3 = new Tuple<Vector2, Vector2>(menuItemPosition, (menuItemPosition + new Vector2(0, menuRectangle.Height)));

                    float angle3 = (float)Math.Atan2(line3.Item2.Y - line3.Item1.Y, line3.Item2.X - line3.Item1.X);
                    float length3 = Vector2.Distance(line3.Item1, line3.Item2);

                    sbatch.Draw(dummyTexture, line3.Item1 + new Vector2(0, 0), null, lineColour, angle3, Vector2.Zero, new Vector2(length3, 3.0f), SpriteEffects.None, 0.15f);

                    // bottom line
                    Tuple<Vector2, Vector2> line4 = new Tuple<Vector2, Vector2>((menuItemPosition + new Vector2(menuRectangle.Width, menuRectangle.Height)), (menuItemPosition + new Vector2(0, menuRectangle.Height)));

                    float angle4 = (float)Math.Atan2(line4.Item2.Y - line4.Item1.Y, line4.Item2.X - line4.Item1.X);
                    float length4 = Vector2.Distance(line4.Item1, line4.Item2);

                    sbatch.Draw(dummyTexture, line4.Item1 + new Vector2(0, 0), null, lineColour, angle4, Vector2.Zero, new Vector2(length4, 3.0f), SpriteEffects.None, 0.15f);

                    // set to height of next text item
                    menuItemPosition -= new Vector2(0, menuRectangle.Height);
                    
                }

                // remove anything that has been built from build list, build, update its position
                if (remove != null)
                {
                    floorObjectList.Add(remove);        // add to floor objects (build)
                    Remove(remove, money);              // remove from build list
                    reachable.Update(floorObjectList);  // update reachable squares of grid, update character's paths
                    scientist.path.Update(reachable);   // update path from updated reachable
                    assistant.path.Update(reachable);   // update path from updated reachable
                    remove = null;
                }

                // set to close menu if clicking is done outside...
                if (menuMouseover == false)
                {
                    if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (cursor.click == false)
                        {
                            menu = false;
                        }

                        cursor.click = true;
                    }
                }
                
            }




            }
        }
    }


