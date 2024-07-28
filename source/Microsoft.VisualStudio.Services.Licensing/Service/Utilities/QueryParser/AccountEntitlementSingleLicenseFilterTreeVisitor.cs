// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementSingleLicenseFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Account;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementSingleLicenseFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<LicenseFilter>
  {
    private static readonly AccountEntitlementLicenseIdFilterTreeVisitor licenseIdVisitor = new AccountEntitlementLicenseIdFilterTreeVisitor();
    private static readonly AccountEntitlementLicenseStatusFilterTreeVisitor licenseStatusVisitor = new AccountEntitlementLicenseStatusFilterTreeVisitor();

    public override LicenseFilter Visit(BinaryOperatorToken tokenIn)
    {
      string name1 = tokenIn.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementSingleLicenseFilterTreeVisitor.licenseIdVisitor);
      if (name1 != null)
        return new LicenseFilter(name1, new AccountUserStatus?());
      if (tokenIn.OperatorKind != 1)
        return (LicenseFilter) null;
      string name2 = tokenIn.Left.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementSingleLicenseFilterTreeVisitor.licenseIdVisitor) ?? tokenIn.Right.Accept<string>((ISyntacticTreeVisitor<string>) AccountEntitlementSingleLicenseFilterTreeVisitor.licenseIdVisitor);
      if (name2 == null)
        return (LicenseFilter) null;
      AccountUserStatus? nullable = tokenIn.Left.Accept<AccountUserStatus?>((ISyntacticTreeVisitor<AccountUserStatus?>) AccountEntitlementSingleLicenseFilterTreeVisitor.licenseStatusVisitor);
      if (!nullable.HasValue)
        nullable = tokenIn.Right.Accept<AccountUserStatus?>((ISyntacticTreeVisitor<AccountUserStatus?>) AccountEntitlementSingleLicenseFilterTreeVisitor.licenseStatusVisitor);
      return !nullable.HasValue ? (LicenseFilter) null : new LicenseFilter(name2, new AccountUserStatus?(nullable.Value));
    }
  }
}
