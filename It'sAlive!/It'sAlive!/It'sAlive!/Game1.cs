using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.CSharp;


namespace It_sAlive_
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            // set window size
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            
            graphics.IsFullScreen = false;
                        
            Content.RootDirectory = "Content";
        }

     
        protected override void Initialize()
        {
           base.Initialize();
        }

        // ====== Declare things... =============

        // viewport

        float xScale;
        float yScale;
        bool f1Down = false;

        // Random number seed
        Random random = new Random();
                
        // Input Data Table
        DataTable machines;
        
        // cursor
        Cursor cursor;
        
        // Backdrops
        NonInteractive room;
        NonInteractive door;
        NonInteractive digger;
        NonInteractive Switch;
        Graveyard graveyard;
        

        // counters
        NumericalCounter research;
        NumericalCounter madness;
        NumericalCounter money;
        NumericalCounter papers;
        NumericalCounter lifeForce;
        NumericalCounter longevity;
        NumericalCounter humanity;

        // mini progress bars
        List<MiniProgressBar> progBars = new List<MiniProgressBar> { };

        // buttons
        Build build;
        Resurrect resurrect;
        
        // characters
        Scientist Simon;
        Assistant Jeremy;

        // objects
        FloorObject table;
        FloorObject desk;
        FloorObject bookcase;
        FloorObject electricalTransmitter;
        FloorObject lightningAbsorber;
        FloorObject steamComputer;
        FloorObject injectionSystem;
        
        // the corpse!
        Corpse corpse;

        // lists
        List<FloorObject> floorObjectList  = new List<FloorObject>();
        List<MenuAction> actionUpdateList = new List<MenuAction>();
        List<MenuAction> menuActions;

        //navigation grid, reachable area thereof;
        Grid grid;
        ReachableArea reachable;
     
        // --- menu actions ----

        // independent
        MenuAction turnOn = new MenuAction("Turn on",true,false,2,5,1,0,0,0,0,true,true);
        MenuAction turnOff = new MenuAction("Turn off", true,false, 2, 5, 1, 0, 0, 0, 0, false,false,true);
        MenuAction getCorpse = new MenuAction("Exhume Cadaver", true,false, 15, 0, 1, 0, 0, 0, 0, true);
        MenuAction studyCorpse = new MenuAction("Study Corpse", true, false, 5, 10, 6, 0, 0, 0, 0, true);
        MenuAction dissectCorpse = new MenuAction("Dissect Corpse", true,false, 7, 18 * 3, 3 * 10, 0, 0, 0, 0, true);
        MenuAction study = new MenuAction("Study", true,false, 10, 50, 0, 0, 0, 0, 0, true); 

        // dependent
        MenuAction partWork;
        MenuAction writePaper;
        MenuAction inject;
        MenuAction compute;
        MenuAction electric;
        MenuAction talk;
        MenuAction studyLiveCorpse;


        // -- fonts --

        SpriteFont cursorFont;
        SpriteFont counterFont;

        // test shizz.............

        positionTextBlob blob;


        // === load things... ==============

        protected override void LoadContent()
        {
            double aspectRatio = (16.0f / 9.0f);
            int targetWidth = 1920;
            int targetHeight = 1080;

            GraphicsDevice.Viewport = new Viewport(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                                            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
           
            int width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int height = (int)(width / aspectRatio);
            if (height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
            {
                height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                width = (int)(height * aspectRatio);
            }

            xScale = (float)((double)width / (double)targetWidth);
            yScale = (float)((double)height / (double)targetHeight);

            int xOffset = (int)((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - width) / 2.0);
            int yOffset = (int)((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - height) / 2.0);

            GraphicsDevice.Viewport = new Viewport(xOffset, yOffset, width,height);
           

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // fonts

            cursorFont = Content.Load<SpriteFont>("font");
            counterFont = Content.Load<SpriteFont>("font2");

            // grid

            grid = new Grid(1900, 1200, 250, 10, 10, new Vector2(10, 1070), false, Content.Load<Texture2D>("rball"),Content.Load<Texture2D>("gball"));
            reachable = new ReachableArea(grid, floorObjectList);

            // HUD

            // buttons
            build = new Build(new Vector2(5, 900), Content.Load<Texture2D>("build_icon_standard"), Content.Load<Texture2D>("build_icon_highlight"), Content.Load<Texture2D>("build_icon_pressed"), Content.Load<Texture2D>("invarrow"), Content.Load<Texture2D>("highinvarrow"), GraphicsDevice);
            resurrect = new Resurrect(new Vector2(1750, 900), Content.Load<Texture2D>("raise_icon_standard"), Content.Load<Texture2D>("raise_icon_highlight"), Content.Load<Texture2D>("raise_icon_pressed"), GraphicsDevice);
            
            // counters
            research = new NumericalCounter("Research", new Vector2(25, 15), 0, 100, 0, 0, counterFont, Color.Green, Color.Green, Content.Load<Texture2D>("counter_box"));
            madness = new NumericalCounter("Madness", new Vector2(25, 85), 0, 0, 0, 0, counterFont, Color.Red, Color.Green, Content.Load<Texture2D>("counter_box"));
            money = new NumericalCounter("Money", new Vector2(25, 155), 0, 500, 0, 60, counterFont, Color.Orange, Color.Yellow, Content.Load<Texture2D>("counter_box"), true);
            papers = new NumericalCounter("Papers Published", new Vector2(10, 300), 0, 0, 0, 0, cursorFont, Color.Black, Color.Black);
            lifeForce = new NumericalCounter("Life Force", new Vector2(10, 320), 100, 100, 0, 0, cursorFont, Color.Black, Color.Black);
            longevity = new NumericalCounter("Longevity", new Vector2(10, 340), 100, 30, 0, 0, cursorFont, Color.Black, Color.Black);
            humanity = new NumericalCounter("Humanity", new Vector2(10, 360), 100, 30, 0, 0, cursorFont, Color.Black, Color.Black);
            

            // dependant actions

            studyLiveCorpse = new MenuAction("Study Corpse", true,false, 5, 30, 4, 0, 0, 0, 0, true,false,false,new List<NumericalCounter>{humanity,longevity},0.1f,0,0,0,0,0);
            talk = new MenuAction("Talk", true, false, 10, 20, 1, 0, 0, 0, 0, true, false, false, new List<NumericalCounter> { humanity }, 0.5f, 0, 0, 0, 0, 0);
            partWork = new MenuAction("Part Time Work", true, false, 10, 15, 0, 0, 0, 0, 0, true);
            writePaper = new MenuAction("Write Paper", true, false, 10, 0, 0, 0, 0, 0, 0, true, false, false, new List<NumericalCounter> { research }, 0, 0, 0.9f, 0, 0, 0);
            electric = new MenuAction("Transfer Electrical Discharge", true, false, 5, 0, 0, 0, 5, 10, 2, true, false, false, new List<NumericalCounter> { research }, 0, 0, 0, 0.1f, 0.1f, 0.1f);
            inject = new MenuAction("Inject", true, false, 5, 0, 0, 0, 15, 10, 5, true, false, false, new List<NumericalCounter> { research }, 0, 0, 0, 0.1f, 0.1f, 0.1f);
            compute = new MenuAction("Compute", true, false, 5, 0, 0, 0, 10, 20, 20, true, false, false, new List<NumericalCounter> { research }, 0, 0, 0, 0.1f, 0.1f, 0.1f);

            // list of dependent actions to update

            actionUpdateList.Add(partWork);
            actionUpdateList.Add(writePaper);


            // background stuff

            room = new NonInteractive(Vector2.Zero, 0.6f, Content.Load<Texture2D>("room"));
            door = new NonInteractive(new Vector2(231, 413), 0.59f, Content.Load<Texture2D>("door"),2);
            graveyard = new Graveyard(Vector2.Zero, 0.8f, new Vector2(10,760),Content.Load<Texture2D>("back"), Content.Load<Texture2D>("dig_icon_standard"), Content.Load<Texture2D>("dig_icon_highlight"), Content.Load<Texture2D>("dig_icon_pressed"), GraphicsDevice, 0.5f);
            digger = new NonInteractive(new Vector2(1100, 400), 0.79f, Content.Load<Texture2D>("digger"), 1, 10);
            Switch = new NonInteractive(new Vector2(860,750), 0.58f, Content.Load<Texture2D>("switch"), 2, 1);


            // cursor

            cursor = new Cursor(Content.Load<Texture2D>("cursor"),GraphicsDevice);
            
            // characters

            Simon = new Scientist(Content.Load<Texture2D>("tmpprof"), new Vector2(6,1),grid,reachable);
            Jeremy = new Assistant(Content.Load<Texture2D>("tmpass"), new Vector2(1, 1), grid,reachable);

            // objects

            table = new FloorObject(Content.Load<Texture2D>("table"), Content.Load<Texture2D>("tableicon"), 1,1, new Vector2(5, 5), grid, "Operating Table",0, menuActions = new List<MenuAction> {  },GraphicsDevice);
            desk = new FloorObject(Content.Load<Texture2D>("desk"), null, 1, 1, new Vector2(7, 10), grid, "Desk", 10, menuActions = new List<MenuAction> { partWork, writePaper }, GraphicsDevice);
            bookcase = new FloorObject(Content.Load<Texture2D>("bookcase"), null, 1, 1, new Vector2(9, 10), grid, "Bookcase", 10, menuActions = new List<MenuAction> { study }, GraphicsDevice);
            lightningAbsorber = new FloorObject(Content.Load<Texture2D>("lightning"), Content.Load<Texture2D>("lightningicon"), 1, 2, new Vector2(7, 3), grid, "Lightning Absorber", 150, menuActions = new List<MenuAction> { turnOn, turnOff }, GraphicsDevice);
            electricalTransmitter = new FloorObject(Content.Load<Texture2D>("electric"), Content.Load<Texture2D>("electricicon"), 1, 1, new Vector2(9, 2), grid, "Electrical Transference Device", 150, menuActions = new List<MenuAction> { electric }, GraphicsDevice);
            steamComputer = new FloorObject(Content.Load<Texture2D>("machine2"), Content.Load<Texture2D>("machine2icon"), 4, 1, new Vector2(1, 9), grid, "Steam Powered Computer", 200, menuActions = new List<MenuAction> { compute }, GraphicsDevice);
            injectionSystem = new FloorObject(Content.Load<Texture2D>("machine"), Content.Load<Texture2D>("machine2icon"), 1, 1, new Vector2(10, 6), grid, "Injection System", 350, menuActions = new List<MenuAction> { inject }, GraphicsDevice);

            // the corpse!

            corpse = new Corpse(new Vector2(800, 865),Content.Load<Texture2D>("corpse"), new List<MenuAction> { studyCorpse, dissectCorpse }, new List<MenuAction> { talk,studyLiveCorpse },GraphicsDevice);

            // populate initial build list

            build.Add(table);
            build.Add(lightningAbsorber);
            build.Add(electricalTransmitter);
            build.Add(steamComputer);
            build.Add(injectionSystem);

           // fill initial existing object list
            
            floorObjectList.Add(desk);
            floorObjectList.Add(bookcase);

            // test items...

            blob = new positionTextBlob(Content.Load<Texture2D>("gball"), new Vector2(1, 1));
            

        }

     
        protected override void UnloadContent()
        {
            // nothing here at the moment....?
        }



        // ======== update things... ============================

        protected override void Update(GameTime gameTime)
        {
            // check for exit key press
            KeyboardState kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.Escape))
                {
                   this.Exit();
                }

            if (kstate.IsKeyDown(Keys.F1))
            {
                if (f1Down == false)
                {
                    if (graphics.IsFullScreen == true)
                    {
                        graphics.IsFullScreen = false;

                        f1Down = true;
                    }
                    else
                    {
                        graphics.IsFullScreen = true;

                        f1Down = true;
                    }

                    graphics.ApplyChanges();
                }
            }

            else
            {
                f1Down = false;
            }
            

            // cursor
            cursor.Update(xScale,yScale);

            // graveyard
            graveyard.Update(gameTime, cursor, floorObjectList, table, Jeremy, corpse,progBars);

            // digging anim
            digger.Update(gameTime);

            // counters
            research.Update(gameTime);
            madness.Update(gameTime);
            money.Update(gameTime);
            longevity.Update(gameTime);
            humanity.Update(gameTime);
            lifeForce.Update(gameTime);
            papers.Update(gameTime);

            // resurrection
            resurrect.Update(corpse,lifeForce,humanity,longevity,lightningAbsorber,gameTime,cursor,Simon,Jeremy);


            // the corpse!
            corpse.Update(gameTime, dissectCorpse,studyCorpse,longevity,humanity,lifeForce,Simon,talk,cursor,progBars);

            // characters
            Simon.Update(gameTime,GraphicsDevice,cursor,research,money,madness,progBars,reachable);
            Jeremy.Update(gameTime, GraphicsDevice, grid, cursor, research, money, madness, progBars,corpse,door,digger,resurrect,Switch,humanity,longevity,random,reachable);

            // objects to build
            
            if (build.menu == true)
            {
                // objects
                foreach (FloorObject floorObject in build.buildList)
                {
                    floorObject.Update(gameTime, cursor, Simon, Jeremy, progBars, build, floorObjectList);
                }
            }


            // build menu
            build.Update(cursor, gameTime, money);

            // floor objects
            foreach (FloorObject floorObject in floorObjectList)
            {
                floorObject.Update(gameTime, cursor, Simon, Jeremy, progBars, build, floorObjectList);
            }

            // create progress bars if necessary
            List<MiniProgressBar> remove = new List<MiniProgressBar> { };

            foreach (MiniProgressBar bar in progBars)
            {
                bar.Update(gameTime,cursor);
                if (bar.value >= bar.init)
                {
                    remove.Add(bar);
                }
            }

            // remove those no longer needed
            foreach (MiniProgressBar bar in remove)
            {
                bar.action.Complete(research, madness, money,longevity,lifeForce,humanity, bar.floorObject);
                progBars.Remove(bar);
            }


            //set number of papers to number of times you've written a paper
            papers.value = writePaper.count;

            // set income to number of papers * 0.1
            money.valueChange = papers.value * 0.1f;

            // test things....
            blob.Update(gameTime, grid);


            // UPDATE!   
            base.Update(gameTime);
        }

     
        protected override void Draw(GameTime gameTime)
        {
            // set whole screen to viewport
            //GraphicsDevice.Viewport = wholeViewport;

            // clear graphics device
            GraphicsDevice.Clear(Color.Black);

            // set drawing to part of screen only
            //GraphicsDevice.Viewport = aspectViewport;

            // ============== Drawing code =============

            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend,null,null,null,null,Matrix.CreateScale(xScale,yScale,1.0f));

            // cursor
            cursor.Render(GraphicsDevice, spriteBatch, cursorFont,build,graveyard,corpse);

            // inactive objects
            graveyard.Render(spriteBatch,cursorFont);
            digger.Render(spriteBatch);
            room.Render(spriteBatch);
            door.Render(spriteBatch);
            Switch.Render(spriteBatch);

            // characters
            Simon.Render(spriteBatch);   
            Jeremy.Render(spriteBatch);

            // buttons
            build.Render(spriteBatch, cursor,cursorFont,money,floorObjectList,reachable,Simon,Jeremy);
            resurrect.Render(spriteBatch, cursor);

            // counters
            research.Render(spriteBatch);
            madness.Render(spriteBatch);
            money.Render(spriteBatch);
            papers.Render(spriteBatch);
            lifeForce.Render(spriteBatch);
            longevity.Render(spriteBatch);
            humanity.Render(spriteBatch);

            // the corpse!
            corpse.Render(spriteBatch,cursorFont);

            // objects
            foreach (FloorObject floorObject in floorObjectList)
            {
                floorObject.Render(spriteBatch,cursorFont, cursor);
            }

            // progress bars
            foreach (MiniProgressBar bar in progBars)
            {
                bar.Render(spriteBatch,cursorFont);
            }

            // movement grid
            grid.Draw(GraphicsDevice, spriteBatch,reachable);

            // test things....

            spriteBatch.DrawString(cursorFont, blob.gridPosition.ToString(), new Vector2(500, 0), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // grid position of blob
            spriteBatch.DrawString(cursorFont, cursor.position.ToString(), new Vector2(500, 30), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // cursor position

            spriteBatch.DrawString(cursorFont, "Alive: "+corpse.alive.ToString(), new Vector2(500, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // alive?
            spriteBatch.DrawString(cursorFont, "Fail: "+resurrect.fail.ToString(), new Vector2(500, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // fail?
            spriteBatch.DrawString(cursorFont, cursor.click.ToString(), new Vector2(500, 120), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // 
            blob.Render(spriteBatch);


            spriteBatch.End();

            // DRAW!
            base.Draw(gameTime);
        }
    }
}
