using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MongoDB.Driver;
using MongoDbCore.Data;
using MongoDbCore.Models;

namespace MongoDbCore.Controller
{
    public class BaseController<T> where T : Entity
    {
        public IQueryable<T> QueryCollection()
        {
            using var context = new Context<T>();
            return context.Collection.AsQueryable();
        }

        public void Insert(T entity)
        {
            using var context = new Context<T>();
            context.Collection.InsertOne(entity);
        }

        public void InsertAll(IEnumerable<T> listEntities)
        {
            using var context = new Context<T>();
            context.Collection.InsertMany(listEntities);
        }

        public void Remove(T entity)
        {
            using var context = new Context<T>();
            context.Collection.DeleteOne(v => v.Id == entity.Id);
        }

        public void RemoveAll(Expression<Func<T, bool>> predicate)
        {
            using var context = new Context<T>();
            context.Collection.DeleteMany(predicate);
        }


        public void Update(T entity, string propertyName, object newValue)
        {
            using var context = new Context<T>();
            var update = Builders<T>.Update.Set(propertyName, newValue);
            context.Collection.UpdateOne(v => v.Id == entity.Id, update);
        }


        public void ReplaceOne(T entity)
        {
            using var context = new Context<T>();
            context.Collection.ReplaceOne(v => v.Id == entity.Id, entity);
        }

        /// <summary>
        /// recherche les Entity dont l'Id est égale à
        /// la foreign key présent dans l'entity en paramètre. .
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private Dictionary<Type, List<Entity>> GetLinkedList(T entity)
        {
            var result = new Dictionary<Type, List<Entity>>();

            var assembly = Assembly.GetAssembly(typeof(T));
            if (assembly == null) return null;
            foreach (var type in assembly.GetTypes().Where(v => v.BaseType == typeof(Entity)))
            {
                var properties = type.GetProperties();
                foreach (var propertyInfo in properties)
                {
                    if (propertyInfo.PropertyType != typeof(IEnumerable<T>)) continue;
                    var genericTypeController = typeof(BaseController<>).MakeGenericType(type);
                    var genericController = Activator.CreateInstance(genericTypeController);
                    var methodInfo = genericTypeController.GetMethod(nameof(QueryCollection));
                    var objectsParents = (IQueryable<Entity>)methodInfo?.Invoke(genericController, new object[] { });

                    if (objectsParents == null) continue;
                    foreach (var parent in objectsParents)
                    {
                        foreach (var property in entity.GetType().GetProperties())
                        {
                            if (property.PropertyType != typeof(string)) continue;
                            var val = (string)property.GetValue(entity);
                            if (val == parent.Id)
                            {
                                if (!result.ContainsKey(type))
                                {
                                    result[type] = new List<Entity>();
                                }

                                result[type].Add(parent);
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Met à jour les entités qui ont une propriété de type IEnumerable
        /// qui contient l'entité en parametre. 
        /// </summary>
        /// <param name="entity"></param>
        public void UpdateLinkedLists(T entity)
        {
            var toUpdate = GetLinkedList(entity);
            foreach (var (type, listEntity) in toUpdate)
            {
                foreach (var entity1 in listEntity)
                {
                    var property = type.GetProperties().First(v => v.PropertyType == typeof(IEnumerable<T>));

                    var listToUpdate = (List<T>)property.GetValue(entity1) ?? new List<T>();

                    if (!listToUpdate.Select(v => v.Id).Contains(entity.Id))
                    {
                        listToUpdate.Add(entity);
                    }
                    else
                    {
                        listToUpdate[listToUpdate.FindIndex(v => v.Id == entity.Id)] = entity;
                    }
                    var genericTypeController = typeof(BaseController<>).MakeGenericType(type);
                    var genericController = Activator.CreateInstance(genericTypeController);
                    var methodInfo = genericTypeController.GetMethod(nameof(Update));
                    methodInfo?.Invoke(genericController, new object[] { entity1, property.Name, listToUpdate });

                    if (GetLinkedList(entity) == null) continue;
                    methodInfo = genericTypeController.GetMethod(nameof(UpdateLinkedLists));
                    methodInfo?.Invoke(genericController, new object[] { entity1 });
                }
            }
        }

        public void DeleteLinkedList(T entity)
        {
            var toUpdate = GetLinkedList(entity);
            foreach (var (type, listEntity) in toUpdate)
            {
                foreach (var entity1 in listEntity)
                {
                    var property = type.GetProperties().First(v => v.PropertyType == typeof(IEnumerable<T>));

                    var listToUpdate = (List<T>)property.GetValue(entity1) ?? new List<T>();

                    if (!listToUpdate.Select(v => v.Id).Contains(entity.Id))
                    {
                        return;
                    }

                    listToUpdate.Remove(listToUpdate.First(v => v.Id == entity.Id));
                    var genericTypeController = typeof(BaseController<>).MakeGenericType(type);
                    var genericController = Activator.CreateInstance(genericTypeController);
                    var methodInfo = genericTypeController.GetMethod(nameof(Update));
                    methodInfo?.Invoke(genericController, new object[] { entity1, property.Name, listToUpdate });

                    if (GetLinkedList(entity) == null) continue;
                    methodInfo = genericTypeController.GetMethod(nameof(UpdateLinkedLists));
                    methodInfo?.Invoke(genericController, new object[] { entity1 });
                }
            }
        }
    }
}
