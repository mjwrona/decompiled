// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementLicenseIdFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementLicenseIdFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<string>
  {
    private static readonly AccountEntitlementFilterPathEndTreeVisitor pathEndVisitor = new AccountEntitlementFilterPathEndTreeVisitor();
    private static readonly AccountEntitlementFilterStringLiteralTreeVisitor stringLiteralVisitor = new AccountEntitlementFilterStringLiteralTreeVisitor();

    public override string Visit(BinaryOperatorToken tokenIn) => tokenIn.OperatorKind == 2 && "licenseId".Equals(tokenIn.Left.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementLicenseIdFilterTreeVisitor.pathEndVisitor)) ? tokenIn.Right.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementLicenseIdFilterTreeVisitor.stringLiteralVisitor) : (string) null;
  }
}
