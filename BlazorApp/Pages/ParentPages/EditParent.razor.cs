using System.Collections.Generic;
using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.ParentPages
{
    public partial class EditParent
    {
        [Parameter]
        public string ParentId { get; set; }

        public Parent Parent { get; set; }

        public List<Family> Families { get; set; }

        public bool Edit { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var parentController = new BaseController<Parent>();

            if (ParentId != null)
            {
                Parent = parentController.QueryCollection().First(v => v.Id == ParentId);
                Edit = true; 
            }
            else
            {
                Parent = new Parent();
            }

            var familyController = new BaseController<Family>();
            Families = familyController.QueryCollection().ToList(); 
        }
        public void Save()
        {
            var parentController = new BaseController<Parent>();
            if (Parent.FamilyId == null) return;
            if (Edit )
            {
                parentController.ReplaceOne(Parent);
            }
            else
            {
                parentController.Insert(Parent);
            }

            BlazoredModal.Close();
        }
        private void Cancel()
        {
            BlazoredModal.Cancel();
        }

    }
}
   