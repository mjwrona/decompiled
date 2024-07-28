// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.CurrentProjectFilter
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal static class CurrentProjectFilter
  {
    internal static IQueryable ApplyCurrentProjectFilter(
      IVssRequestContext requestContext,
      IQueryable queryable,
      ProjectInfo projectInfo,
      Type clrType,
      IList<Expression> currentProjectBooleanFilterExpressions)
    {
      if (projectInfo != null)
        queryable = (IQueryable) typeof (CurrentProjectFilter).GetMethod("ApplyCurrentProjectFilterToProjectScopedQuery", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(clrType).Invoke((object) null, new object[3]
        {
          (object) requestContext,
          (object) queryable,
          (object) projectInfo
        });
      return CurrentProjectFilter.ApplyUserProjectFilterAsCurrentProjectFilter(requestContext, queryable, currentProjectBooleanFilterExpressions, clrType);
    }

    internal static IQueryable ApplyCurrentProjectFilterToProjectScopedQuery<T>(
      IVssRequestContext requestContext,
      IQueryable queryable,
      ProjectInfo projectInfo)
      where T : class
    {
      queryable = (IQueryable) Queryable.Cast<T>(((IQueryable<ICurrentProjectScoped>) queryable).Where<ICurrentProjectScoped>((Expression<Func<ICurrentProjectScoped, bool>>) (i => i.CurrentProjectSK == (Guid?) projectInfo.Id)));
      return queryable;
    }

    private static IQueryable ApplyUserProjectFilterAsCurrentProjectFilter(
      IVssRequestContext requestContext,
      IQueryable queryable,
      IList<Expression> currentProjectBooleanFilterExpressions,
      Type clrType)
    {
      using (requestContext.TraceBlock(12013020, 12013021, AnalyticsService.Area, AnalyticsService.Layer, nameof (ApplyUserProjectFilterAsCurrentProjectFilter)))
      {
        if (currentProjectBooleanFilterExpressions != null && currentProjectBooleanFilterExpressions.Count > 0)
        {
          Expression expression = (Expression) null;
          ParameterExpression parameterExpression = Expression.Parameter(clrType, "currentProjectSKHolder");
          foreach (Expression filterExpression in (IEnumerable<Expression>) currentProjectBooleanFilterExpressions)
          {
            Expression right = new CurrentProjectFilter.MockProjectScopeToICurrentProjectScopeExpressionVisitor(parameterExpression, clrType).Visit(filterExpression);
            expression = expression != null ? (Expression) Expression.AndAlso(expression, right) : right;
          }
          LambdaExpression lambdaExpression = Expression.Lambda(expression, parameterExpression);
          queryable = ExpressionHelpers.QueryableWhereGeneric.MakeGenericMethod(clrType).Invoke((object) null, new object[2]
          {
            (object) queryable,
            (object) lambdaExpression
          }) as IQueryable;
        }
        return queryable;
      }
    }

    private class MockProjectScopeToICurrentProjectScopeExpressionVisitor : ExpressionVisitor
    {
      private ParameterExpression _parameterExpression;
      private Type _clrType;

      public MockProjectScopeToICurrentProjectScopeExpressionVisitor(
        ParameterExpression parameterExpression,
        Type clrType)
      {
        if (!typeof (ICurrentProjectScopeByName).IsAssignableFrom(clrType) && !typeof (ICurrentProjectNavigate).IsAssignableFrom(clrType))
          throw new InvalidOperationException(AnalyticsResources.UNSUPPORTED_CURRENT_PROJECT_FILTER_VISITOR_TYPES((object) clrType.Name));
        this._parameterExpression = parameterExpression;
        this._clrType = clrType;
      }

      protected override Expression VisitMember(MemberExpression node)
      {
        if (!(typeof (PermissionsValidator.MockProjectScoped) == node.Member.ReflectedType))
          return base.VisitMember(node);
        if (node.Member.Name == "ProjectSK")
          return (Expression) Expression.MakeMemberAccess(this.Visit(node.Expression), ((IEnumerable<MemberInfo>) this._clrType.GetMember("CurrentProjectSK")).Single<MemberInfo>());
        if (!(node.Member.Name == "ProjectName"))
          throw new InvalidOperationException(AnalyticsResources.UNEXPECTED_PROPERTY((object) "MockProjectScoped", (object) node.Member.Name));
        Expression expression = this.Visit(node.Expression);
        return typeof (ICurrentProjectScopeByName).IsAssignableFrom(this._clrType) ? (Expression) Expression.MakeMemberAccess(expression, ((IEnumerable<MemberInfo>) this._clrType.GetMember("CurrentProjectName")).Single<MemberInfo>()) : (Expression) Expression.MakeMemberAccess((Expression) Expression.MakeMemberAccess(expression, ((IEnumerable<MemberInfo>) this._clrType.GetMember("CurrentProject")).Single<MemberInfo>()), ((IEnumerable<MemberInfo>) typeof (Project).GetMember("ProjectName")).Single<MemberInfo>());
      }

      protected override Expression VisitParameter(ParameterExpression node)
      {
        if (typeof (PermissionsValidator.MockProjectScoped).IsAssignableFrom(node.Type))
          return (Expression) this._parameterExpression;
        throw new InvalidOperationException(AnalyticsResources.UNEXPECTED_TYPE((object) "MockProjectScoped", (object) node.Type.Name));
      }
    }
  }
}
