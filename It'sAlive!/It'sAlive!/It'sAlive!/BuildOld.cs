﻿using System;
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
        private Rectangle rect;

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
        private Rectangle buildRectangle = new Rectangle(0, 0, 1040, 70);
        public Vector2 buildPos = new Vector2(0, 710);

        // set initial variables on frames

        public List<FloorObject> buildList = new List<FloorObject>();

        public Build(Vector2 iconPosition, Texture2D iconTex, Texture2D highlightTex,
                        Texture2D buildArrow, Texture2D highBuildArrow, GraphicsDevice graphicsDevice)
        {
            this.position = iconPosition;
            
            this.tex = iconTex;
            this.hTex = highlightTex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;

            this.arrow = buildArrow;
            this.highArrow = highBuildArrow;

            this.dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            this.dummyTexture.SetData(new Color[] { Color.Gray });
        }

        public void Add(FloorObject item)
        {
            buildList.Add(item);
            item.onBuildList = true;

            int rank = buildList.IndexOf(item);

            item.iconPosition = new Vector2(120 + 80 * rank, 715);

            if (rank > buildScreenLength)
            {
                item.onBuildList = false;
            }
        }

        public void Remove(FloorObject item, NumericalCounter money)
        {
            buildList.Remove(item);
            money.value -= item.cost;

            foreach (FloorObject curitem in buildList)
            {
                int rank = buildList.IndexOf(curitem);
                curitem.iconPosition = new Vector2(120 + 80 * rank, 715);

                if (rank > buildScreenLength)
                {
                    curitem.onBuildList = false;
                }

                else
                {
                    curitem.onBuildList = true;
                }
            }
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

        public void Update(Cursor cursor)
        {
            MouseState mouseState = Mouse.GetState();

            



        }

        public void Render(SpriteBatch sbatch,Cursor cursor)
        {
            // icon

            if (cursor.position.X >= position.X && cursor.position.X < (position.X + tex.Width) && cursor.position.Y >= position.Y && cursor.position.Y < (position.Y + tex.Height))
            {
                sbatch.Draw(hTex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            }

            else
            {
                sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
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
        }
    }
}

