using System.Collections.Generic;
using System.Linq;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.InscriptionPages
{
    public partial class EditInscription
    {
        [Parameter]
        public string InscriptionId { get; set; }

        public Inscription Inscription { get; set; }

        public List<Child> Children { get; set; }

        [CascadingParameter]
        public BlazoredModalInstance BlazoredModal { get; set; }

        public bool Edit { get; set; }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            var inscriptionController = new BaseController<Inscription>();
            var childController = new BaseController<Child>();
            Children = childController.QueryCollection().ToList(); 

            if (InscriptionId != null)
            {
                Inscription = inscriptionController.QueryCollection().First(v => v.Id == InscriptionId);
                Edit = true;
            }
            else
            {
                Inscription = new Inscription();
            }
        }

        public void Save()
        {
            var inscriptionController = new BaseController<Inscription>();

            if (Inscription.ChildId == null)
            {
                StateHasChanged();
            }
            else
            {
                Inscription.DayChoose = Inscription.DayChoose.Date.ToUniversalTime();
                if (Edit)
                {
                    inscriptionController.ReplaceOne(Inscription);
                }
                else
                {
                    inscriptionController.Insert(Inscription);
                }

                BlazoredModal.Close();
            }
        }

        private void Cancel()
        {
            BlazoredModal.Cancel();
        }
    }
}