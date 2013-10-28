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


        private bool click = false;
        private bool menu = false;
        private bool graveMenu = false;
        private bool corpseMenu = false;
        private bool mouseOver = false;
        private bool barMouseover = false;
        public bool menuMouseover = false;
        public bool buildIconMouseover = false;
        public bool graveMouseOver = false;
        public bool corpseMouseover = false;
        public Color highlightColour = Color.Lime;
                
        public FloorObject menuObject;
        public MenuAction menuHighlightAction = null;
        public MiniProgressBar menuProgBar = null;

        private Vector2 textOffset = new Vector2(20, 20);

        public Vector2 position = Vector2.Zero;
        public Vector2 menuPosition;
        public string text = "none";

        private FloorObject remove;

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

        public void Update(List<FloorObject> floorObjectList, List<MiniProgressBar> progBars,Scientist scientist, Build build, Graveyard graveyard,FloorObject table, Corpse corpse)
        {
            MouseState mouseState = Mouse.GetState();

            // Put the cursor where the mouse is constantly

            position.X = mouseState.X;
            position.Y = mouseState.Y;

            mouseOver = false;

          

            // check if over a progress bar, show values if so

            barMouseover = false;

            foreach (MiniProgressBar bar in progBars)
            {

                // if menu is off, check if over an object, open menu if clicked
                                
                if (menu == false)
                {
                    // objects
                    if (position.X >= bar.position.X && position.X <= (bar.position.X + bar.width)
                            && position.Y >= bar.position.Y && position.Y <= (bar.position.Y + bar.height))
                    {

                        barMouseover = true; // add object mouseover text
                        menuProgBar = bar;

                    }

                }
            }            

            // check for clicking on/mouseover build icons

            

            buildIconMouseover = false;

            foreach (FloorObject curitem in build.buildList)
            {
                if (position.X >= curitem.iconPosition.X && position.X <= curitem.iconPosition.X + 60
                && position.Y >= curitem.iconPosition.Y && position.Y <= curitem.iconPosition.Y + 60)
                {
                    if (curitem.onBuildList == true && build.buildList.IndexOf(curitem) < (build.scrollPos + build.buildScreenLength) && build.buildList.IndexOf(curitem) >= build.scrollPos)
                    {
                        // tooltip + highlighting on
                        buildIconMouseover = true;
                        menuObject = curitem;

                        // build!
                        if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (click == false)
                            {
                                remove = curitem;
                                floorObjectList.Add(curitem);
                                build.menu = false;
                            }

                        }

                        click = true;

                    }
                }

            }

            // remove anything that has been built from build list

            build.Remove(remove);

            // check for clicking on objects

            

                // if menu is off, check if over an object, open menu if clicked & check for clicking on graveyard + tooltip

                if (menu == false && graveMenu == false && corpseMenu == false)
                {
                    foreach (FloorObject floorObject in floorObjectList)
                    {

                        // objects
                        if (position.X >= (floorObject.position.X - floorObject.offset.X) && position.X <= (floorObject.position.X - floorObject.offset.X + ((floorObject.objectTex.Width*floorObject.scale) / floorObject.frames))
                                && position.Y >= floorObject.position.Y - floorObject.offset.Y && position.Y <= (floorObject.position.Y - floorObject.offset.Y + (floorObject.objectTex.Height*floorObject.scale)))
                        {
                            if (floorObject.menuActions.Count > 0)
                            {
                                mouseOver = true; // add object mouseover text
                                menuObject = floorObject;

                                // turn on menu when object is clicked

                                if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                                {
                                    if (click == false)
                                    {

                                        menu = true;

                                    }

                                }

                                click = true;
                            }
                        }
              
                    }

                    graveMouseOver = false;
            

                    // graveyard
                    if (position.X >= graveyard.tlcorner.X && position.X <= graveyard.brcorner.X 
                            && position.Y >= graveyard.tlcorner.Y && position.Y <= graveyard.brcorner.Y)
                    {

                        graveMouseOver = true; // add object mouseover text
                        
                        // turn on menu when object is clicked

                        if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (click == false)
                            {
                                graveMenu = true;
                                graveMouseOver = false;
                            }

                        }

                        click = true;
                    }

                    corpseMouseover = false;
                    

                    // corpse
                    if (position.X >= corpse.position.X && position.X <= corpse.position.X + corpse.width
                            && position.Y >= corpse.position.Y && position.Y <= corpse.position.Y + corpse.height && corpse.visible == true)
                    {

                        corpseMouseover = true; // add object mouseover text

                        // turn on menu when object is clicked

                        if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                        {
                            if (click == false)
                            {
                                corpseMenu = true;
                                corpseMouseover = false;
                            }

                        }

                        click = true;
                    }

                }



                // allow clicking on actions, turn off menu when anything else is clicked (if on)



                if (menu == true)
                {
                    

                    if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (click == false)
                        {
                            if (menu == true)
                            {
                                menu = false;


                                if (menuMouseover == true)
                                {
                                    scientist.action = menuHighlightAction;
                                    scientist.floorObject = menuObject;
                                }
                            }

                                        
                        }

                        click = true;
                    }

                }

                if (graveMenu == true)
                {


                    if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (click == false)
                        {
               
                                graveMenu = false;

                                if (menuMouseover == true)
                                {
                                    if (floorObjectList.Contains(table))
                                    {
                                        scientist.DigUpCorpse(corpse);
                                    }
                                }
                            
                        }

                        click = true;
                    }

                }

                if (corpseMenu == true)
                {


                    if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (click == false)
                        {

                                corpseMenu = false;

                                if (menuMouseover == true)
                                {
                                        scientist.action = menuHighlightAction;
                                        scientist.corpseWork = true;
                                        //scientist.floorObject = menuObject;
                                }
                            
                        }

                        click = true;
                    }

                }

              // clicking on build icon

            if (position.X >= build.position.X && position.X < (build.position.X + build.tex.Width) && position.Y >= build.position.Y && position.Y < (build.position.Y + build.tex.Height))
            {
                if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                {
                    if (click == false)
                    {
                        build.menu = true;
                        build.scrollPos = 0;
                    }

                    click = true;

                }

                
            }

            else
            {
                if ((mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed) && position.Y < build.buildPos.Y)
                {
                    if (click == false)
                    {
                        build.menu = false;
                    }

                    click = true;

                }

                
            }

            // Scrolling build menu....

            // Right
            if (position.X >= build.rightArrowPos.X - build.arrow.Width && position.X <= build.rightArrowPos.X 
                                    && position.Y >= build.rightArrowPos.Y && position.Y <= build.rightArrowPos.Y + build.arrow.Height)
            {
                if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                {
                    if (click == false)
                    {

                        build.ScrollRight();
                    }

                    click = true;

                }

               
            }

            // Left
            if (position.X >= build.leftArrowPos.X && position.X <= build.leftArrowPos.X + build.arrow.Width
                                    && position.Y >= build.leftArrowPos.Y - build.arrow.Height && position.Y <= build.leftArrowPos.Y)
            {
                if (mouseState.LeftButton == ButtonState.Pressed | mouseState.RightButton == ButtonState.Pressed)
                {
                    if (click == false)
                    {
                        build.ScrollLeft();
                    }

                    click = true;

                }

                
            }

            
            // turn off click flag when no longer clicking

            if (mouseState.LeftButton == ButtonState.Released && mouseState.RightButton == ButtonState.Released)
            {
                click = false;
            }

            text = position.ToString();
        }

        //===========================================================================================================================

        public void Render(GraphicsDevice graphicsDevice, SpriteBatch sbatch, SpriteFont font, Build build, Graveyard graveyard, Corpse corpse)
        {
            sbatch.Draw(cursorTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.0f);

            // draw menu

            if (menu == true || graveMenu == true || corpseMenu == true)
            {
                if (click == true)
                {                    
                    menuPosition = position;

                }
                                                               
                // find longest string in menu

                int boxWidth = 0;
                List<MenuAction> actions = new List<MenuAction> { };

                if (menu == true)
                {
                    foreach (MenuAction action in menuObject.menuActions)
                    {
                        actions.Add(action);

                        if (action.name.Length > boxWidth)
                        {
                            boxWidth = action.name.Length; // set box width for below
                            
                        }
                    }
                }

                //grave menu

                if (graveMenu == true)
                {
                    foreach (MenuAction action in graveyard.menuActions)
                    {
                        actions.Add(action);

                        if (action.name.Length > boxWidth)
                        {
                            boxWidth = action.name.Length; // set box width for below
                            
                        }
                    }
                }

                //corpse menu

                if (corpseMenu == true)
                {
                    foreach (MenuAction action in corpse.menuActions)
                    {
                        actions.Add(action);

                        if (action.name.Length > boxWidth)
                        {
                            boxWidth = action.name.Length; // set box width for below

                        }
                    }
                }

                // boxes and outlines

                menuRectangle.Width  = boxWidth * 12 + 10;  // set width to widest text from above
                menuRectangle.Height = 30;                  // menu item height spread
                Vector2 menuItemPosition = menuPosition;    // position of each text menu item

                menuMouseover = false;

                foreach (MenuAction action in actions)
                {
                    // declare colours
                    Color textColour;
                    Color boxColour;
                    Color lineColour;

                    

                    // IF MOUSE-OVERED change colours, set to highlighted item

                    if (position.X >= menuItemPosition.X && position.X < (menuItemPosition.X + menuRectangle.Width) && position.Y >= menuItemPosition.Y && position.Y < (menuItemPosition.Y + menuRectangle.Height))
                    {
                        textColour = Color.Black;
                        boxColour  = Color.LightGray;
                        lineColour = Color.LimeGreen;

                        menuHighlightAction = action;
                        menuMouseover = true;
                    }

                    // IF NOT MOUSE-OVERED
                    else
                    {
                        textColour = Color.White;
                        boxColour = Color.Gray;
                        lineColour = Color.DarkGray;
                                                
                    }

                    // text

                    sbatch.DrawString(font, action.name, menuItemPosition + new Vector2(5,0), textColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.13f);


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
                    menuItemPosition += new Vector2(0, menuRectangle.Height);
                    
                }

            }


            // Mouse-over text

            if (mouseOver == true && menu == false)
            {
                //sbatch.DrawString(font, text, position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
                //sbatch.DrawString(font, menu.ToString() + ' ' + click.ToString()+ ' ' + mouseOver.ToString(), position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);
                sbatch.DrawString(font, menuObject.name, position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

            if (barMouseover == true && mouseOver == false)
            {
                string barText = menuProgBar.value.ToString("#.#") + "/" + menuProgBar.init.ToString("#.#");
                sbatch.DrawString(font, barText, position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
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
                sbatch.DrawString(font, menuObject.name, position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);

                // highlight box

                // top line
                Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(menuObject.iconPosition, (menuObject.iconPosition + new Vector2(60, 0)));

                float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                float length = Vector2.Distance(line.Item1, line.Item2);

                sbatch.Draw(dummyTexture, line.Item1 + new Vector2(0, 0), null, highlightColour, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0.15f);

                // right line
                Tuple<Vector2, Vector2> line2 = new Tuple<Vector2, Vector2>((menuObject.iconPosition + new Vector2(60, 60)), (menuObject.iconPosition + new Vector2(60, 0)));

                float angle2 = (float)Math.Atan2(line2.Item2.Y - line2.Item1.Y, line2.Item2.X - line2.Item1.X);
                float length2 = Vector2.Distance(line2.Item1, line2.Item2);

                sbatch.Draw(dummyTexture, line2.Item1 + new Vector2(0, 0), null, highlightColour, angle2, Vector2.Zero, new Vector2(length2, 3.0f), SpriteEffects.None, 0.15f);

                // left line
                Tuple<Vector2, Vector2> line3 = new Tuple<Vector2, Vector2>(menuObject.iconPosition, (menuObject.iconPosition + new Vector2(0, 60)));

                float angle3 = (float)Math.Atan2(line3.Item2.Y - line3.Item1.Y, line3.Item2.X - line3.Item1.X);
                float length3 = Vector2.Distance(line3.Item1, line3.Item2);

                sbatch.Draw(dummyTexture, line3.Item1 + new Vector2(0, 0), null, highlightColour, angle3, Vector2.Zero, new Vector2(length3, 3.0f), SpriteEffects.None, 0.15f);

                // bottom line
                Tuple<Vector2, Vector2> line4 = new Tuple<Vector2, Vector2>((menuObject.iconPosition + new Vector2(60, 60)), (menuObject.iconPosition + new Vector2(0, 60)));

                float angle4 = (float)Math.Atan2(line4.Item2.Y - line4.Item1.Y, line4.Item2.X - line4.Item1.X);
                float length4 = Vector2.Distance(line4.Item1, line4.Item2);

                sbatch.Draw(dummyTexture, line4.Item1 + new Vector2(0, 0), null, highlightColour, angle4, Vector2.Zero, new Vector2(length4, 3.0f), SpriteEffects.None, 0.15f);

            }

        }
    }
}
