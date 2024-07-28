// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMoveHandler
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMoveHandler : IdentityAttachHandler
  {
    private readonly IdentityDomain m_domainHelper;
    private readonly IdentityDomain m_instanceDomainHelper;
    private readonly string m_sourceDomainSid;
    private readonly string m_sourceInstanceDomainSid;
    private readonly bool m_domainSidChanged;
    private readonly bool m_instanceDomainSidChanged;
    private Guid m_originalCollectionId;
    private Guid m_originalApplicationInstanceId;
    private readonly Dictionary<string, string> m_groupMap = new Dictionary<string, string>((IEqualityComparer<string>) VssStringComparer.IdentityDescriptor);

    public IdentityMoveHandler(
      IVssRequestContext requestContext,
      IServicingContext servicingContext,
      IDictionary<string, IIdentityProvider> syncAgents,
      PlatformIdentityStore identityStore)
      : base(requestContext, servicingContext, syncAgents, identityStore)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<CachedRegistryService>().ReadEntries(requestContext, (RegistryQuery) (FrameworkServerConstants.SnapshotRoot + "/*"));
      this.m_domainHelper = new IdentityDomain(requestContext);
      this.m_sourceDomainSid = registryEntryCollection.GetValueFromPath<string>(FrameworkServerConstants.SnapshotCollectionDomainSid, this.m_domainHelper.DomainSid);
      this.m_domainSidChanged = !string.Equals(this.m_domainHelper.DomainSid, this.m_sourceDomainSid, StringComparison.OrdinalIgnoreCase);
      this.m_instanceDomainHelper = new IdentityDomain(servicingContext.DeploymentRequestContext);
      this.m_sourceInstanceDomainSid = registryEntryCollection.GetValueFromPath<string>(FrameworkServerConstants.SnapshotInstanceDomainSid, this.m_instanceDomainHelper.DomainSid);
      this.m_instanceDomainSidChanged = !string.Equals(this.m_instanceDomainHelper.DomainSid, this.m_sourceInstanceDomainSid, StringComparison.OrdinalIgnoreCase);
      this.m_originalApplicationInstanceId = registryEntryCollection.GetValueFromPath<Guid>(FrameworkServerConstants.SnapshotOriginalApplicationInstanceId, this.m_instanceDomainHelper.DomainId);
      if (servicingContext.TryGetItem<Guid>(ServicingItemConstants.OriginalCollectionId, out this.m_originalCollectionId))
        return;
      this.m_originalCollectionId = requestContext.ServiceHost.InstanceId;
    }

    protected override void PreTransferSteps()
    {
      base.PreTransferSteps();
      using (IdentityMapComponent component = this.RequestContext.CreateComponent<IdentityMapComponent>())
        component.UnlockIdentityMap();
    }

    protected override void PostTransferSteps()
    {
      base.PostTransferSteps();
      IdentityIdMapper idMapper = new IdentityIdMapper(this.RequestContext, false);
      this.ServicingContext.DeploymentRequestContext.GetService<TeamFoundationPropertyService>();
      TeamFoundationPropertyService service = this.RequestContext.GetService<TeamFoundationPropertyService>();
      TeamFoundationDataReader foundationDataReader = (TeamFoundationDataReader) null;
      try
      {
        try
        {
          foundationDataReader = service.GetProperties(this.RequestContext, ArtifactKinds.Identity, (IEnumerable<string>) null);
        }
        catch (PropertyServiceException ex)
        {
          this.ServicingContext.LogInfo("Skipping identity property restore. Snapshot does not include identity properties: " + ex.Message);
          return;
        }
        List<ArtifactPropertyValue> artifactPropertyValues = new List<ArtifactPropertyValue>(1000);
        foreach (ArtifactPropertyValue artifactPropertyValue in foundationDataReader)
        {
          artifactPropertyValues.Add(artifactPropertyValue);
          if (artifactPropertyValues.Count == 1000)
          {
            this.MapFromLocalIds(artifactPropertyValues, idMapper);
            artifactPropertyValues = new List<ArtifactPropertyValue>(1000);
          }
        }
        if (artifactPropertyValues.Count > 0)
          this.MapFromLocalIds(artifactPropertyValues, idMapper);
      }
      finally
      {
        foundationDataReader?.Dispose();
      }
      if (object.Equals((object) this.m_originalCollectionId, (object) this.HostDomain.DomainId))
        return;
      using (SecurityComponent component = this.RequestContext.CreateComponent<SecurityComponent>())
        component.RenameToken(FrameworkSecurity.IdentitiesNamespaceId, this.m_originalCollectionId.ToString(), this.HostDomain.DomainId.ToString(), false, FrameworkSecurity.IdentitySecurityPathSeparator);
    }

    protected override void OnException()
    {
      using (IdentityMapComponent component = this.RequestContext.CreateComponent<IdentityMapComponent>())
        component.LockIdentityMap();
    }

    protected override void ReadSnapshotFromSource(
      out IEnumerable<IdentityScope> scopes,
      out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      out IEnumerable<GroupMembership> memberships,
      out IEnumerable<Guid> identityIds)
    {
      this.ServicingContext.LogInfo("IdentityMoveHandler: Executing ReadSnapshotFromSource.");
      List<IdentityScope> items1;
      List<Microsoft.VisualStudio.Services.Identity.Identity> items2;
      List<GroupMembership> items3;
      using (GroupComponent component = this.RequestContext.CreateComponent<GroupComponent>())
      {
        using (ResultCollection resultCollection = component.ReadSnapshot(this.m_originalCollectionId, true))
        {
          items1 = resultCollection.GetCurrent<IdentityScope>().Items;
          resultCollection.NextResult();
          items2 = resultCollection.GetCurrent<Microsoft.VisualStudio.Services.Identity.Identity>().Items;
          resultCollection.NextResult();
          items3 = resultCollection.GetCurrent<GroupMembership>().Items;
          resultCollection.NextResult();
          identityIds = (IEnumerable<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
      Dictionary<Guid, Guid> dictionary = new Dictionary<Guid, Guid>(items1.Count);
      for (int index = 0; index < items1.Count; ++index)
      {
        IdentityScope identityScope = items1[index];
        if (identityScope.ParentId == this.m_originalCollectionId)
        {
          identityScope.ParentId = this.HostDomain.DomainId;
          identityScope.SecuringHostId = this.HostDomain.DomainId;
          Guid guid = Guid.NewGuid();
          dictionary.Add(identityScope.Id, guid);
          identityScope.Id = guid;
        }
        else if (identityScope.Id == this.m_originalCollectionId)
        {
          dictionary.Add(identityScope.Id, this.HostDomain.DomainId);
          identityScope.Id = this.HostDomain.DomainId;
          identityScope.LocalScopeId = this.HostDomain.DomainId;
          identityScope.SecuringHostId = this.HostDomain.DomainId;
        }
      }
      for (int index = 0; index < items2.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = items2[index];
        Guid property = identity.GetProperty<Guid>("ScopeId", Guid.Empty);
        Guid domainId;
        if (dictionary.TryGetValue(property, out domainId))
        {
          identity.SetProperty("ScopeId", (object) domainId);
          IdentityDomain identityDomain = new IdentityDomain(property);
          string result;
          if (new IdentityDomain(domainId).MapIfSidFromDifferentHost(identity.Descriptor.Identifier, identityDomain.DomainSid, out result))
          {
            this.m_groupMap.Add(identity.Descriptor.Identifier, result);
            identity.Descriptor.Identifier = result;
          }
        }
      }
      foreach (GroupMembership groupMembership in items3)
      {
        string str;
        if (this.m_groupMap.TryGetValue(groupMembership.Descriptor.Identifier, out str))
          groupMembership.Descriptor.Identifier = str;
      }
      scopes = (IEnumerable<IdentityScope>) items1;
      groups = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) items2;
      memberships = (IEnumerable<GroupMembership>) items3;
      this.ServicingContext.LogInfo("IdentityMoveHandler: ReadSnapshotFromSource complete. Scopes: {0}, Groups: {1}, Memberships: {2}", (object) items1.Count, (object) items2.Count, (object) items3.Count);
    }

    protected override IList<Microsoft.VisualStudio.Services.Identity.Identity> ReadIdentitiesFromSource(
      IList<Guid> identityIds)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] identityArray = new Microsoft.VisualStudio.Services.Identity.Identity[identityIds.Count];
      using (IdentityManagementComponent component = this.RequestContext.CreateComponent<IdentityManagementComponent>())
      {
        using (ResultCollection resultCollection = component.ReadIdentities((IEnumerable<IdentityDescriptor>) null, (IEnumerable<Guid>) identityIds))
        {
          foreach (IdentityManagementComponent.IdentityData identityData in resultCollection.GetCurrent<IdentityManagementComponent.IdentityData>())
          {
            identityArray[identityData.OrderId] = identityData.Identity;
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityArray[identityData.OrderId];
            if (identity != null && (string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.BindPendingIdentity", StringComparison.OrdinalIgnoreCase) || string.Equals(identity.Descriptor.IdentityType, "Microsoft.IdentityModel.Claims.ClaimsIdentity", StringComparison.OrdinalIgnoreCase)))
              identity.Descriptor.IdentityType = "Microsoft.TeamFoundation.UnauthenticatedIdentity";
          }
        }
      }
      return (IList<Microsoft.VisualStudio.Services.Identity.Identity>) identityArray;
    }

    protected override void UpdateIdentityMap(IEnumerable<KeyValuePair<Guid, Guid>> mappings)
    {
      using (IdentityMapComponent component = this.RequestContext.CreateComponent<IdentityMapComponent>())
        component.UpdateIdentityMappings2(mappings);
    }

    public override void TransformIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      if (string.Equals(identity.Descriptor.IdentityType, "Microsoft.TeamFoundation.ServiceIdentity", StringComparison.OrdinalIgnoreCase))
        this.TransformServiceIdentity(identity);
      else
        base.TransformIdentity(identity);
    }

    public override IdentityDescriptor MapIfTeamFoundationType(IdentityDescriptor descriptor)
    {
      if (string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase))
      {
        bool flag = false;
        string result = descriptor.Identifier;
        if (this.m_domainSidChanged)
          flag = this.m_domainHelper.MapIfSidFromDifferentHost(descriptor.Identifier, this.m_sourceDomainSid, out result);
        if (!flag && this.m_instanceDomainSidChanged)
          flag = this.m_instanceDomainHelper.MapIfSidFromDifferentHost(descriptor.Identifier, this.m_sourceInstanceDomainSid, out result);
        if (!flag && this.m_groupMap.TryGetValue(descriptor.Identifier, out result))
          flag = true;
        if (flag)
          descriptor = new IdentityDescriptor("Microsoft.TeamFoundation.Identity", result);
        descriptor = this.MapInactiveTargetGroupDescriptor(descriptor);
      }
      else if (string.Equals(descriptor.IdentityType, "Microsoft.TeamFoundation.ServiceIdentity", StringComparison.OrdinalIgnoreCase))
        descriptor = this.MapServiceIdentityDescriptor(descriptor);
      return descriptor;
    }

    private void MapFromLocalIds(
      List<ArtifactPropertyValue> artifactPropertyValues,
      IdentityIdMapper idMapper)
    {
      Guid[] localIds = new Guid[artifactPropertyValues.Count];
      for (int index = 0; index < localIds.Length; ++index)
        localIds[index] = new Guid(artifactPropertyValues[index].Spec.Id);
      Guid[] guidArray = idMapper.MapFromLocalIds(this.RequestContext, localIds);
      for (int index = 0; index < guidArray.Length; ++index)
      {
        if (guidArray[index] != Guid.Empty)
          artifactPropertyValues[index].Spec.Id = guidArray[index].ToByteArray();
        else
          artifactPropertyValues[index] = (ArtifactPropertyValue) null;
      }
      this.ServicingContext.DeploymentRequestContext.GetService<TeamFoundationPropertyService>().SetProperties(this.ServicingContext.DeploymentRequestContext, artifactPropertyValues.Where<ArtifactPropertyValue>((Func<ArtifactPropertyValue, bool>) (a => a != null)));
    }

    private IdentityDescriptor MapServiceIdentityDescriptor(IdentityDescriptor descriptor)
    {
      if (!this.m_domainSidChanged && !this.m_instanceDomainSidChanged)
        return descriptor;
      Guid scopeId;
      string role;
      string identifier;
      if (!IdentityHelper.TryParseFrameworkServiceIdentityDescriptor(descriptor, out scopeId, out role, out identifier))
      {
        this.ServicingContext.Warn("Unable to transform service identity descriptor '{0}' because it is not in the expected format", (object) descriptor);
        return descriptor;
      }
      if (this.m_instanceDomainSidChanged)
        IdentityMoveHandler.UpdateServiceIdentityDescriptor(this.m_originalApplicationInstanceId, this.m_instanceDomainHelper.DomainId, ref scopeId, ref identifier);
      if (this.m_domainSidChanged)
        IdentityMoveHandler.UpdateServiceIdentityDescriptor(this.m_originalCollectionId, this.m_domainHelper.DomainId, ref scopeId, ref identifier);
      return IdentityHelper.CreateFrameworkIdentityDescriptor(FrameworkIdentityType.ServiceIdentity, scopeId, role, identifier);
    }

    private void TransformServiceIdentity(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      identity.Descriptor = this.MapServiceIdentityDescriptor(identity.Descriptor);
      Guid domainId;
      if (this.m_instanceDomainSidChanged)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = identity;
        string pattern = this.m_originalApplicationInstanceId.ToString();
        domainId = this.m_instanceDomainHelper.DomainId;
        string replacement = domainId.ToString();
        IdentityMoveHandler.UpdateServiceIdentityProperties(identity1, pattern, replacement);
      }
      if (!this.m_domainSidChanged)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity;
      string pattern1 = this.m_originalCollectionId.ToString();
      domainId = this.m_domainHelper.DomainId;
      string replacement1 = domainId.ToString();
      IdentityMoveHandler.UpdateServiceIdentityProperties(identity2, pattern1, replacement1);
    }

    private static void UpdateServiceIdentityDescriptor(
      Guid oldScopeId,
      Guid newScopeId,
      ref Guid scopeId,
      ref string localIdentifier)
    {
      if (scopeId == oldScopeId)
        scopeId = newScopeId;
      localIdentifier = localIdentifier.Replace(oldScopeId.ToString(), newScopeId.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static void UpdateServiceIdentityProperties(
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string pattern,
      string replacement)
    {
      identity.ProviderDisplayName = identity.ProviderDisplayName.Replace(pattern, replacement, StringComparison.OrdinalIgnoreCase);
      string input = identity.Properties.GetValue<string>("Account", string.Empty);
      identity.Properties["Account"] = (object) input.Replace(pattern, replacement, StringComparison.OrdinalIgnoreCase);
    }
  }
}
