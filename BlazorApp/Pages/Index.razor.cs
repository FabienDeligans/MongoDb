using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDbCore.Controller;
using MongoDbCore.Data;
using MongoDbCore.Models;

namespace BlazorApp.Pages
{
    public partial class Index
    {
        public List<Family> Families { get; set; }
        public int NumberFamilyToCreate { get; set; }
        public int NumberFamilyInDatabase { get; set; }

        public List<Child> Children { get; set; }
        public int NumberChildToCreateByFamily { get; set; }
        public int NumberChildrenInDatabase { get; set; }

        public List<Parent> Parents { get; set; }
        public int NumberParentToCreateByFamily { get; set; }
        public int NumberParentInDatabase { get; set; }

        public List<Inscription> Inscriptions { get; set; }
        public int NumberInscriptionToCreateByChild { get; set; }
        public int NumberInscriptionInDatabase { get; set; }

        public int CountInscriptions { get; set; }

        public bool Loading { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var inscriptionController = new BaseController<Inscription>();
            CountInscriptions = inscriptionController.QueryCollection().Count();
        }

        public void Drop()
        {
            var context = new Context<Family>();
            context.DropDatabase();
            OnInitialized();
            InvokeAsync(StateHasChanged);
        }

        public void CreateAll()
        {
            CreateFamilies();
            CreateChildren();
            CreateParents();
            CreateInscription();

            var inscriptionController = new BaseController<Inscription>();
            CountInscriptions = inscriptionController.QueryCollection().Count();

        }
        
        public void CreateFamilies()
        {
            var controller = new BaseController<Family>();
            var listFamilies = new List<Family>();

            for (var i = 0; i < NumberFamilyToCreate; i++)
            {
                var family = new Family
                {
                    FamilyName = $"{i}-FamilyName",
                };
                listFamilies.Add(family);
            }
            controller.InsertAll(listFamilies);

            Families = controller.QueryCollection().ToList();
            NumberFamilyInDatabase = Families.Count;
            InvokeAsync(StateHasChanged);

        }

        public void CreateChildren()
        {
            var childController = new BaseController<Child>();
            var listChildren = new List<Child>();

            foreach (var family in Families)
            {
                for (var i = 0; i < NumberChildToCreateByFamily; i++)
                {
                    var child = new Child
                    {
                        FamilyId = family.Id,
                        FirstName = $"{i} - FirstName",
                        LastName = family.FamilyName,
                        Birthday = new DateTime(2010 + i, 1, 1).ToLocalTime()
                    };
                    listChildren.Add(child);
                }

            }

            childController.InsertAll(listChildren);
            Children = childController.QueryCollection().ToList();
            NumberChildrenInDatabase = Children.Count;
            InvokeAsync(StateHasChanged);
        }

        public void CreateParents()
        {
            var parentController = new BaseController<Parent>();
            var listParents = new List<Parent>();

            foreach (var family in Families)
            {
                for (var i = 0; i < NumberParentToCreateByFamily; i++)
                {
                    var parent = new Parent()
                    {
                        FamilyId = family.Id,
                        FirstName = $"{i} - FirstName",
                        LastName = family.FamilyName,
                        Adress = $"{family.Id} - Adress",
                        Cp = "69000",
                        City = "Lyon"
                    };
                    listParents.Add(parent);
                }

            }

            parentController.InsertAll(listParents);
            Parents = parentController.QueryCollection().ToList();
            NumberParentInDatabase = Parents.Count;
            InvokeAsync(StateHasChanged);
        }

        public async void CreateInscription()
        {
            var inscriptionController = new BaseController<Inscription>();

            foreach (var child in Children)
            {
                var listInscription = new List<Inscription>();
                for (var i = 0; i < NumberInscriptionToCreateByChild; i++)
                {
                    var inscription = new Inscription
                    {
                        ChildId = child.Id,
                        DayChoose = DateTime.Now.Date + TimeSpan.FromDays(i),
                        M = true,
                        Am = true,
                        R = true
                    };
                    listInscription.Add(inscription);
                }
                inscriptionController.InsertAll(listInscription);
            }

            Inscriptions = inscriptionController.QueryCollection().ToList();
            NumberInscriptionInDatabase = Inscriptions.Count;
            await InvokeAsync(StateHasChanged);

            using var context = new Context<Inscription>();
            var indexKeysDefinition = Builders<Inscription>.IndexKeys.Ascending(v => v.ChildId);
            var indexKeysDefinition2 = Builders<Inscription>.IndexKeys.Ascending(v => v.DayChoose);
            await context.Collection.Indexes.CreateOneAsync(new CreateIndexModel<Inscription>(indexKeysDefinition));
            await context.Collection.Indexes.CreateOneAsync(new CreateIndexModel<Inscription>(indexKeysDefinition2));
        }
    }
}
