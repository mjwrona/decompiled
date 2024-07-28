// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.ManyToManyJoinInterceptor
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public class ManyToManyJoinInterceptor : IDbCommandTreeInterceptor, IDbInterceptor
  {
    private static readonly ManyToManyJoinInterceptor _instance = new ManyToManyJoinInterceptor();

    public static ManyToManyJoinInterceptor Instance => ManyToManyJoinInterceptor._instance;

    public void TreeCreated(
      DbCommandTreeInterceptionContext interceptionContext)
    {
      if (interceptionContext.OriginalResult.DataSpace != DataSpace.SSpace || !(interceptionContext.Result is DbQueryCommandTree result))
        return;
      AnalyticsContext analyticsContext = interceptionContext.DbContexts.OfType<AnalyticsContext>().FirstOrDefault<AnalyticsContext>();
      if (analyticsContext == null)
        return;
      ManyToManyJoinInterceptor.HasManyToManyMappingTableVisitor visitor = new ManyToManyJoinInterceptor.HasManyToManyMappingTableVisitor();
      result.Query.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) visitor);
      if (!visitor.HasMapping)
        return;
      DbExpression query = result.Query.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) new ManyToManyJoinInterceptor.TagsTableJoinVisitor()
      {
        Version = analyticsContext.Version
      });
      interceptionContext.Result = (DbCommandTree) new DbQueryCommandTree(result.MetadataWorkspace, result.DataSpace, query);
    }

    private static bool IsSnapshotType(EdmType type)
    {
      if (type.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
        return ManyToManyJoinInterceptor.IsSnapshotType(((CollectionType) type).TypeUsage.EdmType);
      return type.Name == "WorkItemSnapshot" || type.Name == "WorkItemSnapshotTag" || type.Name == "WorkItemBoardSnapshot" || type.Name == "WorkItemBoardSnapshotTag";
    }

    internal class TagsTableJoinVisitor : DefaultExpressionVisitor
    {
      private Stack<DbJoinExpression> _currentJoin = new Stack<DbJoinExpression>();
      private Stack<DbFilterExpression> _currentFilter = new Stack<DbFilterExpression>();
      private DbProjectExpression _topProjection;

      public override DbExpression Visit(DbAndExpression expression)
      {
        DbExpression left = this.VisitExpression(expression.Left);
        DbExpression right = this.VisitExpression(expression.Right);
        if (left == null)
          return right;
        return right == null ? left : (DbExpression) left.And(right);
      }

      public override DbExpression Visit(DbJoinExpression expression)
      {
        this._currentJoin.Push(expression);
        DbExpression dbExpression = base.Visit(expression);
        this._currentJoin.Pop();
        return dbExpression;
      }

      public override DbExpression Visit(DbFilterExpression expression)
      {
        this._currentFilter.Push(expression);
        DbExpression dbExpression = base.Visit(expression);
        this._currentFilter.Pop();
        return dbExpression;
      }

      public override DbExpression Visit(DbComparisonExpression expression)
      {
        if (this.IsSnapshotKeyReference(expression.Left) && this.IsSnapshotKeyReference(expression.Right))
          return (DbExpression) null;
        if (this.IsWorkItemIdKeyReference(expression.Left) && this.IsWorkItemRevisionKeyReference(expression.Right))
          return (DbExpression) this.RewriteSKReference(expression.Left, expression.Right, "WorkItemRevisionSK");
        if (this.IsWorkItemIdKeyReference(expression.Right) && this.IsWorkItemRevisionKeyReference(expression.Left))
          return (DbExpression) this.RewriteSKReference(expression.Right, expression.Left, "WorkItemRevisionSK");
        if (this.IsWorkItemIdKeyReference(expression.Left) && this.IsAreaKeyReference(expression.Right))
          return (DbExpression) this.RewriteSKReference(expression.Left, expression.Right, "AreaSK");
        if (this.IsWorkItemIdKeyReference(expression.Right) && this.IsAreaKeyReference(expression.Left))
          return (DbExpression) this.RewriteSKReference(expression.Right, expression.Left, "AreaSK");
        if (this.IsWorkItemRevisionKeyReference(expression.Left) && this.IsAreaKeyReference(expression.Right))
          return (DbExpression) this.RewriteSKReference(expression.Left, expression.Right, "AreaSK");
        if (this.IsWorkItemRevisionKeyReference(expression.Right) && this.IsAreaKeyReference(expression.Left))
          return (DbExpression) this.RewriteSKReference(expression.Right, expression.Left, "AreaSK");
        if (this.IsWorkItemIdKeyReference(expression.Left) && this.IsTeamFieldKeyReference(expression.Right))
          return (DbExpression) this.RewriteSKReference(expression.Left, expression.Right, "TeamFieldSK");
        if (this.IsWorkItemIdKeyReference(expression.Right) && this.IsTeamFieldKeyReference(expression.Left))
          return (DbExpression) this.RewriteSKReference(expression.Right, expression.Left, "TeamFieldSK");
        if (this.IsWorkItemRevisionKeyReference(expression.Left) && this.IsTeamFieldKeyReference(expression.Right))
          return (DbExpression) this.RewriteSKReference(expression.Left, expression.Right, "TeamFieldSK");
        return this.IsWorkItemRevisionKeyReference(expression.Right) && this.IsTeamFieldKeyReference(expression.Left) ? (DbExpression) this.RewriteSKReference(expression.Right, expression.Left, "TeamFieldSK") : base.Visit(expression);
      }

      private DbComparisonExpression RewriteSKReference(
        DbExpression expToRewrite,
        DbExpression expToKeep,
        string keyName)
      {
        DbExpression skReference = this.GetSKReference(expToRewrite, keyName);
        if (!skReference.ResultType.Equals((object) expToKeep.ResultType))
          expToKeep = (DbExpression) expToKeep.CastTo(skReference.ResultType);
        return skReference.Equal(expToKeep);
      }

      public int Version { get; internal set; }

      public override DbExpression Visit(DbDistinctExpression expression)
      {
        ReadOnlyMetadataCollection<EdmProperty> typeProperties = ((CollectionType) expression.ResultType.EdmType).TypeUsage.EdmType.GetTypeProperties();
        DbDistinctExpression distinctExpression = (DbDistinctExpression) base.Visit(expression);
        ReadOnlyMetadataCollection<EdmProperty> newProperties = ((CollectionType) distinctExpression.ResultType.EdmType).TypeUsage.EdmType.GetTypeProperties();
        if (newProperties.Count != typeProperties.Count)
        {
          HashSet<string> originalNames = new HashSet<string>(typeProperties.Select<EdmProperty, string>((Func<EdmProperty, string>) (p => p.Name)));
          DbProjectExpression projectExpression = (DbProjectExpression) distinctExpression.Argument;
          distinctExpression = projectExpression.Input.Project((DbExpression) DbExpressionBuilder.NewRow(((DbNewInstanceExpression) projectExpression.Projection).Arguments.Where<DbExpression>((Func<DbExpression, int, bool>) ((arg, i) => originalNames.Contains(newProperties[i].Name))).Select<DbExpression, KeyValuePair<string, DbExpression>>((Func<DbExpression, int, KeyValuePair<string, DbExpression>>) ((arg, i) => new KeyValuePair<string, DbExpression>(newProperties[i].Name, arg))))).Distinct();
        }
        return (DbExpression) distinctExpression;
      }

      public override DbExpression Visit(DbProjectExpression expression)
      {
        bool flag = false;
        if (this._topProjection == null)
        {
          this._topProjection = expression;
          flag = true;
        }
        expression = (DbProjectExpression) base.Visit(expression);
        if (expression.Projection is DbNewInstanceExpression projection && !flag)
        {
          ManyToManyJoinInterceptor.HasTypeVisitor visitor = new ManyToManyJoinInterceptor.HasTypeVisitor()
          {
            Version = this.Version
          };
          expression.Input.Expression.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) visitor);
          List<KeyValuePair<string, DbExpression>> keyValuePairList = new List<KeyValuePair<string, DbExpression>>();
          KeyValuePair<string, DbExpression>? pair = new KeyValuePair<string, DbExpression>?();
          if (visitor.HasWorkItemType)
          {
            string realKey = "WorkItemId";
            if (projection.Arguments.OfType<DbPropertyExpression>().Any<DbPropertyExpression>((Func<DbPropertyExpression, bool>) (p => p.Property.Name == realKey)))
            {
              if (this.TryAddKey(projection, expression, "WorkItemRevisionSK", out pair))
                keyValuePairList.Add(pair.Value);
              if (this.TryAddKey(projection, expression, "AreaSK", out pair))
                keyValuePairList.Add(pair.Value);
              if (this.TryAddKey(projection, expression, "TeamFieldSK", out pair))
                keyValuePairList.Add(pair.Value);
            }
          }
          else if (visitor.HasWorkItemRevisionType)
          {
            string realKey = "WorkItemRevisionSK";
            if (projection.Arguments.OfType<DbPropertyExpression>().Any<DbPropertyExpression>((Func<DbPropertyExpression, bool>) (p => p.Property.Name == realKey)))
            {
              if (!visitor.HasWorkItemTeamType && this.TryAddKey(projection, expression, "AreaSK", out pair))
                keyValuePairList.Add(pair.Value);
              if (!visitor.HasWorkItemTeamType && this.TryAddKey(projection, expression, "TeamFieldSK", out pair))
                keyValuePairList.Add(pair.Value);
            }
          }
          else if (visitor.HasSnapshotType)
          {
            if (this.TryAddKey(projection, expression, "AreaSK", out pair))
              keyValuePairList.Add(pair.Value);
            if (this.TryAddKey(projection, expression, "TeamFieldSK", out pair))
              keyValuePairList.Add(pair.Value);
          }
          if (keyValuePairList.Any<KeyValuePair<string, DbExpression>>())
          {
            ReadOnlyMetadataCollection<EdmProperty> properties = projection.ResultType.EdmType.GetTypeProperties();
            IEnumerable<KeyValuePair<string, DbExpression>> columnValues = projection.Arguments.Select<DbExpression, KeyValuePair<string, DbExpression>>((Func<DbExpression, int, KeyValuePair<string, DbExpression>>) ((arg, i) => new KeyValuePair<string, DbExpression>(properties[i].Name, arg))).Concat<KeyValuePair<string, DbExpression>>((IEnumerable<KeyValuePair<string, DbExpression>>) keyValuePairList);
            return (DbExpression) this.VisitExpressionBinding(expression.Input).Project((DbExpression) DbExpressionBuilder.NewRow(columnValues));
          }
        }
        return (DbExpression) expression;
      }

      private bool TryAddKey(
        DbNewInstanceExpression newExpression,
        DbProjectExpression expression,
        string ensureKey,
        out KeyValuePair<string, DbExpression>? pair)
      {
        pair = new KeyValuePair<string, DbExpression>?();
        if (!newExpression.Arguments.OfType<DbPropertyExpression>().Any<DbPropertyExpression>((Func<DbPropertyExpression, bool>) (p => p.Property.Name == ensureKey)))
        {
          if (expression.Input.Variable.HasProperty(ensureKey))
          {
            pair = new KeyValuePair<string, DbExpression>?(new KeyValuePair<string, DbExpression>(ensureKey, (DbExpression) expression.Input.Variable.Property(ensureKey)));
            return true;
          }
          DbExpression pathToProperty = this.FindPathToProperty((DbExpression) expression.Input.Variable, ensureKey);
          if (pathToProperty != null)
          {
            pair = new KeyValuePair<string, DbExpression>?(new KeyValuePair<string, DbExpression>(ensureKey, pathToProperty));
            return true;
          }
        }
        return false;
      }

      private DbExpression FindPathToProperty(DbExpression source, string propertyName)
      {
        foreach (EdmProperty edmProperty in source.ResultType.EdmType.GetTypeProperties().Where<EdmProperty>((Func<EdmProperty, bool>) (p => p.TypeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)))
        {
          DbPropertyExpression propertyExpression = source.Property(edmProperty.Name);
          if (propertyExpression.HasProperty(propertyName))
            return (DbExpression) propertyExpression.Property(propertyName);
          DbExpression pathToProperty = this.FindPathToProperty((DbExpression) propertyExpression, propertyName);
          if (pathToProperty != null)
            return pathToProperty;
        }
        return (DbExpression) null;
      }

      private DbExpression GetSKReference(DbExpression exp, string key = "WorkItemRevisionSK") => ((DbPropertyExpression) exp).Instance is DbVariableReferenceExpression instance ? (DbExpression) this.Visit(instance).Property(key) : (DbExpression) ((DbPropertyExpression) exp).Instance.Property(key);

      private bool IsWorkItemIdKeyReference(DbExpression exp) => exp is DbPropertyExpression propertyExpression && propertyExpression.Property.Name == "WorkItemId";

      private bool IsAreaKeyReference(DbExpression exp) => exp is DbPropertyExpression propertyExpression && propertyExpression.Property.Name == "AreaSK";

      private bool IsTeamFieldKeyReference(DbExpression exp) => exp is DbPropertyExpression propertyExpression && propertyExpression.Property.Name == "TeamFieldSK";

      private bool IsWorkItemRevisionKeyReference(DbExpression exp) => exp is DbPropertyExpression propertyExpression && propertyExpression.Property.Name == "WorkItemRevisionSK";

      private bool IsSnapshotKeyReference(DbExpression exp)
      {
        if (exp is DbPropertyExpression propertyExpression && (propertyExpression.Property.Name == "DateSK" || propertyExpression.Property.Name == "BoardLocationSK"))
        {
          if (ManyToManyJoinInterceptor.IsSnapshotType((EdmType) propertyExpression.Property.DeclaringType))
            return true;
          if (this._currentJoin.Count > 0)
          {
            DbJoinExpression dbJoinExpression = this._currentJoin.Peek();
            return this.HasSnapshotType(dbJoinExpression.Left.Variable == propertyExpression.Instance ? dbJoinExpression.Left.Expression : dbJoinExpression.Right.Expression);
          }
          if (this._currentFilter.Count > 0)
            return this.HasSnapshotType(this._currentFilter.Peek().Input.Expression);
        }
        return false;
      }

      private bool HasSnapshotType(DbExpression exp)
      {
        ManyToManyJoinInterceptor.HasTypeVisitor visitor = new ManyToManyJoinInterceptor.HasTypeVisitor()
        {
          Version = this.Version
        };
        exp.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) visitor);
        return visitor.HasSnapshotType;
      }

      private bool HasWorkItemType(DbExpression exp)
      {
        ManyToManyJoinInterceptor.HasTypeVisitor visitor = new ManyToManyJoinInterceptor.HasTypeVisitor()
        {
          Version = this.Version
        };
        exp.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) visitor);
        return visitor.HasWorkItemType;
      }
    }

    private class HasManyToManyMappingTableVisitor : DefaultExpressionVisitor
    {
      private static readonly Regex _mappingTypeRegex = new Regex("WorkItem.*(Tag|Team|BoardLocation|Process)", RegexOptions.Compiled);

      public bool HasMapping { get; private set; }

      public override DbExpression Visit(DbScanExpression expression)
      {
        if (ManyToManyJoinInterceptor.HasManyToManyMappingTableVisitor.IsMappingType(expression.ResultType.EdmType))
          this.HasMapping = true;
        return base.Visit(expression);
      }

      private static bool IsMappingType(EdmType type) => type.BuiltInTypeKind == BuiltInTypeKind.CollectionType ? ManyToManyJoinInterceptor.HasManyToManyMappingTableVisitor.IsMappingType(((CollectionType) type).TypeUsage.EdmType) : ManyToManyJoinInterceptor.HasManyToManyMappingTableVisitor._mappingTypeRegex.IsMatch(type.Name);
    }

    private class HasTypeVisitor : DefaultExpressionVisitor
    {
      private static readonly List<string> _snapshotTypes = new List<string>()
      {
        "AnalyticsModel.vw_WorkItemSnapshot",
        "AnalyticsInternal.ef_WorkItemSnapshotTag",
        "AnalyticsInternal.ef_WorkItemSnapshotTeam",
        "AnalyticsModel.vw_WorkItemBoardSnapshot",
        "AnalyticsInternal.ef_WorkItemBoardSnapshotTag",
        "AnalyticsInternal.ef_WorkItemSnapshotProcess"
      };
      private static readonly List<string> _workItemTypes = new List<string>()
      {
        "AnalyticsModel.vw_WorkItem"
      };
      private static readonly List<string> _workItemRevisionTypes = new List<string>()
      {
        "AnalyticsModel.vw_WorkItemRevision"
      };
      private static readonly List<string> _workItemTagTypes = new List<string>()
      {
        "AnalyticsInternal.ef_WorkItemTag"
      };
      private static readonly List<string> _workItemTeamTypes = new List<string>()
      {
        "AnalyticsInternal.ef_WorkItemTeam"
      };
      private bool _ignore;

      public bool HasWorkItemType { get; private set; }

      public bool HasWorkItemRevisionType { get; private set; }

      public bool HasWorkItemTagType { get; private set; }

      public bool HasWorkItemTeamType { get; private set; }

      public bool HasSnapshotType { get; private set; }

      public int Version { get; internal set; }

      public override DbExpression Visit(DbIsEmptyExpression expression)
      {
        bool ignore = this._ignore;
        this._ignore = true;
        DbExpression dbExpression = base.Visit(expression);
        this._ignore = ignore;
        return dbExpression;
      }

      public override DbExpression Visit(DbScanExpression expression)
      {
        if (!this._ignore)
        {
          if (this.IsSelectedType(expression.ResultType.EdmType, ManyToManyJoinInterceptor.HasTypeVisitor._workItemTypes))
            this.HasWorkItemType = true;
          else if (this.IsSelectedType(expression.ResultType.EdmType, ManyToManyJoinInterceptor.HasTypeVisitor._workItemRevisionTypes))
            this.HasWorkItemRevisionType = true;
          else if (this.IsSelectedType(expression.ResultType.EdmType, ManyToManyJoinInterceptor.HasTypeVisitor._workItemTagTypes))
            this.HasWorkItemTagType = true;
          else if (this.IsSelectedType(expression.ResultType.EdmType, ManyToManyJoinInterceptor.HasTypeVisitor._snapshotTypes))
            this.HasSnapshotType = true;
          else if (this.IsSelectedType(expression.ResultType.EdmType, ManyToManyJoinInterceptor.HasTypeVisitor._workItemTeamTypes))
            this.HasWorkItemTeamType = true;
        }
        return base.Visit(expression);
      }

      private bool IsSelectedType(EdmType type, List<string> tableNames)
      {
        if (type.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
          return this.IsSelectedType(((CollectionType) type).TypeUsage.EdmType, tableNames);
        string str = type.MetadataProperties.FirstOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name == "TableName"))?.Value?.ToString();
        return str != null && tableNames.Contains(str);
      }
    }
  }
}
