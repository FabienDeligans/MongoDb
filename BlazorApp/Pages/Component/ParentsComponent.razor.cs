using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Pages.ParentPages;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDbCore.Controller;
using MongoDbCore.Models;
using MongoDbCore.Pagination;

namespace BlazorApp.Pages.Component
{
    public partial class ParentsComponent
    {
        [Parameter]
        public string FamilyId { get; set; }

        public List<Parent> Parents { get; set; }

        public PaginatedList<Parent> PaginatedParent { get; set; }

        public int Index { get; set; }
        public int PageSize { get; set; } = 10;
        public int NumberOfPage { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            var parentController = new BaseController<Parent>();

            Parents = FamilyId != null ? parentController.QueryCollection().Where(v => v.FamilyId == FamilyId).ToList() : parentController.QueryCollection().ToList();
            NumberOfPage = (int)Math.Ceiling(Parents.Count / (double)PageSize); ;
            Index = 1; 
            PaginatedParent = PaginatedList<Parent>.Create(Parents, Index, PageSize);
        }

        public async Task Detail(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailParent.ParentId), id);

            Modal.Show<DetailParent>("Detail of  this Parent", parameters);
        }

        public async Task Edit(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailParent.ParentId), id);

            var modalForm = Modal.Show<EditParent>("Edit this Parent", parameters);
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }
        public async Task Create()
        {
            var modalForm = Modal.Show<EditParent>("Insert new Parent");
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }
        public async Task Delete(string id)
        {
            var parameters = new ModalParameters();
            parameters.Add(nameof(DetailParent.ParentId), id);

            var modalForm = Modal.Show<DeleteParent>("Remove this Parent", parameters);
            var result = await modalForm.Result;
            
            if (!result.Cancelled)
            {
                OnInitialized();
            }
        }

        public void Pagination(int nb)
        {
            Index = nb;
            PaginatedParent = PaginatedList<Parent>.Create(Parents, Index, PageSize);
            StateHasChanged();
        }
    }
}
