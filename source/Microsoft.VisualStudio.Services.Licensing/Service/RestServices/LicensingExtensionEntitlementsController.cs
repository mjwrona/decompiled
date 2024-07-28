// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Service.RestServices.LicensingExtensionEntitlementsController
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Licensing.Http;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Licensing.Service.RestServices
{
  [ControllerApiVersion(1.0)]
  public class LicensingExtensionEntitlementsController : LicensingApiController
  {
    private readonly ServiceFactory<IExtensionEntitlementService> m_extensionEntitlementServiceFactory;

    public LicensingExtensionEntitlementsController()
      : this((ServiceFactory<IExtensionEntitlementService>) (x => x.GetService<IExtensionEntitlementService>()))
    {
    }

    internal LicensingExtensionEntitlementsController(
      ServiceFactory<IExtensionEntitlementService> extensionEntitlementServiceFactory)
    {
      this.m_extensionEntitlementServiceFactory = extensionEntitlementServiceFactory;
    }

    [HttpPut]
    [ActionName("user")]
    [TraceFilterWithException(1040000, 1040009, 1040008)]
    [ClientLocationId("8CEC75EA-044F-4245-AB0D-A82DAFCC85EA")]
    public IEnumerable<ExtensionOperationResult> AssignExtensionToUsers([FromBody] ExtensionAssignment body)
    {
      ICollection<ExtensionOperationResult> users = this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).AssignExtensionToUsers(this.TfsRequestContext, body.ExtensionGalleryId, body.UserIds, body.IsAutoAssignment);
      IDictionary<Guid, Guid> localIds = LicensingExtensionEntitlementsController.ConvertMasterIdsToLocalIds(this.TfsRequestContext, (IList<Guid>) users.Select<ExtensionOperationResult, Guid>((Func<ExtensionOperationResult, Guid>) (r => r.UserId)).ToList<Guid>());
      foreach (ExtensionOperationResult extensionOperationResult in (IEnumerable<ExtensionOperationResult>) users)
        extensionOperationResult.UserId = localIds[extensionOperationResult.UserId];
      return (IEnumerable<ExtensionOperationResult>) users ?? Enumerable.Empty<ExtensionOperationResult>();
    }

    [HttpGet]
    [ActionName("user")]
    [TraceFilterWithException(1040010, 1040019, 1040018)]
    [ClientLocationId("8CEC75EA-044F-4245-AB0D-A82DAFCC85EA")]
    public IDictionary<string, LicensingSource> GetExtensionsAssignedToUser(Guid userId) => this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).GetExtensionsAssignedToUser(this.TfsRequestContext, userId) ?? (IDictionary<string, LicensingSource>) new Dictionary<string, LicensingSource>();

    [HttpDelete]
    [ActionName("user")]
    [TraceFilterWithException(1040020, 1040029, 1040028)]
    [ClientLocationId("8CEC75EA-044F-4245-AB0D-A82DAFCC85EA")]
    [ClientIgnore]
    public IEnumerable<ExtensionOperationResult> UnassignExtensionFromUsers([FromBody] ExtensionAssignment body)
    {
      ICollection<ExtensionOperationResult> source = this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).UnassignExtensionFromUsers(this.TfsRequestContext, body.ExtensionGalleryId, body.UserIds, body.LicensingSource);
      IDictionary<Guid, Guid> localIds = LicensingExtensionEntitlementsController.ConvertMasterIdsToLocalIds(this.TfsRequestContext, (IList<Guid>) source.Select<ExtensionOperationResult, Guid>((Func<ExtensionOperationResult, Guid>) (r => r.UserId)).ToList<Guid>());
      foreach (ExtensionOperationResult extensionOperationResult in (IEnumerable<ExtensionOperationResult>) source)
        extensionOperationResult.UserId = localIds[extensionOperationResult.UserId];
      return (IEnumerable<ExtensionOperationResult>) source ?? Enumerable.Empty<ExtensionOperationResult>();
    }

    [HttpPut]
    [ActionName("AllUsers")]
    [TraceFilterWithException(1040030, 1040039, 1040038)]
    [ClientLocationId("5434F182-7F32-4135-8326-9340D887C08A")]
    public IEnumerable<ExtensionOperationResult> AssignExtensionToAllEligibleUsers(
      string extensionId)
    {
      ICollection<ExtensionOperationResult> allEligibleUsers = this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).AssignExtensionToAllEligibleUsers(this.TfsRequestContext, extensionId);
      IDictionary<Guid, Guid> localIds = LicensingExtensionEntitlementsController.ConvertMasterIdsToLocalIds(this.TfsRequestContext, (IList<Guid>) allEligibleUsers.Select<ExtensionOperationResult, Guid>((Func<ExtensionOperationResult, Guid>) (r => r.UserId)).ToList<Guid>());
      foreach (ExtensionOperationResult extensionOperationResult in (IEnumerable<ExtensionOperationResult>) allEligibleUsers)
        extensionOperationResult.UserId = localIds[extensionOperationResult.UserId];
      return (IEnumerable<ExtensionOperationResult>) allEligibleUsers ?? Enumerable.Empty<ExtensionOperationResult>();
    }

    [HttpGet]
    [ActionName("AllUsers")]
    [TraceFilterWithException(1040040, 1040049, 1040048)]
    [ClientLocationId("5434F182-7F32-4135-8326-9340D887C08A")]
    public IEnumerable<Guid> GetEligibleUsersForExtension(
      string extensionId,
      ExtensionFilterOptions options)
    {
      return (IEnumerable<Guid>) LicensingExtensionEntitlementsController.ConvertMasterIdsToLocalIds(this.TfsRequestContext, this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).GetEligibleUsersForExtension(this.TfsRequestContext, extensionId, options)).Values;
    }

    [HttpGet]
    [ActionName("AllUsers")]
    [TraceFilterWithException(1040041, 1040043, 1040042)]
    [ClientLocationId("5434F182-7F32-4135-8326-9340D887C08A")]
    public IDictionary<Guid, ExtensionAssignmentDetails> GetExtensionStatusForUsers(
      string extensionId)
    {
      IDictionary<Guid, ExtensionAssignmentDetails> extensionEntitlements = this.m_extensionEntitlementServiceFactory(this.TfsRequestContext).GetExtensionStatusForUsers(this.TfsRequestContext, extensionId);
      return (IDictionary<Guid, ExtensionAssignmentDetails>) this.TfsRequestContext.GetService<IdentityService>().ReadIdentitiesWithFallback(this.TfsRequestContext, (IEnumerable<Guid>) extensionEntitlements.Keys.ToList<Guid>()).Select<Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<Guid, ExtensionAssignmentDetails>>((Func<Microsoft.VisualStudio.Services.Identity.Identity, KeyValuePair<Guid, ExtensionAssignmentDetails>>) (x =>
      {
        Guid key = x.EnterpriseStorageKey(this.TfsRequestContext);
        return new KeyValuePair<Guid, ExtensionAssignmentDetails>(x.Id, extensionEntitlements[key]);
      })).ToDictionary<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid, ExtensionAssignmentDetails>((Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, Guid>) (x => x.Key), (Func<KeyValuePair<Guid, ExtensionAssignmentDetails>, ExtensionAssignmentDetails>) (x => x.Value));
    }

    private static IDictionary<Guid, Guid> ConvertMasterIdsToLocalIds(
      IVssRequestContext context,
      IList<Guid> userIds)
    {
      context.CheckProjectCollectionRequestContext();
      Guid[] array1 = userIds.Distinct<Guid>().ToArray<Guid>();
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Identity.Identity[] array2 = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<Guid>) array1, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)).ToArray<Microsoft.VisualStudio.Services.Identity.Identity>();
      IVssRequestContext collectionRequestContext = context.To(TeamFoundationHostType.ProjectCollection);
      IGraphIdentifierConversionService graphIdentifierConversionService = collectionRequestContext.GetService<IGraphIdentifierConversionService>();
      return (IDictionary<Guid, Guid>) ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) array2).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.Id != Guid.Empty)).Select(identity => new
      {
        Id = identity.Id,
        LocalId = graphIdentifierConversionService.GetStorageKeyByDescriptor(collectionRequestContext, identity.SubjectDescriptor)
      }).Where(x => x.LocalId != Guid.Empty).ToDictionary(x => x.Id, x => x.LocalId);
    }
  }
}
