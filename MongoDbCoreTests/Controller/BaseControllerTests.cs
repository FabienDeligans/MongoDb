using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDbCore.Controller;
using MongoDbCore.Data;
using MongoDbCore.Models;

namespace MongoDbCoreTests.Controller
{
    [TestClass()]
    public class BaseControllerTests
    {
        [TestInitialize]
        public void Initialize()
        {
            var context = new Context<Child>();
            context.DropDatabase();
        }

        [TestMethod()]
        public void InsertAndQueryCollectionTest()
        {
            var controller = new BaseController<Child>();

            var listCreate = new List<Child>();
            var nb = 20;
            for (var i = 1; i <= nb; i++)
            {
                var child = new Child
                {
                    FirstName = $"{i}-firstName",
                    LastName = $"{i}-lastName",
                    Birthday = new DateTime(i, 01, 15).ToLocalTime(),
                };
                listCreate.Add(child);

                // Insert one by one
                controller.Insert(child);
            }

            var orderedEnumerable = listCreate.OrderBy(v => v.Id).ToList();
            var listChildren = controller.QueryCollection().OrderBy(v => v.Id).ToList();
            Assert.AreEqual(nb, listChildren.Count());


            for (var i = 0; i < nb; i++)
            {
                Assert.AreEqual(orderedEnumerable[i].Id, listChildren[i].Id);
                Assert.AreEqual(orderedEnumerable[i].FirstName, listChildren[i].FirstName);
                Assert.AreEqual(orderedEnumerable[i].LastName, listChildren[i].LastName);
                Assert.AreEqual(orderedEnumerable[i].Birthday, listChildren[i].Birthday.ToLocalTime());
            }
        }

        [TestMethod()]
        public void InsertAllTest()
        {
            var controller = new BaseController<Child>();

            var listCreate = new List<Child>();
            var nb = 20;
            for (var i = 1; i <= nb; i++)
            {
                var child = new Child
                {
                    FirstName = $"{i}-firstName",
                    LastName = $"{i}-lastName",
                    Birthday = new DateTime(i, 01, 15).ToLocalTime(),
                };
                listCreate.Add(child);
            }

            // Insert all
            controller.InsertAll(listCreate);

            var orderedEnumerable = listCreate.OrderBy(v => v.Id).ToList();
            var listChildren = controller.QueryCollection().OrderBy(v => v.Id).ToList();
            Assert.AreEqual(nb, listChildren.Count());


            for (var i = 0; i < nb; i++)
            {
                Assert.AreEqual(orderedEnumerable[i].Id, listChildren[i].Id);
                Assert.AreEqual(orderedEnumerable[i].FirstName, listChildren[i].FirstName);
                Assert.AreEqual(orderedEnumerable[i].LastName, listChildren[i].LastName);
                Assert.AreEqual(orderedEnumerable[i].Birthday, listChildren[i].Birthday.ToLocalTime());
            }
        }

        [TestMethod()]
        public void RemoveTest()
        {
            var controller = new BaseController<Child>();
            var child1 = new Child();
            var child2 = new Child();
            var child3 = new Child();
            var child4 = new Child();
            controller.InsertAll(new[] { child1, child2, child3, child4 });
            Assert.AreEqual(controller.QueryCollection().Count(), 4);

            controller.Remove(child1);
            var children = controller.QueryCollection();

            Assert.AreEqual(children.Count(), 3);
            var listIds = children.Select(v => v.Id).ToList();
            Assert.AreEqual(false, listIds.Contains(child1.Id));
        }

        [TestMethod()]
        public void RemoveAllTest()
        {
            var controller = new BaseController<Child>();
            var listChild = new List<Child>
            {
                new Child(),
                new Child(),
                new Child(),
                new Child(),
            };

            controller.InsertAll(listChild);
            Assert.AreEqual(controller.QueryCollection().Count(), 4);

            controller.RemoveAll(v => v.FirstName == null);
            Assert.AreEqual(controller.QueryCollection().Count(), 0);
        }

        [TestMethod()]
        public void UpdateTest()
        {
            var controller = new BaseController<Child>();
            var child = new Child
            {
                FirstName = "aaaa",
                LastName = "bbbb",
            };
            controller.Insert(child);
            Assert.AreEqual(controller.QueryCollection().First(v => v.Id == child.Id).FirstName, child.FirstName);

            child.FirstName = "TOTO";
            controller.Update(child, nameof(Child.FirstName), child.FirstName);
            Assert.AreEqual(controller.QueryCollection().First(v => v.Id == child.Id).FirstName, "TOTO");
        }

        [TestMethod()]
        public void ReplaceOneTest()
        {
            var controller = new BaseController<Child>();
            var child = new Child
            {
                FirstName = "aaaa",
                LastName = "bbbb",
            };
            controller.Insert(child);
            Assert.AreEqual(controller.QueryCollection().First(v => v.Id == child.Id).FirstName, child.FirstName);

            child.FirstName = "TOTO";
            controller.ReplaceOne(child);
            Assert.AreEqual(controller.QueryCollection().First(v => v.Id == child.Id).FirstName, "TOTO");
        }

        [TestMethod()]
        public void UpdateLinkedListsTest()
        {
            var childController = new BaseController<Child>();
            var familyController = new BaseController<Family>();
            var inscriptionController = new BaseController<Inscription>();

            var family = new Family
            {
                FamilyName = "name",
            };
            familyController.Insert(family);

            var child = new Child
            {
                FamilyId = family.Id,
                FirstName = "firstName",
                LastName = "lastName",
            };
            childController.Insert(child);

            //permet de mettre à jour l'Entity family
            childController.UpdateLinkedLists(child);

            var familyDb = familyController.QueryCollection().First(v => v.Id == family.Id);
            var childrenOfFamilyDb = familyDb.Children.ToList();
            Assert.AreEqual(childrenOfFamilyDb.Count(), 1);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).Id, child.Id);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).FamilyId, child.FamilyId);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).FirstName, child.FirstName);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).LastName, child.LastName);

            var inscription = new Inscription
            {
                ChildId = child.Id,
                DayChoose = new DateTime(2020, 10, 10).ToLocalTime(),
                M = true,
                R = false,
                Am = true
            };
            inscriptionController.Insert(inscription);

            // permet de mettre à jour l'Entity child et family
            inscriptionController.UpdateLinkedLists(inscription);

            familyDb = familyController.QueryCollection().First(v => v.Id == family.Id);
            var childFromFamilyDB = familyDb.Children.First(v => v.Id == child.Id);
            var inscriptionFromChild = childFromFamilyDB.Inscriptions.First(v => v.ChildId == childFromFamilyDB.Id);

            Assert.AreEqual(family.Id, familyDb.Id);
            Assert.AreEqual(family.FamilyName, familyDb.FamilyName);

            Assert.AreEqual(child.Id, childFromFamilyDB.Id);
            Assert.AreEqual(child.FirstName, childFromFamilyDB.FirstName);
            Assert.AreEqual(child.LastName, childFromFamilyDB.LastName);
            Assert.AreEqual(child.Birthday, childFromFamilyDB.Birthday);
            Assert.AreEqual(child.FamilyId, childFromFamilyDB.FamilyId);

            Assert.AreEqual(inscription.Id, inscriptionFromChild.Id);
            Assert.AreEqual(inscription.DayChoose, inscriptionFromChild.DayChoose);
            Assert.AreEqual(inscription.M, inscriptionFromChild.M);
            Assert.AreEqual(inscription.R, inscriptionFromChild.R);
            Assert.AreEqual(inscription.Am, inscriptionFromChild.Am);
            Assert.AreEqual(inscription.ChildId, inscriptionFromChild.ChildId);

            
            // UPDATE

            inscriptionFromChild.M = false;
            inscriptionController.ReplaceOne(inscriptionFromChild);
            inscriptionController.UpdateLinkedLists(inscriptionFromChild);

            familyDb = familyController.QueryCollection().First(v => v.Id == family.Id);
            childFromFamilyDB = familyDb.Children.First(v => v.Id == child.Id);
            inscriptionFromChild = childFromFamilyDB.Inscriptions.First(v => v.ChildId == childFromFamilyDB.Id);

            Assert.AreEqual(family.Id, familyDb.Id);
            Assert.AreEqual(family.FamilyName, familyDb.FamilyName);

            Assert.AreEqual(child.Id, childFromFamilyDB.Id);
            Assert.AreEqual(child.FirstName, childFromFamilyDB.FirstName);
            Assert.AreEqual(child.LastName, childFromFamilyDB.LastName);
            Assert.AreEqual(child.Birthday, childFromFamilyDB.Birthday);
            Assert.AreEqual(child.FamilyId, childFromFamilyDB.FamilyId);

            Assert.AreEqual(inscription.Id, inscriptionFromChild.Id);
            Assert.AreEqual(inscription.DayChoose, inscriptionFromChild.DayChoose);
            Assert.AreEqual(false, inscriptionFromChild.M);
            Assert.AreEqual(inscription.R, inscriptionFromChild.R);
            Assert.AreEqual(inscription.Am, inscriptionFromChild.Am);
            Assert.AreEqual(inscription.ChildId, inscriptionFromChild.ChildId);

        }

        [TestMethod()]
        public void DeleteLinkedListTest()
        {
            var childController = new BaseController<Child>();
            var familyController = new BaseController<Family>();
            var inscriptionController = new BaseController<Inscription>();

            var family = new Family
            {
                FamilyName = "name",
            };
            familyController.Insert(family);

            var child = new Child
            {
                FamilyId = family.Id,
                FirstName = "firstName",
                LastName = "lastName",
            };
            childController.Insert(child);

            //permet de mettre à jour l'Entity family
            childController.UpdateLinkedLists(child);

            var familyDb = familyController.QueryCollection().First(v => v.Id == family.Id);
            var childrenOfFamilyDb = familyDb.Children.ToList();
            Assert.AreEqual(childrenOfFamilyDb.Count(), 1);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).Id, child.Id);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).FamilyId, child.FamilyId);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).FirstName, child.FirstName);
            Assert.AreEqual(childrenOfFamilyDb.First(v => v.Id == child.Id).LastName, child.LastName);

            var inscription = new Inscription
            {
                ChildId = child.Id,
                DayChoose = new DateTime(2020, 10, 10).ToLocalTime(),
                M = true,
                R = false,
                Am = true
            };
            inscriptionController.Insert(inscription);

            // permet de mettre à jour l'Entity child et family
            inscriptionController.UpdateLinkedLists(inscription);

            familyDb = familyController.QueryCollection().First(v => v.Id == family.Id);
            var childFromFamilyDB = familyDb.Children.First(v => v.Id == child.Id);
            var inscriptionFromChild = childFromFamilyDB.Inscriptions.First(v => v.ChildId == childFromFamilyDB.Id);

            Assert.AreEqual(family.Id, familyDb.Id);
            Assert.AreEqual(family.FamilyName, familyDb.FamilyName);

            Assert.AreEqual(child.Id, childFromFamilyDB.Id);
            Assert.AreEqual(child.FirstName, childFromFamilyDB.FirstName);
            Assert.AreEqual(child.LastName, childFromFamilyDB.LastName);
            Assert.AreEqual(child.Birthday, childFromFamilyDB.Birthday);
            Assert.AreEqual(child.FamilyId, childFromFamilyDB.FamilyId);

            Assert.AreEqual(inscription.Id, inscriptionFromChild.Id);
            Assert.AreEqual(inscription.DayChoose, inscriptionFromChild.DayChoose);
            Assert.AreEqual(inscription.M, inscriptionFromChild.M);
            Assert.AreEqual(inscription.R, inscriptionFromChild.R);
            Assert.AreEqual(inscription.Am, inscriptionFromChild.Am);
            Assert.AreEqual(inscription.ChildId, inscriptionFromChild.ChildId);

            inscriptionController.DeleteLinkedList(inscriptionFromChild);
            inscriptionController.Remove(inscriptionFromChild);

            familyDb = familyController.QueryCollection().First(v => v.Id == family.Id);
            childFromFamilyDB = familyDb.Children.First(v => v.Id == child.Id);

            Assert.AreEqual(null, familyDb.Children.First().Inscriptions);
            Assert.AreEqual(null, childFromFamilyDB.Inscriptions);
        }
    }
}