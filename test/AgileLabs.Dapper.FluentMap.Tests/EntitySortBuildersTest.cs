using System;
using Xunit;
using AgileLabs.Dapper.FluentMap;

namespace AgileLabs.Dapper.FluentMap.Tests
{
    public class EntitySortBuildersTest
    {
        private class SimpleEntity
        {
            public int Id { get; set; }
            public DateTime CreationTime { get; set; }
        }

        [Fact]
        public void single_sort_test()
        {
            // Arrange

            // Act
            var sortByCreationTimeAsc = EntityBuilder<SimpleEntity>.Sort.Ascending(x => x.CreationTime);
            var sql = sortByCreationTimeAsc.ToSql();

            // Assert
            var expectedOrderSql = " ORDER BY CreationTime ASC";// "order by CreationTime ASC";
            Assert.Equal(expectedOrderSql, sql);
        }

        [Fact]
        public void mulitiple_sort_test()
        {
            // Arrange
            var sortByCreationTimeAsc = EntityBuilder<SimpleEntity>.Sort.Ascending(x => x.CreationTime);
            var sortByIdDesc = EntityBuilder<SimpleEntity>.Sort.Descending(x => x.Id);


            // Act
            var sql = EntityBuilder<SimpleEntity>.Sort.Combine(sortByCreationTimeAsc, sortByIdDesc).ToSql();

            // Assert
            var expectedOrderSql = " ORDER BY CreationTime ASC,Id DESC";// "order by CreationTime ASC";
            Assert.Equal(expectedOrderSql, sql);
        }
    }
}
