// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementLicenseStatusFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Account;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementLicenseStatusFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<AccountUserStatus?>
  {
    private static readonly AccountEntitlementFilterPathEndTreeVisitor pathEndVisitor = new AccountEntitlementFilterPathEndTreeVisitor();
    private static readonly AccountEntitlementFilterLicenseStatusLiteralTreeVisitor licenseStatusVisitor = new AccountEntitlementFilterLicenseStatusLiteralTreeVisitor();

    public override AccountUserStatus? Visit(BinaryOperatorToken tokenIn) => tokenIn.OperatorKind == 2 && "licenseStatus".Equals(tokenIn.Left.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementLicenseStatusFilterTreeVisitor.pathEndVisitor)) ? tokenIn.Right.Accept<AccountUserStatus?>((ISyntacticTreeVisitor<AccountUserStatus?>) AccountEntitlementLicenseStatusFilterTreeVisitor.licenseStatusVisitor) : new AccountUserStatus?();
  }
}
