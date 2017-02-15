using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CSC316_Assignment2
{
    public enum ModelName { earth, gorilla, male, female, wall, cage }
    public class GameObject
    {
        [XmlElement("Position")]
        public Vector3 position = Vector3.Zero;

        [XmlElement("Scale")]
        public float scale = 1;

        [XmlElement("RotationYFacing")]
        public float rotateYFacing = 0;

        [XmlElement("RotateXSpeed")]
        public float rotateXSpeed = 0;

        [XmlElement("RotateYSpeed")]
        public float rotateYSpeed = 0;

        [XmlElement("RotateZSpeed")]
        public float rotateZSpeed = 0;

        [XmlElement("MoveSpeed")]
        public float MoveSpeed = 1;

        [XmlElement("Waypoints")]
        public List<Vector3> Waypoints;

        [XmlElement("ModelName")]
        public ModelName modelName = ModelName.earth;

        [XmlIgnore]
        public int CurrentWaypoint = 0;
    }

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Model earth, sky, ground, avatar, gorilla, male, female, wall, cage;

        // Player settings
        Vector3 cameraPos;
        Vector3 playerPos;
        float rotationY;
        float jumpHeight;

        // GameObjects
        List<GameObject> gameObjects;

        const float ROTATION_SPEED = 0.05f;

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

            gameObjects = new List<GameObject>();

            // Read gameobjects.xml
            try
            {
                XmlSerializer gameObjectSerializer = new XmlSerializer(typeof(List<GameObject>));
                TextReader gameObjectsReader = new StreamReader("gameobjects.xml");
                gameObjects = (List<GameObject>)gameObjectSerializer.Deserialize(gameObjectsReader);
                gameObjectsReader.Close();
            }
            catch (Exception e)
            {
                Exit();
            }

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
            gorilla = Content.Load<Model>("gorilla");
            male = Content.Load<Model>("human_male");
            female = Content.Load<Model>("human_female");
            wall = Content.Load<Model>("wall");
            cage = Content.Load<Model>("cage");

            //List<GameObject> goList = new List<GameObject>();
            //GameObject go = new GameObject();
            //go.Waypoints = new List<Vector3>();
            //go.Waypoints.Add(new Vector3(0, 0, 0));
            //go.Waypoints.Add(new Vector3(0, 0, 0));
            //goList.Add(go);

            //GameObject go1 = new GameObject();
            //goList.Add(go1);
            //XmlSerializer gameObjectSerializer = new XmlSerializer(typeof(List<GameObject>));
            //TextWriter objectsWriter = new StreamWriter("gameobjects.xml");
            //gameObjectSerializer.Serialize(objectsWriter, goList);
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
                rotationY += ROTATION_SPEED;
            if (Keyboard.GetState().IsKeyDown(Keys.D) || Keyboard.GetState().IsKeyDown(Keys.Right))
                rotationY -= ROTATION_SPEED;

            // Move forward/backward
            if (Keyboard.GetState().IsKeyDown(Keys.W) || Keyboard.GetState().IsKeyDown(Keys.Up))
                playerPos += Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(rotationY));
            if (Keyboard.GetState().IsKeyDown(Keys.S) || Keyboard.GetState().IsKeyDown(Keys.Down))
                playerPos -= Vector3.Transform(new Vector3(0, 0, 1), Matrix.CreateRotationY(rotationY));

            // Strafe left/right
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
                playerPos += Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(rotationY));
            if (Keyboard.GetState().IsKeyDown(Keys.E))
                playerPos -= Vector3.Transform(new Vector3(1, 0, 0), Matrix.CreateRotationY(rotationY));

            if (Keyboard.GetState().IsKeyDown(Keys.Space) && playerPos.Y <= 0)
            {
                jumpHeight = 10;
            }

            if (jumpHeight > 0)
            {
                playerPos.Y += 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                jumpHeight -= 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (playerPos.Y > 0 && jumpHeight <= 0)
                playerPos.Y -= 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Waypoints
            foreach (GameObject gameObject in gameObjects)
            {
                // Must have at least 2 waypoints (1 makes no sense)
                if (gameObject.Waypoints.Count > 1)
                {
                    // calculate the distance for error margin
                    float distance = Math.Abs((gameObject.position - gameObject.Waypoints[gameObject.CurrentWaypoint]).Length());
                    if (distance < 0.5f)
                        gameObject.CurrentWaypoint = ((gameObject.CurrentWaypoint + 1 >= gameObject.Waypoints.Count) ? 0 : gameObject.CurrentWaypoint + 1);

                    // move the object
                    Vector3 direction = gameObject.Waypoints[gameObject.CurrentWaypoint] - gameObject.position;
                    direction.Normalize();
                    gameObject.position += direction * (float)gameTime.ElapsedGameTime.TotalSeconds * gameObject.MoveSpeed;
                }
            }

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
            foreach (var mesh in avatar.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.DiffuseColor = Color.Red.ToVector3();
                }
            }
            avatar.Draw(world, view, projection);

            // Draw ground
            world = Matrix.CreateScale(100);
            ground.Draw(world, view, projection);

            // Draw earth
            //world = Matrix.CreateScale(1);
            //earth.Draw(world, view, projection);

            // Draw sky
            world = Matrix.CreateScale(100);
            sky.Draw(world, view, projection);

            // Draw gameobjects
            foreach (GameObject gameObject in gameObjects)
            {
                // Scale * RotationXYZ * Translation
                world = Matrix.CreateScale(gameObject.scale)
                        * Matrix.CreateRotationX(gameObject.rotateXSpeed * (float)gameTime.TotalGameTime.TotalSeconds)
                        * (gameObject.rotateYSpeed != 0 ? Matrix.CreateRotationY(gameObject.rotateYSpeed * (float)gameTime.TotalGameTime.TotalSeconds) : Matrix.CreateRotationY(MathHelper.ToRadians(gameObject.rotateYFacing)))
                        * Matrix.CreateRotationZ(gameObject.rotateZSpeed * (float)gameTime.TotalGameTime.TotalSeconds)
                        * Matrix.CreateTranslation(gameObject.position);

                if (gameObject.modelName == ModelName.earth)
                    earth.Draw(world, view, projection);
                else if (gameObject.modelName == ModelName.gorilla)
                {
                    foreach (var mesh in gorilla.Meshes)
                    {
                        foreach (BasicEffect effect in mesh.Effects)
                        {
                            effect.DiffuseColor = Color.Black.ToVector3();
                        }
                    }
                    gorilla.Draw(world, view, projection);
                }
                else if (gameObject.modelName == ModelName.male)
                    male.Draw(world, view, projection);
                else if (gameObject.modelName == ModelName.female)
                    female.Draw(world, view, projection);
                else if (gameObject.modelName == ModelName.wall)
                    wall.Draw(world, view, projection);
                else if (gameObject.modelName == ModelName.cage)
                    cage.Draw(world, view, projection);
            }

            base.Draw(gameTime);
        }
    }
}
