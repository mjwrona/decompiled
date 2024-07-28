// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementAssignmentSourceFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementAssignmentSourceFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<ISet<AssignmentSource>>
  {
    private static readonly AccountEntitlementFilterPathEndTreeVisitor pathEndVisitor = new AccountEntitlementFilterPathEndTreeVisitor();
    private static readonly AccountEntitlementFilterAssignmentSourceLiteralTreeVisitor assignmentSourceLiteralVisitor = new AccountEntitlementFilterAssignmentSourceLiteralTreeVisitor();

    public override ISet<AssignmentSource> Visit(BinaryOperatorToken tokenIn)
    {
      if (tokenIn.OperatorKind == 2 && "assignmentSource".Equals(tokenIn.Left.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementAssignmentSourceFilterTreeVisitor.pathEndVisitor)))
      {
        AssignmentSource? nullable = tokenIn.Right.Accept<AssignmentSource?>((ISyntacticTreeVisitor<AssignmentSource?>) AccountEntitlementAssignmentSourceFilterTreeVisitor.assignmentSourceLiteralVisitor);
        if (nullable.HasValue)
          return (ISet<AssignmentSource>) new HashSet<AssignmentSource>()
          {
            nullable.Value
          };
      }
      if (tokenIn.OperatorKind != null)
        return (ISet<AssignmentSource>) null;
      ISet<AssignmentSource> collection = tokenIn.Left.Accept<ISet<AssignmentSource>>((ISyntacticTreeVisitor<ISet<AssignmentSource>>) this);
      ISet<AssignmentSource> other = tokenIn.Right.Accept<ISet<AssignmentSource>>((ISyntacticTreeVisitor<ISet<AssignmentSource>>) this);
      if (collection == null || other == null)
        return (ISet<AssignmentSource>) null;
      HashSet<AssignmentSource> assignmentSourceSet = new HashSet<AssignmentSource>((IEnumerable<AssignmentSource>) collection);
      assignmentSourceSet.UnionWith((IEnumerable<AssignmentSource>) other);
      return (ISet<AssignmentSource>) assignmentSourceSet;
    }
  }
}
