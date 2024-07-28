// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityAttachHandler
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal abstract class IdentityAttachHandler : IdentityTransferHandler<PlatformIdentityStore>
  {
    private Guid m_attachGuid;
    private IdentityDescriptor m_holdingGroupDescriptor;
    private readonly Dictionary<IdentityDescriptor, IdentityDescriptor> m_inactiveTargetGroupMap = new Dictionary<IdentityDescriptor, IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
    private HashSet<string> m_reportedSyncFailures = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    protected IdentityAttachHandler(
      IVssRequestContext requestContext,
      IServicingContext servicingContext,
      IDictionary<string, IIdentityProvider> syncAgents,
      PlatformIdentityStore identityStore)
      : base(requestContext, servicingContext, syncAgents, identityStore)
    {
      this.m_attachGuid = Guid.NewGuid();
    }

    protected override void PreTransferSteps()
    {
    }

    protected override void PostTransferSteps()
    {
      if (this.m_holdingGroupDescriptor != (IdentityDescriptor) null)
      {
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.ServicingContext.DeploymentRequestContext, 3600))
          groupComponent.UpdateGroups(this.HostDomain.DomainId, (IEnumerable<string>) new string[1]
          {
            this.m_holdingGroupDescriptor.Identifier
          }, (IEnumerable<GroupComponent.GroupUpdate>) null);
      }
      this.IdentityStore.ProcessIdentityChangeOnAuthor(this.ServicingContext.DeploymentRequestContext, this.IdentityStore.Domain, int.MaxValue, int.MaxValue);
    }

    protected override void SaveSnapshotInTarget(
      IEnumerable<IdentityScope> scopes,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      IEnumerable<GroupMembership> memberships)
    {
      this.ServicingContext.LogInfo("IdentityAttachHandler: Executing SaveSnapshotInTarget.");
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.ServicingContext.DeploymentRequestContext, 3600))
        groupComponent.SaveSnapshot(this.IdentityStore.Domain.DomainId, scopes, groups.Select<Microsoft.VisualStudio.Services.Identity.Identity, GroupDescription>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GroupDescription>) (group => GroupDescription.Convert(this.HostDomain.DomainId, group))), memberships);
      this.ServicingContext.LogInfo("IdentityAttachHandler: SaveSnapshotInTarget complete.");
    }

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromTarget(
      IList<IdentityDescriptor> descriptors)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(this.ServicingContext.DeploymentRequestContext, this.IdentityStore.Domain, descriptors, QueryMembership.None, false, (IEnumerable<string>) null, bypassCache: true);
    }

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromTarget(
      IList<Guid> identityIds)
    {
      this.ServicingContext.LogInfo("IdentityAttachHandler: Executing ReadIdentitiesFromTarget. Number of identityIds: {0}", (object) identityIds.Count);
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(this.ServicingContext.DeploymentRequestContext, this.IdentityStore.Domain, identityIds, QueryMembership.None, false, (IEnumerable<string>) null, bypassCache: true);
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity SyncIdentity(
      IdentityDescriptor descriptor,
      string displayName)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      IIdentityProvider identityProvider;
      if (this.SyncAgents.TryGetValue(descriptor.IdentityType, out identityProvider))
      {
        try
        {
          if (!identityProvider.TrySyncIdentity(this.RequestContext, descriptor, false, (string) null, (SyncErrors) null, out identity))
            identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          if (identity == null)
          {
            if (this.ServicingContext != null)
            {
              if (this.m_reportedSyncFailures.Add(descriptor.Identifier))
              {
                if (!string.IsNullOrEmpty(displayName))
                  this.ServicingContext.Warn(HostingResources.WarningFailedToSyncIdentityWithDisplayName((object) displayName, (object) descriptor.Identifier));
                else
                  this.ServicingContext.Warn(HostingResources.WarningFailedToSyncIdentity((object) descriptor.ToString()));
              }
            }
          }
        }
        catch (Exception ex)
        {
          identity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
          this.ServicingContext?.Warn(HostingResources.WarningExceptionSyncingIdentity((object) descriptor.ToString(), (object) ex.Message));
        }
      }
      if (identity != null && identity.IsActive)
      {
        if (identity.IsContainer)
          new IdentitySynchronizer(this.SyncAgents, this.IdentityStore, 5000, (ITFLogger) null).SyncIdentity(this.RequestContext, identity, true);
        else
          this.UpdateIdentitiesInTarget((IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
          {
            identity
          }, false);
      }
      return identity;
    }

    protected override void HandleInactiveMember(
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      GroupMembership membership)
    {
      if (targetIdentity == null)
      {
        this.UpdateIdentitiesInTarget((IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
        {
          sourceIdentity
        }, true);
        membership.Id = sourceIdentity.Id;
      }
      else
        membership.Id = targetIdentity.Id;
      if (!(sourceIdentity.Descriptor != (IdentityDescriptor) null) || string.Equals(sourceIdentity.Descriptor.IdentityType, "System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase))
        return;
      membership.Active = false;
    }

    protected override void UpdateIdentitiesInTarget(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool favorCurrentlyActive)
    {
      this.ServicingContext.LogInfo("IdentityAttachHandler: Executing UpdateIdentitiesInTarget. Number of identities: {0}", (object) identities.Count);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identities1 = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (identity != null)
        {
          if (IdentityValidation.IsTeamFoundationType(identity.Descriptor))
            identityList.Add(identity);
          else
            identities1.Add(identity);
        }
      }
      if (identityList.Count > 0)
      {
        GroupDescription[] groupDescriptionArray = new GroupDescription[identityList.Count];
        for (int index = 0; index < identityList.Count; ++index)
        {
          Microsoft.VisualStudio.Services.Identity.Identity group = identityList[index];
          GroupDescription groupDescription = GroupDescription.Convert(this.IdentityStore.Domain.DomainId, group);
          groupDescription.Name = string.Format("{0} ({1})", (object) group.DisplayName, (object) this.m_attachGuid.ToString("D"));
          groupDescription.Active = false;
          this.m_inactiveTargetGroupMap[groupDescription.Descriptor] = IdentityHelper.CreateTeamFoundationDescriptor(SidIdentityHelper.NewSid(this.IdentityStore.Domain.DomainId));
          groupDescription.Descriptor = this.m_inactiveTargetGroupMap[groupDescription.Descriptor];
          groupDescriptionArray[index] = groupDescription;
        }
        this.ServicingContext.LogInfo("Creating groups. Number of groups: {0}", (object) identityList.Count);
        this.IdentityStore.CreateGroups(this.ServicingContext.DeploymentRequestContext, this.IdentityStore.Domain, this.IdentityStore.Domain.DomainId, true, true, groupDescriptionArray);
        this.ServicingContext.LogInfo("Deleting group memberships");
        this.IdentityStore.DeleteGroupMemberships(this.ServicingContext.DeploymentRequestContext, this.IdentityStore.Domain, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, true, true);
      }
      if (identities1.Count > 0)
      {
        this.ServicingContext.LogInfo("Updating identities in the database. Number of users: {0}", (object) identities1.Count);
        this.IdentityStore.UpdateIdentitiesInDatabase(this.ServicingContext.DeploymentRequestContext, this.IdentityStore.Domain, (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identities1, false, favorCurrentlyActive: favorCurrentlyActive);
      }
      this.ServicingContext.LogInfo("IdentityAttachHandler: UpdateIdentitiesInTarget complete.");
    }

    protected IdentityDescriptor MapInactiveTargetGroupDescriptor(IdentityDescriptor descriptor)
    {
      IdentityDescriptor identityDescriptor;
      if (!this.m_inactiveTargetGroupMap.TryGetValue(descriptor, out identityDescriptor))
        identityDescriptor = descriptor;
      return identityDescriptor;
    }

    protected override void EnsureIdentitiesRooted(IList<Guid> identities)
    {
      this.EnsureHoldingGroupExists();
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.ServicingContext.DeploymentRequestContext, 3600))
        groupComponent.UpdateGroupMembership(this.HostDomain.DomainId, true, true, false, identities.Select<Guid, Tuple<IdentityDescriptor, Guid, bool>>((Func<Guid, Tuple<IdentityDescriptor, Guid, bool>>) (identity => new Tuple<IdentityDescriptor, Guid, bool>(this.m_holdingGroupDescriptor, identity, true))));
    }

    private void EnsureHoldingGroupExists()
    {
      if (!(this.m_holdingGroupDescriptor == (IdentityDescriptor) null))
        return;
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.ServicingContext.DeploymentRequestContext))
      {
        using (ResultCollection resultCollection = groupComponent.QueryGroups((IEnumerable<Guid>) new Guid[1]
        {
          this.HostDomain.DomainId
        }, false, false))
          identityList = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
      }
      string str = "IdentityAttach-" + this.HostDomain.DomainId.ToString() + "-group";
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identityList)
      {
        if (string.Equals(identity.GetProperty<string>("Account", string.Empty), str, StringComparison.OrdinalIgnoreCase))
        {
          this.m_holdingGroupDescriptor = identity.Descriptor;
          break;
        }
      }
      if (!(this.m_holdingGroupDescriptor == (IdentityDescriptor) null))
        return;
      this.m_holdingGroupDescriptor = this.IdentityStore.CreateGroups(this.RequestContext, this.IdentityStore.Domain, this.HostDomain.DomainId, false, true, new GroupDescription((IdentityDescriptor) null, str, str, hasRestrictedVisibility: true))[0];
    }
  }
}
