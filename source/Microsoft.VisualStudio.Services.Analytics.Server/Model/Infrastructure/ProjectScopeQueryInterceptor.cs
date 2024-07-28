// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure.ProjectScopeQueryInterceptor
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using System;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Analytics.Model.Infrastructure
{
  public class ProjectScopeQueryInterceptor : IDbCommandTreeInterceptor, IDbInterceptor
  {
    public const string ProjectSKParameterName = "projectSK";

    public static ProjectScopeQueryInterceptor Instance { get; } = new ProjectScopeQueryInterceptor();

    public void TreeCreated(
      DbCommandTreeInterceptionContext interceptionContext)
    {
      if (interceptionContext.OriginalResult.DataSpace != DataSpace.SSpace || !(interceptionContext.Result is DbQueryCommandTree result) || !(interceptionContext.DbContexts.FirstOrDefault<DbContext>() is IComponentContext componentContext) || componentContext.Component == null || !componentContext.Component.GetProjectGuid().HasValue)
        return;
      DbExpression query = result.Query.Accept<DbExpression>((DbExpressionVisitor<DbExpression>) new ProjectScopeQueryInterceptor.ProjectScopeVisitor());
      interceptionContext.Result = (DbCommandTree) new DbQueryCommandTree(result.MetadataWorkspace, result.DataSpace, query);
    }

    internal class ProjectScopeVisitor : DefaultExpressionVisitor
    {
      private int tableCounter;

      public override DbExpression Visit(DbScanExpression expression)
      {
        (Type type, bool disableProjectFilter) = ProjectScopeQueryInterceptor.ProjectScopeVisitor.GetTypeMetadata(expression.Target.ElementType);
        if (!(type != (Type) null) || !typeof (IProjectScoped).IsAssignableFrom(type) || disableProjectFilter)
          return base.Visit(expression);
        DbExpression input1 = base.Visit(expression);
        ++this.tableCounter;
        string varName = "Table_" + this.tableCounter.ToString();
        DbExpressionBinding input2 = input1.BindAs(varName);
        DbPropertyExpression left = input2.VariableType.Variable(input2.VariableName).Property("ProjectSK");
        DbComparisonExpression predicate = left.Equal((DbExpression) left.Property.TypeUsage.Parameter("projectSK"));
        return (DbExpression) input2.Filter((DbExpression) predicate);
      }

      public static (Type type, bool disableProjectFilter) GetTypeMetadata(EntityTypeBase entityType) => (entityType.MetadataProperties.SingleOrDefault<MetadataProperty>((Func<MetadataProperty, bool>) (p => p.Name.Equals("http://schemas.microsoft.com/ado/2013/11/edm/customannotation:ClrTypeAnnotation", StringComparison.Ordinal)))?.Value is ClrTypeAnnotation clrTypeAnnotation ? clrTypeAnnotation.ClrType : (Type) null, clrTypeAnnotation != null && clrTypeAnnotation.ProjectFilterExclusion);
    }
  }
}
