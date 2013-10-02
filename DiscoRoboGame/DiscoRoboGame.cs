using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Resources;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using GameFramework;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DiscoRoboGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DiscoRoboGame : GameHost
    {
        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        internal int GameScore = 0;
        private TextObject _score;

        internal const int LeftColumnPos = 100;
        internal const int RightColumPos = 500;
        internal const int ScoreRowBeginPos = 800;
        internal const int ScoreRowEndPos = 1000;

        // List of beats
        private List<int> _leftHand = new List<int>();
        private List<int> _leftFoot = new List<int>();
        private List<int> _rightHand = new List<int>();
        private List<int> _rightFoot = new List<int>();

        struct Beat
        {
            internal int Time { get; set; } // Time of beat
            internal int HoF { get; set; } // Hand or foot: 0: hand, 1: foot
        }

        // Next beat
        private Beat _leftBeat;
        private Beat _rightBeat;



        private int UpdateCount = -60; // Wait time: 1 sec

        public DiscoRoboGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            IsMouseVisible = true;
            IsMouseVisible = false;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures
            Textures.Add("Balloon", Content.Load<Texture2D>("Balloon"));
            Textures.Add("UpArrow", Content.Load<Texture2D>("Arrows-Up-Circular"));
            Textures.Add("DownArrow", Content.Load<Texture2D>("Arrows-Down-Circular"));
            Textures.Add("LeftArrow", Content.Load<Texture2D>("Arrows-Left-Circular"));
            Textures.Add("RightArrow", Content.Load<Texture2D>("Arrows-Right-Circular"));
            Textures.Add("RedLine", Content.Load<Texture2D>("red_line"));

            // Load songs
            Songs.Add("BackgroundMusic", Content.Load<Song>("Tam Biet Nhe 2013 - Lynk Lee Phuc Bang"));

            // Load fonts
            Fonts.Add("Kootenay", Content.Load<SpriteFont>("Kootenay"));

            //Reset the game
            ResetGame();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            // Update all the game objects
            UpdateAll(gameTime);

            base.Update(gameTime);

            UpdateProcessTouchInput();

            UpdateCount++;

            if (_leftBeat.Time == UpdateCount)
            {
                var balloon = GetBalloonObject();
                balloon.IsActive = true;
                var leftPos = new Vector2(LeftColumnPos, 100);
                if (_leftBeat.HoF == 0) // left hand a.k.a up icon
                {
                    balloon.ResetProperties(Textures["UpArrow"], leftPos, ControlObject.ControlType.Up);
                }
                else // left foot a.k.a down icon
                {
                    balloon.ResetProperties(Textures["DownArrow"], leftPos, ControlObject.ControlType.Down);
                }
                GetLeftBeat();
            }

            if (_rightBeat.Time == UpdateCount)
            {
                var balloon = GetBalloonObject();
                balloon.IsActive = true;
                var rightPos = new Vector2(RightColumPos, 100);
                if (_rightBeat.HoF == 0) // right hand a.k.a left icon
                {
                    balloon.ResetProperties(Textures["LeftArrow"], rightPos, ControlObject.ControlType.Left);
                }
                else // right foot a.k.a right icon
                {
                    balloon.ResetProperties(Textures["RightArrow"], rightPos, ControlObject.ControlType.Right);
                }
                GetRightBeat();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            // Begin the main spritebatch
            _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            // Draw the sprites...
            DrawSprites(gameTime, _spriteBatch, Textures["Balloon"]);
            DrawSprites(gameTime, _spriteBatch, Textures["UpArrow"]);
            DrawSprites(gameTime, _spriteBatch, Textures["DownArrow"]);
            DrawSprites(gameTime, _spriteBatch, Textures["LeftArrow"]);
            DrawSprites(gameTime, _spriteBatch, Textures["RightArrow"]);
            DrawSprites(gameTime, _spriteBatch, Textures["RedLine"]);

            // Draw the text
            DrawText(gameTime, _spriteBatch);

            // End the spritebatch

            _spriteBatch.End();

            base.Draw(gameTime);
        }
        private void ResetGame()
        {
            GameObjects.Clear();

            // Play back ground music
            var bgSong = Songs["BackgroundMusic"];
            MediaPlayer.Play(bgSong);

            // Create some balloon object
            BalloonObject balloonObject;
            for (int i = 0; i < 30; i++)
            {
                balloonObject = new BalloonObject(this, Textures["Balloon"],
                                                            new Vector2(LeftColumnPos, 100));
                GameObjects.Add(balloonObject);
            }


            // Add controller
            ControlObject controlObject;
            int controlPosY = GraphicsDevice.Viewport.Height - 200;
            controlObject = new ControlObject(this, new Vector2(0, controlPosY),
                            Textures["UpArrow"], ControlObject.ControlType.Up);
            GameObjects.Add(controlObject);
            controlObject = new ControlObject(this, new Vector2(200, controlPosY),
                            Textures["DownArrow"], ControlObject.ControlType.Down);
            GameObjects.Add(controlObject);
            controlObject = new ControlObject(this, new Vector2(400, controlPosY),
                            Textures["LeftArrow"], ControlObject.ControlType.Left);
            GameObjects.Add(controlObject);
            controlObject = new ControlObject(this, new Vector2(600, controlPosY),
                            Textures["RightArrow"], ControlObject.ControlType.Right);
            GameObjects.Add(controlObject);

            // Add mark line
            LineObject line;
            line = new LineObject(this, new Vector2(0, ScoreRowBeginPos), Textures["RedLine"]);
            line.ScaleX = 5;
            GameObjects.Add(line);
            line = new LineObject(this, new Vector2(0, ScoreRowEndPos), Textures["RedLine"]);
            line.ScaleX = 5;
            GameObjects.Add(line);

            // Add score
            _score = new TextObject(this, Fonts["Kootenay"], new Vector2(GraphicsDevice.Viewport.Width / 2, 100), "0") { ScaleX = 5, ScaleY = 5 };
            GameObjects.Add(_score);

            // Load beat
            LoadBeat("Beat.txt");

            GetRightBeat();
            GetLeftBeat();
        }

        private void LoadBeat(string beatFile)
        {
            StreamResourceInfo resouceStream = Application.GetResourceStream(new Uri(beatFile, UriKind.Relative));
            if (resouceStream != null)
            {
                Stream fileStream = resouceStream.Stream;
                if (fileStream.CanRead)
                {
                    using (var reader = new StreamReader(fileStream))
                    {
                        string line = reader.ReadLine();
                        LoadLeftHand(line);
                        line = reader.ReadLine();
                        LoadLeftFoot(line);
                        line = reader.ReadLine();
                        LoadRightHand(line);
                        line = reader.ReadLine();
                        LoadRightFoot(line);
                    }
                }
            }
        }

        private const int DelayBeat = 3;

        private void LoadLeftHand(string line)
        {
            _leftHand.Clear();
            string[] time = Regex.Split(line, ",");
            int value;
            bool converse;
            foreach (var s in time)
            {
                converse = int.TryParse(s, out value);
                value *= DelayBeat;
                if (converse)
                {
                    _leftHand.Add(value);
                }
            }
        }

        private void LoadLeftFoot(string line)
        {
            _leftFoot.Clear();
            string[] time = Regex.Split(line, ",");
            int value;
            bool converse;
            foreach (var s in time)
            {
                converse = int.TryParse(s, out value);
                value *= DelayBeat;
                if (converse)
                {
                    _leftFoot.Add(value);
                }
            }
        }
        private void LoadRightHand(string line)
        {
            _rightHand.Clear();
            string[] time = Regex.Split(line, ",");
            int value;
            bool converse;
            foreach (var s in time)
            {
                converse = int.TryParse(s, out value);
                value *= DelayBeat;
                if (converse)
                {
                    _rightHand.Add(value);
                }
            }
        }

        private void LoadRightFoot(string line)
        {
            _rightFoot.Clear();
            string[] time = Regex.Split(line, ",");
            int value;
            bool converse;
            foreach (var s in time)
            {
                converse = int.TryParse(s, out value);
                value *= DelayBeat;
                if (converse)
                {
                    _rightFoot.Add(value);
                }
            }
        }

        // Get next left beat
        private void GetLeftBeat()
        {
            if (_leftHand.Count + _leftFoot.Count == 0)
            {
                _leftBeat.Time = -1;
                return;
            }
            if (_leftHand.Count == 0)
            {
                _leftBeat.Time = _leftFoot[0];
                _leftBeat.HoF = 1;
                _leftFoot.RemoveAt(0);
                return;
            }
            if (_leftFoot.Count == 0)
            {
                _leftBeat.Time = _leftHand[0];
                _leftBeat.HoF = 0;
                _leftHand.RemoveAt(0);
                return;
            }
            if (_leftHand[0] < _leftFoot[0])
            {
                _leftBeat.Time = _leftHand[0];
                _leftBeat.HoF = 0;
                _leftHand.RemoveAt(0);
            }
            else
            {
                _leftBeat.Time = _leftFoot[0];
                _leftBeat.HoF = 1;
                _leftFoot.RemoveAt(0);
            }
        }

        // Get next right beat
        private void GetRightBeat()
        {
            if (_rightHand.Count + _rightFoot.Count == 0)
            {
                _rightBeat.Time = -1;
                return;
            }
            if (_rightHand.Count == 0)
            {
                _rightBeat.Time = _rightFoot[0];
                _rightBeat.HoF = 1;
                _rightFoot.RemoveAt(0);
                return;
            }
            if (_rightFoot.Count == 0)
            {
                _rightBeat.Time = _rightHand[0];
                _rightBeat.HoF = 0;
                _rightHand.RemoveAt(0);
                return;
            }
            if (_rightHand[0] < _rightFoot[0])
            {
                _rightBeat.Time = _rightHand[0];
                _rightBeat.HoF = 0;
                _rightHand.RemoveAt(0);
            }
            else
            {
                _rightBeat.Time = _rightFoot[0];
                _rightBeat.HoF = 1;
                _rightFoot.RemoveAt(0);
            }
        }

        private ControlObject _controlButton;
        private void UpdateProcessTouchInput()
        {
            SpriteObject touchSprite;
            TouchCollection tc = TouchPanel.GetState();
            // Is the player tapping the screen?
            if (tc.Count == 1)
            {
                TouchLocation touch = tc[0];
                if (touch.State == TouchLocationState.Pressed)
                {
                    touchSprite = GetSpriteAtPoint(touch.Position);
                    if (touchSprite is ControlObject)
                    {
                        //   Debug.WriteLine("tap");
                        _controlButton = (ControlObject)touchSprite;
                        _controlButton.TapUpdateCount = 10;
                        switch (_controlButton.InputType)
                        {
                            case ControlObject.ControlType.Right:
                                ProcessRightColumn(ControlObject.ControlType.Right);
                                break;
                            case ControlObject.ControlType.Left:
                                ProcessRightColumn(ControlObject.ControlType.Left);
                                break;
                            case ControlObject.ControlType.Up:
                                ProcessLeftColumn(ControlObject.ControlType.Up);
                                break;
                            case ControlObject.ControlType.Down:
                                ProcessLeftColumn(ControlObject.ControlType.Down);
                                break;
                        }
                    }

                }
            }

        }

        // Process left hand and left foot (a.k.a left and right icon)
        private void ProcessLeftColumn(ControlObject.ControlType type)
        {
            GameObjectBase gameObj;
            BalloonObject spriteObj;
            int objectCount = GameObjects.Count;
            for (int i = 0; i < objectCount; i++)
            {
                gameObj = GameObjects[i];
                if (gameObj is BalloonObject)
                {
                    spriteObj = (BalloonObject)gameObj;
                    if (spriteObj.PositionX == LeftColumnPos)
                    {
                        if (!(spriteObj.BoundingBox.Bottom < ScoreRowBeginPos || spriteObj.BoundingBox.Top > ScoreRowEndPos)
                            && spriteObj.BalloonType == type)
                        {

                            GameScore++;
                            _score.Text = GameScore.ToString();
                            //Debug.WriteLine(GameScore);
                        }
                    }
                }
            }
        }

        //// Process left hand and left foot (a.k.a up and down icon)
        private void ProcessRightColumn(ControlObject.ControlType type)
        {
            GameObjectBase gameObj;
            BalloonObject spriteObj;
            int objectCount = GameObjects.Count;
            for (int i = 0; i < objectCount; i++)
            {
                gameObj = GameObjects[i];
                if (gameObj is BalloonObject)
                {
                    spriteObj = (BalloonObject)gameObj;
                    if (spriteObj.PositionX == RightColumPos)
                    {
                        if (!(spriteObj.BoundingBox.Bottom < ScoreRowBeginPos || spriteObj.BoundingBox.Top > ScoreRowEndPos)
                            && spriteObj.BalloonType == type)
                        {
                            GameScore++;
                            _score.Text = GameScore.ToString();
                        }
                    }
                }
            }
        }


        private BalloonObject GetBalloonObject()
        {
            BalloonObject obj = null;
            GameObjectBase gameObj;
            int objectCount = GameObjects.Count;

            for (int i = 0; i < objectCount; i++)
            {
                gameObj = GameObjects[i];
                if (gameObj is BalloonObject)
                {
                    obj = (BalloonObject)gameObj;
                    if (obj.IsActive == false)
                    {
                        return obj;
                    }
                }
            }

            // if all balloon is not active, add more balloon object to objects pull (20% growth)                        
            for (int i = 0; i < objectCount / 5; i++)
            {
                obj = new BalloonObject(this, Textures["Balloon"], new Vector2(0, 0));
                GameObjects.Add(obj);
            }
            return obj;
        }
    }
}
