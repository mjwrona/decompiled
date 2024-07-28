// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDetachHandler
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityDetachHandler : IdentityTransferHandler<PlatformIdentityStore>
  {
    public IdentityDetachHandler(
      IVssRequestContext requestContext,
      IServicingContext servicingContext,
      IDictionary<string, IIdentityProvider> syncAgents,
      PlatformIdentityStore identityStore)
      : base(requestContext, servicingContext, syncAgents, identityStore)
    {
    }

    protected override bool IsExecutedBefore()
    {
      using (IdentityMapComponent component = this.RequestContext.CreateComponent<IdentityMapComponent>())
        return component.IdentityMapLocked();
    }

    protected override void PreTransferSteps()
    {
      CachedRegistryService service = this.RequestContext.GetService<CachedRegistryService>();
      service.SetValue<string>(this.RequestContext, FrameworkServerConstants.SnapshotCollectionDomainSid, this.HostDomain.DomainSid);
      service.SetValue<string>(this.RequestContext, FrameworkServerConstants.SnapshotInstanceDomainSid, this.IdentityStore.Domain.DomainSid);
      using (IdentityManagementComponent component = this.RequestContext.CreateComponent<IdentityManagementComponent>())
        component.Install();
    }

    protected override void PostTransferSteps()
    {
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = this.IdentityStore.ReadIdentitiesInScope(this.RequestContext, this.IdentityStore.Domain, this.HostDomain.DomainId);
      List<ArtifactSpec> artifactSpecList1 = new List<ArtifactSpec>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
        artifactSpecList1.Add(this.IdentityStore.PropertyHelper.MakeArtifactSpec(IdentityPropertyScope.Global, identity));
      if (artifactSpecList1.Count > 0)
      {
        ArtifactKind artifactKind = new ArtifactKind()
        {
          Kind = ArtifactKinds.Identity,
          Description = "Identity",
          DataspaceCategory = "Default",
          IsInternalKind = true,
          IsMonikerBased = false
        };
        TeamFoundationPropertyService service1 = this.RequestContext.GetService<TeamFoundationPropertyService>();
        try
        {
          service1.CreateArtifactKind(this.RequestContext, artifactKind);
        }
        catch (ArtifactKindAlreadyExistsException ex)
        {
        }
        IVssRequestContext context = this.RequestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationPropertyService service2 = context.GetService<TeamFoundationPropertyService>();
        IdentityIdMapper idMapper = new IdentityIdMapper(this.RequestContext, false);
        IVssRequestContext requestContext = context;
        List<ArtifactSpec> artifactSpecList2 = artifactSpecList1;
        using (TeamFoundationDataReader properties = service2.GetProperties(requestContext, (IEnumerable<ArtifactSpec>) artifactSpecList2, (IEnumerable<string>) null))
        {
          List<ArtifactPropertyValue> artifactPropertyValues = new List<ArtifactPropertyValue>(1000);
          foreach (ArtifactPropertyValue artifactPropertyValue in properties)
          {
            artifactPropertyValues.Add(artifactPropertyValue);
            if (artifactPropertyValues.Count == 1000)
            {
              this.MapToLocalIds(artifactPropertyValues, idMapper);
              artifactPropertyValues = new List<ArtifactPropertyValue>(1000);
            }
          }
          if (artifactPropertyValues.Count > 0)
            this.MapToLocalIds(artifactPropertyValues, idMapper);
        }
      }
      using (IdentityMapComponent component = this.RequestContext.CreateComponent<IdentityMapComponent>())
      {
        component.CommitSnapshot();
        component.DeleteIdentityMap();
        component.LockIdentityMap();
      }
    }

    private void MapToLocalIds(
      List<ArtifactPropertyValue> artifactPropertyValues,
      IdentityIdMapper idMapper)
    {
      Guid[] masterIds = new Guid[artifactPropertyValues.Count];
      for (int index = 0; index < masterIds.Length; ++index)
        masterIds[index] = new Guid(artifactPropertyValues[index].Spec.Id);
      Guid[] localIds = idMapper.MapToLocalIds(this.RequestContext, masterIds);
      for (int index = 0; index < masterIds.Length; ++index)
        artifactPropertyValues[index].Spec.Id = localIds[index].ToByteArray();
      this.RequestContext.GetService<TeamFoundationPropertyService>().SetProperties(this.RequestContext, (IEnumerable<ArtifactPropertyValue>) artifactPropertyValues);
    }

    protected override void ReadSnapshotFromSource(
      out IEnumerable<IdentityScope> scopes,
      out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      out IEnumerable<GroupMembership> memberships,
      out IEnumerable<Guid> identityIds)
    {
      this.ServicingContext.LogInfo("IdentityDetachHandler: Executing ReadSnapshotFromSource.");
      List<IdentityScope> items1;
      List<Microsoft.VisualStudio.Services.Identity.Identity> items2;
      List<GroupMembership> items3;
      List<Guid> items4;
      using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.RequestContext))
      {
        using (ResultCollection resultCollection = groupComponent.ReadSnapshot(this.HostDomain.DomainId, false))
        {
          scopes = (IEnumerable<IdentityScope>) (items1 = resultCollection.GetCurrent<IdentityScope>().Items);
          resultCollection.NextResult();
          groups = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) (items2 = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items);
          resultCollection.NextResult();
          memberships = (IEnumerable<GroupMembership>) (items3 = resultCollection.GetCurrent<GroupMembership>().Items);
          resultCollection.NextResult();
          identityIds = (IEnumerable<Guid>) (items4 = resultCollection.GetCurrent<Guid>().Items);
        }
      }
      this.ServicingContext.LogInfo("IdentityDetachHandler: ReadSnapshotFromSource complete. Scopes: {0}, Groups: {1}, Memberships: {2}, identityIds: {3}", (object) items1.Count, (object) items2.Count, (object) items3.Count, (object) items4.Count);
    }

    protected override void SaveSnapshotInTarget(
      IEnumerable<IdentityScope> scopes,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      IEnumerable<GroupMembership> memberships)
    {
      this.ServicingContext.LogInfo("IdentityDetachHandler: Executing SaveSnapshotInTarget.");
      using (GroupComponent component = this.RequestContext.CreateComponent<GroupComponent>())
        component.SaveSnapshot(Guid.Empty, scopes, groups.Select<Microsoft.VisualStudio.Services.Identity.Identity, GroupDescription>((Func<Microsoft.VisualStudio.Services.Identity.Identity, GroupDescription>) (group => GroupDescription.Convert(this.HostDomain.DomainId, group))), memberships);
      this.ServicingContext.LogInfo("IdentityDetachHandler: SaveSnapshotInTarget complete.");
    }

    protected override bool ChangeDomainIds => false;

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IList<Guid> identityIds)
    {
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.IdentityStore.ReadIdentities(this.RequestContext, this.IdentityStore.Domain, identityIds, QueryMembership.None, false, (IEnumerable<string>) null, bypassCache: true);
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
      this.ServicingContext.LogInfo("IdentityDetachHandler: Executing UpdateIdentitiesInTarget.");
      this.ServicingContext.LogInfo("Number of identities: {0}", (object) identities.Count);
      bool updateIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(this.RequestContext);
      using (IdentityManagementComponent component = this.RequestContext.CreateComponent<IdentityManagementComponent>())
        component.UpdateIdentities(identities, (HashSet<string>) null, favorCurrentlyActive, updateIdentityAudit, false, out List<Guid> _, out List<Guid> _, out IdentityChangedData _, out List<KeyValuePair<Guid, Guid>> _);
      this.ServicingContext.LogInfo("IdentityDetachHandler: UpdateIdentitiesInTarget complete.");
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
