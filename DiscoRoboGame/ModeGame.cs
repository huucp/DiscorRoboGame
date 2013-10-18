using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;
using GameFramework;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DiscoRoboGame
{
    internal class ModeGame : GameModeBase
    {
        private DiscoRoboGame _game;

        internal bool GameIsActive { get; set; }

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

        private const int DelayBeat = 3;


        private int UpdateCount = -60; // Wait time: 1 sec

        protected double EPSILON
        {
            get { return 0.001; }
        }

        public ModeGame(DiscoRoboGame game)
            : base(game)
        {
            _game = game;

            // Indicate that the game is not yet active
            GameIsActive = false;
        }



        public override void Reset()
        {
            base.Reset();

            GameObjects.Clear();

            // Play back ground music
            var bgSong = Game.Songs["BackgroundMusic"];
            MediaPlayer.Play(bgSong);

            // Create some balloon object
            BalloonObject balloonObject;
            for (int i = 0; i < 30; i++)
            {
                balloonObject = new BalloonObject(_game, Game.Textures["Balloon"],
                                                            new Vector2(LeftColumnPos, 100));
                GameObjects.Add(balloonObject);
            }


            // Add controller
            ControlObject controlObject;
            controlObject = new ControlObject(_game, new Vector2(LeftColumnPos, ScoreRowBeginPos),
                            Game.Textures["UpArrow"], ControlObject.ControlType.Up);
            GameObjects.Add(controlObject);
            controlObject = new ControlObject(_game, new Vector2(RightColumPos, ScoreRowBeginPos),
                            Game.Textures["DownArrow"], ControlObject.ControlType.Down);
            GameObjects.Add(controlObject);

            // Add score            
            _score = new TextObject(_game, Game.Fonts["Kootenay"], new Vector2(_game.GraphicsDevice.Viewport.Width * 0.5f, 100), "0") { ScaleX = 5, ScaleY = 5 };
            GameObjects.Add(_score);

            // Load beat
            LoadBeat("Beat.txt");

            GetRightBeat();
            GetLeftBeat();

            // The game is now playing
            GameIsActive = true;
        }

        public override void Deactivate()
        {
            base.Deactivate();
            MediaPlayer.Pause();
        }

        public override void Activate()
        {
            base.Activate();
            MediaPlayer.Resume();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Game.SetGameMode<ModeMenu>();
            }
            // Update all the game objects
            Game.UpdateAll(gameTime);

            UpdateProcessTouchInput();

            UpdateCount++;

            if (_leftBeat.Time == UpdateCount)
            {
                var balloon = GetBalloonObject();
                balloon.IsActive = true;
                var leftPos = new Vector2(LeftColumnPos, 100);
                if (_leftBeat.HoF == 0) // left hand a.k.a up icon
                {
                    balloon.ResetProperties(Game.Textures["UpArrow"], leftPos, ControlObject.ControlType.Up);
                }
                else // left foot a.k.a down icon
                {
                    balloon.ResetProperties(Game.Textures["DownArrow"], leftPos, ControlObject.ControlType.Down);
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
                    balloon.ResetProperties(Game.Textures["LeftArrow"], rightPos, ControlObject.ControlType.Left);
                }
                else // right foot a.k.a right icon
                {
                    balloon.ResetProperties(Game.Textures["RightArrow"], rightPos, ControlObject.ControlType.Right);
                }
                GetRightBeat();
            }
        }

        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Clear(Color.CornflowerBlue);


            // Begin the main spritebatch
            _game._spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied);
            // Draw the sprites...
            //_game.DrawSprites(gameTime, _game._spriteBatch, Game.Textures["Balloon"]);
            //_game.DrawSprites(gameTime, _game._spriteBatch, Game.Textures["UpArrow"]);
            //_game.DrawSprites(gameTime, _game._spriteBatch, Game.Textures["DownArrow"]);
            //_game.DrawSprites(gameTime, _game._spriteBatch, Game.Textures["LeftArrow"]);
            //_game.DrawSprites(gameTime, _game._spriteBatch, Game.Textures["RightArrow"]);
            //_game.DrawSprites(gameTime, _game._spriteBatch, Game.Textures["RedLine"]);
            _game.DrawSprites(gameTime, _game._spriteBatch); // Draw all sprite objects
            // Draw the text
            _game.DrawText(gameTime, _game._spriteBatch);

            // End the spritebatch

            _game._spriteBatch.End();
            base.Draw(gameTime);
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
        private int continueTapping = 0;
        private int lastCotinutTapping = 0;
        private void UpdateProcessTouchInput()
        {
            SpriteObject touchSprite;
            TouchCollection tc = TouchPanel.GetState();
            // Is the player tapping the screen?
            if (tc.Count < 3)
            {
                for (int i = 0; i < tc.Count; i++)
                {
                    TouchLocation touch = tc[i];
                    if (touch.State == TouchLocationState.Pressed)
                    {
                        touchSprite = Game.GetSpriteAtPoint(touch.Position);
                        if (touchSprite is ControlObject)
                        {
                            //continueTapping++;

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
                                    ProcessRightColumn(ControlObject.ControlType.Down);
                                    break;
                            }
                        }
                        //else if (i == tc.Count)
                        //{
                        //    continueTapping = 0;
                        //}
                    }

                }
            }
            if (lastCotinutTapping != continueTapping)
            {
                Debug.WriteLine(continueTapping);
                lastCotinutTapping = continueTapping;
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
                    continueTapping++;
                    spriteObj = (BalloonObject)gameObj;
                    if (Math.Abs(spriteObj.PositionX - LeftColumnPos) < EPSILON)
                    {
                        int center = (spriteObj.BoundingBox.Bottom + spriteObj.BoundingBox.Top) / 2;
                        if (!(center < ScoreRowBeginPos || center > ScoreRowEndPos))
                        {
                            GameScore++;
                            _score.Text = GameScore.ToString();
                            return;
                            //Debug.WriteLine(GameScore);
                        }
                    }
                }
                else if (i == objectCount - 1)
                {
                    continueTapping = 0;
                }
            }
        }

        // Process left hand and left foot (a.k.a up and down icon)
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
                    continueTapping++;
                    spriteObj = (BalloonObject)gameObj;
                    if (Math.Abs(spriteObj.PositionX - RightColumPos) < EPSILON)
                    {
                        int center = (spriteObj.BoundingBox.Bottom + spriteObj.BoundingBox.Top) / 2;
                        if (!(center < ScoreRowBeginPos || center > ScoreRowEndPos))
                        {
                            GameScore++;
                            _score.Text = GameScore.ToString();
                            return;
                        }
                    }
                }
                else if (i == objectCount - 1)
                {
                    continueTapping = 0;
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
                obj = new BalloonObject(_game, Game.Textures["Balloon"], new Vector2(0, 0));
                GameObjects.Add(obj);
            }
            return obj;
        }

    }
}
