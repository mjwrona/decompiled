// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser.AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.OData.UriParser;
using Microsoft.VisualStudio.Services.Identity;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing.Service.Utilities.QueryParser
{
  internal class AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor : 
    AccountEntitlementFilterQuerySyntacticTreeVisitorBase<AccountEntitlementSinglePropertyFilter>
  {
    private static readonly AccountEntitlementNameFilterTreeVisitor nameVisitor = new AccountEntitlementNameFilterTreeVisitor();
    private static readonly AccountEntitlementUserTypeFilterTreeVisitor userTypeVisitor = new AccountEntitlementUserTypeFilterTreeVisitor();
    private static readonly AccountEntitlementLicensesFilterTreeVisitor licensesVisitor = new AccountEntitlementLicensesFilterTreeVisitor();
    private static readonly AccountEntitlementAssignmentSourceFilterTreeVisitor assignmentSourceVisitor = new AccountEntitlementAssignmentSourceFilterTreeVisitor();

    public override AccountEntitlementSinglePropertyFilter Visit(BinaryOperatorToken tokenIn)
    {
      AccountEntitlementNameFilter entitlementNameFilter = tokenIn.Accept<AccountEntitlementNameFilter>((ISyntacticTreeVisitor<AccountEntitlementNameFilter>) AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor.nameVisitor);
      if (entitlementNameFilter != null)
        return (AccountEntitlementSinglePropertyFilter) entitlementNameFilter;
      ISet<IdentityMetaType> identityMetaTypeSet = tokenIn.Accept<ISet<IdentityMetaType>>((ISyntacticTreeVisitor<ISet<IdentityMetaType>>) AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor.userTypeVisitor);
      if (identityMetaTypeSet != null)
        return (AccountEntitlementSinglePropertyFilter) new AccountEntitlementUserTypeFilter(identityMetaTypeSet);
      ISet<AssignmentSource> assignmentSourceSet = tokenIn.Accept<ISet<AssignmentSource>>((ISyntacticTreeVisitor<ISet<AssignmentSource>>) AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor.assignmentSourceVisitor);
      if (assignmentSourceSet != null)
        return (AccountEntitlementSinglePropertyFilter) new AccountEntitlementAssignmentSourceFilter(assignmentSourceSet);
      ISet<LicenseFilter> licenseFilterSet = tokenIn.Accept<ISet<LicenseFilter>>((ISyntacticTreeVisitor<ISet<LicenseFilter>>) AccountEntitlementSinglePropertyFilterQuerySyntacticTreeVisitor.licensesVisitor);
      return licenseFilterSet != null ? (AccountEntitlementSinglePropertyFilter) new AccountEntitlementLicensesFilter(licenseFilterSet) : (AccountEntitlementSinglePropertyFilter) null;
    }
  }
}
