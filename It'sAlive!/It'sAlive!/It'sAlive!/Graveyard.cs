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
        public Vector2 tlcorner;
        public Vector2 brcorner;
        public List<MenuAction> menuActions;

        // menu
        private bool menu = false;
        private bool menuMouseover = false;
        private bool menuStart = false;
        public Rectangle menuRectangle;
        public Vector2 menuPosition;

        private Texture2D dummyTexture;
        private List<Tuple<string, Color, Color, Color>> actions;
        private MenuAction menuHighlightAction = null;

        //  colours
        Color textColour;
        Color boxColour;
        Color lineColour;

        public Graveyard(Vector2 position, float drawLayer, Texture2D tex, Vector2 topLeftWindowCorner, Vector2 bottomRightWindowCorner, List<MenuAction> menuActions, GraphicsDevice graphicsDevice)
        {
            this.position = position;
            this.layer = drawLayer;
            this.tex = tex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.tlcorner = topLeftWindowCorner;
            this.brcorner = bottomRightWindowCorner;
            this.menuActions = menuActions;


            // menu tex
            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Gray });
        }

        public void Update(GameTime gameTime, Cursor cursor, List<FloorObject> floorObjectList, FloorObject table, Assistant assistant, Corpse corpse, List<MiniProgressBar> bars)
        {

            // check for mouseover and click

            if (menu == false)
            {

                cursor.graveMouseOver = false;
                
                // graveyard
                if (cursor.position.X >= tlcorner.X && cursor.position.X <= brcorner.X
                        && cursor.position.Y >= tlcorner.Y && cursor.position.Y <= brcorner.Y)
                {

                    cursor.graveMouseOver = true; // add object mouseover text

                    // turn on menu when object is clicked

                    if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (cursor.click == false)
                        {
                            menu = true;
                            menuStart = true;
                            menuPosition = cursor.position;
                            cursor.graveMouseOver = false;
                        }

                        cursor.click = true;

                    }                    
                }
            }

            if (menu == true)
            {


                if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                {
                    if (cursor.click == false)
                    {

                        menu = false;

                        if (menuMouseover == true)
                        {
                            if (floorObjectList.Contains(table))
                            {
                                assistant.DigUpCorpse(corpse);
                            }
                        }

                    }

                    cursor.click = true;
                }

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
                    actions.Add(new Tuple<string, Color, Color, Color>(action.name, Color.White, Color.Gray, Color.DarkGray));

                    if (action.name.Length > boxWidth)
                    {
                        boxWidth = action.name.Length; // set box width for below

                    }
                }

                // boxes and outlines

                menuRectangle.Width = boxWidth * 12 + 10;  // set width to widest text from above
                menuRectangle.Height = 30;                  // menu item height spread
            }

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

            // draw graveyard
            sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);

             // draw menu

            if (menu == true)
            {
                Vector2 menuItemPosition = menuPosition;    // position of first text menu item

                foreach (Tuple<string, Color, Color, Color> item in actions)
                {
                    textColour = item.Item2;
                    boxColour = item.Item3;
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
        }
    }
}

