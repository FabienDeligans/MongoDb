using System.Collections.Generic;
using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.ChildPages
{
    public partial class EditChild
    {
        [Parameter]
        public string ChildId { get; set; }

        public Child Child { get; set; }
        public List<Family> Families { get; set; }

        public bool Edit { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var childController = new BaseController<Child>();
            var familyController = new BaseController<Family>();
            Families = familyController.QueryCollection().ToList();

            if (ChildId != null)
            {
                Child = childController.QueryCollection().First(v => v.Id == ChildId);
                Edit = true;
            }
            else
            {
                Child = new Child();
            }
        }

        public void Save()
        {
            var childController = new BaseController<Child>();

            if (Child.FamilyId == null) return;
            if (Edit)
            {
                childController.ReplaceOne(Child);
            }
            else
            {
                childController.Insert(Child);
            }
                
            BlazoredModal.Close();
        }

        private void Cancel()
        {
            BlazoredModal.Cancel();
        }
    }
}
