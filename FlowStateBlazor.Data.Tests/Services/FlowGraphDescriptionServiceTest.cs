using FlowStateBlazor.Data.Data;
using FlowStateBlazor.Data.Models;
using FlowStateBlazor.Data.Services;
using FlowStateBlazor.Data.Tests.Builders;

namespace FlowStateBlazor.Data.Tests.Services
{
    [TestClass]
    [DoNotParallelize]
    public sealed class FlowGraphDescriptionServiceTest
    {
        private TestDatabaseFixture? _fixture;
        private FlowGraphDescriptionService? _service;
        private FlowStateContext? _context;

        [TestInitialize]
        public void Setup()
        {
            // Utiliser SQLite in-memory pour les tests (plus rapide)
            var sqliteContext = TestContextBuilder.UseSqliteFile("unit_tests_sqlite.db").BuildSqliteContext();
            _fixture = new TestDatabaseFixture(sqliteContext);
            _context = sqliteContext;
            _service = new FlowGraphDescriptionService(sqliteContext);
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (_fixture != null)
            {
                await _fixture.DisposeAsync();
            }
        }

        #region AddAsync Tests

        [TestMethod]
        [Description("Ajoute une nouvelle FlowGraphDescription")]
        public async Task AddAsync_CreatesNewFlowGraphDescription()
        {
            // Arrange
            var flow = new FlowGraphDescription
            {
                Name = "TestFlow",
                Description = "Test Description",
                JsonFlowSerialized = "{\"test\": \"data\"}"
            };

            // Act
            await _service!.AddAsync(flow);

            // Assert
            var added = await _service.FindByNameAsync("TestFlow");
            Assert.IsNotNull(added);
            Assert.AreEqual("TestFlow", added.Name);
            Assert.AreEqual("Test Description", added.Description);
        }

        [TestMethod]
        [Description("Génère un ID auto-incrémenté")]
        public async Task AddAsync_GeneratesAutoIncrementId()
        {
            // Arrange
            var flow1 = new FlowGraphDescription { Name = "Flow1", JsonFlowSerialized = "{}" };
            var flow2 = new FlowGraphDescription { Name = "Flow2", JsonFlowSerialized = "{}" };

            // Act
            await _service!.AddAsync(flow1);
            await _service.AddAsync(flow2);

            // Assert
            Assert.AreNotEqual(flow1.Id, flow2.Id);
            Assert.IsTrue(flow1.Id > 0);
            Assert.IsTrue(flow2.Id > flow1.Id);
        }

        [TestMethod]
        [Description("Lance une exception si le nom est vide")]
        public async Task AddAsync_ThrowsException_WhenNameIsEmpty()
        {
            // Arrange
            var flow = new FlowGraphDescription { Name = "", JsonFlowSerialized = "{}" };

            // Act & Assert
            try
            {
                await _service!.AddAsync(flow);
                Assert.Fail("Exception attendue");
            }
            catch (Exception)
            {
                // Exception attendue
            }
        }

        #endregion

        #region UpdateAsync Tests

        [TestMethod]
        [Description("Met à jour une FlowGraphDescription existante")]
        public async Task UpdateAsync_UpdatesExistingFlow()
        {
            // Arrange
            var flow = new FlowGraphDescription 
            { 
                Name = "OriginalName", 
                Description = "Original",
                JsonFlowSerialized = "{\"original\": true}" 
            };
            await _service!.AddAsync(flow);

            // Act
            flow.Name = "UpdatedName";
            flow.Description = "Updated";
            flow.JsonFlowSerialized = "{\"updated\": true}";
            await _service.UpdateAsync(flow);

            // Assert
            var updated = await _service.FindByIdAsync(flow.Id);
            Assert.IsNotNull(updated);
            Assert.AreEqual("UpdatedName", updated.Name);
            Assert.AreEqual("Updated", updated.Description);
            Assert.AreEqual("{\"updated\": true}", updated.JsonFlowSerialized);
        }

        #endregion

        #region DeleteAsync Tests

        [TestMethod]
        [Description("Supprime une FlowGraphDescription par ID")]
        public async Task RemoveAsync_RemovesFlowById()
        {
            // Arrange
            var flow = new FlowGraphDescription { Name = "ToDelete", JsonFlowSerialized = "{}" };
            await _service!.AddAsync(flow);
            var flowId = flow.Id;

            // Act
            await _service.RemoveAsync(flow);

            // Assert
            var deleted = await _service.FindByIdAsync(flowId);
            Assert.IsNull(deleted);
        }

        #endregion

        #region FindByIdAsync Tests

        [TestMethod]
        [Description("Trouve une FlowGraphDescription par ID")]
        public async Task FindByIdAsync_ReturnsFlowById()
        {
            // Arrange
            var flow = new FlowGraphDescription { Name = "TestFlow", JsonFlowSerialized = "{}" };
            await _service!.AddAsync(flow);

            // Act
            var found = await _service.FindByIdAsync(flow.Id);

            // Assert
            Assert.IsNotNull(found);
            Assert.AreEqual(flow.Id, found.Id);
            Assert.AreEqual("TestFlow", found.Name);
        }

        [TestMethod]
        [Description("Retourne null si ID n'existe pas")]
        public async Task FindByIdAsync_ReturnsNull_WhenIdNotFound()
        {
            // Act
            var found = await _service!.FindByIdAsync(99999);

            // Assert
            Assert.IsNull(found);
        }

        #endregion

        #region FindByNameAsync Tests

        [TestMethod]
        [Description("Trouve une FlowGraphDescription par nom")]
        public async Task FindByNameAsync_ReturnsFlowByName()
        {
            // Arrange
            var flow = new FlowGraphDescription { Name = "UniqueFlow", Description = "Test", JsonFlowSerialized = "{}" };
            await _service!.AddAsync(flow);

            // Act
            var found = await _service.FindByNameAsync("UniqueFlow");

            // Assert
            Assert.IsNotNull(found);
            Assert.AreEqual("UniqueFlow", found.Name);
        }

        [TestMethod]
        [Description("Retourne null si le nom n'existe pas")]
        public async Task FindByNameAsync_ReturnsNull_WhenNameNotFound()
        {
            // Act
            var found = await _service!.FindByNameAsync("NonExistentName");

            // Assert
            Assert.IsNull(found);
        }

        #endregion

        #region ExistsByIdAsync Tests

        [TestMethod]
        [Description("Vérifie l'existance d'une FlowGraphDescription par ID")]
        public async Task ExistsByIdAsync_ReturnsTrueWhenExists()
        {
            // Arrange
            var flow = new FlowGraphDescription { Name = "Test", JsonFlowSerialized = "{}" };
            await _service!.AddAsync(flow);

            // Act
            var exists = await _service.ExistsByIdAsync(flow.Id);

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        [Description("Retourne false si ID n'existe pas")]
        public async Task ExistsByIdAsync_ReturnsFalseWhenNotExists()
        {
            // Act
            var exists = await _service!.ExistsByIdAsync(99999);

            // Assert
            Assert.IsFalse(exists);
        }

        #endregion

        #region ExistsByNameAsync Tests

        [TestMethod]
        [Description("Vérifie l'existance d'une FlowGraphDescription par nom")]
        public async Task ExistsByNameAsync_ReturnsTrueWhenExists()
        {
            // Arrange
            var flow = new FlowGraphDescription { Name = "UniqueTestName", JsonFlowSerialized = "{}" };
            await _service!.AddAsync(flow);

            // Act
            var exists = await _service.ExistsByNameAsync("UniqueTestName");

            // Assert
            Assert.IsTrue(exists);
        }

        [TestMethod]
        [Description("Retourne false si le nom n'existe pas")]
        public async Task ExistsByNameAsync_ReturnsFalseWhenNotExists()
        {
            // Act
            var exists = await _service!.ExistsByNameAsync("NonExistentName");

            // Assert
            Assert.IsFalse(exists);
        }

        #endregion

        #region GetAllAsync Tests

        [TestMethod]
        [Description("Retourne toutes les FlowGraphDescriptions")]
        public async Task GetAllAsync_ReturnsAllFlows()
        {
            // Arrange
            var flows = new[]
            {
                new FlowGraphDescription { Name = "Flow1", JsonFlowSerialized = "{}" },
                new FlowGraphDescription { Name = "Flow2", JsonFlowSerialized = "{}" },
                new FlowGraphDescription { Name = "Flow3", JsonFlowSerialized = "{}" }
            };

            foreach (var flow in flows)
            {
                await _service!.AddAsync(flow);
            }

            // Act
            var allFlows = await _service!.GetAllAsync();

            // Assert
            Assert.IsNotNull(allFlows);
            Assert.IsTrue(allFlows.Count() >= 3);
        }

        [TestMethod]
        [Description("Retourne une liste vide si aucun enregistrement")]
        public async Task GetAllAsync_ReturnsEmptyList_WhenNoFlows()
        {
            // Act
            var allFlows = await _service!.GetAllAsync();

            // Assert
            Assert.IsNotNull(allFlows);
            Assert.AreEqual(0, allFlows.Count());
        }

        #endregion

        #region AddRangeAsync Tests

        [TestMethod]
        [Description("Ajoute plusieurs FlowGraphDescriptions en même temps")]
        public async Task AddRangeAsync_AddMultipleFlows()
        {
            // Arrange
            var flows = new[]
            {
                new FlowGraphDescription { Name = "Batch1", JsonFlowSerialized = "{}" },
                new FlowGraphDescription { Name = "Batch2", JsonFlowSerialized = "{}" },
                new FlowGraphDescription { Name = "Batch3", JsonFlowSerialized = "{}" }
            };

            // Act
            await _service!.AddRangeAsync(flows);

            // Assert
            var all = await _service.GetAllAsync();
            Assert.AreEqual(3, all.Count());
        }

        #endregion

        #region Large JSON Serialized Tests

        [TestMethod]
        [Description("Gère un JSON JsonFlowSerialized très volumineux")]
        public async Task AddAsync_HandlesLargeJsonSerialized()
        {
            // Arrange
            var largeJson = new string('x', 100000); // 100KB de données
            var flow = new FlowGraphDescription 
            { 
                Name = "LargeFlow", 
                JsonFlowSerialized = largeJson 
            };

            // Act
            await _service!.AddAsync(flow);

            // Assert
            var found = await _service.FindByNameAsync("LargeFlow");
            Assert.IsNotNull(found);
            Assert.AreEqual(largeJson, found.JsonFlowSerialized);
            Assert.AreEqual(100000, found.JsonFlowSerialized.Length);
        }

        #endregion

        #region Edge Cases Tests

        [TestMethod]
        [Description("Gère les caractères spéciaux dans le nom")]
        public async Task AddAsync_HandlesSpecialCharactersInName()
        {
            // Arrange
            var specialName = "Flow-Test_123@ŝpëçiål";
            var flow = new FlowGraphDescription 
            { 
                Name = specialName, 
                JsonFlowSerialized = "{}" 
            };

            // Act
            await _service!.AddAsync(flow);

            // Assert
            var found = await _service.FindByNameAsync(specialName);
            Assert.IsNotNull(found);
            Assert.AreEqual(specialName, found.Name);
        }

        [TestMethod]
        [Description("Gère les noms avec espaces")]
        public async Task AddAsync_HandlesNamesWithSpaces()
        {
            // Arrange
            var name = "Flow With Multiple Spaces";
            var flow = new FlowGraphDescription 
            { 
                Name = name, 
                JsonFlowSerialized = "{}" 
            };

            // Act
            await _service!.AddAsync(flow);

            // Assert
            var found = await _service.FindByNameAsync(name);
            Assert.IsNotNull(found);
            Assert.AreEqual(name, found.Name);
        }

        [TestMethod]
        [Description("Gère les noms avec longueur maximale")]
        public async Task AddAsync_HandlesMaxLengthName()
        {
            // Arrange
            var maxLengthName = new string('a', 255);
            var flow = new FlowGraphDescription 
            { 
                Name = maxLengthName, 
                JsonFlowSerialized = "{}" 
            };

            // Act
            await _service!.AddAsync(flow);

            // Assert
            var found = await _service.FindByNameAsync(maxLengthName);
            Assert.IsNotNull(found);
            Assert.AreEqual(255, found.Name.Length);
        }

        #endregion
    }
}

