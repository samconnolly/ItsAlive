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
    class Corpse
    {
        public Vector2 position;
        public float layer = 0.2f;
        private Texture2D tex;
        private Rectangle rect;
        public bool visible;
        private float scale = 1.0f;

        public int width;
        public int height;
        public int fwidth;
        public List<MenuAction> aliveMenuActions;
        public List<MenuAction> deadMenuActions;
        public List<MenuAction> workMenuActions;
        private int frames = 1;
        private int frate = 80;
        private int currentFrame = 0;

        private int anim = 0;
        private List<int> nframes = new List<int>{1, 1, 1,1};

        private int timer = 0;
        private int rotTimer = 0;
        private int rotTime = 60;
        public NumericalCounter rot;
        public int cut = 0;

        public bool alive = false;
        public bool dead = false;
        public bool talking = false;

        // menu
        public bool corpseMenu = false;
        public bool menuOpen = false;
        public bool menuMouseover = false;
        private Vector2 menuPosition;
        private MenuAction menuHighlightAction;
        private List<MenuAction> menuActions;
        private List<Tuple<string, Color, Color, Color>> actions;

        private Rectangle menuRectangle;
        private Texture2D dummyTexture;

        private Color textColour;
        private Color boxColour;
        private Color lineColour;


        public Corpse(Vector2 position,Texture2D itemTex, List<MenuAction> workMenuActions, List<MenuAction> aliveMenuActions,List<MenuAction> deadMenuActions,GraphicsDevice graphicsDevice, SpriteFont font)
        {
            this.position = position;
            this.tex = itemTex;
            this.width = tex.Width/1;
            this.height = tex.Height/4;
            this.fwidth = tex.Width / 1;
            this.rect.Width = fwidth;
            this.rect.Height = height;
            this.aliveMenuActions = aliveMenuActions;
            this.deadMenuActions = deadMenuActions;
            this.workMenuActions = workMenuActions;

            // menu text
            dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            dummyTexture.SetData(new Color[] { Color.Gray });

            // rot counter
            this.rot = new NumericalCounter("Rot", new Vector2(10, 380), 4, 3, 0, 0, font, Color.Red, Color.Red);
        }

        public void Die()
        {
            alive = false;
            dead = true;
            cut = 3;
            frames = nframes[3];            
        }

        public void Update(GameTime gameTime, MenuAction dissect, MenuAction study,NumericalCounter longevity, NumericalCounter humanity, NumericalCounter lifeforce, Scientist scientist, Assistant assistant,
                                MenuAction talk, Cursor cursor,List<MiniProgressBar> bars, MenuAction clearCorpse)
        {
            cursor.corpseMouseover = false;

            if (this.visible == true)
            {


                // update cut up-ness
                if (dead == false)
                {
                    cut = dissect.count * 3 + study.count;
                }

                if (cut > 3)
                {
                    cut = 3;
                }


                // check for mouseover & click
                               

                if (cursor.position.X >= position.X && cursor.position.X <= position.X + fwidth*scale
                           && cursor.position.Y >= position.Y  && cursor.position.Y <= position.Y + height  && visible == true && corpseMenu == false)
                {

                    cursor.corpseMouseover = true; // add object mouseover text

                    // turn on menu when object is clicked

                    if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                    {
                        if (cursor.click == false)
                        {
                            corpseMenu = true;
                            menuOpen = true;
                            cursor.corpseMouseover = false;
                        }

                        cursor.click = true;
                    }                    
                }                


                // rotting timer and count

                timer += gameTime.ElapsedGameTime.Milliseconds;
                rotTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (rotTimer >= rotTime * 1000.0 && alive == false && dead == false)
                {
                    rotTimer = 0;

                    if (rot.value > 1)
                    {
                        rot.value -= 1;
                    }

                    else
                    {
                        Die();
                        rot.value = 3;
                    }
                }

                // live corpse dead count && anim change

                if (alive == true)
                {
                    longevity.valueChange = -0.01f;

                    if (longevity.value == 0)
                    {
                        Die();
                    }

                    if (scientist.action == talk && scientist.doing == true)
                    {

                        talking = true;
                        anim = 2;
                        frames = nframes[2];
                    }
                    else
                    {
                        talking = false;
                        anim = 1;
                        frames = nframes[1];
                    }

                }

                else if (dead == false)
                {
                    longevity.valueChange = 0;

                    anim = 0;
                    frames = nframes[0];
                }

                // kill if dissected

                if (dissect.done == true)
                {
                    Die();
                }

                // kill if studies 3 times

                if (study.count >= 3)
                {
                    Die();
                }

                // update anim

                if (timer >= frate)
                {
                    timer = 0;

                    if (currentFrame++ == frames - 1)
                    {
                        currentFrame = 0;
                    }

                    rect.Y = cut * height + currentFrame; // * fwidth
                }



                // menu setup

                if (corpseMenu == true && menuOpen == true)
                {
                    menuPosition = cursor.position;
                    menuOpen = false;

                    // find longest string in menu

                    int boxWidth = 0;
                    actions = new List<Tuple<string, Color, Color, Color>> { };

                    if (alive == false && dead == false)
                    {
                        menuActions = workMenuActions;

                        foreach (MenuAction action in workMenuActions)
                        {  
                            // add
                            actions.Add(new Tuple<string, Color, Color, Color>(action.name, Color.White, Color.Gray, Color.DarkGray));

                            if (action.name.Length > boxWidth)
                            {
                                boxWidth = action.name.Length; // set box width for below

                            }
                        }
                    }

                    if (alive == true)
                    {
                        menuActions = aliveMenuActions;
                                                
                        foreach (MenuAction action in aliveMenuActions)
                        {
                            // add
                            actions.Add(new Tuple<string, Color, Color, Color>(action.name, Color.White, Color.Gray, Color.DarkGray));

                            if (action.name.Length > boxWidth)
                            {
                                boxWidth = action.name.Length; // set box width for below

                            }
                        }
                    }

                    if (dead == true)
                    {
                        menuActions = deadMenuActions;

                        foreach (MenuAction action in deadMenuActions)
                        {
                            // add
                            actions.Add(new Tuple<string, Color, Color, Color>(action.name, Color.White, Color.Gray, Color.DarkGray));

                            if (action.name.Length > boxWidth)
                            {
                                boxWidth = action.name.Length; // set box width for below

                            }
                        }
                    }

                    // boxes and outlines

                    menuRectangle.Width = boxWidth * 12 + 10;  // set width to widest text from above
                    menuRectangle.Height = 30;                  // menu item height spread
                }

                // menu update

                if (corpseMenu == true)
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

                            if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                            {
                                if (cursor.click == false)
                                {
                                    corpseMenu = false;
                                    
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
                                            scientist.corpseWork = true;
                                        }

                                        if (action.assistant == true)
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
                                            assistant.corpseWork = true;

                                        }

                                    }

                                

                                cursor.click = true;
                            }
                        }

                        else
                        {
                            textColour = Color.White;
                            boxColour = Color.Gray;
                            lineColour = Color.DarkGray;

                            if (cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed)
                            {
                                if (cursor.click == false)
                                {
                                    corpseMenu = false;
                                }
                            }
                        }

                        int index = menuActions.IndexOf(action);
                        actions[index] = new Tuple<string, Color, Color, Color>(actions[index].Item1, textColour, boxColour, lineColour);

                       

                        menuItemPosition = menuItemPosition + new Vector2(0, menuRectangle.Height);

                    }
                }
            }

            // check for cleared up dead corpse, reset corpse if so.

            if (clearCorpse.count > 0)
            {
                clearCorpse.count = 0;
                visible = false;
                dead = false;
                rot.value = 3;                
            }
            
            // update the rot counter
            rot.Update(gameTime);
        }

        public void Render(SpriteBatch sbatch,SpriteFont font)
        {
            // corpse
            if (visible == true)
            {
                sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, scale, SpriteEffects.None, layer);
            }

            // menu

            if (corpseMenu == true)
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

