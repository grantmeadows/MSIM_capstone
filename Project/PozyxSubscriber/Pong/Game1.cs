using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PozyxPositioner.Framework;

namespace Ping_Pong
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>  
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Simulation Environment
        SimObject simObject;
        SimEnvironment sim;
        string tag1;
        string tag2;
        static float paddleWidth;

        // Audio
        SoundEffect soundPaddleHit;
        SoundEffectInstance soundPaddleInstance;
        SoundEffect soundBoarderHit;
        SoundEffectInstance soundBoarderHitInstance;
        Song backgroundSong;

        // User Input
        KeyboardState keyboardState;
        KeyboardState lastKeyboardState;

        //fonts
        SpriteFont scoreFont;
        SpriteFont myNameFont;

        // the score
        int m_Score1 = 0;
        int m_Score2 = 0;
        Rectangle[] m_ScoreRect = null;

        // the ball
        Ball m_ball;
        Texture2D m_textureBall;
        Random m_initialSpeed = new Random();

        // the paddles
        Paddle m_paddle1;
        Paddle m_paddle2;
        Texture2D m_texturePaddle;

        // the background
        Texture2D m_backGround;
        Rectangle m_backgroundDims = new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT);

        // constants
        const int SCREEN_WIDTH = (1280);
        const int SCREEN_HEIGHT = (720);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {

            // use a fixed frame rate of 30 frames per second
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 33);

            
            InitScreen();
            InitGameObjects();
            InitializeSimEnviornment();

            base.Initialize();
        }

        
        /// <summary>
        /// Start capturing of position
        /// </summary>
        private void InitializeSimEnviornment()
        {
            var host = "10.0.0.254";
            var port = 1883;
            int numTags = 2;
            tag1 = "5772";
            //tag2 = "6985";

            int tagRefreshRate = 24;

            sim = SimEnvironment.Instance;
            simObject = new SimObject();

            //sim.Initialize(host, port,"Pong.txt", tagRefreshRate);
            sim.Initialize("Pong.txt", tagRefreshRate);
            Tag t1 = sim.newTag(tag1, tagRefreshRate);
            //Tag t2 = sim.newTag(tag2, 15);

            simObject.AddTag(t1);
            //simObject.AddTag(t2);

            sim.StartEnvironment();
            while (!sim.ConnectedStatus) ;
            simObject.Calibrate(m_paddle1.X, m_paddle1.Y, 0.0f, (1.0f/3.0f));
        }

        /// <summary>
        /// screen-related init tasks
        /// </summary>
        public void InitScreen()
        {
            // back buffer
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferMultiSampling = false;
            graphics.ApplyChanges();
        }
        /// <summary>
        /// game-related init tasks
        /// </summary>
        public void InitGameObjects()
        {
            // create an instance of our ball
            m_ball = new Ball();

            // set the size of the ball
            m_ball.Width = 20.0f;
            m_ball.Height = 20.0f;

            // create 2 instances of our paddle
            m_paddle1 = new Paddle();
            m_paddle2 = new Paddle();

            // set the size of the paddles
            paddleWidth = 100.0f;
            m_paddle1.Width = 15.0f;
            m_paddle1.Height = paddleWidth;
            m_paddle2.Width = 15.0f;
            m_paddle2.Height = paddleWidth;

            // map the digits in the image to actual numbers
            m_ScoreRect = new Rectangle[10];
            for (int i = 0; i < 10; i++)
            {
                m_ScoreRect[i] = new Rectangle(
                    i * 45, // X
                    0,      // Y
                    45,     // Width
                    75);    // Height
            }

            ResetGame();
        }

        /// <summary>
        /// initial play state, called when the game is first
        /// run, and whever a player scores 10 goals
        /// </summary>
        public void ResetGame()
        {
            // reset scores
            m_Score1 = 0;
            m_Score2 = 0;

            // place the ball at the center of the screen
            m_ball.X =
                SCREEN_WIDTH / 2 - m_ball.Width / 2;
            m_ball.Y =
                SCREEN_HEIGHT / 2 - m_ball.Height / 2;

            // set a random speed and direction for the ball
            if (m_initialSpeed.NextDouble() >= .5)
            {
                float random = (float)((m_initialSpeed.NextDouble() * 2) + 4);
                m_ball.DX = random;
            }
            else
            {
                float random = -(float)((m_initialSpeed.NextDouble() * 2) + 4);
                m_ball.DX = random;
            }
            if (m_initialSpeed.NextDouble() >= .5)
            {
                float random = (float)((m_initialSpeed.NextDouble() * 2) + 3);
                m_ball.DY = random;
            }
            else
            {
                float random = -(float)((m_initialSpeed.NextDouble() * 2) + 3);
                m_ball.DY = random;
            }

            // place the paddles at either end of the screen
            m_paddle1.X = 30;
            m_paddle1.Y =
                SCREEN_HEIGHT / 2 - m_paddle1.Height / 2;
            m_paddle2.X =
                SCREEN_WIDTH - 30 - m_paddle2.Width;
            m_paddle2.Y =
                SCREEN_HEIGHT / 2 - m_paddle1.Height / 2;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load images from disk
            LoadGameGraphics();
        }

        /// <summary>
        /// load our textures from disk
        /// </summary>
        protected void LoadGameGraphics()
        {
            // load the texture for the ball
            m_textureBall =
                Content.Load<Texture2D>("ball");
            m_ball.Visual = m_textureBall;

            // load the texture for the paddles
            m_texturePaddle =
                Content.Load<Texture2D>(@"media\paddle");
            m_paddle1.Visual = m_texturePaddle;
            m_paddle2.Visual = m_texturePaddle;

            // load the font for the score
            scoreFont =
                Content.Load<SpriteFont>("score");
            // load font for my name
            myNameFont =
                Content.Load<SpriteFont>("myName");

            //load the background
            m_backGround =
                Content.Load<Texture2D>("table");

            //load sound effects
            soundPaddleHit =
                Content.Load<SoundEffect>(@"soundEffects\onPaddleHit");
            soundPaddleInstance = soundPaddleHit.CreateInstance();
            soundBoarderHit =
                Content.Load<SoundEffect>(@"soundEffects\onBoarderHit");
            soundBoarderHitInstance = soundBoarderHit.CreateInstance();

            //load background music
            backgroundSong =
                Content.Load<Song>(@"soundEffects\backgroundMusic");

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
            // Control User Input
            keyboardState = Keyboard.GetState();
            CheckButtonStates(keyboardState);

            // Plays Background Music
            MediaPlayer.Volume = 0.25f;
            soundBoarderHitInstance.Volume = 0.75f;
            soundPaddleInstance.Volume = 0.75f;

            if(MediaPlayer.State == MediaState.Stopped)
            {
                MediaPlayer.Play(backgroundSong);
            }

            // update the ball's location on the screen
            MoveBall();
            // update the paddles' locations on the screen
            MovePaddles();

            base.Update(gameTime);
        }

        /// <summary>
        /// Method to control user input, exiting, 
        /// pausing music, reseting game
        /// </summary>
        /// <param name="buttonPressed">state of keyboard</param>
        private void CheckButtonStates(KeyboardState buttonPressed)
        {
            if(buttonPressed.IsKeyDown(Keys.R))
            {
                ResetGame();
            }
            else if(buttonPressed.IsKeyDown(Keys.Q) || buttonPressed.IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }
            else if (buttonPressed.IsKeyDown(Keys.P) && lastKeyboardState.IsKeyUp(Keys.P))
            {
                if(MediaPlayer.State == MediaState.Playing)
                {
                    MediaPlayer.Pause();
                }
                else if (MediaPlayer.State == MediaState.Paused) 
                {
                    MediaPlayer.Resume();
                }
            }

            lastKeyboardState = keyboardState;
        }

        /// <summary>
        /// move the ball based on it's current DX and DY
        /// settings. check for collisions
        /// </summary>
        private void MoveBall()
        {
            // actually move the ball
            m_ball.X += m_ball.DX;
            m_ball.Y += m_ball.DY;

            // did ball touch top or bottom side?
            if (m_ball.Y <= 0 ||
                m_ball.Y >= SCREEN_HEIGHT - m_ball.Height)
            {
                // reverse vertical direction
                m_ball.DY *= -1;
            }

            // did ball touch the left side?
            if (m_ball.X <= 0)
            {
                // Play sound when ball hits boarder
                soundBoarderHitInstance.Play();
                // at higher speeds, the ball can leave the 
                // playing field, make sure that doesn't happen
                m_ball.X = 0;

                // increment player 2's score
                m_Score2++;

                // reduce speed, reverse direction
                m_ball.DX = 5.0f;
            }

            // did ball touch the right side?
            if (m_ball.X >= SCREEN_WIDTH - m_ball.Width)
            {
                // Play sound when ball hits boarder
                soundBoarderHitInstance.Play();
                // at higher speeds, the ball can leave the 
                // playing field, make sure that doesn't happen
                m_ball.X = SCREEN_WIDTH - m_ball.Width;

                // increment player 1's score
                m_Score1++;

                // reduce speed, reverse direction
                m_ball.DX = -5.0f;
            }

            // reset game if a player scores 10 goals
            if (m_Score1 > 99 || m_Score2 > 99)
            {
                ResetGame();
            }

            // did ball hit the paddle from the front?
            if (CollisionOccurred())
            {
                //play sound
                soundPaddleInstance.Play();
                // reverse hoizontal direction
                m_ball.DX *= -1;

                // increase the speed a little.
                m_ball.DX *= 1.15f;
            }
        }

        /// <summary>
        /// check for a collision between the ball and paddles
        /// </summary>
        /// <returns>If collision has occured or not</returns>
        private bool CollisionOccurred()
        {
            // assume no collision
            bool retval = false;

            // heading towards player one
            if (m_ball.DX < 0)
            {
                Rectangle b = m_ball.Rect;
                Rectangle p = m_paddle1.Rect;
                retval =
                    b.Left < p.Right &&
                    b.Right > p.Left &&
                    b.Top < p.Bottom &&
                    b.Bottom > p.Top;
            }
            // heading towards player two
            else // m_ball.DX > 0
            {
                Rectangle b = m_ball.Rect;
                Rectangle p = m_paddle2.Rect;
                retval =
                    b.Left < p.Right &&
                    b.Right > p.Left &&
                    b.Top < p.Bottom &&
                    b.Bottom > p.Top;
            }

            return retval;
        }

        /// <summary>
        /// how much to move paddle each frame
        /// </summary>
        private const float PADDLE_STRIDE = 10.0f;

        /// <summary>
        /// actually move the paddles
        /// </summary>
        private void MovePaddles()
        {
            // define bounds for the paddles
            float MIN_Y = 0.0f;
            float MAX_Y = SCREEN_HEIGHT - m_paddle1.Height;

            // get player input
            GamePadState pad1 =
                GamePad.GetState(PlayerIndex.One);
            GamePadState pad2 =
                GamePad.GetState(PlayerIndex.Two);
            KeyboardState keyb =
                Keyboard.GetState();

            // check the controller, PLAYER ONE
            bool PlayerUp =
                pad1.DPad.Up == ButtonState.Pressed;
            bool PlayerDown =
                pad1.DPad.Down == ButtonState.Pressed;

            //update paddle position from tag info
            m_paddle1.Y = simObject.Position.y;

            // check the controller, PLAYER TWO
            PlayerUp =
                pad2.DPad.Up == ButtonState.Pressed;
            PlayerDown =
                pad2.DPad.Down == ButtonState.Pressed;

            // also check the keyboard, PLAYER TWO
            PlayerUp |= keyb.IsKeyDown(Keys.Up);
            PlayerDown |= keyb.IsKeyDown(Keys.Down);

            // move the paddle
            if (PlayerUp)
            {
                m_paddle2.Y -= PADDLE_STRIDE;
                if (m_paddle2.Y < MIN_Y)
                {
                    m_paddle2.Y = MIN_Y;
                }
            }
            else if (PlayerDown)
            {
                m_paddle2.Y += PADDLE_STRIDE;
                if (m_paddle2.Y > MAX_Y)
                {
                    m_paddle2.Y = MAX_Y;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // our game-specific drawing logic
            Render();

            base.Draw(gameTime);
        }
        /// <summary>
        /// draw the score at the specified location
        /// </summary>
        /// <param name="x">x position to draw score</param>
        /// <param name="y">y position to draw score</param>
        /// <param name="score">String representing score</param>
        public void DrawScore(float x, float y, int score)
        {
            spriteBatch.DrawString(scoreFont, 
                $"{score}", 
                new Vector2(x, y), 
                Color.Black);
        }

        /// <summary>
        /// actually draw our game objects
        /// </summary>
        public void Render()
        {
            // black background
            graphics.GraphicsDevice.Clear(Color.Black);
            var vp = GraphicsDevice.Viewport;

            // start rendering our game graphics
            spriteBatch.Begin();

            //draw background first so everything else is viewed above
            spriteBatch.Draw(m_backGround, new Rectangle(0, 0, vp.Width, vp.Height), Color.White);

            // draw the score first, so the ball can
            // move over it without being obscured
            DrawScore((float)SCREEN_WIDTH * 0.25f,
                20, m_Score1);
            DrawScore((float)SCREEN_WIDTH * 0.65f,
                20, m_Score2);

            // render the game objects
            spriteBatch.Draw((Texture2D)m_ball.Visual,
                m_ball.Rect, Color.White);
            spriteBatch.Draw((Texture2D)m_paddle1.Visual,
                m_paddle1.Rect, Color.White);
            spriteBatch.Draw((Texture2D)m_paddle2.Visual,
                m_paddle2.Rect, Color.White);

            // draw my name
            spriteBatch.DrawString(myNameFont,
                $"Grant Meadows",
                new Vector2(10,10),
                Color.Black);

            // we're done drawing
            spriteBatch.End();
        }

    }
}
