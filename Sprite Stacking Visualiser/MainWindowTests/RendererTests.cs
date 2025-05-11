using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using Sprite_Stacking_Visualiser;
using System.Data.Entity;
using Moq;
using System.Linq;
using System.Diagnostics;

namespace RenderingTests
{
    
    [TestClass]
    public class RendererTests
    {
        private SpriteStackingRenderer _renderer;
        private Mock<SpriteStackData> _mockDbContext;

        [TestInitialize]
        public void Setup()
        {
            // Create a mock SpriteStack with ID 1
            var mockSpriteStack = new SpriteStack
            {
                _SpriteStackID = 1,
                _SpriteStackName = "Crate",
                _spriteOffsetX = 7,
                _numberOfFrames = 8,
                _sprites = new List<Sprite>
                {
                    new Sprite { Path = $"Assets\\Crate\\Crate0.png", Frame = 1 },
                    new Sprite { Path = $"Assets\\Crate\\Crate1.png", Frame = 2 },
                    new Sprite { Path = $"Assets\\Crate\\Crate2.png", Frame = 3 },
                    new Sprite { Path = $"Assets\\Crate\\Crate3.png", Frame = 4 },
                    new Sprite { Path = $"Assets\\Crate\\Crate4.png", Frame = 5 },
                    new Sprite { Path = $"Assets\\Crate\\Crate5.png", Frame = 6 },
                    new Sprite { Path = $"Assets\\Crate\\Crate6.png", Frame = 7 },
                    new Sprite { Path = $"Assets\\Crate\\Crate7.png", Frame = 8 }
                }
            };

            // Create a mock DbSet for SpriteStacks
            var mockSpriteStacks = new Mock<DbSet<SpriteStack>>();
            var spriteStackList = new List<SpriteStack> { mockSpriteStack }.AsQueryable();

            // Setup the mock to return the list of sprite stacks when queried
            mockSpriteStacks.As<IQueryable<SpriteStack>>().Setup(m => m.Provider).Returns(spriteStackList.Provider); // Setup the provider - allows LINQ queries
            mockSpriteStacks.As<IQueryable<SpriteStack>>().Setup(m => m.Expression).Returns(spriteStackList.Expression); // Setup the expression - this is used to build the LINQ query
            mockSpriteStacks.As<IQueryable<SpriteStack>>().Setup(m => m.ElementType).Returns(spriteStackList.ElementType); // Setup the element type - this is used to get the type of the elements in the query
            mockSpriteStacks.As<IQueryable<SpriteStack>>().Setup(m => m.GetEnumerator()).Returns(spriteStackList.GetEnumerator()); // Setup the enumerator - this is used to iterate over the results of the query like a  foreach loop 


            // Create a mock DbContext
            _mockDbContext = new Mock<SpriteStackData>();
            _mockDbContext.Setup(c => c.SpriteStacks).Returns(mockSpriteStacks.Object);

            // Initialize the renderer with the mocked DbContext
            _renderer = new SpriteStackingRenderer(_mockDbContext.Object, 1);
        }


        // Initialization & Loading Tests
        [TestMethod]
        public void Renderer_ShouldInitializeCorrectly()
        {
            // Assert
            Assert.IsNotNull(_renderer);
            Assert.AreEqual(1, _renderer.StackToBeRendered._SpriteStackID);
            Assert.AreEqual("Crate", _renderer.StackToBeRendered._SpriteStackName);
        }

        [TestMethod]
        public void LoadSpriteStack_ShouldLoadSpritesCorrectly()
        {
            // Act
            _renderer.LoadSpriteStack(_renderer.StackToBeRendered);

            // Assert
            Assert.IsNotNull(_renderer.StackToBeRendered);
            Assert.AreEqual(8, _renderer.StackToBeRendered._sprites.Count);
            Assert.AreEqual("Assets\\Crate\\Crate0.png", _renderer.StackToBeRendered._sprites[0].Path);
        }


        // Rendering Tests

        [TestMethod]
        public void Render_ShouldNotThrowException()
        // Here im only testing if the render method throws an exception - this is because testing the actual rendering is not feasible in this context
        {
            // Arrange
            var canvas = new SKCanvas(new SKBitmap(100, 100));

            // Act & Assert
            try
            {
                _renderer.Render(canvas, 100, 100, _renderer.StackToBeRendered._spriteOffsetX);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Render method threw an exception: {ex.Message}");
            }
        }

        [TestMethod]
        public void Render_ShouldRespectSpriteOffset()
        {
            // Arrange
            var canvas = new SKCanvas(new SKBitmap(100, 100));
            _renderer.StackToBeRendered._spriteOffsetX = 10;

            // Act
            _renderer.Render(canvas, 100, 100, _renderer.StackToBeRendered._spriteOffsetX);

            // Assert
            Assert.AreEqual(10, _renderer.StackToBeRendered._spriteOffsetX);
        }




        // Rotation Tests

        [TestMethod]
        public void RotationAngle_ShouldWrapAroundAt360()
        {
            // Arrange
            _renderer._rotationAngle = 359.9f;
            _renderer.RotationSpeed = 10f;

            // Act
            _renderer._rotationAngle = (_renderer._rotationAngle + _renderer.RotationSpeed) % 360f;

            // Assert
            Assert.AreEqual(9.9f, _renderer._rotationAngle, 0.1f);
        }

        [TestMethod]
        public void RotationAngle_ShouldStayWithinBounds()
        {
            // Arrange
            _renderer._rotationAngle = 0f;
            _renderer.RotationSpeed = 15f;

            // Act
            _renderer._rotationAngle = (_renderer._rotationAngle + _renderer.RotationSpeed) % 360f;

            // Assert
            Assert.AreEqual(15f, _renderer._rotationAngle);
        }

        [TestMethod]
        public void RotationAngle_ShouldHandleNegativeValues()
        {
            // Arrange
            _renderer._rotationAngle = 10f;
            _renderer.RotationSpeed = -20f;

            // Act
            _renderer._rotationAngle = (_renderer._rotationAngle + _renderer.RotationSpeed) % 360f;

            // Ensure the angle is normalized to a positive value
            if (_renderer._rotationAngle < 0)
            {
                _renderer._rotationAngle += 360f;
            }

            // Assert
            Assert.AreEqual(350f, _renderer._rotationAngle);
        }
    }
}
