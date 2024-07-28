// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementUserTypeFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementUserTypeFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<ISet<IdentityMetaType>>
  {
    private static readonly AccountEntitlementFilterPathEndTreeVisitor pathEndVisitor = new AccountEntitlementFilterPathEndTreeVisitor();
    private static readonly AccountEntitlementFilterUserTypeLiteralTreeVisitor userTypeVisitor = new AccountEntitlementFilterUserTypeLiteralTreeVisitor();

    public override ISet<IdentityMetaType> Visit(BinaryOperatorToken tokenIn)
    {
      if (tokenIn.OperatorKind == 2 && "userType".Equals(tokenIn.Left.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementUserTypeFilterTreeVisitor.pathEndVisitor)))
      {
        IdentityMetaType? nullable = tokenIn.Right.Accept<IdentityMetaType?>((ISyntacticTreeVisitor<IdentityMetaType?>) AccountEntitlementUserTypeFilterTreeVisitor.userTypeVisitor);
        if (nullable.HasValue)
          return (ISet<IdentityMetaType>) new HashSet<IdentityMetaType>()
          {
            nullable.Value
          };
      }
      if (tokenIn.OperatorKind != null)
        return (ISet<IdentityMetaType>) null;
      ISet<IdentityMetaType> collection = tokenIn.Left.Accept<ISet<IdentityMetaType>>((ISyntacticTreeVisitor<ISet<IdentityMetaType>>) this);
      ISet<IdentityMetaType> other = tokenIn.Right.Accept<ISet<IdentityMetaType>>((ISyntacticTreeVisitor<ISet<IdentityMetaType>>) this);
      if (collection == null || other == null)
        return (ISet<IdentityMetaType>) null;
      HashSet<IdentityMetaType> identityMetaTypeSet = new HashSet<IdentityMetaType>((IEnumerable<IdentityMetaType>) collection);
      identityMetaTypeSet.UnionWith((IEnumerable<IdentityMetaType>) other);
      return (ISet<IdentityMetaType>) identityMetaTypeSet;
    }
  }
}
