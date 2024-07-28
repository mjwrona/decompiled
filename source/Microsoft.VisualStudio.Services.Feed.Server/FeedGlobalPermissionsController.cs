// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedGlobalPermissionsController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Service Settings")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "GlobalPermissions")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedGlobalPermissionsController : ResourcePermissionsControllerBase<object>
  {
    private static readonly string RootSecurityToken = "$/";

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) FeedApiController.HttpExceptionMapping;

    public override string TraceArea => FeedApiController.Area;

    public override string ActivityLogArea => FeedApiController.Area;

    [HttpPatch]
    [ClientSwaggerOperationId("SetGlobalPermissions")]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public IEnumerable<GlobalPermission> SetGlobalPermissions(
      IEnumerable<GlobalPermission> globalPermissions)
    {
      if (!(globalPermissions is IReadOnlyList<GlobalPermission> globalPermissionList))
        globalPermissionList = (IReadOnlyList<GlobalPermission>) globalPermissions.ToList<GlobalPermission>();
      IReadOnlyList<GlobalPermission> globalPermission = globalPermissionList;
      FeedGlobalPermissionsController.ValidateGlobalPermissions((IEnumerable<GlobalPermission>) globalPermission);
      IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> queryable = this.SetPermissions((object) null, this.GetAccessControlEntriesToUpdate((IEnumerable<GlobalPermission>) globalPermission), false);
      List<GlobalPermission> list1 = this.GetGlobalPermissionsToDelete((IEnumerable<GlobalPermission>) globalPermission).ToList<GlobalPermission>();
      List<IdentityDescriptor> list2 = list1.Select<GlobalPermission, IdentityDescriptor>((Func<GlobalPermission, IdentityDescriptor>) (i => i.IdentityDescriptor)).ToList<IdentityDescriptor>();
      if (list2.Any<IdentityDescriptor>() && !this.RemovePermissions((object) null, (IEnumerable<IdentityDescriptor>) list2))
        throw new PermissionRemovalFailedException();
      FeedCiPublisher.PublishSetGlobalCreateFeedPermissionEvent(this.TfsRequestContext, this.ConvertAcesToGlobalPermissions((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) queryable).Union<GlobalPermission>((IEnumerable<GlobalPermission>) list1));
      return this.GetGlobalPermissions();
    }

    [HttpGet]
    [ClientSwaggerOperationId("GetGlobalPermissions")]
    public IEnumerable<GlobalPermission> GetGlobalPermissions() => this.GetGlobalPermissions(false);

    [HttpGet]
    [ClientSwaggerOperationId("GetGlobalPermissions")]
    public IEnumerable<GlobalPermission> GetGlobalPermissions(bool includeIds) => this.ConvertAcesToGlobalPermissions((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) this.QueryPermissions((object) null, (IEnumerable<IdentityDescriptor>) null), includeIds);

    internal static void ValidateGlobalPermissions(IEnumerable<GlobalPermission> globalPermission)
    {
      if (globalPermission.Any<GlobalPermission>((Func<GlobalPermission, bool>) (x => x.Role == GlobalRole.Custom)))
        throw new PermissionInvalidRoleException();
    }

    protected override Guid GetSecurityNamespace() => FeedConstants.FeedSecurityNamespaceId;

    protected override string CreateSecurityToken(object obj) => FeedGlobalPermissionsController.RootSecurityToken;

    private IEnumerable<GlobalPermission> ConvertAcesToGlobalPermissions(
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> accessControlEntries,
      bool includeIds = false)
    {
      List<GlobalPermission> globalPermissionList = new List<GlobalPermission>();
      IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> identities = includeIds ? FeedPermissionsHelper.GetIdentitiesFromAces(this.TfsRequestContext, accessControlEntries) : (IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) new Dictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>();
      bool isGlobalPermMappedIdentifiersEnabled = this.TfsRequestContext.IsFeatureEnabled("Packaging.Feed.GlobalPermissionsMapWellKnownIdentifier");
      return accessControlEntries.Select<Microsoft.VisualStudio.Services.Security.AccessControlEntry, GlobalPermission>((Func<Microsoft.VisualStudio.Services.Security.AccessControlEntry, GlobalPermission>) (accessControlEntry => CreateGlobalPermissionFromAce(accessControlEntry)));

      static GlobalRole GetRoleForAce(Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry)
      {
        if (accessControlEntry.Deny != 0)
          return GlobalRole.Custom;
        switch ((FeedPermissionConstants) (accessControlEntry.Allow & 3583))
        {
          case FeedPermissionConstants.None:
            return GlobalRole.None;
          case FeedPermissionConstants.CreateFeed:
            return GlobalRole.FeedCreator;
          case FeedPermissionConstants.AdministerFeed | FeedPermissionConstants.ArchiveFeed | FeedPermissionConstants.DeleteFeed | FeedPermissionConstants.CreateFeed | FeedPermissionConstants.EditFeed | FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DeletePackage | FeedPermissionConstants.DelistPackage | FeedPermissionConstants.AddUpstreamPackage:
            return GlobalRole.Administrator;
          default:
            return GlobalRole.Custom;
        }
      }

      GlobalPermission CreateGlobalPermissionFromAce(Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry)
      {
        GlobalPermission permissionFromAce = new GlobalPermission()
        {
          Role = GetRoleForAce(accessControlEntry),
          IdentityDescriptor = GetIdentityDescriptorForAce(accessControlEntry)
        };
        Microsoft.VisualStudio.Services.Identity.Identity identity;
        if (includeIds && (identities.TryGetValue(permissionFromAce.IdentityDescriptor, out identity) || identities.TryGetValue(accessControlEntry.Descriptor, out identity)))
          permissionFromAce.IdentityId = new Guid?(identity.Id);
        return permissionFromAce;
      }

      IdentityDescriptor GetIdentityDescriptorForAce(Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry) => isGlobalPermMappedIdentifiersEnabled ? IdentityDomain.MapFromWellKnownIdentifier(this.TfsRequestContext.ServiceHost.InstanceId, accessControlEntry.Descriptor) : accessControlEntry.Descriptor;
    }

    private IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> GetAccessControlEntriesToUpdate(
      IEnumerable<GlobalPermission> globalPermissions)
    {
      List<Microsoft.VisualStudio.Services.Security.AccessControlEntry> controlEntriesToUpdate = new List<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
      foreach (GlobalPermission globalPermission in globalPermissions)
      {
        FeedPermissionConstants allow;
        switch (globalPermission.Role)
        {
          case GlobalRole.Custom:
            throw new PermissionInvalidRoleException();
          case GlobalRole.None:
            continue;
          case GlobalRole.FeedCreator:
            allow = FeedPermissionConstants.CreateFeed;
            break;
          case GlobalRole.Administrator:
            allow = FeedPermissionConstants.AdministerFeed | FeedPermissionConstants.ArchiveFeed | FeedPermissionConstants.DeleteFeed | FeedPermissionConstants.CreateFeed | FeedPermissionConstants.EditFeed | FeedPermissionConstants.ReadPackages | FeedPermissionConstants.AddPackage | FeedPermissionConstants.UpdatePackage | FeedPermissionConstants.DeletePackage | FeedPermissionConstants.DelistPackage | FeedPermissionConstants.AddUpstreamPackage;
            break;
          default:
            throw new ArgumentOutOfRangeException();
        }
        controlEntriesToUpdate.Add(new Microsoft.VisualStudio.Services.Security.AccessControlEntry(globalPermission.IdentityDescriptor, (int) allow, 0, (AceExtendedInformation) null));
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) controlEntriesToUpdate;
    }

    private IEnumerable<GlobalPermission> GetGlobalPermissionsToDelete(
      IEnumerable<GlobalPermission> globalPermissions)
    {
      return (IEnumerable<GlobalPermission>) globalPermissions.Where<GlobalPermission>((Func<GlobalPermission, bool>) (x => x.Role == GlobalRole.None)).ToList<GlobalPermission>();
    }
  }
}
