// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementNameFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementNameFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<AccountEntitlementNameFilter>
  {
    private static readonly AccountEntitlementFilterPathEndTreeVisitor pathEndVisitor = new AccountEntitlementFilterPathEndTreeVisitor();
    private static readonly AccountEntitlementFilterStringLiteralTreeVisitor stringLiteralVisitor = new AccountEntitlementFilterStringLiteralTreeVisitor();

    public override AccountEntitlementNameFilter Visit(BinaryOperatorToken tokenIn)
    {
      if (tokenIn.OperatorKind == 2 && "name".Equals(tokenIn.Left.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementNameFilterTreeVisitor.pathEndVisitor)))
      {
        string str = tokenIn.Right.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementNameFilterTreeVisitor.stringLiteralVisitor);
        if (str != null)
          return new AccountEntitlementNameFilter(str);
      }
      return (AccountEntitlementNameFilter) null;
    }
  }
}
