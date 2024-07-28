// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Linq.QueryUnderConstruction
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.SqlObjects;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Microsoft.Azure.Cosmos.Linq
{
  internal sealed class QueryUnderConstruction
  {
    private readonly Func<string, ParameterExpression> aliasCreatorFunc;
    public const string DefaultSubqueryRoot = "r";
    private SqlSelectClause selectClause;
    private SqlWhereClause whereClause;
    private SqlOrderByClause orderByClause;
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
          if ((SqlObject) inputCollectionExpression != (SqlObject) null)
            continue;
        }
        ParameterExpression parameter = binding.Parameter;
        SqlCollection parameterDefinition = binding.ParameterDefinition;
        SqlIdentifier sqlIdentifier = SqlIdentifier.Create(parameter.Name);
        SqlCollectionExpression right = binding.IsInCollection ? (SqlCollectionExpression) SqlArrayIteratorCollectionExpression.Create(sqlIdentifier, parameterDefinition) : (SqlCollectionExpression) SqlAliasedCollectionExpression.Create(parameterDefinition ?? (SqlCollection) SqlInputPathCollection.Create(sqlIdentifier, (SqlPathExpression) null), (SqlObject) parameterDefinition == (SqlObject) null ? (SqlIdentifier) null : sqlIdentifier);
        inputCollectionExpression = !((SqlObject) inputCollectionExpression != (SqlObject) null) ? right : (SqlCollectionExpression) SqlJoinCollectionExpression.Create(inputCollectionExpression, right);
      }
      return SqlFromClause.Create(inputCollectionExpression);
    }

    private SqlFromClause CreateSubqueryFromClause() => this.CreateFrom((SqlCollectionExpression) SqlAliasedCollectionExpression.Create((SqlCollection) SqlSubqueryCollection.Create(this.inputQuery.GetSqlQuery()), SqlIdentifier.Create(this.inputQuery.Alias.Name)));

    public SqlQuery GetSqlQuery()
    {
      SqlFromClause fromClause = this.inputQuery == null ? this.CreateFrom((SqlCollectionExpression) null) : this.CreateSubqueryFromClause();
      SqlSelectClause sqlSelectClause = this.selectClause;
      if ((SqlObject) sqlSelectClause == (SqlObject) null)
        sqlSelectClause = this.selectClause = SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(this.fromParameters.GetInputParameter().Name))));
      SqlSelectClause selectClause = SqlSelectClause.Create(sqlSelectClause.SelectSpec, sqlSelectClause.TopSpec ?? this.topSpec, sqlSelectClause.HasDistinct);
      SqlOffsetLimitClause offsetLimitClause1;
      SqlOffsetLimitClause offsetLimitClause2 = (SqlObject) this.offsetSpec != (SqlObject) null ? SqlOffsetLimitClause.Create(this.offsetSpec, this.limitSpec ?? SqlLimitSpec.Create(SqlNumberLiteral.Create((Number64) (long) int.MaxValue))) : (offsetLimitClause1 = (SqlOffsetLimitClause) null);
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
          if ((SqlObject) binding.ParameterDefinition != (SqlObject) null && binding.ParameterDefinition is SqlSubqueryCollection)
          {
            underConstruction2 = this;
            break;
          }
        }
        if (underConstruction3.inputQuery != null && underConstruction3.fromParameters.GetBindings().First<FromParameterBindings.Binding>().Parameter.Name == underConstruction3.inputQuery.Alias.Name && underConstruction3.fromParameters.GetBindings().Any<FromParameterBindings.Binding>((Func<FromParameterBindings.Binding, bool>) (b => (SqlObject) b.ParameterDefinition != (SqlObject) null)))
        {
          underConstruction2 = this;
          break;
        }
        if (underConstruction2 == null)
        {
          if ((((SqlObject) underConstruction3.topSpec != (SqlObject) null || (SqlObject) underConstruction3.offsetSpec != (SqlObject) null ? 1 : ((SqlObject) underConstruction3.limitSpec != (SqlObject) null ? 1 : 0)) & (flag2 ? 1 : 0)) != 0 || ((!((SqlObject) underConstruction3.selectClause != (SqlObject) null) ? 0 : (underConstruction3.selectClause.HasDistinct ? 1 : 0)) & (flag1 ? 1 : 0)) != 0)
          {
            underConstruction1.inputQuery = underConstruction3.FlattenAsPossible();
            underConstruction2 = this;
            break;
          }
          flag1 = flag1 || (SqlObject) underConstruction3.selectClause != (SqlObject) null && !underConstruction3.selectClause.HasDistinct;
          flag2 = ((flag2 ? 1 : 0) | ((SqlObject) underConstruction3.whereClause != (SqlObject) null || (SqlObject) underConstruction3.orderByClause != (SqlObject) null || (SqlObject) underConstruction3.topSpec != (SqlObject) null || (SqlObject) underConstruction3.offsetSpec != (SqlObject) null || underConstruction3.fromParameters.GetBindings().Any<FromParameterBindings.Binding>((Func<FromParameterBindings.Binding, bool>) (b => (SqlObject) b.ParameterDefinition != (SqlObject) null)) ? 1 : (!((SqlObject) underConstruction3.selectClause != (SqlObject) null) ? 0 : (underConstruction3.selectClause.HasDistinct ? 1 : (this.HasSelectAggregate() ? 1 : 0))))) != 0;
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
        this.selectClause = !((SqlObject) this.selectClause == (SqlObject) null) ? SqlSelectClause.Create(this.selectClause.SelectSpec, this.topSpec, this.selectClause.HasDistinct) : SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create((SqlScalarExpression) SqlPropertyRefScalarExpression.Create((SqlScalarExpression) null, SqlIdentifier.Create(this.fromParameters.GetInputParameter().Name))));
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
        if ((SqlObject) binding.ParameterDefinition == (SqlObject) null || stringSet.Contains(binding.Parameter.Name))
          str = binding.Parameter.Name;
      }
      SqlIdentifier inputParam = SqlIdentifier.Create(str);
      SqlSelectClause sqlSelectClause = this.Substitute(selectClause, selectClause.TopSpec ?? this.topSpec, inputParam, this.selectClause);
      SqlWhereClause second = this.Substitute(selectClause.SelectSpec, inputParam, this.whereClause);
      SqlOrderByClause sqlOrderByClause = this.Substitute(selectClause.SelectSpec, inputParam, this.orderByClause);
      SqlWhereClause sqlWhereClause = QueryUnderConstruction.CombineWithConjunction(whereClause, second);
      QueryUnderConstruction.CombineInputParameters(underConstruction.fromParameters, this.fromParameters);
      SqlOffsetSpec offsetSpec;
      SqlLimitSpec limitSpec;
      if ((SqlObject) underConstruction.offsetSpec != (SqlObject) null)
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
        orderByClause = sqlOrderByClause ?? this.inputQuery.orderByClause,
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
      if ((SqlObject) selectClause == (SqlObject) null)
        return !((SqlObject) selectSpec1 != (SqlObject) null) ? (SqlSelectClause) null : SqlSelectClause.Create(selectSpec1, topSpec, inputSelectClause.HasDistinct);
      if (selectSpec1 is SqlSelectStarSpec)
        return SqlSelectClause.Create(selectSpec1, topSpec, inputSelectClause.HasDistinct);
      SqlSelectValueSpec sqlSelectValueSpec1 = selectSpec1 as SqlSelectValueSpec;
      if (!((SqlObject) sqlSelectValueSpec1 != (SqlObject) null))
        throw new DocumentQueryException("Unexpected SQL select clause type: " + selectSpec1.GetType()?.ToString());
      SqlSelectSpec selectSpec2 = selectClause.SelectSpec;
      if (selectSpec2 is SqlSelectStarSpec)
        return SqlSelectClause.Create(selectSpec1, topSpec, selectClause.HasDistinct || inputSelectClause.HasDistinct);
      SqlSelectValueSpec sqlSelectValueSpec2 = selectSpec2 as SqlSelectValueSpec;
      if ((SqlObject) sqlSelectValueSpec2 != (SqlObject) null)
        return SqlSelectClause.Create((SqlSelectSpec) SqlSelectValueSpec.Create(SqlExpressionManipulation.Substitute(sqlSelectValueSpec1.Expression, inputParam, sqlSelectValueSpec2.Expression)), topSpec, selectClause.HasDistinct || inputSelectClause.HasDistinct);
      throw new DocumentQueryException("Unexpected SQL select clause type: " + selectSpec2.GetType()?.ToString());
    }

    private SqlWhereClause Substitute(
      SqlSelectSpec spec,
      SqlIdentifier inputParam,
      SqlWhereClause whereClause)
    {
      if ((SqlObject) whereClause == (SqlObject) null)
        return (SqlWhereClause) null;
      if (spec is SqlSelectStarSpec)
        return whereClause;
      SqlSelectValueSpec sqlSelectValueSpec = spec as SqlSelectValueSpec;
      SqlScalarExpression replacement = (SqlObject) sqlSelectValueSpec != (SqlObject) null ? sqlSelectValueSpec.Expression : throw new DocumentQueryException("Unexpected SQL select clause type: " + spec.GetType()?.ToString());
      SqlScalarExpression filterExpression = whereClause.FilterExpression;
      SqlIdentifier toReplace = inputParam;
      SqlScalarExpression into = filterExpression;
      return SqlWhereClause.Create(SqlExpressionManipulation.Substitute(replacement, toReplace, into));
    }

    private SqlOrderByClause Substitute(
      SqlSelectSpec spec,
      SqlIdentifier inputParam,
      SqlOrderByClause orderByClause)
    {
      if ((SqlObject) orderByClause == (SqlObject) null)
        return (SqlOrderByClause) null;
      if (spec is SqlSelectStarSpec)
        return orderByClause;
      SqlSelectValueSpec sqlSelectValueSpec = spec as SqlSelectValueSpec;
      SqlScalarExpression scalarExpression1 = (SqlObject) sqlSelectValueSpec != (SqlObject) null ? sqlSelectValueSpec.Expression : throw new DocumentQueryException("Unexpected SQL select clause type: " + spec.GetType()?.ToString());
      SqlOrderByItem[] sqlOrderByItemArray1 = new SqlOrderByItem[orderByClause.OrderByItems.Length];
      for (int index1 = 0; index1 < sqlOrderByItemArray1.Length; ++index1)
      {
        SqlScalarExpression replacement = scalarExpression1;
        SqlIdentifier toReplace = inputParam;
        ImmutableArray<SqlOrderByItem> orderByItems = orderByClause.OrderByItems;
        SqlScalarExpression expression1 = orderByItems[index1].Expression;
        SqlScalarExpression scalarExpression2 = SqlExpressionManipulation.Substitute(replacement, toReplace, expression1);
        SqlOrderByItem[] sqlOrderByItemArray2 = sqlOrderByItemArray1;
        int index2 = index1;
        SqlScalarExpression expression2 = scalarExpression2;
        orderByItems = orderByClause.OrderByItems;
        int num = orderByItems[index1].IsDescending ? 1 : 0;
        SqlOrderByItem sqlOrderByItem = SqlOrderByItem.Create(expression2, num != 0);
        sqlOrderByItemArray2[index2] = sqlOrderByItem;
      }
      return SqlOrderByClause.Create(sqlOrderByItemArray1);
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
          flag = (SqlObject) this.topSpec != (SqlObject) null || (SqlObject) this.offsetSpec != (SqlObject) null || (SqlObject) this.selectClause != (SqlObject) null && !this.selectClause.HasDistinct;
          break;
        case "Average":
        case "Max":
        case "Min":
        case "Sum":
          flag = (SqlObject) this.selectClause != (SqlObject) null || (SqlObject) this.offsetSpec != (SqlObject) null || (SqlObject) this.topSpec != (SqlObject) null;
          break;
        case "Count":
          flag = argumentCount == 2 && this.ShouldBeOnNewQuery("Where", 2) || this.ShouldBeOnNewQuery("Sum", 1);
          break;
        case "Select":
          flag = (SqlObject) this.selectClause != (SqlObject) null;
          break;
        case "SelectMany":
          flag = (SqlObject) this.topSpec != (SqlObject) null || (SqlObject) this.offsetSpec != (SqlObject) null || (SqlObject) this.selectClause != (SqlObject) null;
          break;
        case "Skip":
          flag = (SqlObject) this.topSpec != (SqlObject) null || (SqlObject) this.limitSpec != (SqlObject) null;
          break;
      }
      return flag;
    }

    public QueryUnderConstruction AddSelectClause(SqlSelectClause select)
    {
      this.selectClause = (SqlObject) this.selectClause != (SqlObject) null && this.selectClause.HasDistinct && this.selectClause.HasDistinct || (SqlObject) this.selectClause == (SqlObject) null ? select : throw new DocumentQueryException("Internal error: attempting to overwrite SELECT clause");
      return this;
    }

    public QueryUnderConstruction AddSelectClause(
      SqlSelectClause select,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      if ((!((SqlObject) underConstruction.selectClause != (SqlObject) null) || !underConstruction.selectClause.HasDistinct || !this.selectClause.HasDistinct) && !((SqlObject) underConstruction.selectClause == (SqlObject) null))
        throw new DocumentQueryException("Internal error: attempting to overwrite SELECT clause");
      underConstruction.selectClause = select;
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        underConstruction.AddBinding(binding);
      return underConstruction;
    }

    public QueryUnderConstruction AddOrderByClause(
      SqlOrderByClause orderBy,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      underConstruction.orderByClause = orderBy;
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        underConstruction.AddBinding(binding);
      return underConstruction;
    }

    public QueryUnderConstruction UpdateOrderByClause(
      SqlOrderByClause thenBy,
      TranslationContext context)
    {
      List<SqlOrderByItem> items = new List<SqlOrderByItem>((IEnumerable<SqlOrderByItem>) context.currentQuery.orderByClause.OrderByItems);
      items.AddRange((IEnumerable<SqlOrderByItem>) thenBy.OrderByItems);
      context.currentQuery.orderByClause = SqlOrderByClause.Create(items.ToImmutableArray<SqlOrderByItem>());
      foreach (FromParameterBindings.Binding binding in context.CurrentSubqueryBinding.TakeBindings())
        context.currentQuery.AddBinding(binding);
      return context.currentQuery;
    }

    public QueryUnderConstruction AddOffsetSpec(
      SqlOffsetSpec offsetSpec,
      TranslationContext context)
    {
      QueryUnderConstruction underConstruction = context.PackageCurrentQueryIfNeccessary();
      if ((SqlObject) underConstruction.offsetSpec != (SqlObject) null)
      {
        long offsetCount1 = QueryUnderConstruction.GetOffsetCount(underConstruction.offsetSpec);
        long offsetCount2 = QueryUnderConstruction.GetOffsetCount(offsetSpec);
        underConstruction.offsetSpec = SqlOffsetSpec.Create(SqlNumberLiteral.Create((Number64) (offsetCount1 + offsetCount2)));
      }
      else
        underConstruction.offsetSpec = offsetSpec;
      return underConstruction;
    }

    private static long GetOffsetCount(SqlOffsetSpec offsetSpec)
    {
      if ((SqlObject) offsetSpec == (SqlObject) null)
        throw new ArgumentNullException(nameof (offsetSpec));
      if (!(offsetSpec.OffsetExpression is SqlLiteralScalarExpression offsetExpression))
        throw new ArgumentException("Expected number literal scalar expression.");
      if (!(offsetExpression.Literal is SqlNumberLiteral literal))
        throw new ArgumentException("Expected number literal.");
      if (!literal.Value.IsInteger)
        throw new ArgumentException("Expected integer literal.");
      return Number64.ToLong(literal.Value);
    }

    public QueryUnderConstruction AddLimitSpec(SqlLimitSpec limitSpec, TranslationContext context)
    {
      QueryUnderConstruction underConstruction = this;
      if ((SqlObject) underConstruction.limitSpec != (SqlObject) null)
      {
        long limitCount = QueryUnderConstruction.GetLimitCount(underConstruction.limitSpec);
        if (QueryUnderConstruction.GetLimitCount(limitSpec) < limitCount)
          underConstruction.limitSpec = limitSpec;
      }
      else
        underConstruction.limitSpec = limitSpec;
      return underConstruction;
    }

    private static long GetLimitCount(SqlLimitSpec limitSpec)
    {
      if ((SqlObject) limitSpec == (SqlObject) null)
        throw new ArgumentNullException(nameof (limitSpec));
      if (!(limitSpec.LimitExpression is SqlLiteralScalarExpression limitExpression))
        throw new ArgumentException("Expected number literal scalar expression.");
      if (!(limitExpression.Literal is SqlNumberLiteral literal))
        throw new ArgumentException("Expected number literal.");
      if (!literal.Value.IsInteger)
        throw new ArgumentException("Expected integer literal.");
      return Number64.ToLong(literal.Value);
    }

    public QueryUnderConstruction AddTopSpec(SqlTopSpec topSpec)
    {
      QueryUnderConstruction underConstruction = this;
      if ((SqlObject) underConstruction.topSpec != (SqlObject) null)
      {
        long topCount = QueryUnderConstruction.GetTopCount(underConstruction.topSpec);
        if (QueryUnderConstruction.GetTopCount(topSpec) < topCount)
          underConstruction.topSpec = topSpec;
      }
      else
        underConstruction.topSpec = topSpec;
      return underConstruction;
    }

    private static long GetTopCount(SqlTopSpec sqlTopSpec)
    {
      if ((SqlObject) sqlTopSpec == (SqlObject) null)
        throw new ArgumentNullException(nameof (sqlTopSpec));
      if (!(sqlTopSpec.TopExpresion is SqlLiteralScalarExpression topExpresion))
        throw new ArgumentException("Expected number literal scalar expression.");
      if (!(topExpresion.Literal is SqlNumberLiteral literal))
        throw new ArgumentException("Expected number literal.");
      if (!literal.Value.IsInteger)
        throw new ArgumentException("Expected integer literal.");
      return Number64.ToLong(literal.Value);
    }

    private static SqlWhereClause CombineWithConjunction(
      SqlWhereClause first,
      SqlWhereClause second)
    {
      if ((SqlObject) first == (SqlObject) null)
        return second;
      return (SqlObject) second == (SqlObject) null ? first : SqlWhereClause.Create((SqlScalarExpression) SqlBinaryScalarExpression.Create(SqlBinaryScalarOperatorKind.And, first.FilterExpression, second.FilterExpression));
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
        if ((SqlObject) binding.ParameterDefinition != (SqlObject) null && !stringSet.Contains(binding.Parameter.Name))
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

    public bool HasOffsetSpec() => (SqlObject) this.offsetSpec != (SqlObject) null;

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
      if ((SqlObject) this.whereClause != (SqlObject) null)
      {
        stringBuilder.Append("->");
        stringBuilder.Append((object) this.whereClause);
      }
      if ((SqlObject) this.selectClause != (SqlObject) null)
      {
        stringBuilder.Append("->");
        stringBuilder.Append((object) this.selectClause);
      }
      return stringBuilder.ToString();
    }
  }
}
