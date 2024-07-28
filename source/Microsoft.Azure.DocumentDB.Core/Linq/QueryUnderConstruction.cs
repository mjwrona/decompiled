// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Linq.QueryUnderConstruction
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using Microsoft.Azure.Documents.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.Documents.Linq
{
  internal sealed class QueryUnderConstruction
  {
    private readonly Func<string, ParameterExpression> aliasCreatorFunc;
    public const string DefaultSubqueryRoot = "r";
    private SqlSelectClause selectClause;
    private SqlWhereClause whereClause;
    private SqlOrderbyClause orderByClause;
    private SqlTopSpec topSpec;
    private SqlOffsetSpec offsetSpec;
    private SqlLimitSpec limitSpec;
    private Lazy<ParameterExpression> alias;
    private QueryUnderConstruction inputQuery;

    public FromParameterBindings fromParameters { get; set; }

    public ParameterExpression Alias => this.alias.Value;

    public QueryUnderConstruction(Func<string, ParameterExpression> aliasCreatorFunc)
      : this(aliasCreatorFunc, (QueryUnderConstruction) null)
    {
    }

    public QueryUnderConstruction(
      Func<string, ParameterExpression> aliasCreatorFunc,
      QueryUnderConstruction inputQuery)
    {
      this.fromParameters = new FromParameterBindings();
      this.aliasCreatorFunc = aliasCreatorFunc;
      this.inputQuery = inputQuery;
      this.alias = new Lazy<ParameterExpression>((Func<ParameterExpression>) (() => aliasCreatorFunc("r")));
    }

    public void Bind(ParameterExpression parameter, SqlCollection collection) => this.AddBinding(new FromParameterBindings.Binding(parameter, collection, true));

    public void AddBinding(FromParameterBindings.Binding binding) => this.fromParameters.Add(binding);

    public ParameterExpression GetInputParameterInContext(bool isInNewQuery) => !isInNewQuery ? this.fromParameters.GetInputParameter() : this.Alias;

    private SqlFromClause CreateFrom(SqlCollectionExpression inputCollectionExpression)
    {
      bool flag = true;
      foreach (FromParameterBindings.Binding binding in this.fromParameters.GetBindings())
      {
        if (flag)
        {
          flag = false;
          if (inputCollectionExpression != null)
            continue;
        }
        ParameterExpression parameter = binding.Parameter;
        SqlCollection parameterDefinition = binding.ParameterDefinition;
        SqlIdentifier sqlIdentifier = SqlIdentifier.Create(parameter.Name);
        SqlCollectionExpression rightExpression = binding.IsInCollection ? (SqlCollectionExpression) SqlArrayIteratorCollectionExpression.Create(sqlIdentifier, parameterDefinition) : (SqlCollectionExpression) SqlAliasedCollectionExpression.Create(parameterDefinition ?? (SqlCollection) SqlInputPathCollection.Create(sqlIdentifier, (SqlPathExpression) null), parameterDefinition == null ? (SqlIdentifier) null : sqlIdentifier);
        inputCollectionExpression = inputCollectionExpression == null ? rightExpression : (SqlCollectionExpression) SqlJoinCollectionExpression.Create(inputCollectionExpression, rightExpression);
      }
      return SqlFromClause.Create(inputCollectionExpression);
    }

    private SqlFromClause CreateSubqueryFromClause() => this.CreateFrom((SqlCollectionExpression) SqlAliasedCollectionExpression.Create((SqlCollection) SqlSubqueryCollection.Create(this.inputQuery.GetSqlQuery()), SqlIdentifier.Create(this.inputQuery.Alias.Name)));

    public SqlQuery GetSqlQuery()
    {
      SqlFromClause fromClause = this.inputQuery == null ? this.CreateFrom((SqlCollectionExpression) null) : this.CreateSubqueryFromClause();
      SqlSelectClause sqlSelectClause = this.selectClause ?? (this.selectClause = SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(this.fromParameters.GetInputParameter().Name)))));
      SqlSelectClause selectClause = SqlSelectClause.Create(sqlSelectClause.SelectSpec, sqlSelectClause.TopSpec ?? this.topSpec, sqlSelectClause.HasDistinct);
      SqlOffsetLimitClause offsetLimitClause1;
      SqlOffsetLimitClause offsetLimitClause2 = this.offsetSpec != null ? SqlOffsetLimitClause.Create(this.offsetSpec, this.limitSpec ?? SqlLimitSpec.Create((long) int.MaxValue)) : (offsetLimitClause1 = (SqlOffsetLimitClause) null);
      return SqlQuery.Create(selectClause, fromClause, this.whereClause, (SqlGroupByClause) null, this.orderByClause, offsetLimitClause2);
    }

    public QueryUnderConstruction PackageQuery(HashSet<ParameterExpression> inScope)
    {
      QueryUnderConstruction underConstruction = new QueryUnderConstruction(this.aliasCreatorFunc);
      underConstruction.fromParameters.SetInputParameter(typeof (object), this.Alias.Name, inScope);
      underConstruction.inputQuery = this;
      return underConstruction;
    }

    public QueryUnderConstruction FlattenAsPossible()
    {
      QueryUnderConstruction underConstruction1 = (QueryUnderConstruction) null;
      QueryUnderConstruction underConstruction2 = (QueryUnderConstruction) null;
      bool flag1 = false;
      bool flag2 = false;
      for (QueryUnderConstruction underConstruction3 = this; underConstruction3 != null; underConstruction3 = underConstruction3.inputQuery)
      {
        foreach (FromParameterBindings.Binding binding in underConstruction3.fromParameters.GetBindings())
        {
          if (binding.ParameterDefinition != null && binding.ParameterDefinition is SqlSubqueryCollection)
          {
            underConstruction2 = this;
            break;
          }
        }
        if (underConstruction3.inputQuery != null && underConstruction3.fromParameters.GetBindings().First<FromParameterBindings.Binding>().Parameter.Name == underConstruction3.inputQuery.Alias.Name && underConstruction3.fromParameters.GetBindings().Any<FromParameterBindings.Binding>((Func<FromParameterBindings.Binding, bool>) (b => b.ParameterDefinition != null)))
        {
          underConstruction2 = this;
          break;
        }
        if (underConstruction2 == null)
        {
          if (((underConstruction3.topSpec != null || underConstruction3.offsetSpec != null ? 1 : (underConstruction3.limitSpec != null ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 || ((underConstruction3.selectClause == null ? 0 : (underConstruction3.selectClause.HasDistinct ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
          {
            underConstruction1.inputQuery = underConstruction3.FlattenAsPossible();
            underConstruction2 = this;
            break;
          }
          flag1 = flag1 || underConstruction3.selectClause != null && !underConstruction3.selectClause.HasDistinct;
          flag2 = ((flag2 ? 1 : 0) | (underConstruction3.whereClause != null || underConstruction3.orderByClause != null || underConstruction3.topSpec != null || underConstruction3.offsetSpec != null || underConstruction3.fromParameters.GetBindings().Any<FromParameterBindings.Binding>((Func<FromParameterBindings.Binding, bool>) (b => b.ParameterDefinition != null)) ? 1 : (underConstruction3.selectClause == null ? 0 : (underConstruction3.selectClause.HasDistinct ? 1 : (this.HasSelectAggregate() ? 1 : 0))))) != 0;
          underConstruction1 = underConstruction3;
        }
        else
          break;
      }
      if (underConstruction2 == null)
        underConstruction2 = this.Flatten();
      return underConstruction2;
    }

    private QueryUnderConstruction Flatten()
    {
      if (this.inputQuery == null)
      {
        this.selectClause = this.selectClause != null ? SqlSelectClause.Create(this.selectClause.SelectSpec, this.topSpec, this.selectClause.HasDistinct) : SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(this.fromParameters.GetInputParameter().Name))));
        return this;
      }
      QueryUnderConstruction underConstruction = this.inputQuery.Flatten();
      SqlSelectClause selectClause = underConstruction.selectClause;
      SqlWhereClause whereClause = underConstruction.whereClause;
      string str = (string) null;
      HashSet<string> stringSet = new HashSet<string>();
      foreach (FromParameterBindings.Binding binding in this.inputQuery.fromParameters.GetBindings())
        stringSet.Add(binding.Parameter.Name);
      foreach (FromParameterBindings.Binding binding in this.fromParameters.GetBindings())
      {
        if (binding.ParameterDefinition == null || stringSet.Contains(binding.Parameter.Name))
          str = binding.Parameter.Name;
      }
      SqlIdentifier inputParam = SqlIdentifier.Create(str);
      SqlSelectClause sqlSelectClause = this.Substitute(selectClause, selectClause.TopSpec ?? this.topSpec, inputParam, this.selectClause);
      SqlWhereClause second = this.Substitute(selectClause.SelectSpec, inputParam, this.whereClause);
      SqlOrderbyClause sqlOrderbyClause = this.Substitute(selectClause.SelectSpec, inputParam, this.orderByClause);
      SqlWhereClause sqlWhereClause = QueryUnderConstruction.CombineWithConjunction(whereClause, second);
      QueryUnderConstruction.CombineInputParameters(underConstruction.fromParameters, this.fromParameters);
      SqlOffsetSpec offsetSpec;
      SqlLimitSpec limitSpec;
      if (underConstruction.offsetSpec != null)
      {
        offsetSpec = underConstruction.offsetSpec;
        limitSpec = underConstruction.limitSpec;
      }
      else
      {
        offsetSpec = this.offsetSpec;
        limitSpec = this.limitSpec;
      }
      return new QueryUnderConstruction(this.aliasCreatorFunc)
      {
        selectClause = sqlSelectClause,
        whereClause = sqlWhereClause,
        inputQuery = (QueryUnderConstruction) null,
        fromParameters = underConstruction.fromParameters,
        orderByClause = sqlOrderbyClause ?? this.inputQuery.orderByClause,
        offsetSpec = offsetSpec,
        limitSpec = limitSpec,
        alias = new Lazy<ParameterExpression>((Func<ParameterExpression>) (() => this.Alias))
      };
    }

    private SqlSelectClause Substitute(
      SqlSelectClause inputSelectClause,
      SqlTopSpec topSpec,
      SqlIdentifier inputParam,
      SqlSelectClause selectClause)
    {
      SqlSelectSpec selectSpec1 = inputSelectClause.SelectSpec;
      if (selectClause == null)
        return selectSpec1 == null ? (SqlSelectClause) null : SqlSelectClause.Create(selectSpec1, topSpec, inputSelectClause.HasDistinct);
      switch (selectSpec1)
      {
        case SqlSelectStarSpec _:
          return SqlSelectClause.Create(selectSpec1, topSpec, inputSelectClause.HasDistinct);
        case SqlSelectValueSpec sqlSelectValueSpec2:
          SqlSelectSpec selectSpec2 = selectClause.SelectSpec;
          switch (selectSpec2)
          {
            case SqlSelectStarSpec _:
              return SqlSelectClause.Create(selectSpec1, topSpec, selectClause.HasDistinct || inputSelectClause.HasDistinct);
            case SqlSelectValueSpec sqlSelectValueSpec1:
              return SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create(SqlExpressionManipulation.Substitute(sqlSelectValueSpec2.Expression, inputParam, sqlSelectValueSpec1.Expression)), topSpec, selectClause.HasDistinct || inputSelectClause.HasDistinct);
            default:
              throw new DocumentQueryException("Unexpected SQL select clause type: " + (object) selectSpec2.Kind);
          }
        default:
          throw new DocumentQueryException("Unexpected SQL select clause type: " + (object) selectSpec1.Kind);
      }
    }

    private SqlWhereClause Substitute(
      SqlSelectSpec spec,
      SqlIdentifier inputParam,
      SqlWhereClause whereClause)
    {
      if (whereClause == null)
        return (SqlWhereClause) null;
      switch (spec)
      {
        case SqlSelectStarSpec _:
          return whereClause;
        case SqlSelectValueSpec sqlSelectValueSpec:
          SqlScalarExpression expression = sqlSelectValueSpec.Expression;
          SqlScalarExpression filterExpression = whereClause.FilterExpression;
          SqlIdentifier toReplace = inputParam;
          SqlScalarExpression into = filterExpression;
          return SqlWhereClause.Create(SqlExpressionManipulation.Substitute(expression, toReplace, into));
        default:
          throw new DocumentQueryException("Unexpected SQL select clause type: " + (object) spec.Kind);
      }
    }

    private SqlOrderbyClause Substitute(
      SqlSelectSpec spec,
      SqlIdentifier inputParam,
      SqlOrderbyClause orderByClause)
    {
      if (orderByClause == null)
        return (SqlOrderbyClause) null;
      switch (spec)
      {
        case SqlSelectStarSpec _:
          return orderByClause;
        case SqlSelectValueSpec sqlSelectValueSpec:
          SqlScalarExpression expression1 = sqlSelectValueSpec.Expression;
          SqlOrderByItem[] sqlOrderByItemArray = new SqlOrderByItem[orderByClause.OrderbyItems.Count];
          for (int index = 0; index < sqlOrderByItemArray.Length; ++index)
          {
            SqlScalarExpression expression2 = SqlExpressionManipulation.Substitute(expression1, inputParam, orderByClause.OrderbyItems[index].Expression);
            sqlOrderByItemArray[index] = SqlOrderByItem.Create(expression2, orderByClause.OrderbyItems[index].IsDescending);
          }
          return SqlOrderbyClause.Create(sqlOrderByItemArray);
        default:
          throw new DocumentQueryException("Unexpected SQL select clause type: " + (object) spec.Kind);
      }
    }

    public bool ShouldBeOnNewQuery(string methodName, int argumentCount)
    {
      bool flag = false;
      switch (methodName)
      {
        case "Any":
        case "Distinct":
        case "OrderBy":
        case "OrderByDescending":
        case "ThenBy":
        case "ThenByDescending":
        case "Where":
          flag = this.topSpec != null || this.offsetSpec != null || this.selectClause != null && !this.selectClause.HasDistinct;
          break;
        case "Average":
        case "Max":
        case "Min":
        case "Sum":
          flag = this.selectClause != null || this.offsetSpec != null || this.topSpec != null;
          break;
        case "Count":
          flag = argumentCount == 2 && this.ShouldBeOnNewQuery("Where", 2) || this.ShouldBeOnNewQuery("Sum", 1);
          break;
        case "Select":
          flag = this.selectClause != null;
          break;
        case "SelectMany":
          flag = this.topSpec != null || this.offsetSpec != null || this.selectClause != null;
          break;
        case "Skip":
          flag = this.topSpec != null || this.limitSpec != null;
          break;
      }
      return flag;
    }

    public QueryUnderConstruction AddSelectClause(SqlSelectClause select)
    {
      this.selectClause = this.selectClause != null && this.selectClause.HasDistinct && this.selectClause.HasDistinct || this.selectClause == null ? select : throw new DocumentQueryException("Internal error: attempting to overwrite SELECT clause");
      return this;
    }

    public QueryUnderConstruction AddSelectClause(
      SqlSelectClause select,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      if ((underConstruction.selectClause == null || !underConstruction.selectClause.HasDistinct || !this.selectClause.HasDistinct) && underConstruction.selectClause != null)
        throw new DocumentQueryException("Internal error: attempting to overwrite SELECT clause");
      underConstruction.selectClause = select;
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        underConstruction.AddBinding(binding);
      return underConstruction;
    }

    public QueryUnderConstruction AddOrderByClause(
      SqlOrderbyClause orderBy,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      underConstruction.orderByClause = orderBy;
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        underConstruction.AddBinding(binding);
      return underConstruction;
    }

    public QueryUnderConstruction UpdateOrderByClause(
      SqlOrderbyClause thenBy,
      TranslationContext context)
    {
      List<SqlOrderByItem> orderbyItems = new List<SqlOrderByItem>((IEnumerable<SqlOrderByItem>) context.currentQuery.orderByClause.OrderbyItems);
      orderbyItems.AddRange((IEnumerable<SqlOrderByItem>) thenBy.OrderbyItems);
      context.currentQuery.orderByClause = SqlOrderbyClause.Create((IReadOnlyList<SqlOrderByItem>) orderbyItems);
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        context.currentQuery.AddBinding(binding);
      return context.currentQuery;
    }

    public QueryUnderConstruction AddOffsetSpec(
      SqlOffsetSpec offsetSpec,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      underConstruction.offsetSpec = underConstruction.offsetSpec == null ? offsetSpec : SqlOffsetSpec.Create(underConstruction.offsetSpec.Offset + offsetSpec.Offset);
      return underConstruction;
    }

    public QueryUnderConstruction AddLimitSpec(SqlLimitSpec limitSpec, TranslationContext context)
    {
      QueryUnderConstruction underConstruction = this;
      underConstruction.limitSpec = underConstruction.limitSpec == null ? limitSpec : (underConstruction.limitSpec.Limit < limitSpec.Limit ? underConstruction.limitSpec : limitSpec);
      return underConstruction;
    }

    public QueryUnderConstruction AddTopSpec(SqlTopSpec topSpec)
    {
      QueryUnderConstruction underConstruction = this;
      underConstruction.topSpec = underConstruction.topSpec == null ? topSpec : (underConstruction.topSpec.Count < topSpec.Count ? underConstruction.topSpec : topSpec);
      return underConstruction;
    }

    private static SqlWhereClause CombineWithConjunction(
      SqlWhereClause first,
      SqlWhereClause second)
    {
      if (first == null)
        return second;
      return second == null ? first : SqlWhereClause.Create((SqlScalarExpression) SqlBinaryScalarExpression.Create(SqlBinaryScalarOperatorKind.And, first.FilterExpression, second.FilterExpression));
    }

    private static FromParameterBindings CombineInputParameters(
      FromParameterBindings inputQueryParams,
      FromParameterBindings currentQueryParams)
    {
      HashSet<string> stringSet = new HashSet<string>();
      foreach (FromParameterBindings.Binding binding in inputQueryParams.GetBindings())
        stringSet.Add(binding.Parameter.Name);
      FromParameterBindings parameterBindings = inputQueryParams;
      foreach (FromParameterBindings.Binding binding in currentQueryParams.GetBindings())
      {
        if (binding.ParameterDefinition != null && !stringSet.Contains(binding.Parameter.Name))
        {
          parameterBindings.Add(binding);
          stringSet.Add(binding.Parameter.Name);
        }
      }
      return parameterBindings;
    }

    public QueryUnderConstruction AddWhereClause(
      SqlWhereClause whereClause,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      whereClause = QueryUnderConstruction.CombineWithConjunction(underConstruction.whereClause, whereClause);
      underConstruction.whereClause = whereClause;
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        underConstruction.AddBinding(binding);
      return underConstruction;
    }

    public QueryUnderConstruction GetSubquery(QueryUnderConstruction queryBeforeVisit)
    {
      QueryUnderConstruction underConstruction1 = (QueryUnderConstruction) null;
      for (QueryUnderConstruction underConstruction2 = this; underConstruction2 != queryBeforeVisit; underConstruction2 = underConstruction2.inputQuery)
        underConstruction1 = underConstruction2;
      underConstruction1.inputQuery = (QueryUnderConstruction) null;
      return this;
    }

    public bool HasOffsetSpec() => this.offsetSpec != null;

    private bool HasSelectAggregate()
    {
      string str = (this.selectClause?.SelectSpec is SqlSelectValueSpec selectSpec ? selectSpec.Expression : (SqlScalarExpression) null) is SqlFunctionCallScalarExpression expression ? expression.Name.Value : (string) null;
      switch (str)
      {
        case null:
          return false;
        case "MAX":
        case "MIN":
        case "AVG":
        case "COUNT":
          return true;
        default:
          return str == "SUM";
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (this.inputQuery != null)
        stringBuilder.Append((object) this.inputQuery);
      if (this.whereClause != null)
      {
        stringBuilder.Append("->");
        stringBuilder.Append((object) this.whereClause);
      }
      if (this.selectClause != null)
      {
        stringBuilder.Append("->");
        stringBuilder.Append((object) this.selectClause);
      }
      return stringBuilder.ToString();
    }
  }
}
