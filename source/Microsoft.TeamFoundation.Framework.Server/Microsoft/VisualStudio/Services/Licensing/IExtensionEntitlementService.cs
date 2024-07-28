// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.IExtensionEntitlementService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DefaultServiceImplementation(typeof (FrameworkExtensionEntitlementService))]
  public interface IExtensionEntitlementService : IVssFrameworkService
  {
    ICollection<ExtensionOperationResult> AssignExtensionToUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      bool autoAssignment = false);

    ICollection<ExtensionOperationResult> AssignExtensionToAllEligibleUsers(
      IVssRequestContext requestContext,
      string extensionId);

    ICollection<ExtensionOperationResult> UnassignExtensionFromUsers(
      IVssRequestContext requestContext,
      string extensionId,
      IList<Guid> userIds,
      LicensingSource source);

    IDictionary<string, LicensingSource> GetExtensionsAssignedToUser(
      IVssRequestContext requestContext,
      Guid userId);

    IDictionary<Guid, IList<ExtensionSource>> GetExtensionsAssignedToUsers(
      IVssRequestContext requestContext,
      IList<Guid> userIds);

    IDictionary<Guid, ExtensionAssignmentDetails> GetExtensionStatusForUsers(
      IVssRequestContext requestContext,
      string extensionId);

    IEnumerable<AccountLicenseExtensionUsage> GetExtensionLicenseUsage(
      IVssRequestContext requestContext);

    IList<Guid> GetEligibleUsersForExtension(
      IVssRequestContext requestContext,
      string extensionId,
      ExtensionFilterOptions options);

    void TransferExtensionsForIdentities(
      IVssRequestContext requestContext,
      IList<Tuple<Microsoft.VisualStudio.Services.Identity.Identity, Microsoft.VisualStudio.Services.Identity.Identity>> identityMapping);

    void EvaluateExtensionAssignmentsOnAccessLevelChange(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      License license);
  }
}
