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
    class FlySwarm
    {
        public Vector2 position;
        public double width;
        public int nflies;

        private Texture2D dummyTexture;
        private Rectangle flyrect;
        private float layer;

        private Random random;

        // list of flies - position of ellipse, radius, eccentricity(foci, +ve), angle of fly's position, speed
        public List<Tuple<Vector2, float, float, float, float, float>> flies = new List<Tuple<Vector2, float, float, float, float, float>> { };

        public FlySwarm(Vector2 position, double width, int numberOfFlies,float layer, GraphicsDevice graphicsDevice, Random random)
        {
            this.position = position;
            this.width = width;
            this.nflies = numberOfFlies;

            this.dummyTexture = new Texture2D(graphicsDevice, 1, 1);
            this.dummyTexture.SetData(new Color[] { Color.Black });
            this.flyrect = new Rectangle(0, 0, 2, 2);
            this.layer = layer;

            this.random = random;

            for (int i = 0; i < nflies; i++)
            {             
                // initial params
                int polarity = random.Next(2);
                if (polarity == 0) { polarity -= 1; }
                float angle = (float) (random.NextDouble() * 2.0 * Math.PI);
                float radius = (float)(Math.Pow(random.NextDouble(), 0.8) * width + 0.05*width);
                float angleOfDangle = (float)(random.NextDouble() * 4.0 * Math.PI - 2.0 * Math.PI);
                float speed = (float)(random.NextDouble() * random.NextDouble() *6*polarity + 1)/(float)(radius);
                //                                                               position         radius, ecc  ,angle,speed,angle of the dangle
                flies.Add(new Tuple<Vector2, float, float, float, float, float>(new Vector2(0, 0), radius, 0.5f, angle, speed, angleOfDangle));
            }
        }

        public void Restart(int n)
        {
            nflies = n;

            flies = new List<Tuple<Vector2, float, float, float, float, float>> { };

            for (int i = 0; i < nflies; i++)
            {
                // initial params
                int polarity = random.Next(2);
                if (polarity == 0) { polarity -= 1; }
                float angle = (float)(random.NextDouble() * 2.0 * Math.PI);
                float radius = (float)(Math.Pow(random.NextDouble(), 0.8) * width + 0.05 * width);
                float angleOfDangle = (float)(random.NextDouble() * 4.0 * Math.PI - 2.0 * Math.PI);
                float speed = (float)(random.NextDouble() * random.NextDouble() * 8 * polarity + 1) / (float)(radius);
                //                                                               position         radius, ecc  ,angle,speed,angle of the dangle
                flies.Add(new Tuple<Vector2, float, float, float, float, float>(new Vector2(0, 0), radius, 0.5f, angle, speed, angleOfDangle));
            }
        }

        public void AddFlies(int n)
        {
            for (int i = 0; i < n; i++)
            {
                // initial params
                int polarity = random.Next(2);
                if (polarity == 0) { polarity -= 1; }
                float angle = (float)(random.NextDouble() * 2.0 * Math.PI);
                float radius = (float)(Math.Pow(random.NextDouble(), 0.8) * width + 0.05 * width);
                float angleOfDangle = (float)(random.NextDouble() * 4.0 * Math.PI - 2.0 * Math.PI);
                float speed = (float)(random.NextDouble() * random.NextDouble() * 8 * polarity + 1) / (float)(radius);
                //                                                               position         radius, ecc  ,angle,speed,angle of the dangle
                flies.Add(new Tuple<Vector2, float, float, float, float, float>(new Vector2(0, 0), radius, 0.5f, angle, speed, angleOfDangle));
            }
        }

        public void RemoveFlies(int n)
        {
            for (int i = 0; i < n; i++)
            {
                flies.RemoveAt(0);
            }
        }

        public void Update()
        {
            if (flies.Count > 0)
            {
                List<Tuple<Vector2, float, float, float, float, float>> movedFlies = new List<Tuple<Vector2, float, float, float, float, float>> { };

                foreach (Tuple<Vector2, float, float, float, float, float> fly in flies)
                {
                    // calculate drifts

                    float radiusDrift = 0;
                    if (fly.Item2 > 0.05 * width)
                    {
                        radiusDrift = 1.0f + (float)((random.NextDouble() - 0.5f) * 0.02f);
                    }
                    else
                    {
                        radiusDrift = 1.0f + (float)((random.NextDouble()) * 0.02f);
                    }

                    float eccDrift = 1.0f + (float)((random.NextDouble() - 0.5f) * 0.02f);
                    float angleOfDangleDrift = (float)((random.NextDouble() - 0.5f) * 0.02f) / (fly.Item2 * radiusDrift);

                    // add modified tuples to list of new positions and attributes
                    movedFlies.Add(new Tuple<Vector2, float, float, float, float, float>(fly.Item1, fly.Item2 * radiusDrift, fly.Item3 * eccDrift, fly.Item4 + fly.Item5, fly.Item5, fly.Item6 + angleOfDangleDrift));
                }

                flies = movedFlies;
            }
        }


        public void Render(SpriteBatch sbatch)
        {
            if (flies.Count > 0)
            {
                foreach (Tuple<Vector2, float, float, float, float, float> fly in flies)
                {
                    // position on personal ellipse
                    float x = fly.Item3 * fly.Item2 * (float)Math.Cos(fly.Item4);
                    float y = fly.Item2 * (float)Math.Sin(fly.Item4);

                    // rotate ellipse
                    float rx = x * (float)Math.Cos(fly.Item6) - y * (float)Math.Sin(fly.Item6);
                    float ry = x * (float)Math.Sin(fly.Item6) + y * (float)Math.Cos(fly.Item6);

                    // add global position plus personal offset
                    Vector2 pos = position + fly.Item1 + new Vector2(rx, ry);

                    sbatch.Draw(dummyTexture, pos, flyrect, Color.Black, 0, Vector2.Zero, 1.0f, SpriteEffects.None, layer);
                }
            }
        }
    }
}
