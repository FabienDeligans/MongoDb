using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Pages.InscriptionPages;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;

namespace BlazorApp.Pages.Component
{
    public partial class ListInscription
    {
        [Parameter]
        public string ChildId { get; set; }

        public Child Child { get; set; }

        public List<Inscription> Inscriptions { get; set; }
        public List<Child> Children { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var inscriptionController = new BaseController<Inscription>();
            var childController = new BaseController<Child>();

            if (ChildId != null)
            {
                Child = childController.QueryCollection().First(v => v.Id == ChildId);
                Inscriptions = inscriptionController.QueryCollection().Where(v => v.ChildId == ChildId).ToList();
                InvokeAsync(StateHasChanged);
            }
            
            Children = childController.QueryCollection().ToList();
        }

        public async Task Create()
        {
            var modalForm = Modal.Show<EditInscription>("Insert new Inscription");
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
                await InvokeAsync(StateHasChanged);

            }
        }

        public void SearchByDate()
        {
            var inscriptionController = new BaseController<Inscription>();
            Inscriptions = inscriptionController.QueryCollection().Where(v => v.DayChoose == Date.Date).ToList();
            ChildId = null;
            InvokeAsync(StateHasChanged);
        }

        public void SearchByChild()
        {
            var inscriptionController = new BaseController<Inscription>();
            ChildId = ChildIdSearch; 
            Inscriptions = inscriptionController.QueryCollection().Where(v => v.ChildId == ChildId).ToList();
            InvokeAsync(StateHasChanged);
            OnInitialized();
        }

        public string ChildIdSearch { get; set; }
        public DateTime Date { get; set; }
    }
}
