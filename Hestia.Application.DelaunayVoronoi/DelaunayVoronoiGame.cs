using System;
using System.Linq;
using Hestia.Base.Geometry.Models;
using Hestia.Base.Geometry.Utilities;
using Hestia.Base.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DelVoro = Hestia.Base.RandomGenerators.DelaunayVoronoi;

namespace Hestia.Application.DelaunayVoronoi
{
    public class DelaunayVoronoiGame : Game
    {
        #region Fields

        private const int WINDOW_WIDTH = 800;
        private const int WINDOW_HEIGHT = 600;

        private const int DEFAULT_POINT_WIDTH = 3;
        private const int DEFAULT_LINE_THICKNESS = 3;

        private readonly GraphicsDeviceManager _graphicsDeviceManager;

        private SpriteBatch _spriteBatch;
        private SpriteFont _dvFont;
        private DelVoro _voro;
        private Polygon2D _randomPolygon;
        private Triangle2D[] _polygonTriangulation;
        private bool _drawPolygon = true;
        private bool _drawTriangles;
        private bool _drawTriangleCircumcenters;
        private DateTime _timeOfLastKeyProcessed;

        private static readonly string[] INSTRUCTIONS = new string[]
        {
            "Press 1 to generate a Delaunay-Voronoi Diagram",
            "Press 2 to generate a random triangulated polygon",
            "Press 3 to show/hide polygons",
            "Press 4 to show/hide triangles",
            "Press 5 to show/hide triangle circumcenters",
        };

        #endregion Fields

        #region Constructors

        public DelaunayVoronoiGame()
        {
            _graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        #endregion Constructors

        #region Protected Methods

        protected override void Initialize()
        {
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = false;
            Window.Title = "Look at the lines, they're beautiful!";

            _graphicsDeviceManager.IsFullScreen = false;
            _graphicsDeviceManager.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphicsDeviceManager.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _dvFont = Content.Load<SpriteFont>("DVFont");

            _voro = DelVoro.GenerateBowyerWatsonResult(WINDOW_WIDTH, WINDOW_HEIGHT);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if ((DateTime.UtcNow - _timeOfLastKeyProcessed).TotalSeconds > .5d)
            {
                ProcessKeyPresses();
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch!.Begin();

            DrawInstructions();
            DrawDelaunayVoronoi();
            DrawRandomPolygon();

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion Protected Methods

        #region Private Methods

        private void ProcessKeyPresses()
        {
            var pressedKeys = Keyboard.GetState().GetPressedKeys();

            if (!pressedKeys.Any())
            {
                return;
            }

            var firstPressedKey = pressedKeys[0];

            switch (firstPressedKey)
            {
                case Keys.D1: // Generate and draw Delaunay-Voronoi
                    _randomPolygon = null;
                    _polygonTriangulation = null;
                    _voro = DelVoro.GenerateBowyerWatsonResult(WINDOW_WIDTH, WINDOW_HEIGHT);

                    _drawPolygon = true;
                    _drawTriangles = true;
                    _drawTriangleCircumcenters = false;
                    _timeOfLastKeyProcessed = DateTime.UtcNow;
                    break;

                case Keys.D2: // Generate and draw random triangulated polygon
                    var x = Random.Shared.Next(-WINDOW_WIDTH / 4, (WINDOW_WIDTH / 4) + 1);
                    var y = Random.Shared.Next(-WINDOW_HEIGHT / 4, (WINDOW_HEIGHT / 4) - 1);
                    var randPoint = new Point2D(x, y) + new Point2D(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2);

                    _randomPolygon = RandomPolygonAndTriangulation();
                    _polygonTriangulation = _randomPolygon.TriangulateAtPoint(randPoint);
                    _voro = null;

                    _drawPolygon = true;
                    _drawTriangles = true;
                    _drawTriangleCircumcenters = false;
                    _timeOfLastKeyProcessed = DateTime.UtcNow;
                    break;

                case Keys.D3:
                    _drawPolygon = !_drawPolygon;
                    _timeOfLastKeyProcessed = DateTime.UtcNow;
                    break;

                case Keys.D4:
                    _drawTriangles = !_drawTriangles;
                    _timeOfLastKeyProcessed = DateTime.UtcNow;
                    break;

                case Keys.D5:
                    _drawTriangleCircumcenters = !_drawTriangleCircumcenters;
                    _timeOfLastKeyProcessed = DateTime.UtcNow;
                    break;
            }
        }

        private void DrawInstructions()
        {
            for (var i = 0; i < INSTRUCTIONS.Length; i++)
            {
                _spriteBatch.DrawString(_dvFont, INSTRUCTIONS[i], new Vector2(10, 10 + (20 * i)), Color.Black);
            }
        }

        private void DrawDelaunayVoronoi()
        {
            if (_voro == null)
            {
                return;
            }

            if (_drawPolygon)
            {
                foreach (var edge in _voro.VoronoiPolygonDistinctEdges)
                {
                    DrawLine(edge, Color.Orange);
                }
            }

            if (_drawTriangles)
            {
                foreach (var edge in _voro.DelaunayTriangleDistinctEdges)
                {
                    DrawLine(edge, Color.Green);
                }
            }

            if (_drawTriangleCircumcenters)
            {
                foreach (var triangle in _voro.DelaunayTriangles)
                {
                    DrawPoint(triangle.Circumcircle.CenterPoint, Color.MediumPurple);
                }
            }
        }

        private void DrawRandomPolygon()
        {
            if (_randomPolygon == null || _polygonTriangulation == null)
            {
                return;
            }

            if (_drawPolygon)
            {
                foreach (var edge in _randomPolygon.Edges)
                {
                    DrawLine(edge, Color.MediumPurple);
                }
            }

            if (_drawTriangles)
            {
                foreach (var triangle in _polygonTriangulation)
                {
                    foreach (var edge in triangle.Edges)
                    {
                        DrawLine(edge, Color.Orange);
                    }
                }
            }

            if (_drawTriangleCircumcenters)
            {
                foreach (var triangle in _polygonTriangulation)
                {
                    DrawPoint(triangle.CentroidPoint, Color.MediumPurple);
                }
            }
        }

        private void DrawPoint(Point2D point, Color color, int diameter = DEFAULT_POINT_WIDTH)
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

        private static Polygon2D RandomPolygonAndTriangulation()
        {
            var vertexCount = Random.Shared.Next(6, 12);
            var radius = Random.Shared.Next(75, 125);

            var pointToRotateAroundCircle = new Point2D(WINDOW_WIDTH / 2, (WINDOW_HEIGHT / 2) - radius);
            var origin = new Point2D(WINDOW_WIDTH / 2, WINDOW_HEIGHT / 2);

            var angleStep = 360d / vertexCount;

            var points = Enumerable.Range(0, vertexCount).Select(x =>
            {
                var angleInRadians = angleStep * x * MathEnhanced.DEG_2_RAD;
                var rotatedPoint = GeometryUtilities.RotatePointAroundOrigin(pointToRotateAroundCircle, origin, angleInRadians);

                // Wiggle the points a little
                rotatedPoint.X += Random.Shared.Next(-5, 5);
                rotatedPoint.Y += Random.Shared.Next(-5, 5);

                return rotatedPoint;
            }).ToArray();

            return new Polygon2D(points);
        }

        #endregion Private Methods
    }
}
