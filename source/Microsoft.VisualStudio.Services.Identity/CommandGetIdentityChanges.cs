// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.CommandGetIdentityChanges
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class CommandGetIdentityChanges : Command
  {
    private readonly IdentityDomain m_hostDomain;
    private List<Microsoft.VisualStudio.Services.Identity.Identity> m_identities;
    private int m_index;
    private const string s_Area = "CommandGetIdentityChanges";
    private const string s_Layer = "IdentityService";
    private const string GetIdentityChangesPageSizeRegistryKey = "/Configuration/Identity/GetIdentityChanges/ReadIdentityPageSize";
    private const int DefaultIdentityPageSize = 5000;

    public CommandGetIdentityChanges(IVssRequestContext requestContext, IdentityDomain hostDomain)
      : base(requestContext)
    {
      this.m_hostDomain = hostDomain;
    }

    public void Execute(
      int deploymentIdentitySequenceId,
      int groupSequenceId,
      int organizationIdentitySequenceId,
      PlatformIdentityStore identityStore,
      Guid scopeId,
      int pageSize)
    {
      int lastSequenceId1 = 0;
      long lastSequenceId2 = 0;
      IdentityDescriptor identityDescriptor = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.EveryoneGroup, scopeId);
      bool flag = false;
      Dictionary<Guid, bool> identityIds = new Dictionary<Guid, bool>();
      List<Guid> first = new List<Guid>();
      List<Guid> guidList = first;
      IEnumerable<Guid> guids = (IEnumerable<Guid>) first;
      bool scopedGroupChanges = IdentityMembershipHelper.ShouldGetScopedGroupChanges(this.RequestContext);
      bool useIdentityAudit = IdentityStoreUtilities.IdentityAuditEnabled(this.RequestContext);
      int num1;
      Guid everyoneGroupMasterId;
      if (groupSequenceId == 0)
      {
        flag = true;
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.RequestContext.GetService<IdentityService>().ReadIdentities(this.RequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).First<Microsoft.VisualStudio.Services.Identity.Identity>();
        everyoneGroupMasterId = identity != null ? identity.MasterId : Guid.Empty;
        if (everyoneGroupMasterId == Guid.Empty)
          throw new IdentityNotFoundException(string.Format("Failed to find the Valid Users group in host: {0}.", (object) this.RequestContext.ServiceHost));
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.RequestContext))
        {
          num1 = checked ((int) groupComponent.GetLatestGroupSequenceId(identityDescriptor.Identifier, scopeId));
          identityIds.Add(everyoneGroupMasterId, true);
        }
      }
      else
      {
        int num2 = this.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.RequestContext, (RegistryQuery) "/Configuration/Identity/GetIdentityChanges/ReadIdentityPageSize", 5000);
        using (GroupComponent groupComponent = PlatformIdentityStore.CreateGroupComponent(this.RequestContext))
        {
          long lastSequenceId3;
          using (ResultCollection changes = groupComponent.GetChanges((long) groupSequenceId, identityDescriptor.Identifier, scopeId, out everyoneGroupMasterId, out lastSequenceId3, getScopedGroupChanges: scopedGroupChanges))
          {
            num1 = checked ((int) lastSequenceId3);
            ObjectBinder<MembershipChangeInfo> current1 = changes.GetCurrent<MembershipChangeInfo>();
            while (current1.MoveNext())
            {
              MembershipChangeInfo current2 = current1.Current;
              if (current2.MemberId == everyoneGroupMasterId)
              {
                identityIds.Clear();
                identityIds.Add(current2.MemberId, true);
                flag = true;
                groupComponent.Cancel();
                break;
              }
              identityIds[current2.MemberId] = true;
              if (!identityIds.ContainsKey(current2.ContainerId))
                identityIds[current2.ContainerId] = false;
            }
            if (!flag)
            {
              changes.NextResult();
              ObjectBinder<Guid> current3 = changes.GetCurrent<Guid>();
              while (current3.MoveNext())
              {
                Guid current4 = current3.Current;
                if (!identityIds.ContainsKey(current4))
                  identityIds[current4] = false;
              }
            }
          }
        }
        if (!flag && this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(this.RequestContext))
          {
            if (pageSize > num2)
            {
              this.RequestContext.Trace(80730, TraceLevel.Error, nameof (CommandGetIdentityChanges), "IdentityService", string.Format("A pagesize {0} was passed while we were expecting maximum pagesize of {1}", (object) pageSize, (object) num2));
              pageSize = num2;
            }
            using (ResultCollection resultCollection = pageSize > 0 ? identityComponent.GetScopedPagedIdentityChanges((long) organizationIdentitySequenceId, scopeId, useIdentityAudit, pageSize, out lastSequenceId2) : identityComponent.GetScopedIdentityChanges((long) organizationIdentitySequenceId, scopeId, useIdentityAudit, out lastSequenceId2))
            {
              guidList = new List<Guid>();
              guids = first.Concat<Guid>((IEnumerable<Guid>) guidList);
              foreach (IdentityManagementComponent.ReferencedIdentity referencedIdentity in resultCollection.GetCurrent<IdentityManagementComponent.ReferencedIdentity>())
              {
                if (IdentityManagementComponent.ReferencedIdentityLocation.Remote == referencedIdentity.Location)
                  guidList.Add(referencedIdentity.IdentityId);
                else
                  first.Add(referencedIdentity.IdentityId);
              }
            }
          }
        }
      }
      if (!flag)
      {
        if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          if (first != guidList)
          {
            foreach (Guid key in first)
            {
              if (!identityIds.ContainsKey(key))
                identityIds[key] = false;
            }
          }
          else if (guids.Any<Guid>())
          {
            using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(this.RequestContext))
              GetChanges(identityComponent, 1, guids);
          }
          using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(this.RequestContext))
            lastSequenceId1 = checked ((int) identityComponent.GetLatestIdentitySequenceId(useIdentityAudit));
        }
        else
        {
          using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateDeploymentIdentityComponent(this.RequestContext))
            identityComponent.GetChanges(int.MaxValue, ref lastSequenceId1, 1L, useIdentityAudit).Dispose();
          if (lastSequenceId1 > deploymentIdentitySequenceId)
          {
            using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateDeploymentIdentityComponent(this.RequestContext))
              lastSequenceId1 = GetChanges(identityComponent, deploymentIdentitySequenceId, (IEnumerable<Guid>) guidList);
          }
        }
      }
      else if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateOrganizationIdentityComponent(this.RequestContext))
          lastSequenceId2 = (long) checked ((int) identityComponent.GetLatestIdentitySequenceId(useIdentityAudit));
        lastSequenceId1 = checked ((int) lastSequenceId2);
      }
      else
      {
        using (IdentityManagementComponent identityComponent = PlatformIdentityStore.CreateDeploymentIdentityComponent(this.RequestContext))
          identityComponent.GetChanges(int.MaxValue, ref lastSequenceId1, 1L, useIdentityAudit).Dispose();
      }
      Dictionary<Guid, bool> source = new Dictionary<Guid, bool>();
      bool enableUseXtpProc = this.RequestContext.IsFeatureEnabled(PlatformIdentityStore.EnableUseReadGroupMembershipXtpProcFeatureFlag);
      using (ReadGroupMembershipsComponentBase membershipsComponent = PlatformIdentityStore.CreateReadGroupMembershipsComponent(this.RequestContext))
      {
        if (!flag && !scopedGroupChanges)
        {
          using (ResultCollection resultCollection = membershipsComponent.ReadMemberships(identityIds.Select<KeyValuePair<Guid, bool>, Guid>((Func<KeyValuePair<Guid, bool>, Guid>) (x => x.Key)), QueryMembership.None, QueryMembership.None, false, new int?(), scopeId, true, enableUseXtpProc))
          {
            resultCollection.NextResult();
            resultCollection.NextResult();
            foreach (ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData filteredIdentityData in resultCollection.GetCurrent<ReadGroupMembershipsComponentBase.ScopeFilteredIdentityData>())
            {
              if (filteredIdentityData.IdentityId != Guid.Empty)
                source.Add(filteredIdentityData.IdentityId, identityIds[filteredIdentityData.IdentityId]);
            }
          }
        }
        else
          source = identityIds;
        using (ResultCollection resultCollection = membershipsComponent.ReadMemberships(source.Where<KeyValuePair<Guid, bool>>((Func<KeyValuePair<Guid, bool>, bool>) (x => x.Value)).Select<KeyValuePair<Guid, bool>, Guid>((Func<KeyValuePair<Guid, bool>, Guid>) (y => y.Key)), QueryMembership.Expanded, QueryMembership.None, true, new int?(), scopeId, false, enableUseXtpProc))
        {
          resultCollection.NextResult();
          foreach (GroupMembership groupMembership in resultCollection.GetCurrent<GroupMembership>())
          {
            if (!source.ContainsKey(groupMembership.Id))
              source.Add(groupMembership.Id, false);
          }
        }
      }
      this.LastSequenceId = new Tuple<int, int, int>(lastSequenceId1, num1, checked ((int) lastSequenceId2));
      this.m_identities = new List<Microsoft.VisualStudio.Services.Identity.Identity>(source.Count);
      if (source.Count > 0)
      {
        Guid[] guidArray = source.Select<KeyValuePair<Guid, bool>, Guid>((Func<KeyValuePair<Guid, bool>, Guid>) (x => x.Key)).ToArray<Guid>();
        if (!this.m_hostDomain.IsMaster)
          guidArray = identityStore.GetIdMapper(this.RequestContext, this.m_hostDomain).MapToLocalIds(this.RequestContext, guidArray);
        this.m_identities.AddRange(this.ResolveIdentitiesWithDirectExpansion(identityStore, guidArray).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)));
        if (this.m_identities.Count != source.Count)
        {
          Guid[] array1 = this.m_identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id)).ToArray<Guid>();
          Guid[] array2 = ((IEnumerable<Guid>) guidArray).Except<Guid>((IEnumerable<Guid>) array1).ToArray<Guid>();
          this.RequestContext.Trace(80730, TraceLevel.Error, nameof (CommandGetIdentityChanges), "IdentityService", array2.Length == 0 ? string.Empty : string.Join<Guid>(",", (IEnumerable<Guid>) array2));
          Guid[] array3 = ((IEnumerable<Guid>) array1).Except<Guid>((IEnumerable<Guid>) guidArray).ToArray<Guid>();
          this.RequestContext.Trace(80730, TraceLevel.Error, nameof (CommandGetIdentityChanges), "IdentityService", array3.Length == 0 ? string.Empty : string.Join<Guid>(",", (IEnumerable<Guid>) array3));
        }
      }
      if (!this.RequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) && identityIds.ContainsKey(everyoneGroupMasterId) && this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x.Id == everyoneGroupMasterId)).First<Microsoft.VisualStudio.Services.Identity.Identity>();
        IVssRequestContext vssRequestContext = this.RequestContext.To(TeamFoundationHostType.Deployment);
        foreach (Microsoft.VisualStudio.Services.Identity.Identity principalIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) vssRequestContext.GetService<SystemIdentityService>().GetServicePrincipalIdentities(vssRequestContext))
        {
          if (!identityIds.ContainsKey(principalIdentity.Id))
          {
            identity.Members.Add(principalIdentity.Descriptor);
            identity.MemberIds.Add(principalIdentity.Id);
            this.m_identities.Add(principalIdentity);
          }
        }
      }
      for (int index = 0; index < this.m_identities.Count; ++index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_identities[index];
        if (identity.GetProperty<Guid>("ScopeId", Guid.Empty).Equals(scopeId))
          identity.SetProperty("Domain", (object) string.Empty);
      }
      this.m_index = 0;
      this.IdentitiesStream = new StreamingCollection<Microsoft.VisualStudio.Services.Identity.Identity>((Command) this);
      this.ContinueExecution();

      int GetChanges(
        IdentityManagementComponent component,
        int sequenceId,
        IEnumerable<Guid> identityIdFilter)
      {
        int lastSequenceId = -1;
        foreach (Guid key in component.GetChanges(sequenceId, ref lastSequenceId, 1L, useIdentityAudit, identityIdFilter).GetCurrent<Guid>())
        {
          if (!identityIds.ContainsKey(key))
            identityIds[key] = false;
        }
        return lastSequenceId;
      }
    }

    public override void ContinueExecution()
    {
      for (; !this.IsCacheFull && this.m_index < this.m_identities.Count; ++this.m_index)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_identities[this.m_index];
        if (identity != null)
          this.IdentitiesStream.Enqueue(identity);
      }
      if (this.IsCacheFull)
        return;
      this.IdentitiesStream.IsComplete = true;
    }

    protected override void Dispose(bool disposing)
    {
    }

    private IList<Microsoft.VisualStudio.Services.Identity.Identity> ResolveIdentitiesWithDirectExpansion(
      PlatformIdentityStore identityStore,
      Guid[] ids)
    {
      Microsoft.VisualStudio.Services.Identity.Identity[] source = identityStore.ReadIdentities(this.RequestContext, this.m_hostDomain, (IList<Guid>) ids, QueryMembership.None, false, (IEnumerable<string>) null, true, true);
      string message = string.Empty;
      if (source.Length == ids.Length)
      {
        for (int index = 0; index < ids.Length; ++index)
        {
          if (ids[index] != Guid.Empty && source[index] == null)
          {
            message = string.Format("Null identity returned in ReadIdentities. Ids: {0}, Results: {1}", (object) ids.Serialize<Guid[]>(), (object) source.Serialize<Microsoft.VisualStudio.Services.Identity.Identity[]>());
            break;
          }
        }
      }
      else
        message = string.Format("Results count {0} does not match ids count {1}. Ids: {2}, Results: {3}", (object) source.Length, (object) ids.Length, (object) ids.Serialize<Guid[]>(), (object) source.Serialize<Microsoft.VisualStudio.Services.Identity.Identity[]>());
      if (!string.IsNullOrEmpty(message))
        this.RequestContext.Trace(80730, TraceLevel.Error, nameof (CommandGetIdentityChanges), "IdentityService", message);
      IList<Microsoft.VisualStudio.Services.Identity.Identity> list = (IList<Microsoft.VisualStudio.Services.Identity.Identity>) ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => AadIdentityHelper.IsAadGroup(x?.Descriptor))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      Guid[] array = ((IEnumerable<Guid>) ids).Except<Guid>(list.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id))).ToArray<Guid>();
      Microsoft.VisualStudio.Services.Identity.Identity[] values = identityStore.ReadIdentities(this.RequestContext, this.m_hostDomain, (IList<Guid>) array, QueryMembership.Direct, false, (IEnumerable<string>) null, true, true);
      return list.AddRange<Microsoft.VisualStudio.Services.Identity.Identity, IList<Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) values);
    }

    public IList<Microsoft.VisualStudio.Services.Identity.Identity> Identities => (IList<Microsoft.VisualStudio.Services.Identity.Identity>) this.m_identities;

    public StreamingCollection<Microsoft.VisualStudio.Services.Identity.Identity> IdentitiesStream { get; private set; }

    public Tuple<int, int, int> LastSequenceId { get; private set; }
  }
}
