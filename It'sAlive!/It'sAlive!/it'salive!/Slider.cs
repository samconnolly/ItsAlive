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
    class Slider
    {
        public Vector2 position;
        public Vector2 knobPosition;
        public float layer;
        private Texture2D tex;
        private Texture2D knobTex;
        private Rectangle rect;
        private Rectangle knobRect;
        private bool start = true;
        public float value = 0;
        private float mouseNum = 0;
        private int min;
        private int max;
        private float extent;

        private MachineControlParameter param;

        public Slider(Vector2 position, float drawLayer, Texture2D sliderTex, Texture2D sliderKnobTex,MachineControlParameter param, float slideExtent)
        {
            this.position = position;
            this.layer = drawLayer;
            this.tex = sliderTex;
            this.knobTex = sliderKnobTex;
            this.rect.Width = tex.Width;
            this.rect.Height = tex.Height;
            this.knobRect.Width = knobTex.Width;
            this.knobRect.Height = knobTex.Height;

            this.min = param.min;
            this.max = param.max;
            this.value = param.value;
            this.extent = slideExtent;

            this.knobPosition = new Vector2(position.X + (rect.Width * (1.0f - extent)) / 2.0f + ((max - min) / (rect.Width * extent - knobRect.Width)) * value,
                                                position.Y + (rect.Height - knobRect.Height)/2.0f);
            this.param = param;

        }


        public void Update(Cursor cursor)
        {

            // check cursor value adjust
            if (cursor.position.X >= knobPosition.X && cursor.position.X <= knobPosition.X + rect.Width
                 && cursor.position.Y >= knobPosition.Y && cursor.position.Y <= knobPosition.Y + rect.Height
                  && cursor.mouseState.LeftButton == ButtonState.Pressed | cursor.mouseState.RightButton == ButtonState.Pressed
                   && cursor.click == false)
            {
                if (start == true)
                {
                    cursor.click = true;
                    start = false;
                    mouseNum = cursor.position.X;
                }
            }
            if (cursor.position.X >= knobPosition.X && cursor.position.X <= knobPosition.X + rect.Width
                 && cursor.position.Y >= knobPosition.Y && cursor.position.Y <= knobPosition.Y + rect.Height
                  && start == false)
            {
                value += (cursor.position.X - mouseNum) * (max - min) / (rect.Width*extent - knobRect.Width);
                knobPosition.X += cursor.position.X - mouseNum;
                mouseNum = cursor.position.X;

                if (value > max) { value = max; }
                if (value < min) { value = min; }

                if (knobPosition.X > position.X + rect.Width * extent + (rect.Width * (1.0f - extent)) / 2.0f - knobTex.Width)
                {
                    knobPosition.X = position.X + rect.Width * extent + (rect.Width * (1.0f - extent)) / 2.0f - knobTex.Width;
                }

                if (knobPosition.X < position.X + (rect.Width * (1.0f - extent)) / 2.0f)
                {
                    knobPosition.X = position.X + (rect.Width * (1.0f - extent)) / 2.0f;
                }
            }

            param.value = value;


            if (cursor.mouseState.LeftButton == ButtonState.Released && cursor.mouseState.RightButton == ButtonState.Released)
            {
                start = true;
            }
            
        }

        public void Render(SpriteBatch sbatch, SpriteFont font)
        {
            sbatch.Draw(tex, position, rect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
            sbatch.Draw(knobTex, knobPosition, knobRect, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer - 0.01f);

            sbatch.DrawString(font, value.ToString(), position + new Vector2(80, 80), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f);

        }
    }
}

