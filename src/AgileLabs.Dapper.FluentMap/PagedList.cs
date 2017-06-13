using System.Collections.Generic;

namespace AgileLabs.Dapper.FluentMap
{
    public class PagedList<T> : List<T>
    {
        public PagedList()
        {

        }

        public PagedList(IEnumerable<T> entityList, int pageNo, int pageSize, int totalCount)
        {
            this.AddRange(entityList);
            this.PageNo = pageNo;
            this.PageSize = pageSize;
            this.Total = totalCount;
        }

        public int Total { get; set; }

        public int PageSize { get; set; }

        public int PageNo { get; set; }
    }
}
