// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.FrameworkIdentityDetachHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class FrameworkIdentityDetachHandler : IdentityTransferHandler<FrameworkIdentityStore>
  {
    private List<IdentityScope> m_scopes;
    private List<Guid> m_identities;

    public FrameworkIdentityDetachHandler(
      IVssRequestContext requestContext,
      IServicingContext servicingContext,
      IDictionary<string, IIdentityProvider> syncAgents,
      FrameworkIdentityStore identityStore)
      : base(requestContext, servicingContext, syncAgents, identityStore)
    {
    }

    protected override void PreTransferSteps()
    {
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, executing PreTransferSteps.");
      CachedRegistryService service = this.RequestContext.GetService<CachedRegistryService>();
      service.SetValue<string>(this.RequestContext, FrameworkServerConstants.SnapshotCollectionDomainSid, this.HostDomain.DomainSid);
      service.SetValue<string>(this.RequestContext, FrameworkServerConstants.SnapshotInstanceDomainSid, this.IdentityStore.Domain.DomainSid);
      using (IdentityManagementComponent component = this.RequestContext.CreateComponent<IdentityManagementComponent>())
        component.Install();
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, PreTransferSteps complete.");
    }

    protected override void PostTransferSteps()
    {
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, executing PostTransferSteps.");
      IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Application);
      FrameworkIdentityService service1 = vssRequestContext.GetService<FrameworkIdentityService>();
      this.ServicingContext.LogInfo("Reading all identities' extended properties.");
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = service1.ReadIdentities(vssRequestContext, (IList<Guid>) this.m_identities, QueryMembership.None, (IEnumerable<string>) new string[1]
      {
        "*"
      }, false);
      List<Microsoft.VisualStudio.Services.Identity.Identity> identitiesToMap = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      this.ServicingContext.Items[ServicingItemConstants.ExportIdentities] = (object) identitiesToMap;
      Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper identityPropertyHelper = new Microsoft.VisualStudio.Services.Identity.IdentityPropertyHelper();
      ArtifactKind artifactKind = new ArtifactKind();
      artifactKind.Kind = ArtifactKinds.Identity;
      artifactKind.Description = "Identity";
      artifactKind.DataspaceCategory = "Default";
      artifactKind.IsInternalKind = true;
      artifactKind.IsMonikerBased = false;
      TeamFoundationPropertyService service2 = this.RequestContext.GetService<TeamFoundationPropertyService>();
      try
      {
        this.ServicingContext.LogInfo("Creating Identity artifact kind in the collection.");
        service2.CreateArtifactKind(this.RequestContext, artifactKind);
      }
      catch (ArtifactKindAlreadyExistsException ex)
      {
        this.ServicingContext.LogInfo("Identity artifact kind already existed.");
      }
      this.ServicingContext.LogInfo("Migrating extended properties into the collection snapshot.");
      IdentityIdMapper identityIdMapper = new IdentityIdMapper(this.RequestContext, true);
      Guid[] masterIds = new Guid[identities.Count];
      for (int index = 0; index < masterIds.Length; ++index)
        masterIds[index] = identities[index].Id;
      Guid[] localIds = identityIdMapper.MapToLocalIds(this.RequestContext, masterIds);
      List<PropertyValue> propertyValueList = new List<PropertyValue>();
      for (int index = 0; index < masterIds.Length; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identities[index];
        propertyValueList.Clear();
        ArtifactSpec artifactSpec = identityPropertyHelper.MakeArtifactSpec(IdentityPropertyScope.Global, identity);
        Guid guid = localIds[index];
        artifactSpec.Id = guid.ToByteArray();
        foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) identity.Properties)
        {
          if (!IdentityAttributeTags.ReadOnlyProperties.Contains(property.Key))
            propertyValueList.Add(new PropertyValue(property.Key, property.Value));
        }
        if (propertyValueList.Count > 0)
          service2.SetProperties(this.RequestContext, artifactSpec, (IEnumerable<PropertyValue>) propertyValueList);
      }
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
        this.AddIdentityToCsvMapping(identitiesToMap, identity);
      this.ConvertAcsIdentities(this.RequestContext.To(TeamFoundationHostType.Deployment), identities);
      this.ServicingContext.LogInfo("Locking the Identity Snapshot. No more identity access after this point.");
      using (IdentityMapComponent component = this.RequestContext.CreateComponent<IdentityMapComponent>())
      {
        component.CommitSnapshot();
        component.LockIdentityMap();
        component.DeleteIdentityMap();
      }
      IVssSecurityNamespace securityNamespace = this.RequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(this.RequestContext, FrameworkSecurity.IdentitiesNamespaceId);
      this.ServicingContext.LogInfo("Getting Identity ACLs from SPS.");
      List<IAccessControlList> accessControlListList = new List<IAccessControlList>();
      foreach (IdentityScope scope in this.m_scopes)
        accessControlListList.AddRange(securityNamespace.QueryAccessControlLists(this.RequestContext, scope.LocalScopeId.ToString("D"), (IEnumerable<IdentityDescriptor>) null, false, true));
      List<DatabaseAccessControlEntry> accessControlEntryList = new List<DatabaseAccessControlEntry>();
      foreach (IAccessControlList accessControlList in accessControlListList)
      {
        foreach (IAccessControlEntry accessControlEntry in accessControlList.AccessControlEntries)
        {
          Microsoft.VisualStudio.Services.Identity.Identity readIdentity = service1.ReadIdentities(this.RequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
          {
            accessControlEntry.Descriptor
          }, QueryMembership.None, (IEnumerable<string>) null, false)[0];
          if (readIdentity != null && !ServicePrincipals.IsServicePrincipal(this.RequestContext, readIdentity.Descriptor) && !IdentityHelper.IsServiceIdentity(this.RequestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) readIdentity))
            accessControlEntryList.Add(new DatabaseAccessControlEntry(readIdentity.Id, accessControlList.Token, accessControlEntry.Allow, accessControlEntry.Deny, false));
        }
      }
      this.ServicingContext.LogInfo("Writing Identity ACEs locally.");
      using (SecurityComponent component = this.RequestContext.CreateComponent<SecurityComponent>())
        component.SetAccessControlLists(FrameworkSecurity.IdentitiesNamespaceId, (IEnumerable<IAccessControlList>) accessControlListList, (IEnumerable<DatabaseAccessControlEntry>) accessControlEntryList, FrameworkSecurity.IdentitySecurityPathSeparator, SecurityNamespaceStructure.Hierarchical);
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, PostTransferSteps complete.");
    }

    private void ConvertAcsIdentities(
      IVssRequestContext deploymentRequestContext,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      {
        if (ServicePrincipals.IsServicePrincipal(deploymentRequestContext, identity.Descriptor) || IdentityHelper.IsServiceIdentity(deploymentRequestContext, (IReadOnlyVssIdentity) identity))
        {
          using (DetachedHostedCollectionComponent component = this.RequestContext.CreateComponent<DetachedHostedCollectionComponent>(connectionType: new DatabaseConnectionType?(DatabaseConnectionType.Dbo)))
            component.ChangeIdentityType(identity.Descriptor, "Microsoft.TeamFoundation.UnauthenticatedIdentity", this.RequestContext.ServiceHost.PartitionId);
        }
      }
    }

    private void AddIdentityToCsvMapping(List<Microsoft.VisualStudio.Services.Identity.Identity> identitiesToMap, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      IVssRequestContext requestContext = this.RequestContext.To(TeamFoundationHostType.Deployment);
      if (identity.Descriptor.IdentityType != "Microsoft.TeamFoundation.Identity" && identity.Descriptor.IdentityType != "Microsoft.TeamFoundation.UnauthenticatedIdentity" && !IdentityHelper.IsServiceIdentity(requestContext, (IReadOnlyVssIdentity) identity) && !ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor))
        identitiesToMap.Add(identity);
      else
        this.ServicingContext.LogInfo("Not adding identity to csv mapping list: {0}, {1}", (object) identity.DisplayName, (object) identity.Descriptor);
    }

    protected override void ReadSnapshotFromSource(
      out IEnumerable<IdentityScope> scopes,
      out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      out IEnumerable<GroupMembership> memberships,
      out IEnumerable<Guid> identityIds)
    {
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, executing ReadSnapshotFromSource.");
      IdentitySnapshot identitySnapshot = this.IdentityStore.GetHttpClient(this.RequestContext).GetIdentitySnapshotAsync(this.HostDomain.DomainId, (object) null, new CancellationToken()).SyncResult<IdentitySnapshot>();
      scopes = (IEnumerable<IdentityScope>) (this.m_scopes = identitySnapshot.Scopes);
      groups = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identitySnapshot.Groups;
      memberships = (IEnumerable<GroupMembership>) identitySnapshot.Memberships;
      identityIds = (IEnumerable<Guid>) (this.m_identities = identitySnapshot.IdentityIds);
      foreach (IdentityScope identityScope in scopes)
      {
        if (identityScope.ScopeType == GroupScopeType.Generic && string.Equals(identityScope.Name, "DefaultCollection", StringComparison.OrdinalIgnoreCase))
        {
          identityScope.ScopeType = GroupScopeType.ServiceHost;
          identityScope.SecuringHostId = this.RequestContext.ServiceHost.InstanceId;
        }
        else if (identityScope.ScopeType == GroupScopeType.TeamProject)
          identityScope.SecuringHostId = this.RequestContext.ServiceHost.InstanceId;
      }
      IList<Microsoft.VisualStudio.Services.Identity.Identity> source = this.ReadIdentitiesFromSource((IList<Guid>) identityIds.ToList<Guid>());
      foreach (GroupMembership groupMembership1 in memberships)
      {
        GroupMembership groupMembership = groupMembership1;
        Microsoft.VisualStudio.Services.Identity.Identity identity = source.FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null && i.Id == groupMembership.Id));
        if (identity != null && (IdentityHelper.IsServiceIdentity(this.RequestContext.To(TeamFoundationHostType.Deployment), (IReadOnlyVssIdentity) identity) || ServicePrincipals.IsServicePrincipal(this.RequestContext, identity.Descriptor)))
          groupMembership.Active = false;
      }
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, ReadSnapshotFromSource complete.");
    }

    protected override void SaveSnapshotInTarget(
      IEnumerable<IdentityScope> scopes,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      IEnumerable<GroupMembership> memberships)
    {
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, executing SaveSnapshotInTarget.");
      using (GroupComponent component = this.RequestContext.CreateComponent<GroupComponent>())
        component.SaveSnapshot(Guid.Empty, scopes, groups.Select<Microsoft.VisualStudio.Services.Identity.Identity, GroupDescription>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GroupDescription>) (group => GroupDescription.Convert(this.HostDomain.DomainId, group))), memberships);
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, SaveSnapshotInTarget complete.");
    }

    protected override bool ChangeDomainIds => false;

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IList<Guid> identityIds)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.IdentityStore.ReadIdentities(this.RequestContext, this.IdentityStore.Domain, identityIds, QueryMembership.None, (IEnumerable<string>) null, false);
      for (int index = 0; index < identityList.Count; ++index)
      {
        if (identityList[index] == null)
          this.ServicingContext.LogInfo("A null value has been detected in the result of an identity ReadIdentitiesFromSource. This is expected if the identity is a group defined at a different scope. Identity Id {0}", (object) identityIds[index]);
      }
      return identityList;
    }

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromTarget(
      IList<IdentityDescriptor> descriptors)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[descriptors.Count];
    }

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromTarget(
      IList<Guid> identityIds)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
    }

    protected override Microsoft.VisualStudio.Services.Identity.Identity SyncIdentity(
      IdentityDescriptor descriptor,
      string displayName)
    {
      return (Microsoft.VisualStudio.Services.Identity.Identity) null;
    }

    protected override void HandleInactiveMember(
      Microsoft.VisualStudio.Services.Identity.Identity sourceIdentity,
      Microsoft.VisualStudio.Services.Identity.Identity targetIdentity,
      GroupMembership membership)
    {
    }

    protected override void UpdateIdentitiesInTarget(
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      bool favorCurrentlyActive)
    {
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, executing UpdateIdentitiesInTarget.");
      bool updateIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(this.RequestContext);
      using (IdentityManagementComponent component = this.RequestContext.CreateComponent<IdentityManagementComponent>())
        component.UpdateIdentities(identities, (HashSet<string>) null, favorCurrentlyActive, updateIdentityAudit, false, out List<Guid> _, out IdentityChangedData _, out List<KeyValuePair<Guid, Guid>> _);
      this.ServicingContext.LogInfo("FrameworkIdentityDetachHandler, UpdateIdentitiesInTarget complete.");
    }

    protected override void EnsureIdentitiesRooted(IList<Guid> identities)
    {
    }

    protected override void UpdateIdentityMap(IEnumerable<KeyValuePair<Guid, Guid>> mappings)
    {
    }

    public override IdentityDescriptor MapIfTeamFoundationType(IdentityDescriptor descriptor) => descriptor;
  }
}
