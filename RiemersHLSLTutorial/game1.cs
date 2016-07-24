using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace XNAseries3
{
    public struct MyOwnVertexFormatT
    {
        private Vector3 position;
        private Vector2 texCoord;
        private Vector3 normal;

        public MyOwnVertexFormatT(Vector3 position, Vector2 texCoord, Vector3 normal)
        {
            this.position = position;
            this.texCoord = texCoord;
            this.normal = normal;
        }

        public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
             (
                 new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
                new VertexElement(sizeof(float) * (3 + 2), VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
             );
    }


    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;

        public KeyboardState KeyboardInput;
        public KeyboardState PreviousKeyboardInput;

        Effect effect;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        VertexBuffer vertexBuffer;
        Vector3 cameraPos;
        Texture2D streetTexture;
        Model carModel;
        Model lamppost;
        Texture2D[] carTextures;
        Texture2D[] lampTextures;

        Vector3 lightPos;
        float lightPower;
        float ambientPower;


        Matrix lightsViewProjectionMatrix;
        RenderTarget2D renderTarget;
        Texture2D shadowMap;

        private SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Riemer's XNA Tutorials -- Series 3";

            lightPos = new Vector3(-18, 5, -2);
            ambientPower = 0.2f;
            lightPower = 1.0f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            device = GraphicsDevice;

            spriteBatch = new SpriteBatch(GraphicsDevice);

            PresentationParameters pp = device.PresentationParameters;
            //renderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, true, device.DisplayMode.Format, DepthFormat.Depth24);
            renderTarget = new RenderTarget2D(device, pp.BackBufferWidth, pp.BackBufferHeight, false, SurfaceFormat.Color, DepthFormat.Depth24);

            //effect = Content.Load<Effect>("effects");
            effect = Content.Load<Effect>("Effect4");
            streetTexture = Content.Load<Texture2D>("streettexture");
            carModel = LoadModel("car", out carTextures);
            lamppost = LoadModel("lamppost", out lampTextures);
            //lamppost = Content.Load<Model>("lamppost");
            SetUpVertices();
            SetUpCamera();
        }


        private void SetUpVertices()
        {
            MyOwnVertexFormatT[] vertices = new MyOwnVertexFormatT[18];

            vertices[0] = new MyOwnVertexFormatT(new Vector3(-20, 0, 10), new Vector2(-0.25f, 25.0f), new Vector3(0, 1, 0));
            vertices[1] = new MyOwnVertexFormatT(new Vector3(-20, 0, -100), new Vector2(-0.25f, 0.0f), new Vector3(0, 1, 0));
            vertices[2] = new MyOwnVertexFormatT(new Vector3(2, 0, 10), new Vector2(0.25f, 25.0f), new Vector3(0, 1, 0));
            vertices[3] = new MyOwnVertexFormatT(new Vector3(2, 0, -100), new Vector2(0.25f, 0.0f), new Vector3(0, 1, 0));
            vertices[4] = new MyOwnVertexFormatT(new Vector3(2, 0, 10), new Vector2(0.25f, 25.0f), new Vector3(-1, 0, 0));
            vertices[5] = new MyOwnVertexFormatT(new Vector3(2, 0, -100), new Vector2(0.25f, 0.0f), new Vector3(-1, 0, 0));
            vertices[6] = new MyOwnVertexFormatT(new Vector3(2, 1, 10), new Vector2(0.375f, 25.0f), new Vector3(-1, 0, 0));
            vertices[7] = new MyOwnVertexFormatT(new Vector3(2, 1, -100), new Vector2(0.375f, 0.0f), new Vector3(-1, 0, 0));
            vertices[8] = new MyOwnVertexFormatT(new Vector3(2, 1, 10), new Vector2(0.375f, 25.0f), new Vector3(0, 1, 0));
            vertices[9] = new MyOwnVertexFormatT(new Vector3(2, 1, -100), new Vector2(0.375f, 0.0f), new Vector3(0, 1, 0));
            vertices[10] = new MyOwnVertexFormatT(new Vector3(3, 1, 10), new Vector2(0.5f, 25.0f), new Vector3(0, 1, 0));
            vertices[11] = new MyOwnVertexFormatT(new Vector3(3, 1, -100), new Vector2(0.5f, 0.0f), new Vector3(0, 1, 0));
            vertices[12] = new MyOwnVertexFormatT(new Vector3(13, 1, 10), new Vector2(0.75f, 25.0f), new Vector3(0, 1, 0));
            vertices[13] = new MyOwnVertexFormatT(new Vector3(13, 1, -100), new Vector2(0.75f, 0.0f), new Vector3(0, 1, 0));
            vertices[14] = new MyOwnVertexFormatT(new Vector3(13, 1, 10), new Vector2(0.75f, 25.0f), new Vector3(-1, 0, 0));
            vertices[15] = new MyOwnVertexFormatT(new Vector3(13, 1, -100), new Vector2(0.75f, 0.0f), new Vector3(-1, 0, 0));
            vertices[16] = new MyOwnVertexFormatT(new Vector3(13, 21, 10), new Vector2(1.25f, 25.0f), new Vector3(-1, 0, 0));
            vertices[17] = new MyOwnVertexFormatT(new Vector3(13, 21, -100), new Vector2(1.25f, 0.0f), new Vector3(-1, 0, 0));

            vertexBuffer = new VertexBuffer(device, MyOwnVertexFormatT.VertexDeclaration, vertices.Length, BufferUsage.WriteOnly);
            vertexBuffer.SetData(vertices);
        }


        private void SetUpCamera()
        {
            cameraPos = new Vector3(-25, 13, 18);
            viewMatrix = Matrix.CreateLookAt(cameraPos, new Vector3(0, 2, -12), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 200.0f);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            PreviousKeyboardInput = KeyboardInput;
            KeyboardInput = Keyboard.GetState();

            if (KeyboardInput.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (KeyboardInput.IsKeyDown(Keys.NumPad0))
            {
                lightPos.X += 0.5f;
            }
            if (KeyboardInput.IsKeyDown(Keys.NumPad8))
            {
                lightPos.X -= 0.5f;
            }
            if (KeyboardInput.IsKeyDown(Keys.NumPad4))
            {
                lightPos.Z += 0.5f;
            }
            if (KeyboardInput.IsKeyDown(Keys.NumPad6))
            {
                lightPos.Z -= 0.5f;
            }
            if (KeyboardInput.IsKeyDown(Keys.NumPad7))
            {
                lightPos.Y += 0.5f;
            }
            if (KeyboardInput.IsKeyDown(Keys.NumPad1))
            {
                lightPos.Y -= 0.5f;
            }

            if (KeyboardInput.IsKeyDown(Keys.A))
            {
                ambientPower += 0.1f;
            }
            if (KeyboardInput.IsKeyDown(Keys.Q))
            {
                ambientPower -= 0.1f;
            }

            if (KeyboardInput.IsKeyDown(Keys.Z))
            {
                lightPower += 0.1f;
            }
            if (KeyboardInput.IsKeyDown(Keys.S))
            {
                lightPower -= 0.1f;
            }

            

            UpdateLightData();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            //device.SetRenderTarget(renderTarget);
            //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            //DrawScene("ShadowMap");
            //device.SetRenderTarget(null);
            //shadowMap = (Texture2D)renderTarget;

            //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            //using (SpriteBatch sprite = new SpriteBatch(device))
            //{
            //    sprite.Begin();
            //    sprite.Draw(shadowMap, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 0.4f, SpriteEffects.None, 1);
            //    sprite.End();
            //}

            


            device.SetRenderTarget(renderTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            DrawScene("ShadowMap");

            device.SetRenderTarget(null);
            shadowMap = (Texture2D)renderTarget;

            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            DrawScene("ShadowedScene");
            

            //debug => spb mess up with drawing here
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, null, null);
            Rectangle rect = new Rectangle(0, 0, 256, 256);
            spriteBatch.Draw((Texture2D)renderTarget, rect, Color.White);
            spriteBatch.End();

            GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            //debug

            shadowMap = null;

            base.Draw(gameTime);





            //device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.DarkSlateBlue, 1.0f, 0);
            //GraphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };

            //DrawScene("ShadowMap");

            //effect.CurrentTechnique = effect.Techniques["SimplestTex"];
            //effect.Parameters["xTexture"].SetValue(streetTexture);
            //effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * viewMatrix * projectionMatrix);

            //effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            //effect.Parameters["xLightPos"].SetValue(lightPos);
            //effect.Parameters["xLightPower"].SetValue(lightPower);
            //effect.Parameters["xAmbient"].SetValue(ambientPower);

            //effect.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);

            //foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            //{
            //    pass.Apply();

            //    device.SetVertexBuffer(vertexBuffer);
            //    device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 16);
            //}

            //Matrix car1Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-3, 0, -15);
            //DrawModel(carModel, carTextures, car1Matrix, "SimplestTex");

            //Matrix car2Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi * 5.0f / 8.0f) * Matrix.CreateTranslation(-28, 0, -1.9f);
            //DrawModel(carModel, carTextures, car2Matrix, "SimplestTex");

            //base.Draw(gameTime);
        }

        private Model LoadModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = Content.Load<Model>(assetName);
            textures = new Texture2D[7];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }

        private void DrawModel(Model model, Texture2D[] textures, Matrix wMatrix, string technique)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            int i = 0;
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    Matrix worldMatrix = modelTransforms[mesh.ParentBone.Index] * wMatrix;
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    currentEffect.Parameters["xWorldViewProjection"].SetValue(worldMatrix * viewMatrix * projectionMatrix);

                    if (textures != null && textures[i] != null)
                    {
                        currentEffect.Parameters["xTexture"].SetValue(textures[i++]);
                    }

                    currentEffect.Parameters["xWorld"].SetValue(worldMatrix);
                    currentEffect.Parameters["xLightPos"].SetValue(lightPos);
                    currentEffect.Parameters["xLightPower"].SetValue(lightPower);
                    currentEffect.Parameters["xAmbient"].SetValue(ambientPower);

                    currentEffect.Parameters["xLightsWorldViewProjection"].SetValue(worldMatrix * lightsViewProjectionMatrix);
                    currentEffect.Parameters["xShadowMap"].SetValue(shadowMap);
                }
                mesh.Draw();
            }
        }

        //private void UpdateLightData()
        //{
        //    lightPos = new Vector3(-10, 4, -2);
        //    lightPower = 1.0f;
        //    ambientPower = 0.001f;
        //}

        private void UpdateLightData()
        {
            

            Matrix lightsView = Matrix.CreateLookAt(lightPos, new Vector3(-2, 3, -10), new Vector3(0, 1, 0));
            Matrix lightsProjection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1f, 5f, 100f);

            lightsViewProjectionMatrix = lightsView * lightsProjection;
        }

        private void DrawScene(string technique)
        {
            effect.CurrentTechnique = effect.Techniques[technique];
            effect.Parameters["xWorldViewProjection"].SetValue(Matrix.Identity * viewMatrix * projectionMatrix);
            effect.Parameters["xTexture"].SetValue(streetTexture);
            effect.Parameters["xWorld"].SetValue(Matrix.Identity);
            effect.Parameters["xLightPos"].SetValue(lightPos);
            effect.Parameters["xLightPower"].SetValue(lightPower);
            effect.Parameters["xAmbient"].SetValue(ambientPower);
            effect.Parameters["xLightsWorldViewProjection"].SetValue(Matrix.Identity * lightsViewProjectionMatrix);
            effect.Parameters["xShadowMap"].SetValue(shadowMap);

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();

                device.SetVertexBuffer(vertexBuffer);
                device.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 16);
            }

            //Matrix lampp1matrix = Matrix.CreateScale(0.05f) * Matrix.CreateRotationY(MathHelper.PiOver2) * Matrix.CreateTranslation(-4.0f, 5, 1);
            //DrawModel(lamppost, null, lampp1matrix, technique);

            Matrix car1Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi) * Matrix.CreateTranslation(-3, 0, -15);
            DrawModel(carModel, carTextures, car1Matrix, technique);

            Matrix car2Matrix = Matrix.CreateScale(4f) * Matrix.CreateRotationY(MathHelper.Pi * 5.0f / 8.0f) * Matrix.CreateTranslation(-28, 0, -1.9f);
            DrawModel(carModel, carTextures, car2Matrix, technique);
        }
    }
}