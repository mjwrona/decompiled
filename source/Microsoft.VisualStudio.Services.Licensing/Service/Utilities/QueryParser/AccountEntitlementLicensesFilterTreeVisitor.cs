// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementLicensesFilterTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementLicensesFilterTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<ISet<LicenseFilter>>
  {
    private static readonly AccountEntitlementSingleLicenseFilterTreeVisitor singleLicenseVisitor = new AccountEntitlementSingleLicenseFilterTreeVisitor();

    public override ISet<LicenseFilter> Visit(BinaryOperatorToken tokenIn)
    {
      LicenseFilter licenseFilter = tokenIn.Accept<LicenseFilter>((ISyntacticTreeVisitor<LicenseFilter>) AccountEntitlementLicensesFilterTreeVisitor.singleLicenseVisitor);
      if (licenseFilter != null)
        return (ISet<LicenseFilter>) new HashSet<LicenseFilter>()
        {
          licenseFilter
        };
      if (tokenIn.OperatorKind != null)
        return (ISet<LicenseFilter>) null;
      ISet<LicenseFilter> collection = tokenIn.Left.Accept<ISet<LicenseFilter>>((ISyntacticTreeVisitor<ISet<LicenseFilter>>) this);
      ISet<LicenseFilter> other = tokenIn.Right.Accept<ISet<LicenseFilter>>((ISyntacticTreeVisitor<ISet<LicenseFilter>>) this);
      if (collection == null || other == null)
        return (ISet<LicenseFilter>) null;
      HashSet<LicenseFilter> licenseFilterSet = new HashSet<LicenseFilter>((IEnumerable<LicenseFilter>) collection);
      licenseFilterSet.UnionWith((IEnumerable<LicenseFilter>) other);
      return (ISet<LicenseFilter>) licenseFilterSet;
    }
  }
}
