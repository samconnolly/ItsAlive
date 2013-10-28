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
        public Vector2 position = new Vector2(430, 560);
        public float layer = 0.29f;
        private Texture2D tex;
        private Rectangle rect;
        public bool visible;

        public int width;
        public int height;
        public int fwidth;
        public List<MenuAction> aliveMenuActions;
        public List<MenuAction> deadMenuActions;
        private int frames = 1;
        private int frate = 80;
        private int currentFrame = 0;

        private int anim = 0;
        private List<int> nframes = new List<int>{1, 3, 3};

        private int timer = 0;
        private int rotTimer = 0;
        private int rotTime = 60;
        public int rot = 3;

        public bool alive = false;
        public bool talking = false;

        public Corpse(Texture2D itemTex, List<MenuAction> deadMenuActions, List<MenuAction> aliveMenuActions)
        {
            
            this.tex = itemTex;
            this.width = tex.Width/3;
            this.height = tex.Height/3;
            this.fwidth = tex.Width / 9;
            this.rect.Width = fwidth;
            this.rect.Height = height;
            this.aliveMenuActions = aliveMenuActions;
            this.deadMenuActions = deadMenuActions;
        }

        public void Update(GameTime gameTime, MenuAction dissect, MenuAction study,NumericalCounter longevity, NumericalCounter humanity, NumericalCounter lifeforce, Scientist scientist, MenuAction talk)
        {
            if (this.visible == true)
            {

                timer += gameTime.ElapsedGameTime.Milliseconds;
                rotTimer += gameTime.ElapsedGameTime.Milliseconds;

                if (rotTimer >= rotTime * 1000.0 && alive == false)
                {
                    rotTimer = 0;

                    if (rot > 1)
                    {
                        rot -= 1;
                        dissect.research -= 18;
                        dissect.madness -= 10;
                        
                    }

                    else
                    {
                        this.visible = false;
                        rot = 3;
                        dissect.research = 54;
                        dissect.madness = 30;
                    }
                }

                if (alive == true)
                {
                    longevity.valueChange = -0.01f;

                    if (longevity.value == 0)
                    {
                        alive = false;
                        visible = false;
                    }

                    if (scientist.action == talk && scientist.doing == true   )
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

                else
                {                    
                    longevity.valueChange = 0;

                    anim = 0;
                    frames = nframes[0];
                }


                if (dissect.done == true)
                {
                    this.visible = false;
                }

                if (study.count >= 3)
                {
                    study.count = 0;
                    this.visible = false;
                }
                
                if (timer >= frate)
                {
                    timer = 0;

                    if (currentFrame++ == frames - 1)
                    {
                        currentFrame = 0;
                    }

                    rect.X = (3 - rot)*width + currentFrame* fwidth;
                    rect.Y = anim * height;
                }

            }
        }

        public void Render(SpriteBatch sbatch)
        {
            if (visible == true)
            {
                sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            }
        }
    }
}

