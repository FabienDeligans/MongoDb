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
        public List<Inscription> Inscriptions { get; set; }

        public List<Child> Children { get; set; }

        public PaginatedList<Inscription> PaginatedInscription { get; set; }

        public int Index { get; set; }
        public int PageSize { get; set; } = 10;
        public int NumberOfPage { get; set; }

        public int MCount { get; set; }
        public int RCount { get; set; }
        public int AmCount { get; set; }


        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            OnInitialized();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var childController = new BaseController<Child>();

            NumberOfPage = (int)Math.Ceiling(Inscriptions.Count / (double)PageSize); ;
            Index = 1;
            PaginatedInscription = PaginatedList<Inscription>.Create(Inscriptions, Index, PageSize);

            MCount = Inscriptions.Count(v => v.M == true);
            RCount = Inscriptions.Count(v => v.R == true);
            AmCount = Inscriptions.Count(v => v.Am == true);

            Children = childController.QueryCollection().ToList();
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
