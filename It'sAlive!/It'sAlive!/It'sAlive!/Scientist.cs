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
    class Scientist
    {


        public Texture2D charTex;
        private Rectangle rect;

        public int width = 0;
        public int height = 0;
        public int timer = 0;
        public int msecsTweenFrames = 120;
        public int currentFrame = 0;
        public int numberOfFrames = 4;
        public int numberOfAnims = 5;
        public int anim = 0;
        private float scale = 1.0f;
        private float layer;

        public bool walking = false;
        public bool doing = false;
        public bool corpseWork = false;
        public bool animStart = true;

        public Vector2 gridPosition = Vector2.Zero;
        public Vector2 position = Vector2.Zero;
        public Vector2 walkingOffset = Vector2.Zero;
        public Vector2 walkingTarget = Vector2.Zero;
        public Vector2 direction = Vector2.Zero;
        public double targetDistance = 0;
        public double distanceGone = 0;
        public Vector2 offset = Vector2.Zero;
        public int pathStep = 0;

        public MenuAction action = null;
        public FloorObject floorObject = null;

        // The public instance of the object

        public Scientist(Texture2D texture, Vector2 gridPosition, Grid grid)
        {

            this.charTex = texture;
            this.width = texture.Width / numberOfFrames;
            this.height = texture.Height / numberOfAnims;

            this.gridPosition = gridPosition;
            this.layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y;
            // Create rectangle for animation

            rect.X = 0;
            rect.Y = 0;
            rect.Width = width;
            rect.Height = height;


        }


        public List<Vector2> Update(GameTime gametime, GraphicsDevice graphicsDevice, Grid grid, Cursor cursor, Path path, List<Vector2> drawPath,NumericalCounter research,NumericalCounter money, NumericalCounter madness, List<MiniProgressBar> proglist)
        {


            // update position from grid position

            
            position = grid.CartesianCoords(gridPosition);
            layer = 0.2f + (0.2f / (float)grid.rows) * gridPosition.Y - 0.01f;

            offset = new Vector2((width * scale) / 2.0f, height * scale * 2.0f); // factor of 2 here and in the draw command are just for this test anim, so it's a decent size...

            // walking....

            if (action != null && walking == false && doing == false)
            {

                if (corpseWork == true)
                {
                    if (gridPosition != new Vector2(5, 4))
                    {
                        walking = true;
                        drawPath = path.PathList(gridPosition, new Vector2(5, 4), grid);
                        pathStep = 1;
                        walkingTarget = drawPath[pathStep];
                    }

                    else
                    {
                        doing = true;
                        currentFrame = 0;
                        gridPosition = drawPath[pathStep - 1];
                        position = grid.CartesianCoords(gridPosition);
                    }
                }

                else if (gridPosition != cursor.menuObject.gridPosition + new Vector2(0, -1))
                {
                    walking = true;
                    drawPath = path.PathList(gridPosition, cursor.menuObject.gridPosition + new Vector2(0, -1), grid);
                    pathStep = 1;
                    walkingTarget = drawPath[pathStep];
                }

                else
                {                    
                    doing = true;
                    currentFrame = 0;
                    gridPosition = drawPath[pathStep - 1];
                    position = grid.CartesianCoords(gridPosition);
                }

                //cursor.scientistAction = "none";
            }

            if (walking == true)
            {
                direction = (grid.CartesianCoords(walkingTarget) - grid.CartesianCoords(gridPosition));
                targetDistance = direction.Length();
                direction.Normalize();

                walkingOffset += direction * gametime.ElapsedGameTime.Milliseconds * 0.2f;
                distanceGone = walkingOffset.Length();

                if (distanceGone < targetDistance)
                {
                    Vector2 move = walkingOffset;
                    position += move;
                }

                else
                {
                    pathStep += 1;


                    if (pathStep < drawPath.Count)
                    {
                        gridPosition = drawPath[pathStep - 1];
                        walkingTarget = drawPath[pathStep];
                        position = grid.CartesianCoords(gridPosition);
                    }

                    else
                    {
                        walking = false;
                        doing = true;
                        currentFrame = 0;
                        gridPosition = drawPath[pathStep-1];
                        position = grid.CartesianCoords(gridPosition);
                    }

                    walkingOffset = Vector2.Zero;
                    targetDistance = 0;
                    distanceGone = 0;


                }
            }

            // update animation frame

            timer += gametime.ElapsedGameTime.Milliseconds;


            if (timer >= msecsTweenFrames)
            {
                timer = 0;

                if (walking == true)
                {
                    if (Math.Abs(direction.X) >= Math.Abs(direction.Y))
                    {
                        if (direction.X >= 0)
                        {
                            anim = 1;
                        }

                        else
                        {
                            anim = 0;
                        }

                        if (direction.Y >= 0)
                        {
                            layer -= 0.2f / (float)grid.rows + 0.01f;
                        }
                    }

                    if (Math.Abs(direction.Y) >= Math.Abs(direction.X))
                    {
                        if (direction.Y >= 0)
                        {
                            anim = 2;
                            layer -= 0.2f / (float)grid.rows + 0.01f;
                        }

                        else
                        {
                            anim = 3;
                        }
                    }

                    if (currentFrame++ == numberOfFrames - 1)
                    {
                        currentFrame = 0;
                    }
                }


                // start up menu action if doing is done, animate doing
                if (doing == true)
                {
                    anim = 4;

                    if (action.remain == false)
                    {
                        if (currentFrame++ == numberOfFrames - 1)
                        {
                            doing = false;
                            proglist.Add(new MiniProgressBar(graphicsDevice, floorObject.position + new Vector2(-5, -105), action, floorObject));

                            action = null;
                            
                        }
                    }

                    else
                    {
                        if (animStart == true)
                        {
                            animStart = false;

                            if (corpseWork == true)
                            {
                                proglist.Add(new MiniProgressBar(graphicsDevice, position + new Vector2(0, -100), action,floorObject));

                            }

                            else
                            {
                                proglist.Add(new MiniProgressBar(graphicsDevice, floorObject.position + new Vector2(-5, -105), action,floorObject));
                            }
                        }

                        else
                        {
                            if (currentFrame++ == numberOfFrames - 1)
                            {
                                currentFrame = 0;

                                if (action.done == true)
                                {
                                    doing = false;
                                    action.done = false;
                                    action = null;
                                    animStart = true;
                                    
                                    if (corpseWork == true)
                                    {
                                        corpseWork = false;
                                    }
                                }
                            }


                        }
                    }

                }

                if (walking == false && doing == false)
                {
                    anim = 2;
                    currentFrame = 0;
                }

                rect.X = currentFrame * width;
                rect.Y = height * anim;
            }

            return drawPath;

        }

        public void Render(SpriteBatch sbatch)
        {
            // scaling from perspective grid class!

            sbatch.Draw(charTex, position - offset, rect, Color.White, 0, Vector2.Zero, scale * 2.0f, SpriteEffects.None, layer);


        }
    }


}

