// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.FeedPermissionsController
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.Server.MessageBus;
using Microsoft.VisualStudio.Services.Feed.Server.Telemetry;
using Microsoft.VisualStudio.Services.Feed.Server.Utils;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("Feed Management")]
  [VersionedApiControllerCustomName(Area = "Packaging", ResourceName = "Permissions")]
  [FeatureEnabled("Packaging.Feed.Service")]
  public class FeedPermissionsController : ProjectResourcePermissionsControllerBase<Microsoft.VisualStudio.Services.Feed.WebApi.Feed>
  {
    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) FeedApiController.HttpExceptionMapping;

    public override string TraceArea => FeedApiController.Area;

    public override string ActivityLogArea => FeedApiController.Area;

    [HttpPatch]
    [RequestContentTypeRestriction(AllowJsonPatch = false)]
    public IEnumerable<FeedPermission> SetFeedPermissions(
      string feedId,
      IEnumerable<FeedPermission> feedPermission)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      string feedIdOrName = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed fromFeedIdOrName = this.GetFeedFromFeedIdOrName(feedIdOrName, projectReference);
      FeedPermissionsController.ValidatePermissions(feedPermission, fromFeedIdOrName.View != null);
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> controlEntriesToUpdate = FeedPermissionsController.GetAccessControlEntriesToUpdate(feedPermission);
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> accessControlEntries = Enumerable.Empty<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
      if (controlEntriesToUpdate.Any<Microsoft.VisualStudio.Services.Security.AccessControlEntry>())
        accessControlEntries = (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) this.SetPermissions(fromFeedIdOrName, controlEntriesToUpdate, false);
      IEnumerable<FeedPermission> permissionForDelete = this.GetFeedPermissionForDelete(feedPermission);
      if (permissionForDelete.Any<FeedPermission>())
      {
        IEnumerable<IdentityDescriptor> identities = permissionForDelete.Select<FeedPermission, IdentityDescriptor>((Func<FeedPermission, IdentityDescriptor>) (i => i.IdentityDescriptor));
        if (!this.RemovePermissions(fromFeedIdOrName, identities))
          throw new PermissionRemovalFailedException();
      }
      IEnumerable<FeedPermission> feedPermissions = this.ConvertAcesToFeedPermissions(accessControlEntries).Union<FeedPermission>(permissionForDelete);
      this.PublishFeedChangedEvent(this.TfsRequestContext, fromFeedIdOrName);
      FeedCiPublisher.PublishSetFeedPermissionEvent(this.TfsRequestContext, fromFeedIdOrName.Id, feedPermissions);
      FeedAuditHelper.AuditFeedPermissionsChanges(this.TfsRequestContext, fromFeedIdOrName, feedPermission);
      return feedPermissions;
    }

    [HttpGet]
    public IEnumerable<FeedPermission> GetFeedPermissions(
      string feedId,
      bool includeIds = false,
      bool excludeInheritedPermissions = false,
      string identityDescriptor = null,
      bool includeDeletedFeeds = false)
    {
      feedId.ThrowIfNullOrWhitespace((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedId))));
      string feedIdOrName = feedId;
      ProjectInfo projectInfo = this.ProjectInfo;
      ProjectReference projectReference = projectInfo != null ? projectInfo.ToProjectReference() : (ProjectReference) null;
      int num = includeDeletedFeeds ? 1 : 0;
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed fromFeedIdOrName = this.GetFeedFromFeedIdOrName(feedIdOrName, projectReference, num != 0);
      if (FeedSecurityHelper.HasReadFeedPermissions(this.TfsRequestContext, (FeedCore) fromFeedIdOrName, true))
      {
        IList<IdentityDescriptor> descriptorList = (IList<IdentityDescriptor>) null;
        if (!string.IsNullOrEmpty(identityDescriptor))
          descriptorList = IdentityParser.GetDescriptorsFromString(identityDescriptor);
        return this.GetFeedPermissions(this.TfsRequestContext, includeIds, excludeInheritedPermissions, fromFeedIdOrName, descriptorList);
      }
      FeedSecurityHelper.ThrowFeedNeedsPermissionException(this.TfsRequestContext, System.Enum.GetName(typeof (FeedPermissionConstants), (object) 32));
      return (IEnumerable<FeedPermission>) null;
    }

    private IEnumerable<FeedPermission> GetFeedPermissions(
      IVssRequestContext requestContext,
      bool includeIds,
      bool excludeInheritedPermissions,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IList<IdentityDescriptor> descriptorList)
    {
      return this.ConvertAcesToFeedPermissions((IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) this.QueryPermissions(requestContext.Elevate(), feed, (IEnumerable<IdentityDescriptor>) descriptorList, !excludeInheritedPermissions), includeIds);
    }

    internal static void ValidatePermissions(
      IEnumerable<FeedPermission> feedPermission,
      bool isView)
    {
      feedPermission.ThrowIfNull<IEnumerable<FeedPermission>>((Func<Exception>) (() => (Exception) InvalidUserInputException.Create(nameof (feedPermission))));
      if (!(feedPermission is IList<FeedPermission> source))
        source = (IList<FeedPermission>) feedPermission.ToList<FeedPermission>();
      if (source.Any<FeedPermission>((Func<FeedPermission, bool>) (x => x.Role == FeedRole.Custom)))
        throw new PermissionInvalidRoleException();
      if (isView && feedPermission.Any<FeedPermission>((Func<FeedPermission, bool>) (x => x.Role != FeedRole.None && x.Role != FeedRole.Reader)))
        throw new ViewInvalidRoleException();
    }

    internal static IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> GetAccessControlEntriesToUpdate(
      IEnumerable<FeedPermission> feedPermission)
    {
      if (!(feedPermission is IList<FeedPermission> source))
        source = (IList<FeedPermission>) feedPermission.ToList<FeedPermission>();
      List<FeedPermission> list = source.Where<FeedPermission>((Func<FeedPermission, bool>) (x => x.Role != FeedRole.None && x.Role != 0)).ToList<FeedPermission>();
      List<Microsoft.VisualStudio.Services.Security.AccessControlEntry> controlEntriesToUpdate = new List<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
      foreach (FeedPermission feedPermission1 in list)
      {
        Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry = new Microsoft.VisualStudio.Services.Security.AccessControlEntry();
        accessControlEntry.Descriptor = feedPermission1.IdentityDescriptor;
        switch (feedPermission1.Role)
        {
          case FeedRole.Reader:
            accessControlEntry.Allow = 32;
            break;
          case FeedRole.Contributor:
            accessControlEntry.Allow = 3296;
            break;
          case FeedRole.Administrator:
            accessControlEntry.Allow = 3575;
            break;
          case FeedRole.Collaborator:
            accessControlEntry.Allow = 2080;
            break;
        }
        controlEntriesToUpdate.Add(accessControlEntry);
      }
      return (IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>) controlEntriesToUpdate;
    }

    protected override string CreateSecurityToken(Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed) => FeedSecurityHelper.CalculateSecurityToken((FeedCore) feed);

    protected override Guid GetSecurityNamespace() => FeedConstants.FeedSecurityNamespaceId;

    private Microsoft.VisualStudio.Services.Feed.WebApi.Feed GetFeedFromFeedIdOrName(
      string feedIdOrName,
      ProjectReference project,
      bool includeDeleted = false)
    {
      return this.TfsRequestContext.GetService<IFeedService>().GetFeed(this.TfsRequestContext, feedIdOrName, project, includeDeleted);
    }

    private IEnumerable<FeedPermission> ConvertAcesToFeedPermissions(
      IEnumerable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> accessControlEntries,
      bool includeIdentityIds = false)
    {
      List<FeedPermission> feedPermissions = new List<FeedPermission>();
      IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity> identitiesFromAces = includeIdentityIds ? FeedPermissionsHelper.GetIdentitiesFromAces(this.TfsRequestContext, accessControlEntries) : (IDictionary<IdentityDescriptor, Microsoft.VisualStudio.Services.Identity.Identity>) null;
      foreach (Microsoft.VisualStudio.Services.Security.AccessControlEntry accessControlEntry in accessControlEntries)
      {
        FeedPermission feedPermission = FeedPermissionsHelper.ConvertAceToFeedPermission(accessControlEntry);
        feedPermission.IdentityDescriptor = IdentityDomain.MapFromWellKnownIdentifier(this.TfsRequestContext.ServiceHost.InstanceId, feedPermission.IdentityDescriptor);
        if (includeIdentityIds)
        {
          if (identitiesFromAces != null && identitiesFromAces.ContainsKey(feedPermission.IdentityDescriptor))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identitiesFromAces[feedPermission.IdentityDescriptor];
            feedPermission.IdentityId = new Guid?(identity.Id);
            feedPermission.DisplayName = identity.DisplayName;
          }
          else if (identitiesFromAces != null && identitiesFromAces.ContainsKey(accessControlEntry.Descriptor))
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identitiesFromAces[accessControlEntry.Descriptor];
            feedPermission.IdentityId = new Guid?(identity.Id);
            feedPermission.DisplayName = identity.DisplayName;
          }
        }
        feedPermissions.Add(feedPermission);
      }
      feedPermissions.Sort((Comparison<FeedPermission>) ((x, y) => y.Role.CompareTo((object) x.Role)));
      return (IEnumerable<FeedPermission>) feedPermissions;
    }

    private IEnumerable<FeedPermission> GetFeedPermissionForDelete(
      IEnumerable<FeedPermission> feedPermission)
    {
      if (!(feedPermission is IList<FeedPermission> source))
        source = (IList<FeedPermission>) feedPermission.ToList<FeedPermission>();
      return (IEnumerable<FeedPermission>) source.Where<FeedPermission>((Func<FeedPermission, bool>) (x => x.Role == FeedRole.None)).ToList<FeedPermission>();
    }

    private IQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry> QueryPermissions(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed identifier,
      IEnumerable<IdentityDescriptor> identities,
      bool includeExtendedInfo)
    {
      string securityToken = this.CreateSecurityToken(identifier);
      Guid securityNamespace = this.GetSecurityNamespace();
      return SecurityConverter.Convert((this.TfsRequestContext.GetService<ISecuredTeamFoundationSecurityService>().GetSecurityNamespace(this.TfsRequestContext, securityNamespace) ?? throw new InvalidSecurityNamespaceException(securityNamespace)).QueryAccessControlList(requestContext, securityToken, identities, includeExtendedInfo).AccessControlEntries).AsQueryable<Microsoft.VisualStudio.Services.Security.AccessControlEntry>();
    }

    private void PublishFeedChangedEvent(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed)
    {
      if (feed.View != null)
      {
        List<FeedPermission> list = this.GetFeedPermissions(requestContext, false, true, feed, (IList<IdentityDescriptor>) null).ToList<FeedPermission>();
        FeedChangedPublisher.ViewPermissionChanged(requestContext, feed, list);
      }
      else
      {
        List<FeedPermission> list = this.GetFeedPermissions(requestContext, false, false, feed, (IList<IdentityDescriptor>) null).ToList<FeedPermission>();
        FeedChangedPublisher.FeedPermissionChanged(requestContext, feed, list);
      }
    }
  }
}
