using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Pages.InscriptionPages;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;
using MongoDbCore.Pagination;

namespace BlazorApp.Pages.Component
{
    public partial class InscriptionComponent
    {
        [Parameter]
        public string ChildId { get; set; }

        public Child Child { get; set; }

        public List<Inscription> Inscriptions { get; set; }

        public PaginatedList<Inscription> PaginatedInscription { get; set; }

        public int Index { get; set; }
        public int PageSize { get; set; } = 10;
        public int NumberOfPage { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var inscriptionController = new BaseController<Inscription>();
            var childController = new BaseController<Child>();

            if (ChildId != null)
            {
                Child = childController.QueryCollection().First(v => v.Id == ChildId);
                Inscriptions = inscriptionController.QueryCollection().Where(v => v.ChildId == ChildId).ToList();
            }
            else
            {
                Inscriptions = inscriptionController.QueryCollection().ToList();
            }

            NumberOfPage = (int)Math.Ceiling(Inscriptions.Count / (double)PageSize); ;
            Index = 1;
            PaginatedInscription = PaginatedList<Inscription>.Create(Inscriptions, Index, PageSize);
        }

        public async Task Detail(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailInscription.InscriptionId), id);

            Modal.Show<DetailInscription>("Detail of this Inscription", parameters);
        }

        public async Task Edit(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(EditInscription.InscriptionId), id);

            var modalForm = Modal.Show<EditInscription>("Edit this Inscription", parameters);
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public async Task Create()
        {
            var modalForm = Modal.Show<EditInscription>("Insert new Inscription");
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public async Task Delete(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DeleteInscription.InscriptionId), id);

            var modalForm = Modal.Show<DeleteInscription>("Remove this Inscription", parameters);
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public void Pagination(int nb)
        {
            Index = nb;
            PaginatedInscription = PaginatedList<Inscription>.Create(Inscriptions, Index, PageSize);
            StateHasChanged();
        }
    }
}
