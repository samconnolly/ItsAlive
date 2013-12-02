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
    class FloorObject
    {
        public string name;
        public List<MenuAction> menuActions;
        private List<Tuple<string,Color,Color,Color>> actions;

        public Texture2D objectTex;
        private Rectangle rect;

        public int width = 0;
        public int height = 0;
        public int frames;
        public int anims;
        private int frate = 80;
        private int timer = 0;
        private int currentFrame = 0;
        private int currentAnim = 0;
       
        public Vector2 position = Vector2.Zero;
        public Vector2 gridPosition = Vector2.Zero;
        public Vector2 offset;

        public float layer;
        public float scale = 1.0f;

        public int cost;
        public bool on = false;

        // menu        
        public bool menu = false;
        public bool menuStart = false;
        public bool menuMouseover = false;
        public bool iconMouseover = false;
        public MenuAction menuHighlightAction = null;
        private Vector2 menuPosition;
        private Vector2 textOffset = new Vector2(25, 0);
        private Rectangle menuRectangle;
        private bool mouseOver;

        //  colours
        Color textColour;
        Color boxColour;
        Color lineColour;
        Color highlightColour = Color.LimeGreen;

        // build icon
        public bool onBuildList = false;
        public Vector2 iconPosition = Vector2.Zero;
        private Texture2D iconTex;
        private Rectangle iconRect;
        private float iconLayer = 0.1f;
        private Texture2D dummyTexture;

        // The public instance of the object

        public FloorObject(Texture2D texture,Texture2D iconTexture, int frameNumber,int animNumber, Vector2 gridPosition, Grid grid, string name,int cost, List<MenuAction> menuActions, GraphicsDevice graphicsDevice)
        {

            this.objectTex = texture;
            this.gridPosition = gridPosition;
            this.position = grid.EdgeCartesianCoords(gridPosition);
            this.layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y;

            this.name = name;
            this.menuActions = menuActions;
            this.cost = cost;

            this.frames = frameNumber;
            this.anims = animNumber;

            // Create rectangle for animation

            this.width = texture.Width/frames;
            this.height = texture.Height/anims;

            rect.X = 0;
            rect.Y = 0;
            rect.Width = width;
            rect.Height = height;

            offset = new Vector2(0, ((texture.Height/anims) * scale));// new Vector2((((objectTex.Width * scale / frames) * scale) / 2.0f), (texture.Height * scale) / 2.0f);

            // icon

            this.iconTex = iconTexture;

            if (iconTex != null)
            {
                this.iconRect.Width = iconTex.Width;
                this.iconRect.Height = iconTex.Height;
            }

            // menu tex
            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Gray });
        }

        // build this object
 

        public void Update(GameTime gametime,Cursor cursor,Scientist scientist, Assistant assistant, List<MiniProgressBar> bars, Build build, List<FloorObject> floorObjectList)
        {
            // check for mouseover/click on machine

            mouseOver = false;

            if (cursor.position.X >= (position.X - offset.X) && cursor.position.X <= (position.X - offset.X + ((objectTex.Width * scale) / frames))
                    && cursor.position.Y >= position.Y - offset.Y && cursor.position.Y <= (position.Y - offset.Y + (objectTex.Height * scale))
                        && cursor.corpseMouseover == false && menu == false && menuActions.Count > 0)
            {
                    // turn on menu when object is clicked

                if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (cursor.click == false)
                        {
                            menu = true;
                            menuStart = true;
                            menuPosition = cursor.position;
                            cursor.click = true;
                        }
                    }

                    if (menu == false)
                    {
                        mouseOver = true; // add object mouseover text if no menu
                    }
                
            }

            // check for mouseover/click on build menu icon

            iconMouseover = false;

            if (onBuildList == true)
            {
                if (cursor.position.X >= iconPosition.X && cursor.position.X <= iconPosition.X + iconTex.Width
                       && cursor.position.Y >= iconPosition.Y && cursor.position.Y <= iconPosition.Y + iconTex.Height)
                {

                    iconMouseover = true;
                    cursor.buildIconMouseover = true;
                    cursor.menuObject = this;

                    // build object is clicked

                    if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (cursor.click == false)
                        {
                            build.BuildThis(floorObjectList, this);
                            cursor.click = true;
                        }
                    }


                }
            }

            // animation
            timer += gametime.ElapsedGameTime.Milliseconds;

            if (on == true)
            {
                currentAnim = 1;
            }
            else
            {
                currentAnim = 0;
            }

            
            if (timer >= frate)
            {
                timer = 0;

                if (currentFrame++ == frames - 1)
                {
                    currentFrame = 0;
                }

                rect.X = currentFrame * width;
                rect.Y = currentAnim * height;
            }

            // set menu dimensions, make list of names and colours for each object

            if (menu == true && menuStart == true)
            {

                // find longest string in menu

                int boxWidth = 0;
                actions = new List<Tuple<string, Color, Color, Color>> { };

                
                    foreach (MenuAction action in menuActions)
                    {
                        // add
                        actions.Add(new Tuple<string, Color, Color, Color> ( action.name, Color.White, Color.Gray, Color.DarkGray ) );

                        if (action.name.Length > boxWidth)
                        {
                            boxWidth = action.name.Length; // set box width for below

                        }
                    }

                // boxes and outlines

                menuRectangle.Width = boxWidth * 12 + 10;  // set width to widest text from above
                menuRectangle.Height = 30;                  // menu item height spread
            }

            // menu update

            if (menu == true)
            {

                menuMouseover = false;
                Vector2 menuItemPosition = menuPosition;

                foreach (MenuAction action in menuActions)
                {


                    // IF MOUSE-OVERED change colours, set to highlighted item

                    if (cursor.position.X >= menuItemPosition.X && cursor.position.X < (menuItemPosition.X + menuRectangle.Width) && cursor.position.Y >= menuItemPosition.Y && cursor.position.Y < (menuItemPosition.Y + menuRectangle.Height))
                    {
                        textColour = Color.Black;
                        boxColour = Color.LightGray;
                        lineColour = Color.LimeGreen;

                        menuHighlightAction = action;
                        menuMouseover = true;
                    }

                    else
                    {
                        textColour = Color.White;
                        boxColour = Color.Gray;
                        lineColour = Color.DarkGray;
                    }

                    int index = menuActions.IndexOf(action);
                    actions[index] = new Tuple<string, Color, Color, Color>(actions[index].Item1, textColour, boxColour, lineColour);

                    if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (cursor.click == false)
                        {
                            menu = false;

                            if (menuMouseover == true)
                            {
                                if (action.scientist == true)
                                {
                                    if (scientist.walking == true)
                                    {
                                        scientist.walking = false;
                                        scientist.floorObject = null;
                                        scientist.action = null;
                                    }

                                    else if (scientist.doing == true)
                                    {
                                        scientist.doing = false;
                                        scientist.corpseWork = false;
                                        scientist.floorObject = null;
                                        bars.Remove(scientist.progBar);
                                        scientist.action = null;
                                        scientist.animStart = true;
                                    }

                                    scientist.action = menuHighlightAction;
                                    scientist.floorObject = this;
                                }

                                if (action.assistant == true && assistant.outside == false && assistant.corpseCarrying == false)
                                {
                                    if (assistant.walking == true)
                                    {
                                        assistant.walking = false;
                                        assistant.floorObject = null;
                                        assistant.action = null;
                                    }

                                    else if (assistant.doing == true)
                                    {
                                        assistant.doing = false;
                                        assistant.digging = false;
                                        assistant.floorObject = null;
                                        bars.Remove(assistant.progBar);
                                        assistant.action = null;
                                        assistant.animStart = true;
                                    }

                                    assistant.action = menuHighlightAction;
                                    assistant.floorObject = this;
                                }

                                // both using same machine?
                                if (action.assistant == true && action.scientist == true)
                                {
                                    assistant.twoWork = true;
                                }

                            }

                        }

                        cursor.click = true;
                    }

                    menuItemPosition = menuItemPosition + new Vector2(0, menuRectangle.Height);

                }
            }
            
        }

        public void Render(SpriteBatch sbatch, SpriteFont font, Cursor cursor)
        {
           
            // object itself
            sbatch.Draw(objectTex, position - offset, rect, Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);

           

            // menu

            if (menu == true)
            {
                Vector2 menuItemPosition = menuPosition;    // position of first text menu item
                
                foreach (Tuple<string,Color,Color,Color> item in actions)
                {
                    textColour = item.Item2;
                    boxColour  = item.Item3;
                    lineColour = item.Item4;

                    string actionName = item.Item1;

                    // text

                    sbatch.DrawString(font, actionName, menuItemPosition + new Vector2(5, 0), textColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.13f);


                    // text rectangle
                    sbatch.Draw(dummyTexture, menuItemPosition, menuRectangle, boxColour, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.17f);

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

            if (mouseOver == true)
            {
               sbatch.DrawString(font, name, cursor.position + textOffset, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.01f);
            }

        }

        public void IconRender(SpriteBatch sbatch, Build build)
        {

            
                sbatch.Draw(iconTex, iconPosition, iconRect, Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, iconLayer);

                if (iconMouseover == true)
                {
                    // top line
                    Tuple<Vector2, Vector2> line = new Tuple<Vector2, Vector2>(iconPosition, (iconPosition + new Vector2(60, 0)));

                    float angle = (float)Math.Atan2(line.Item2.Y - line.Item1.Y, line.Item2.X - line.Item1.X);
                    float length = Vector2.Distance(line.Item1, line.Item2);

                    sbatch.Draw(dummyTexture, line.Item1 + new Vector2(0, 0), null, highlightColour, angle, Vector2.Zero, new Vector2(length, 3.0f), SpriteEffects.None, 0.15f);

                    // right line
                    Tuple<Vector2, Vector2> line2 = new Tuple<Vector2, Vector2>((iconPosition + new Vector2(60, 60)), (iconPosition + new Vector2(60, 0)));

                    float angle2 = (float)Math.Atan2(line2.Item2.Y - line2.Item1.Y, line2.Item2.X - line2.Item1.X);
                    float length2 = Vector2.Distance(line2.Item1, line2.Item2);

                    sbatch.Draw(dummyTexture, line2.Item1 + new Vector2(0, 0), null, highlightColour, angle2, Vector2.Zero, new Vector2(length2, 3.0f), SpriteEffects.None, 0.15f);

                    // left line
                    Tuple<Vector2, Vector2> line3 = new Tuple<Vector2, Vector2>(iconPosition, (iconPosition + new Vector2(0, 60)));

                    float angle3 = (float)Math.Atan2(line3.Item2.Y - line3.Item1.Y, line3.Item2.X - line3.Item1.X);
                    float length3 = Vector2.Distance(line3.Item1, line3.Item2);

                    sbatch.Draw(dummyTexture, line3.Item1 + new Vector2(0, 0), null, highlightColour, angle3, Vector2.Zero, new Vector2(length3, 3.0f), SpriteEffects.None, 0.15f);

                    // bottom line
                    Tuple<Vector2, Vector2> line4 = new Tuple<Vector2, Vector2>((iconPosition + new Vector2(60, 60)), (iconPosition + new Vector2(0, 60)));

                    float angle4 = (float)Math.Atan2(line4.Item2.Y - line4.Item1.Y, line4.Item2.X - line4.Item1.X);
                    float length4 = Vector2.Distance(line4.Item1, line4.Item2);

                    sbatch.Draw(dummyTexture, line4.Item1 + new Vector2(0, 0), null, highlightColour, angle4, Vector2.Zero, new Vector2(length4, 3.0f), SpriteEffects.None, 0.15f);
                }

        }
    }
}
