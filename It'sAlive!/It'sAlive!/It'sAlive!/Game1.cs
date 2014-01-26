using System;
using System.Data;
using System.Data.OleDb;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.CSharp;
using System.Windows.Forms;
using System.Diagnostics;

namespace It_sAlive_
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
             
            graphics.IsFullScreen = false; // initial fullscreen?
            
            // set graphics buffer size  

            if (graphics.IsFullScreen == true) // set to whole screen if full screen
            {
                graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            }
            if (graphics.IsFullScreen == false) // set to size of dektop if windowed (excluding taskbar and window caption)
            {
                graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - 5;
                graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.WorkingArea.Width - 5;                                               
            }

            Content.RootDirectory = "Content"; // content folder (assets here)
        }

     
        protected override void Initialize()
        {
           
           base.Initialize();
        }

        // ====== Declare things... =============

        // graphics & viewport

        double aspectRatio = (16.0f / 9.0f);
        int targetWidth = 1920;
        int targetHeight = 1080;
        int borderSpace = 5;

        // - fullscreen
        int width;
        int height;
        int xOffset;
        int yOffset;

        float xScale;
        float yScale;

        // - windowed
        int wwidth;
        int wheight;
        int wxOffset;
        int wyOffset;
        
        float wxScale;
        float wyScale;

        bool f1Down = false; // changing fullscreen/windowed toggle

        // Random number seed
        Random random = new Random();
                
        // Input Data Table
        //DataTable machines;
        
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
        FloorObject lightningAbsorber;
               
        // the corpse!
        Corpse corpse;

        // lists
        List<FloorObject> floorObjectList  = new List<FloorObject>();
        List<MenuAction> actionUpdateList = new List<MenuAction>();
        List<MenuAction> menuActions;
        List<NumericalCounter> counters;

        //navigation grid, reachable area thereof;
        Grid grid;
        ReachableArea reachable;
     
        // --- menu actions ----

        // independent
        MenuAction studyCorpse;
        MenuAction dissectCorpse;
        MenuAction clearCorpse;
                 
        // dependent
        MenuAction writePaper;
        MenuAction talk;
        MenuAction studyLiveCorpse;


        // -- fonts --

        SpriteFont cursorFont;
        SpriteFont counterFont;

        // test shizz.............

        positionTextBlob blob;

        MachineControlParameter pressure;
        MachineControlParameter volume;

        MachineControlParameter aggression;
        MachineDependentParameter temperature;
        MachineDependentParameter meltPercentage;
        MachineDependentParameter dogNumbers ;
        MachineDependentParameter viscosity;

        List<MachineControlParameter> machineparams;
        List<MachineDependentParameter> machineDisplays;

        MachineControls machineControls;


        // === load things... ==============

        protected override void LoadContent()
        {
            //========== GRAPHICS ==============================================

            // set viewport to size of screen, dependant on fullscreen/windowed
            if (graphics.IsFullScreen == true)
            {
                GraphicsDevice.Viewport = new Viewport(0, 0, GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
                                                GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height);
            }
            else
            {
                GraphicsDevice.Viewport = new Viewport(0, 0, Screen.PrimaryScreen.WorkingArea.Width - borderSpace,
                                                                Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - borderSpace);                
            }

            // find maximum size of render field with 16:9 aspect ratio when full screen

            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = (int)(width / aspectRatio);

            if (height > GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
            {
                height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                width = (int)(height * aspectRatio);
            }

            xScale = (float)((double)width / (double)targetWidth);
            yScale = (float)((double)height / (double)targetHeight);

            xOffset = (int)((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - width) / 2.0);
            yOffset = (int)((GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - height) / 2.0);

            // find maximum size of render field with 16:9 aspect ratio when windowed

            wheight = Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - borderSpace;
            wwidth = (int)(wheight * aspectRatio);

            if (wwidth > Screen.PrimaryScreen.WorkingArea.Width - borderSpace)
            {
                wwidth = Screen.PrimaryScreen.WorkingArea.Width - borderSpace;
                wheight = (int)(wwidth / aspectRatio);
            }

            wxScale = (float)((double)wwidth / (double)targetWidth);
            wyScale = (float)((double)wheight / (double)targetHeight);

            wxOffset = (int)((Screen.PrimaryScreen.WorkingArea.Width - borderSpace - wwidth) / 2.0);
            wyOffset = (int)((Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - borderSpace - wheight) / 2.0);

            // set viewport to this maximum render size, dependent on fullscreen/windowed
            if (graphics.IsFullScreen == true)
            {
                GraphicsDevice.Viewport = new Viewport(xOffset, yOffset, width, height);
            }

            if (graphics.IsFullScreen == false)
            {
                GraphicsDevice.Viewport = new Viewport(wxOffset, wyOffset, wwidth, wheight);

                var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                form.Location = new System.Drawing.Point(0, 0);
            }
            
            // spritebatch

            spriteBatch = new SpriteBatch(GraphicsDevice);

            // ========= GAME OBJECTS ================================================

            // fonts

            cursorFont = Content.Load<SpriteFont>("font");
            counterFont = Content.Load<SpriteFont>("font2");

            // grid

            grid = new Grid(3600, 1750, 450, 20, 20, new Vector2(-850, 1070), true);
            reachable = new ReachableArea(grid, floorObjectList);
                        
            // counters
            research = new NumericalCounter("Research", new Vector2(25, 15), 0, 500, 0, 0, counterFont, Color.Green, Color.Green, Content.Load<Texture2D>("counter_box"));
            madness = new NumericalCounter("Madness", new Vector2(25, 85), 0, 0, 0, 0, counterFont, Color.Red, Color.Green, Content.Load<Texture2D>("counter_box"));
            money = new NumericalCounter("Money", new Vector2(25, 155), 0, 1000, 0, 60, counterFont, Color.Orange, Color.Yellow, Content.Load<Texture2D>("counter_box"), true);
            papers = new NumericalCounter("Papers Published", new Vector2(10, 300), 0, 0, 0, 0, cursorFont, Color.Black, Color.Black);
            lifeForce = new NumericalCounter("Life Force", new Vector2(10, 320), 100, 0, 0, 0, cursorFont, Color.Black, Color.Black);
            longevity = new NumericalCounter("Longevity", new Vector2(10, 340), 100, 0, 0, 0, cursorFont, Color.Black, Color.Black);
            humanity = new NumericalCounter("Humanity", new Vector2(10, 360), 100, 0, 0, 0, cursorFont, Color.Black, Color.Black);

            counters = new List<NumericalCounter>{research,madness,money,papers,lifeForce,longevity,humanity};

            build = new Build(new Vector2(5, 900), Content.Load<Texture2D>("build_icon_standard"), Content.Load<Texture2D>("build_icon_highlight"), Content.Load<Texture2D>("build_icon_pressed"), Content.Load<Texture2D>("invarrow"), Content.Load<Texture2D>("highinvarrow"), GraphicsDevice);
           
            //=====================================================

            // load menu actions from XML
            System.IO.Stream stream = TitleContainer.OpenStream("XMLActions.xml");

            XDocument doc = XDocument.Load(stream);

            menuActions = (from action in doc.Descendants("menuAction")
                           select new MenuAction(
                                                           action.Element("name").Value,
                                                           Convert.ToBoolean(action.Element("scientist").Value),
                                                           Convert.ToBoolean(action.Element("assistant").Value),
                                                           Convert.ToInt32(action.Element("time").Value),
                                                           (float)Convert.ToDouble(action.Element("reasearchUp").Value),
                                                           (float)Convert.ToDouble(action.Element("madnessUp").Value),
                                                           (float)Convert.ToDouble(action.Element("moneyChange").Value),
                                                           (float)Convert.ToDouble(action.Element("lifeForceUp").Value),
                                                           (float)Convert.ToDouble(action.Element("longevityUp").Value),
                                                           (float)Convert.ToDouble(action.Element("humanityUp").Value),
                                                           Convert.ToBoolean(action.Element("remain").Value),
                                                           Convert.ToBoolean(action.Element("turnOn").Value),
                                                           Convert.ToBoolean(action.Element("turnOff").Value),
                                                           new List<NumericalCounter> { research },
                                                           (float)Convert.ToDouble(action.Element("reasearchUpMultiplier").Value),
                                                           (float)Convert.ToDouble(action.Element("madnessUpMultiplier").Value),
                                                           (float)Convert.ToDouble(action.Element("moneyChangeMultiplier").Value),
                                                           (float)Convert.ToDouble(action.Element("lifeForceUpMultiplier").Value),
                                                           (float)Convert.ToDouble(action.Element("longevityUpMultiplier").Value),
                                                           (float)Convert.ToDouble(action.Element("humanityUpMultiplier").Value)
                                                           )).ToList();


            // dependant actions

            studyLiveCorpse = menuActions[6];
            writePaper = menuActions[9];

            // independent actions
            studyCorpse = menuActions[2];
            dissectCorpse = menuActions[3];
            clearCorpse = menuActions[4];
            talk = menuActions[7];

            // load in multiplying counters

            List<Tuple<string, string, string, string>> multipliers = new List<Tuple<string, string, string, string>>();
            
            multipliers = (from floorObject in doc.Descendants("menuAction")
                           select new Tuple<string, string, string, string>(
                            floorObject.Element("name").Value,
                            floorObject.Element("multiplyingCounter").Value,
                            floorObject.Element("multiplyingCounter2").Value,
                            floorObject.Element("multiplyingCounter3").Value)
                               ).ToList();

            foreach (MenuAction action in menuActions)
            {
                foreach (Tuple<string, string, string, string> tuple in multipliers)
                {
                    if (tuple.Item1 == action.name)
                    {
                        foreach (NumericalCounter counter in counters)
                        {
                            foreach (string name in new List<string> { tuple.Item2, tuple.Item3, tuple.Item4 })
                            {
                                if (counter.name == name)
                                {
                                    action.dependent.Add(counter);
                                }
                            }
                        }
                    }
                }
            }

            


            // load floor objects from XML
            System.IO.Stream stream2 = TitleContainer.OpenStream("XMLFloorObjects.xml");

            XDocument doc2 = XDocument.Load(stream2);

            
            build.buildList = (from floorObject in doc2.Descendants("FloorObject") select new FloorObject(Content.Load<Texture2D>(
                                    Convert.ToString(floorObject.Element("texture").Value)),
                                    Content.Load<Texture2D>(Convert.ToString(floorObject.Element("icon").Value)),                                       
                                    Convert.ToInt32(floorObject.Element("frameNumber").Value),
                                    Convert.ToInt32(floorObject.Element("animNumber").Value),
                                    new Vector2(Convert.ToInt32(floorObject.Element("gridPositionX").Value),Convert.ToInt32(floorObject.Element("gridPositionY").Value)),
                                    new Vector2(Convert.ToInt32(floorObject.Element("offsetX").Value), Convert.ToInt32(floorObject.Element("offsetY").Value)),
                                    new Vector2(Convert.ToInt32(floorObject.Element("operationPositionX").Value), Convert.ToInt32(floorObject.Element("operationPositionY").Value)), grid,
                                    floorObject.Element("name").Value, 
                                    Convert.ToInt32(floorObject.Element("cost").Value), 
                                    new List<MenuAction>{}, GraphicsDevice, 
                                    new Vector2(Convert.ToInt32(floorObject.Element("footprintX").Value),Convert.ToInt32(floorObject.Element("footprintY").Value)),
                                    Convert.ToBoolean(floorObject.Element("prebuilt").Value))
                                    ).ToList();

            // load in menu actions

            List<Tuple<string,string,string,string>> actions = new List<Tuple<string,string,string,string>>();

            actions = (from floorObject in doc2.Descendants("FloorObject")
                           select new Tuple<string,string,string,string>(
                            floorObject.Element("name").Value,
                            floorObject.Element("menuAction").Value,
                            floorObject.Element("menuAction2").Value,
                            floorObject.Element("menuAction3").Value)
                               ).ToList();

            foreach (FloorObject machine in build.buildList)
            {
                foreach (Tuple<string,string,string,string> tuple in actions)
                {
                    if (tuple.Item1 == machine.name)
                    {
                        foreach (MenuAction action in menuActions)
                        {
                            foreach (string name in new List<string>{tuple.Item2,tuple.Item3,tuple.Item4})
                            {
                                if (action.name == name)
                                {
                                    machine.menuActions.Add(action);
                                }
                            }
                        }
                    }
                }
            }

            // build any prebuilt objects

            foreach (FloorObject machine in build.buildList)
            {
                if (machine.prebuilt == true)
                {
                    build.BuildThis(floorObjectList, machine, reachable);
                }
            }

            build.removeUpdate();

            // objects

            table = build.buildList[0];
            lightningAbsorber = build.buildList[1];

            resurrect = new Resurrect(new Vector2(1750, 900), Content.Load<Texture2D>("raise_icon_standard"), Content.Load<Texture2D>("raise_icon_highlight"), Content.Load<Texture2D>("raise_icon_pressed"), GraphicsDevice, table);
            

            //===================================================
            
            // background stuff

            room = new NonInteractive(Vector2.Zero, 0.6f, Content.Load<Texture2D>("room"));
            door = new NonInteractive(new Vector2(245, 200), 0.59f, Content.Load<Texture2D>("door"),2);
            graveyard = new Graveyard(new Vector2(1140,200), 0.8f, new Vector2(10,760),Content.Load<Texture2D>("back"), Content.Load<Texture2D>("dig_icon_standard"), Content.Load<Texture2D>("dig_icon_highlight"), Content.Load<Texture2D>("dig_icon_pressed"), GraphicsDevice, 0.5f);
            digger = new NonInteractive(new Vector2(1200, 300), 0.79f, Content.Load<Texture2D>("digger"), 1, 10);
            Switch = new NonInteractive(new Vector2(860,400), 0.58f, Content.Load<Texture2D>("switch"), 2, 1);
            
            // cursor

            cursor = new Cursor(Content.Load<Texture2D>("cursor"),GraphicsDevice);
            
            // characters

            Simon = new Scientist(Content.Load<Texture2D>("tmpprof"), new Vector2(10,1),grid,reachable,table);
            Jeremy = new Assistant(Content.Load<Texture2D>("tmpass"), new Vector2(8, 1), grid,reachable,table);

            // the corpse!

            corpse = new Corpse(new Vector2(660, 600),Content.Load<Texture2D>("corpse"), new List<MenuAction> { studyCorpse, dissectCorpse }, new List<MenuAction> { talk,studyLiveCorpse }, new List<MenuAction> { clearCorpse },GraphicsDevice,cursorFont);
                       
            // update reachable area to account for these

            reachable.Update(floorObjectList);

            // test items...

            blob = new positionTextBlob(Content.Load<Texture2D>("gball"), new Vector2(1, 1));

            // machine parameters...

            pressure = new MachineControlParameter("Pressure", 100, 0, 20, 1);
            volume = new MachineControlParameter("Volume", 50, 0, 10, 2);
            aggression = new MachineControlParameter("Aggression", 1000, 0, 10, 1);    

            Func<double, double> tempFunction = delegate(double x) { return 3 * x + 1; };

            temperature = new MachineDependentParameter("Temperature", 0, 100, 20, pressure,tempFunction, 1);
            meltPercentage = new MachineDependentParameter("Melt Percentage", 0, 50, 10, volume, Math.Exp, 2);
            dogNumbers = new MachineDependentParameter("Number of Dogs", 0, 10, 1, volume, Math.Exp, 2);
            viscosity = new MachineDependentParameter("Viscosity", 0, 30, 5, pressure, Math.Exp, 2);

            machineparams = new List<MachineControlParameter> { pressure, volume, aggression };
            machineDisplays = new List<MachineDependentParameter> { temperature, meltPercentage,dogNumbers,viscosity };
            
            // machine controls...

            machineControls = new MachineControls(Content.Load<Texture2D>("knob"), Content.Load<Texture2D>("gauge"), Content.Load<Texture2D>("hand"), Content.Load<Texture2D>("slider"), 
                                                    Content.Load<Texture2D>("sliderknob"), Content.Load<Texture2D>("slidegauge"), Content.Load<Texture2D>("slidegaugeknob"));


            // test saving...

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

            if (kstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                   this.Exit();
                }

            // test controls menu...

            if (kstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
            {
                machineControls.OpenMenu(machineparams, machineDisplays);
            }

            if (kstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.B))
            {
                machineControls.CloseMenu();
            }

            // check for switch from full screen to windowed or back

            if (kstate.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.F1))
            {
                if (f1Down == false)
                {
                    if (graphics.IsFullScreen == true) // swap viewport and buffer to appropriate size
                    {
                        graphics.IsFullScreen = false;

                        graphics.PreferredBackBufferHeight = Screen.PrimaryScreen.WorkingArea.Height - SystemInformation.CaptionHeight - borderSpace;
                        graphics.PreferredBackBufferWidth = Screen.PrimaryScreen.WorkingArea.Width - borderSpace;
                        graphics.ApplyChanges();
                        GraphicsDevice.Viewport = new Viewport(wxOffset, wyOffset, wwidth, wheight);

                        var form = (System.Windows.Forms.Form)System.Windows.Forms.Control.FromHandle(this.Window.Handle);
                        form.Location = new System.Drawing.Point(0, 0);

                        f1Down = true;
                    }

                    else // swap viewport and buffer to appropriate size
                    {
                        graphics.IsFullScreen = true;

                        graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                        graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                        graphics.ApplyChanges();
                        GraphicsDevice.Viewport = new Viewport(xOffset, yOffset, width, height);
                        

                        f1Down = true;
                    }

                    graphics.ApplyChanges(); // apply changes!
                }
            }

            else
            {
                f1Down = false;
            }
            

            // cursor
            cursor.Update(graphics, xScale, yScale, wxScale, wyScale);

            // graveyard
            graveyard.Update(gameTime, cursor, floorObjectList, table, Jeremy, corpse, progBars, menuActions[3], menuActions[2]);

            // digging anim
            digger.Update(gameTime);

            // counters
            foreach (NumericalCounter counter in counters)
            {
                counter.Update(gameTime);
            }

            // resurrection
            resurrect.Update(corpse,lifeForce,humanity,longevity,lightningAbsorber,gameTime,cursor,Simon,Jeremy);


            // the corpse!
            corpse.Update(gameTime, dissectCorpse,studyCorpse,longevity,humanity,lifeForce,Simon,Jeremy,talk,cursor,progBars,clearCorpse);

            // characters
            Simon.Update(gameTime,GraphicsDevice,cursor,research,money,madness,progBars,reachable);
            Jeremy.Update(gameTime, GraphicsDevice, grid, cursor, research, money, madness, progBars,corpse,door,digger,resurrect,Switch,humanity,longevity,lifeForce, random,reachable);

            // objects to build
            
            if (build.menu == true)
            {
                // objects
                foreach (FloorObject floorObject in build.buildList)
                {
                    floorObject.Update(gameTime, cursor, Simon, Jeremy, progBars, build, floorObjectList,reachable,money);
                }
            }


            // build menu
            build.Update(cursor, gameTime, money);

            // floor objects
            foreach (FloorObject floorObject in floorObjectList)
            {
                floorObject.Update(gameTime, cursor, Simon, Jeremy, progBars, build, floorObjectList,reachable,money);
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
            
            machineControls.Update(cursor, random, gameTime);
            
            // UPDATE!   
            base.Update(gameTime);
        }

     
        protected override void Draw(GameTime gameTime)
        {

            // clear graphics device
            GraphicsDevice.Clear(Color.Black);
            
            // ============== Drawing code =============

            if (graphics.IsFullScreen == true)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(xScale, yScale, 1.0f));
            }

            if (graphics.IsFullScreen == false)
            {
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, null, null, null, null, Matrix.CreateScale(wxScale, wyScale, 1.0f));
            }

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
            foreach (NumericalCounter counter in counters)
            {
                counter.Render(spriteBatch);
            }
            
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

            //knob.Render(spriteBatch, cursorFont);

            //gauge.Render(spriteBatch,cursorFont);

            //slider.Render(spriteBatch,cursorFont);
            //slideGauge.Render(spriteBatch,cursorFont);

            machineControls.Render(spriteBatch, cursorFont);

            spriteBatch.DrawString(cursorFont, blob.gridPosition.ToString(), new Vector2(500, 0), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // grid position of blob
            spriteBatch.DrawString(cursorFont, cursor.position.ToString(), new Vector2(500, 30), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // cursor position

            spriteBatch.DrawString(cursorFont, "Alive: "+corpse.alive.ToString(), new Vector2(500, 90), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // alive?
            spriteBatch.DrawString(cursorFont, "Fail: "+resurrect.fail.ToString(), new Vector2(500, 60), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // fail?
            spriteBatch.DrawString(cursorFont, floorObjectList[0].menuMouseover.ToString(), new Vector2(500, 120), Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 1.0f, SpriteEffects.None, 0.2f); // 
            blob.Render(spriteBatch);


            spriteBatch.End();

            // DRAW!
            base.Draw(gameTime);
        }
    }
}
