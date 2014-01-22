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
    class SlideGauge
    {
        public Vector2 position;
        public Vector2 handPosition;
        public float layer;
        private Texture2D tex;
        private Texture2D handTex;
        private Rectangle rect;
        private Rectangle handRect;

        private int min;
        private int max;
        
        private bool jitter;
        private int jitterSpeed;
        private int timer;

        private float value;
        private float extent;
        private float reaction;

        private MachineDependentParameter param;

        public SlideGauge(Vector2 itemPosition, float drawLayer, Texture2D gaugeTex, Texture2D handTex, MachineDependentParameter param, float extent, bool jitter = true, int jitterSpeed = 0, float reactionSpeed = 0.01f)
        {
            this.position = itemPosition;
            this.layer = drawLayer;
            this.tex = gaugeTex;
            this.handTex = handTex;

            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.handRect.Width = handTex.Width;
            this.handRect.Height = handTex.Height;

            this.extent = extent;
            this.handPosition = new Vector2(position.X + (rect.Width * (1.0f - extent)) / 2.0f + ((rect.Width * extent - handRect.Width) / (max - min)) * value,
                                                position.Y + (rect.Height - handRect.Height) / 2.0f);

            this.min = param.min;
            this.max = param.max;
            this.value = param.value;

            this.jitter = jitter;
            this.jitterSpeed = jitterSpeed;
            this.reaction = reactionSpeed;

            this.param = param;
        }


        public void Update(Random rand, GameTime gameTime)
        {
            param.Update();
            float nvalue = param.value; 
            
            // apply jitter - pseudo-Gaussian variation

            if (jitter == true)
            {
                timer += gameTime.ElapsedGameTime.Milliseconds;

                if (timer > jitterSpeed)
                {
                    timer = 0;

                    float jit = (max - min) * 0.005f * (float)(rand.Next(10) * rand.Next(10) * rand.Next(10));
                    int dir = rand.Next(2);

                    if (dir == 0) { jit *= -1; }

                    nvalue += jit;
                }
            }

            // move hand towards current value

            value += (nvalue - value) * reaction;

            // check for range containment

            if (value > max) { value = max; }
            if (value < min) { value = min; }

            // update position for value

            handPosition.X = position.X + (rect.Width * (1.0f - extent)) / 2.0f + ((rect.Width * extent - handRect.Width) / (max - min)) * value;

        }

        public void Render(SpriteBatch sbatch, SpriteFont font)
        {
            sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            sbatch.Draw(handTex, handPosition, handRect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer - 0.01f);

            sbatch.DrawString(font, value.ToString(), position + new Vector2(80, 80), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);

        }
    }
}