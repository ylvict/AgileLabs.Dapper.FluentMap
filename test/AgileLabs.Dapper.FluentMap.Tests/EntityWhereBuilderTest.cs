using Dapper;
using System;
using Xunit;
using System.Linq;

namespace AgileLabs.Dapper.FluentMap.Tests
{
    public class EntityWhereBuilderTest
    {
        private class SimpleEntity
        {
            public Guid Id { get; set; }
            public DateTime CreationTime { get; set; }
            public SubEntity UserName { get; set; }
        }

        private class SubEntity
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }

        [Fact]
        public void expression_condition_test()
        {
            // Arrange

            // Act
            var expressionCondition = EntityBuilder<SimpleEntity>.Where.Expression(x => x.CreationTime == DateTime.MaxValue && x.Id == Guid.Empty);
            DynamicParameters parameters;
            var sql = expressionCondition.ToSql(out parameters);

            // Assert
            var expectedOrderSql = " where CreationTime = @CreationTime and Id = @Id";// "order by CreationTime ASC";
            Assert.Equal(expectedOrderSql, sql, ignoreCase: false);
            Assert.Equal(2, parameters.ParameterNames.Count());
        }

        //[Fact]
        //public void combin_condition_test()
        //{
        //    // Arrange

        //    // Act
        //    var expressionCondition = EntityBuilder<SimpleEntity>.Where
        //        .Expression(x => SqlMethod.Method<int>("DateDiff", x.CreationTime, x.CreationTime) == 1
        //            && (x.CreationTime == DateTime.MaxValue && x.Id == Guid.Empty) || (x.Id == Guid.NewGuid()));

        //    DynamicParameters parameters;
        //    var sql = new EntityExpression<SimpleEntity>().Where(expressionCondition.Condition).ToSql(out parameters);

        //    // Assert
        //    var expectedOrderSql = " where CreationTime = @CreationTime and Id = @Id";// "order by CreationTime ASC";
        //    Assert.Equal(expectedOrderSql, sql, ignoreCase: false);
        //    Assert.Equal(2, parameters.ParameterNames.Count());
        //}

        //[Fact]
        //public void sqlMethod_test()
        //{
        //    // Arrange

        //    // Act
        //    var expressionCondition = EntityBuilder<SimpleEntity>.Where
        //        .Expression(x => SqlMethod.Method<int>("DateDiff", x.CreationTime, x.CreationTime) == 1);

        //    DynamicParameters parameters;
        //    var sql = new EntityExpression<SimpleEntity>().Where(expressionCondition.Condition).ToSql(out parameters);

        //    // Assert
        //    var expectedOrderSql = " where CreationTime = @CreationTime and Id = @Id";// "order by CreationTime ASC";
        //    Assert.Equal(expectedOrderSql, sql, ignoreCase: false);
        //    Assert.Equal(2, parameters.ParameterNames.Count());
        //}

        [Fact(Skip = "Not support ChildProperty Condition")]
        public void childProperty_condition_test()
        {
            // Arrange

            // Act
            var sortByCreationTimeAsc = EntityBuilder<SimpleEntity>.Where.Expression(x => x.UserName.FirstName == "Duke");
            DynamicParameters parameters;
            var sql = sortByCreationTimeAsc.ToSql(out parameters);

            // Assert
            var expectedOrderSql = " Where CreationTime = '2017' AND Id = '000000'";// "order by CreationTime ASC";
            Assert.Equal(expectedOrderSql, sql);
        }
    }

    public class SqlMethod
    {
        public static TReturn Method<TReturn>(string methodName, DateTime a, DateTime b)
        {
            return default(TReturn);
        }
    }
}
