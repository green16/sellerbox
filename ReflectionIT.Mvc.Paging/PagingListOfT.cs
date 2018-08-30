using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;

namespace ReflectionIT.Mvc.Paging {

    public class PagingList<T> : List<T>, IPagingList<T> where T : class {

        public int PageIndex { get; }
        public int PageCount { get; }
        public int PageSize { get; set; }
        public int TotalRecordCount { get; }
        public string Action { get; set; }
        public string Controller { get; set; }
        public string PageParameterName { get; set; }
        public string SortExpressionParameterName { get; set; }
        public string SortExpression { get; }

        public string DefaultSortExpression { get; }

        [Obsolete("Use PagingList.CreateAsync<T>() instead")]
        public static Task<PagingList<T>> CreateAsync(IOrderedQueryable<T> qry, string action, string controller, int pageSize, int pageIndex) {
            return PagingList.CreateAsync(qry, action, controller, pageSize, pageIndex);
        }

        [Obsolete("Use PagingList.CreateAsync<T>() instead")]
        public static Task<PagingList<T>> CreateAsync(IQueryable<T> qry, string action, string controller, int pageSize, int pageIndex, string sortExpression, string defaultSortExpression) {
            return PagingList.CreateAsync(qry, action, controller, pageSize, pageIndex, sortExpression, defaultSortExpression);
        }

        internal PagingList(List<T> list, string action, string controller, int pageSize, int pageIndex, int pageCount, int totalRecordCount)
            : base(list) {
            TotalRecordCount = totalRecordCount;
            PageIndex = pageIndex;
            PageCount = pageCount;
            PageSize = pageSize;
            Action = action;
            Controller = controller;
            PageParameterName = "page";
            SortExpressionParameterName = "sortExpression";
        }

        internal PagingList(List<T> list, string action, string controller, int pageSize, int pageIndex, int pageCount, string sortExpression, string defaultSortExpression, int totalRecordCount)
            : this(list, action, controller, pageSize, pageIndex, pageCount, totalRecordCount) {

            SortExpression = sortExpression;
            DefaultSortExpression = defaultSortExpression;
        }

        public RouteValueDictionary RouteValue { get; set; }

        public RouteValueDictionary GetRouteValueForPage(int pageIndex) {

            var dict = RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(RouteValue);

            dict[PageParameterName] = pageIndex;

            if (SortExpression != DefaultSortExpression) {
                dict[SortExpressionParameterName] = SortExpression;
            }

            return dict;
        }

        public RouteValueDictionary GetRouteValueForSort(string sortExpression) {

            var dict = RouteValue == null ? new RouteValueDictionary() :
                                                 new RouteValueDictionary(RouteValue);

            if (sortExpression == SortExpression) {
                sortExpression = "-" + sortExpression;
            }

            dict[SortExpressionParameterName] = sortExpression;

            return dict;
        }

        public int NumberOfPagesToShow { get; set; } = PagingOptions.Current.DefaultNumberOfPagesToShow;

        public int StartPageIndex {
            get {
                var half = (int)((NumberOfPagesToShow - 0.5) / 2);
                var start = Math.Max(1, PageIndex - half);
                if (start + NumberOfPagesToShow - 1 > PageCount) {
                    start = PageCount - NumberOfPagesToShow + 1;
                }
                return Math.Max(1, start);
            }
        }

        public int StopPageIndex => Math.Min(PageCount, StartPageIndex + NumberOfPagesToShow - 1);

    }
}