// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IInternalPlatformEntitlementService
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Licensing.DataAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DefaultServiceImplementation(typeof (PlatformAccountEntitlementService))]
  internal interface IInternalPlatformEntitlementService : 
    ILicensingEntitlementService,
    IVssFrameworkService
  {
    AccountEntitlement AssignAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource);

    AccountEntitlement AssignAccountEntitlementWithAutoUpgradeAndFallback(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource);

    AccountEntitlement AssignAvailableAccountEntitlement(
      IVssRequestContext requestContext,
      Guid userId,
      LicensingOrigin origin,
      AssignmentSource assignmentSource,
      bool overwriteExistingEntitlement);

    AccountEntitlement AssignAccountEntitlementInternal(
      IVssRequestContext requestContext,
      Guid userId,
      License license,
      LicensingOrigin origin,
      AssignmentSource assignmentSource);

    (AccountEntitlement, bool) GetAccountEntitlementForAccountUserInternal(
      IVssRequestContext requestContext,
      Guid userId,
      bool transferIdentity,
      bool determineRights,
      bool createIfNotExists);

    List<AccountEntitlement> GetPreviousAccountEntitlements(
      IVssRequestContext requestContext,
      IEnumerable<SubjectDescriptor> userDescriptors);

    List<AccountEntitlement> GetPreviousAccountEntitlements(
      IVssRequestContext requestContext,
      IEnumerable<Guid> userIds);

    void ReconcileLicenceUsage(IVssRequestContext requestContext);

    License GetMsdnLicenseForUser(IVssRequestContext requestContext, Guid userId);

    bool IsInternalHost(IVssRequestContext requestContext);

    void ImportLicenses(IVssRequestContext requestContext, LicensingSnapshot licensingSnapshot);

    LicensingSnapshot ExportLicenses(IVssRequestContext requestContext, int pageSize, int pageNo);

    void RemoveAllLicenses(IVssRequestContext requestContext);

    IList<UserLicenseCount> GetExplicitLicenseUsage(IVssRequestContext requestContext);
  }
}
