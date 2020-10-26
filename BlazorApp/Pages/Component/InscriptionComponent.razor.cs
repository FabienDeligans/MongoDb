using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp.Pages.InscriptionPages;
using Blazored.Modal;
using Microsoft.AspNetCore.Components;
using MongoDB.Driver;
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

        public List<Child> Children { get; set; }

        public PaginatedList<Inscription> PaginatedInscription { get; set; }

        public int Index { get; set; }
        public int PageSize { get; set; } = 10;
        public int NumberOfPage { get; set; }

        public int MCount { get; set; }
        public int RCount { get; set; }
        public int AmCount { get; set; }

        public bool FromSearchByDate { get; set; }
        public bool FromSearchByChild { get; set; }

        public int Max { get; set; } = 30;

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            OnInitialized();
        }


        protected override void OnInitialized()
        {
            base.OnInitialized();
            var childController = new BaseController<Child>();

            if (ChildId != null)
            {
                FromSearchByChild = true;
                SearchByChild();
                InvokeAsync(StateHasChanged);
            }

            Children = childController.QueryCollection().ToList();
        }

        public void Detail(string id)
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
                if (FromSearchByDate)
                {
                    SearchByDate();
                }

                if (FromSearchByChild)
                {
                    SearchByChild();
                }
                await InvokeAsync(StateHasChanged);
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
                if (FromSearchByDate)
                {
                    SearchByDate();
                }

                if (FromSearchByChild)
                {
                    SearchByChild();
                }
                await InvokeAsync(StateHasChanged);
            }
        }

        public void Pagination(int index)
        {
            Index = index;
            NumberOfPage = (int)Math.Ceiling(Inscriptions.Count / (double)PageSize);
            PaginatedInscription = PaginatedList<Inscription>.Create(Inscriptions, Index, PageSize);

            MCount = Inscriptions.Count(v => v.M);
            RCount = Inscriptions.Count(v => v.R);
            AmCount = Inscriptions.Count(v => v.Am);
            StateHasChanged();
        }

        public async Task Create()
        {
            var modalForm = Modal.Show<EditInscription>("Insert new Inscription");
            var result = await modalForm.Result;

            if (!result.Cancelled)
            {
                if (FromSearchByDate)
                {
                    SearchByDate();
                }

                if (FromSearchByChild)
                {
                    SearchByChild();
                }
                await InvokeAsync(StateHasChanged);
            }
        }

        public void SearchByDate()
        {
            var start = DateTime.Now;

            ChildId = null;

            if (DbCall == false)
            {
                Inscriptions = new List<Inscription>();
                foreach (var insc in Children.Select(child => child.Inscriptions.FirstOrDefault(v => v.DayChoose == Date.Date)).Where(insc => insc != null))
                {
                    Inscriptions.Add(insc);
                }

                var inscriptions = Inscriptions.OrderBy(v => v.DayChoose).ToList();
                Inscriptions = inscriptions; 
            }
            else
            {
                var inscriptionController = new BaseController<Inscription>();
                Inscriptions = inscriptionController.QueryCollection().Where(v => v.DayChoose == Date.Date).OrderBy(v => v.DayChoose).ToList();
            }

            Pagination(1);
            FromSearchByDate = true;
            FromSearchByChild = false;
            InvokeAsync(StateHasChanged);

            ElapsedTime = (DateTime.Now - start).TotalMilliseconds;
        }

        public void SearchByChild()
        {
            var start = DateTime.Now;

            ChildId = ChildIdSearch ?? ChildId;
            if (ChildId == null) return;

            if (DbCall == false)
            {
                Child = Children.First(v => v.Id == ChildId);
                Inscriptions = Child.Inscriptions.OrderBy(v => v.DayChoose).ToList();
            }
            else
            {
                var childController = new BaseController<Child>();
                Child = childController.QueryCollection().First(v => v.Id == ChildId); 
                var inscriptionController = new BaseController<Inscription>();
                Inscriptions = inscriptionController.QueryCollection().Where(v => v.ChildId == ChildId).OrderBy(v => v.DayChoose).ToList();
            }

            Pagination(1);
            FromSearchByChild = true;
            FromSearchByDate = false;
            InvokeAsync(StateHasChanged);

            ElapsedTime = (DateTime.Now - start).TotalMilliseconds;
        }

        public void AppelDb()
        {
            DbCall = !DbCall;
        }

        public bool DbCall { get; set; }

        public string ChildIdSearch { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;

        public double ElapsedTime { get; set; }
    }
}
