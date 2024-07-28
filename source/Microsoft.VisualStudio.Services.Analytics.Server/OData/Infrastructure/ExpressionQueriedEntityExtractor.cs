// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ExpressionQueriedEntityExtractor
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  internal class ExpressionQueriedEntityExtractor
  {
    public static ISet<string> Extract(Expression expression)
    {
      HashSet<MemberInfo> memberInfoSet = new HashSet<MemberInfo>();
      new ExpressionQueriedEntityExtractor.EntityExtractorExpressionVisitor((ISet<MemberInfo>) memberInfoSet).Visit(expression);
      return EntityMemberInfoToTableMapper.Instance.GetModelTables((IEnumerable<MemberInfo>) memberInfoSet);
    }

    private class EntityExtractorExpressionVisitor : ExpressionVisitor
    {
      private ISet<MemberInfo> _entities;

      public EntityExtractorExpressionVisitor(ISet<MemberInfo> entities) => this._entities = entities;

      private void CollectEntityType(Expression node) => this._entities.Add((MemberInfo) node.Type);

      protected override Expression VisitMember(MemberExpression node)
      {
        this.CollectEntityType((Expression) node);
        if (node.Member.MemberType == MemberTypes.Property)
          this._entities.Add(node.Member);
        return base.VisitMember(node);
      }

      protected override Expression VisitParameter(ParameterExpression node)
      {
        this.CollectEntityType((Expression) node);
        return base.VisitParameter(node);
      }

      protected override MemberBinding VisitMemberBinding(MemberBinding node) => base.VisitMemberBinding(node);

      protected override MemberAssignment VisitMemberAssignment(MemberAssignment node) => base.VisitMemberAssignment(node);

      protected override Expression VisitUnary(UnaryExpression node) => base.VisitUnary(node);

      protected override Expression VisitDynamic(DynamicExpression node) => base.VisitDynamic(node);
    }
  }
}
