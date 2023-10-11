﻿using Amazon.Runtime.Internal.Transform;
using despesas_backend_api_net_core.Infrastructure.Data.Repositories.Generic;
using System.Collections;
using System.Reflection;

namespace Test.XUnit.Infrastructure.Data.Repositories.Generic
{
    public class GenericRepositorioTest
    {
        private Dictionary<Type, object> entityInstances;
        private static Dictionary<Type, object> entityListInstances;
        private RegisterContext _context;
        private object GetEntityInstance(Type entityType)
        {
            if (entityInstances.ContainsKey(entityType))
            {
                return entityInstances[entityType];
            }
            else
            {
                throw new InvalidOperationException($"Tipo de Lista da entidade não mapeada: {entityType}");
            }
        }

        private object GetEntityListInstance(Type entityType)
        {
            if (entityInstances.ContainsKey(entityType))
            {
                return entityListInstances[entityType];
            }
            else
            {
                throw new InvalidOperationException($"Tipo de entidade não mapeado: {entityType}");
            }
        }

        private Mock<IRepositorio<T>> MockRepositorio<T>(List<T> _dataSet) where T : BaseModel
        {
            var _mock = new Mock<IRepositorio<T>>();
            _mock.Setup(repo => repo.Get(It.IsAny<int>())).Returns((int id) => { return _dataSet.SingleOrDefault(item => item.Id == id); });
            _mock.Setup(repo => repo.GetAll()).Returns(() => _dataSet.ToList());
            _mock.Setup(repo => repo.Insert(It.IsAny<T>())).Returns((T item) => item);
            _mock.Setup(repo => repo.Update(It.IsAny<T>())).Returns((T updatedItem) =>
            {
                var existingItem = _dataSet.FirstOrDefault(item => item.Id == updatedItem.Id);
                if (existingItem != null)
                {
                    existingItem = updatedItem;

                }
                return updatedItem;
            });
            _mock.Setup(repo => repo.Delete(It.IsAny<T>())).Returns((int id) =>
            {
                var itemToRemove = _dataSet.FirstOrDefault(item => item.Id == id);
                if (itemToRemove != null)
                {
                    _dataSet.Remove(itemToRemove);
                    return true;
                }

                return false;
            });
            return _mock;
        }

        public GenericRepositorioTest()
        {
            entityInstances = new Dictionary<Type, object>
            {
                { typeof(Usuario), UsuarioFaker.Usuarios().First() },
                { typeof(Categoria), CategoriaFaker.Categorias().First() },
                { typeof(Despesa), DespesaFaker.Despesas().First() },
                { typeof(Receita), ReceitaFaker.Receitas().First() }

            };

            entityListInstances = new Dictionary<Type, object>
            { 
                { typeof(Usuario), UsuarioFaker.Usuarios() },
                { typeof(Categoria), CategoriaFaker.Categorias() },
                { typeof(Despesa), DespesaFaker.Despesas() },
                { typeof(Receita), ReceitaFaker.Receitas() }
            };

            var options = new DbContextOptionsBuilder<RegisterContext>()
            .UseInMemoryDatabase(databaseName: "Test DataBase")
            .Options;

            _context = new RegisterContext(options);
        }

        [Theory]        
        [InlineData(typeof(Usuario))]
        [InlineData(typeof(Categoria))]
        public void Insert_Should_Add_Item_To_Database(Type entityType)
        {
            // Arrange
            var repositoryType = typeof(GenericRepositorio<>).MakeGenericType(entityType);
            var repository = Activator.CreateInstance(repositoryType, _context);
            var entityInstance = GetEntityInstance(entityType);
            var insertMethod = repository.GetType().GetMethod("Insert");

            // Act
            var insertedItem = insertMethod.Invoke(repository, new object[] { entityInstance });

            // Assert
            Assert.NotNull(insertedItem);
        }

        [Theory]        
        [InlineData(typeof(Usuario))]
        [InlineData(typeof(Categoria))]

        public void Update_Should_Update_Item(Type entityType)
        {
            // Arrange
            var repositoryType = typeof(GenericRepositorio<>).MakeGenericType(entityType);
            var repository = Activator.CreateInstance(repositoryType, _context);
            var entityInstance = GetEntityInstance(entityType);
            var insertMethod = repository.GetType().GetMethod("Insert");
            var updateMethod = repository.GetType().GetMethod("Update");

            // Act
            var insertedItem = insertMethod.Invoke(repository, new object[] { entityInstance });            
            var updateItem = updateMethod.Invoke(repository, new object[] { insertedItem });

            // Assert
            Assert.NotNull(updateItem);
        }

        [Theory]        
        [InlineData(typeof(Usuario))]
        [InlineData(typeof(Categoria))]
        public void Get_Should_Return_Item_By_Id(Type entityType)
        {
            // Arrange
            var repositoryType = typeof(GenericRepositorio<>).MakeGenericType(entityType);
            var repository = Activator.CreateInstance(repositoryType, _context);
            var entityInstance = GetEntityInstance(entityType);
            var insertMethod = repository.GetType().GetMethod("Insert");
            var getMethod = repository.GetType().GetMethod("Get");
            var idProperty = entityInstance.GetType().GetProperty("Id");

            // Insert an item into the repository
            var insertedItem = insertMethod.Invoke(repository, new object[] { entityInstance });

            // Get the Id of the inserted item
            var itemId = (int)idProperty.GetValue(insertedItem);

            // Act
            var retrievedItem = getMethod.Invoke(repository, new object[] { itemId });

            // Assert
            Assert.NotNull(retrievedItem);
            Assert.Equal(itemId, (int)idProperty.GetValue(retrievedItem));
        }

        [Theory]        
        [InlineData(typeof(Despesa))]
        [InlineData(typeof(Receita))]
        public void GetAll_Should_Return_All_Items(Type entityType)
        {

            // Arrange
            var repositoryType = typeof(GenericRepositorio<>).MakeGenericType(entityType);
            var repository = Activator.CreateInstance(repositoryType, _context);
            var entityList = GetEntityListInstance(entityType);
            var entityInstance = GetEntityInstance(entityType);
            var getAllMethod = repository.GetType().GetMethod("GetAll");
            var insertMethod = repository.GetType().GetMethod("Insert");
            
            // Act
            foreach (var _entityInstance in (IEnumerable)entityList)
            {
                var insertedItem = insertMethod.Invoke(repository, new object[] { _entityInstance });
                
            }

            var items = getAllMethod.Invoke(repository, null);

            // Get the Id of the inserted item
            PropertyInfo countProperty = items.GetType().GetProperty("Count");
            int count = (int)countProperty.GetValue(items);            
            // Assert
            Assert.NotNull(items);
            Assert.Equal(count, ((IEnumerable)entityList).Cast<object>().Count());
        }

        [Theory]
        [InlineData(typeof(Usuario))]
        [InlineData(typeof(Categoria))]
        public void Delete_Should_Delete_Item(Type entityType)
        {
            // Arrange
            var repositoryType = typeof(GenericRepositorio<>).MakeGenericType(entityType);
            var repository = Activator.CreateInstance(repositoryType, _context);
            var entityList = GetEntityListInstance(entityType);
            var entityInstance = GetEntityInstance(entityType);
            var insertMethod = repository.GetType().GetMethod("Insert");
            var deleteMethod = repository.GetType().GetMethod("Delete");

            var insertedItem = insertMethod.Invoke(repository, new object[] { entityInstance });
            
            // Act
            foreach (var _entityInstance in (IEnumerable)entityList)
            {
                insertedItem = insertMethod.Invoke(repository, new object[] { _entityInstance });                
            }
            var deletedItem = deleteMethod.Invoke(repository, new object[] { insertedItem });

            // Assert
            Assert.NotNull(deletedItem);            
            Assert.True((bool)deletedItem);
        }

    }
}