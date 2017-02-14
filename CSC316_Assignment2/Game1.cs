using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CSC316_Assignment2
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model earth, sky, ground, avatar;

        Vector3 cameraPos;
        Vector3 playerPos;
        float rotationY;

        const float rotationSpeed = 0.05f;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            cameraPos = new Vector3(0, 10, -50);
            playerPos = new Vector3(0, 0, 0);
            rotationY = 0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            earth = Content.Load<Model>("earth");
            sky = Content.Load<Model>("sky");
            ground = Content.Load<Model>("floor");
            avatar = Content.Load<Model>("avatar");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Turn left/right
            if (Keyboard.GetState().IsKeyDown(Keys.A) || Keyboard.GetState().IsKeyDown(Keys.Left))
                rotationY += rotationSpeed;
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
                rotationY -= rotationSpeed;

            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
                playerPos += Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(rotationY));
            else if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
                playerPos -= Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(rotationY));

            // Strafe left/right
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                playerPos += Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(rotationY));
            else if (Keyboard.GetState().IsKeyDown(Keys.E))
                playerPos -= Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(rotationY));

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            Matrix world = Matrix.Identity;
            Matrix view = Matrix.CreateLookAt(Vector3.Transform(cameraPos, Matrix.CreateRotationY(rotationY)) + playerPos, playerPos, Vector3.Up);
            Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, 1, 1f, 1000.0f);

            // Draw avatar
            world = Matrix.CreateScale(1) * Matrix.CreateRotationY(rotationY) * Matrix.CreateTranslation(playerPos);
            avatar.Draw(world, view, projection);

            // Draw ground
            world = Matrix.CreateScale(100);
            ground.Draw(world, view, projection);

            // Draw earth
            world = Matrix.CreateScale(1);
            earth.Draw(world, view, projection);

            // Draw sky
            world = Matrix.CreateScale(100);
            sky.Draw(world, view, projection);

            

            base.Draw(gameTime);
        }
    }
}
