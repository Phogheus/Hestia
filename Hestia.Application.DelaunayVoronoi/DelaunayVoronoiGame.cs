using System;
using Hestia.Base.Geometry.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DelVoro = Hestia.Base.Geometry.Utilities.DelaunayVoronoi;

namespace Hestia.Application.DelaunayVoronoi
{
    public class DelaunayVoronoiGame : Game
    {
        private const int DEFAULT_POINT_WIDTH = 3;
        private const int DEFAULT_LINE_THICKNESS = 3;

        private readonly GraphicsDeviceManager _graphicsDeviceManager;
        private readonly DelVoro _voro = new DelVoro(800, 600);

        private SpriteBatch _spriteBatch;

        public DelaunayVoronoiGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            Window.Title = "Look at the lines, they're beautiful!";

            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.PreferredBackBufferWidth = 800;
            _graphicsDeviceManager.PreferredBackBufferHeight = 600;
            _graphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _voro.GenerateBowyerWatsonResult();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                _voro.GenerateBowyerWatsonResult();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch!.Begin();

            foreach (var edge in _voro.DelaunayTriangleDistinctEdges)
            {
                DrawLine(edge, Color.Green);
            }

            foreach (var edge in _voro.VoronoiPolygonDistinctEdges)
            {
                DrawLine(edge, Color.Orange);
            }

            foreach (var triangle in _voro.DelaunayTriangles)
            {
                DrawPoint(triangle.Circumcircle.CenterPoint, Color.MediumPurple);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        protected void DrawPoint(Point2D point, Color color, int diameter = DEFAULT_POINT_WIDTH)
        {
            diameter = Math.Max(1, diameter);

            var pointTexture = new Texture2D(_spriteBatch!.GraphicsDevice, diameter, diameter);

            var pixels = new Color[diameter * diameter];
            Array.Fill(pixels, color);
            pointTexture.SetData(pixels);

            var position = new Vector2((float)point.X, (float)point.Y);

            _spriteBatch.Draw(pointTexture, position, color);
        }

        private void DrawLine(Line2D line, Color color, int lineThickness = DEFAULT_LINE_THICKNESS)
        {
            lineThickness = Math.Max(1, lineThickness);

            var lineLength = (int)line.Length;

            if (lineLength <= 0)
            {
                return;
            }

            var lineTexture = new Texture2D(_spriteBatch!.GraphicsDevice, lineLength, lineThickness);

            var pixels = new Color[(int)line.Length * lineThickness];
            Array.Fill(pixels, color);
            lineTexture.SetData(pixels);

            // Get line rotation, origin, and position
            var rotation = (float)Math.Atan2(line.End.Y - line.Start.Y, line.End.X - line.Start.X);
            var origin = new Vector2(0, lineThickness / 2);
            var position = new Vector2((float)line.Start.X, (float)line.Start.Y);

            _spriteBatch.Draw(lineTexture, position, null, Color.White, rotation, origin, 1.0f, SpriteEffects.None, 1.0f);
        }
    }
}