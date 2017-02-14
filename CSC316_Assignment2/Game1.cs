using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CSC316_Assignment2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model earth, sky, ground;

        Vector3 cameraPos;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            cameraPos = new Vector3(0, 10, -50);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            earth = Content.Load<Model>("earth");
            sky = Content.Load<Model>("sky");
            ground = Content.Load<Model>("floor");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 0.0001f, 1000.0f);

            // Scale ground 100x
            world = Matrix.CreateScale(100);
            ground.Draw(world, view, projection);

            // Scale earth 1x
            world = Matrix.CreateScale(1);
            earth.Draw(world, view, projection);

            // Scale sky 100x
            world = Matrix.CreateScale(100);
            sky.Draw(world, view, projection);

            base.Draw(gameTime);
        }
    }
}
