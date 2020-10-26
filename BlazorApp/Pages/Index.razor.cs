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


        private BaseController<Family> familyController { get; set; } = new BaseController<Family>();
        private BaseController<Child> childController { get; set; } = new BaseController<Child>();
        private BaseController<Parent> parentController { get; set; } = new BaseController<Parent>();
        private BaseController<Inscription> inscriptionController { get; set; } = new BaseController<Inscription>();

        public double ElapsedUpdateLinkedList { get; set; }

        public bool Loading { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Families = familyController.QueryCollection().ToList();
            NumberFamilyInDatabase = Families.Count;

            Children = childController.QueryCollection().ToList();
            NumberChildrenInDatabase = Children.Count;

            Parents = parentController.QueryCollection().ToList();
            NumberParentInDatabase = Parents.Count;

            Inscriptions = inscriptionController.QueryCollection().ToList();
            NumberInscriptionInDatabase = Inscriptions.Count;
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
            Indexation();

            var start = DateTime.Now;

            //UpLinkedList();
            //UpLinkedListManualy();
            
            ElapsedUpdateLinkedList = (DateTime.Now - start).TotalMilliseconds;

        }

        public void CreateFamilies()
        {
            var listFamilies = new List<Family>();

            for (var i = 0; i < NumberFamilyToCreate; i++)
            {
                var family = new Family
                {
                    FamilyName = $"{i}-FamilyName",
                };
                listFamilies.Add(family);
            }
            familyController.InsertAll(listFamilies);
            
            Families = familyController.QueryCollection().ToList();
            NumberFamilyInDatabase = Families.Count;
            InvokeAsync(StateHasChanged);

        }

        public void CreateChildren()
        {
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

        public void CreateInscription()
        {
            var trueORFalse = new[] { true, false };
            var rand = new Random();

            foreach (var child in Children)
            {
                var listInscription = new List<Inscription>();
                for (var i = 0; i < NumberInscriptionToCreateByChild; i++)
                {
                    var doInscription = trueORFalse[rand.Next(0, 2)];
                    if (!doInscription) continue;
                    var inscription = new Inscription
                    {
                        ChildId = child.Id,
                        DayChoose = DateTime.Now.Date + TimeSpan.FromDays(i),
                        M = trueORFalse[rand.Next(0, 2)],
                        Am = trueORFalse[rand.Next(0, 2)],
                        R = trueORFalse[rand.Next(0, 2)]
                    };
                    while (inscription.M == false && inscription.R == false && inscription.Am == false)
                    {
                        inscription.M = trueORFalse[rand.Next(0, 2)];
                        inscription.R = trueORFalse[rand.Next(0, 2)];
                        inscription.Am = trueORFalse[rand.Next(0, 2)];
                    }
                    listInscription.Add(inscription);
                }

                if (listInscription.Count != 0)
                {
                    inscriptionController.InsertAll(listInscription);
                }
            }

            Inscriptions = inscriptionController.QueryCollection().ToList();
            NumberInscriptionInDatabase = Inscriptions.Count;
            InvokeAsync(StateHasChanged);

        }

        public async void Indexation()
        {
            using var contextInscription = new Context<Inscription>();
            var indexKeysDefinition = Builders<Inscription>.IndexKeys.Ascending(v => v.ChildId);
            var indexKeysDefinition2 = Builders<Inscription>.IndexKeys.Ascending(v => v.DayChoose);
            await contextInscription.Collection.Indexes.CreateOneAsync(new CreateIndexModel<Inscription>(indexKeysDefinition));
            await contextInscription.Collection.Indexes.CreateOneAsync(new CreateIndexModel<Inscription>(indexKeysDefinition2));

            using var contextParent = new Context<Parent>();
            var indexKeyDefinition3 = Builders<Parent>.IndexKeys.Ascending(v => v.FamilyId);
            await contextParent.Collection.Indexes.CreateOneAsync(new CreateIndexModel<Parent>(indexKeyDefinition3));

            using var contextChild = new Context<Child>();
            var indexKeyDefinition4 = Builders<Child>.IndexKeys.Ascending(v => v.FamilyId);
            await contextChild.Collection.Indexes.CreateOneAsync(new CreateIndexModel<Child>(indexKeyDefinition4));
        }

        public void UpLinkedList()
        {
            foreach (var inscription in Inscriptions)
            {
                inscriptionController.UpdateLinkedLists(inscription);
            }

            foreach (var parent in Parents)
            {
                parentController.UpdateLinkedLists(parent);
            }
        }

        public void UpLinkedListManualy()
        {
            Children = childController.QueryCollection().ToList();

            foreach (var child in Children)
            {
                var insciptions = Inscriptions.Where(v => v.ChildId == child.Id); 
                childController.Update(child, nameof(Child.Inscriptions), insciptions);
            }

            Children = childController.QueryCollection().ToList();
            Parents = parentController.QueryCollection().ToList();

            foreach (var family in Families)
            {
                var children = Children.Where(v => v.FamilyId == family.Id);
                family.Children = children; 
                familyController.Update(family, nameof(Family.Children), children);

                var parents = Parents.Where(v => v.FamilyId == family.Id);
                family.Parents = parents;
                familyController.Update(family, nameof(Family.Parents), parents);
            }
        }
    }
}
