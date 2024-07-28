// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TeamFoundationWorkItemTrackingMetadataService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Utility;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class TeamFoundationWorkItemTrackingMetadataService : 
    ITeamFoundationWorkItemTrackingMetadataService,
    IVssFrameworkService
  {
    private static readonly ISet<MetadataTable> s_metadataTableSet = (ISet<MetadataTable>) new HashSet<MetadataTable>(Enum.GetValues(typeof (MetadataTable)).Cast<MetadataTable>());
    private WorkItemTrackingConstantsCache m_constantsCache;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/WorkItemTracking/Settings/...");
      IWorkItemTrackingConfigurationInfo serverSettings = systemRequestContext.WitContext().ServerSettings;
      if (serverSettings.ConfigWebLayoutVersion <= 0 || serverSettings.ConfigWebLayoutVersion <= serverSettings.CollectionWebLayoutVersion)
        return;
      this.UpdateStampsForWebLayoutChanged(systemRequestContext, serverSettings.ConfigWebLayoutVersion);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    public virtual MetadataDBStamps GetMetadataTableTimestamps(IVssRequestContext requestContext) => requestContext.TraceBlock<MetadataDBStamps>(900447, 900448, "Services", "MetadataService", nameof (GetMetadataTableTimestamps), (Func<MetadataDBStamps>) (() => this.GetMetadataTableTimestampsInternal(requestContext)));

    protected virtual MetadataDBStamps GetMetadataTableTimestampsInternal(
      IVssRequestContext requestContext)
    {
      using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
      {
        IDictionary<MetadataTable, long> metadataTableTimestamps = replicaAwareComponent.GetMetadataTableTimestamps(TeamFoundationWorkItemTrackingMetadataService.s_metadataTableSet, 0);
        metadataTableTimestamps[MetadataTable.NewWitFormLayout] = 3L;
        return new MetadataDBStamps(metadataTableTimestamps);
      }
    }

    public MetadataDBStamps GetMetadataTableTimestamps(
      IVssRequestContext requestContext,
      IEnumerable<MetadataTable> tableNames)
    {
      return tableNames == null || !tableNames.Any<MetadataTable>() ? MetadataDBStamps.Empty : this.GetMetadataTableTimestamps(requestContext).SubSet(tableNames);
    }

    public ConstantRecord GetConstantRecord(
      IVssRequestContext requestContext,
      int constantId,
      WitReadReplicaContext? readReplicaContext = null)
    {
      return this.GetConstantRecords(requestContext, (IEnumerable<int>) new int[1]
      {
        constantId
      }, false, readReplicaContext).FirstOrDefault<ConstantRecord>();
    }

    public ConstantRecord GetConstantRecord(
      IVssRequestContext requestContext,
      Guid teamFoundationId,
      WitReadReplicaContext? readReplicaContext = null)
    {
      return this.GetConstantRecords(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        teamFoundationId
      }, false, readReplicaContext).FirstOrDefault<ConstantRecord>();
    }

    public IEnumerable<ConstantRecord> GetConstantRecords(
      IVssRequestContext requestContext,
      IEnumerable<int> constantIds,
      bool includeInactiveConstants = false,
      WitReadReplicaContext? readReplicaContext = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<int>>(constantIds, nameof (constantIds));
      if (!constantIds.Any<int>())
        return Enumerable.Empty<ConstantRecord>();
      WorkItemTrackingConstantsCache constantsCache = this.GetConstantsCache(requestContext);
      IEnumerable<ConstantRecord> constantRecords;
      if (!constantsCache.TryGetConstants(requestContext, constantIds, includeInactiveConstants, out constantRecords))
      {
        using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(readReplicaContext))
          constantRecords = replicaAwareComponent.GetConstantRecords(constantIds, includeInactiveConstants);
        constantsCache.AddConstants(requestContext, constantRecords, includeInactiveConstants);
      }
      return constantRecords;
    }

    public IEnumerable<ConstantRecord> GetConstantRecords(
      IVssRequestContext requestContext,
      IEnumerable<Guid> teamFoundationIds,
      bool includeInactiveIdentities = false,
      WitReadReplicaContext? readReplicaContext = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Guid>>(teamFoundationIds, nameof (teamFoundationIds));
      if (!teamFoundationIds.Any<Guid>())
        return Enumerable.Empty<ConstantRecord>();
      WorkItemTrackingConstantsCache constantsCache = this.GetConstantsCache(requestContext);
      IEnumerable<ConstantRecord> constantRecords;
      if (!constantsCache.TryGetConstants(requestContext, teamFoundationIds, includeInactiveIdentities, out constantRecords))
      {
        using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(readReplicaContext))
          constantRecords = replicaAwareComponent.GetConstantRecords(teamFoundationIds, includeInactiveIdentities);
        constantsCache.AddConstants(requestContext, constantRecords, includeInactiveIdentities);
      }
      return constantRecords;
    }

    public virtual IEnumerable<ConstantRecord> GetConstantRecords(
      IVssRequestContext requestContext,
      IEnumerable<string> displayNames,
      bool includeInactiveIdentities = false,
      WitReadReplicaContext? readReplicaContext = null,
      bool includeInactiveNonIdentityConstants = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(displayNames, nameof (displayNames));
      if (!displayNames.Any<string>())
        return Enumerable.Empty<ConstantRecord>();
      WorkItemTrackingConstantsCache constantsCache = this.GetConstantsCache(requestContext);
      IEnumerable<ConstantRecord> constantRecords;
      if (!constantsCache.TryGetConstants(requestContext, displayNames, includeInactiveIdentities, out constantRecords))
      {
        using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(readReplicaContext))
          constantRecords = replicaAwareComponent.GetConstantRecords(displayNames, includeInactiveIdentities, includeInactiveNonIdentityConstants);
        constantsCache.AddConstants(requestContext, constantRecords, includeInactiveIdentities);
      }
      return constantRecords;
    }

    public IEnumerable<IdentityConstantRecord> SearchConstantIdentityRecords(
      IVssRequestContext requestContext,
      string searchTerm,
      SearchIdentityType identityType = SearchIdentityType.All)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(searchTerm, nameof (searchTerm));
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.SearchConstantIdentityRecords(searchTerm, identityType);
    }

    public virtual IEnumerable<PersonNameConstantRecord> GetConstantRecordsFromPersonNames(
      IVssRequestContext requestContext,
      IEnumerable<string> personNames)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(personNames, nameof (personNames));
      if (!personNames.Any<string>())
        return Enumerable.Empty<PersonNameConstantRecord>();
      using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(new WitReadReplicaContext?(new WitReadReplicaContext("WorkItemTracking.Server.ReadFromReadReplica"))))
        return replicaAwareComponent.GetConstantRecordsFromPersonNames(personNames);
    }

    public virtual bool SyncIdentity(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string source = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, nameof (identity));
      return this.SyncIdentities(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) new Microsoft.VisualStudio.Services.Identity.Identity[1]
      {
        identity
      }, source);
    }

    private void WriteIdentityCI(
      IVssRequestContext requestContext,
      CustomerIntelligenceService ciService,
      EuiiTracingService euiiService,
      string name,
      string values)
    {
      euiiService.TraceEuii(requestContext, "Services", "MetadataService", name, values);
    }

    public virtual bool SyncIdentities(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      string source = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
      ArgumentUtility.CheckEnumerableForEmpty((IEnumerable) identities, nameof (identities));
      CustomerIntelligenceService service1 = requestContext.GetService<CustomerIntelligenceService>();
      EuiiTracingService service2 = requestContext.GetService<EuiiTracingService>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
      {
        if (!IdentityConstantsNormalizer.CanSyncIdentity(identity))
          identityList.Add(identity);
      }
      if (identityList.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        this.WriteIdentityCI(requestContext, service1, service2, "CanSyncIdentity-" + (source ?? "AdHoc"), string.Join<Microsoft.VisualStudio.Services.Identity.Identity>(";", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList));
        return false;
      }
      IdentityService service3 = requestContext.GetService<IdentityService>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
      {
        if (!ServicePrincipals.IsServicePrincipal(requestContext, identity.Descriptor) && !service3.IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, identity.Descriptor))
          identityList.Add(identity);
      }
      if (identityList.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
      {
        this.WriteIdentityCI(requestContext, service1, service2, "NotIsMember-" + (source ?? "AdHoc"), string.Join<Microsoft.VisualStudio.Services.Identity.Identity>(";", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList));
        return false;
      }
      string instanceId = requestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToString();
      string collectionId = requestContext.ServiceHost.InstanceId.ToString();
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identitiesWithGroupMembership = service3.ReadIdentities(requestContext, (IList<IdentityDescriptor>) identities.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (x => x.Descriptor)).ToList<IdentityDescriptor>(), QueryMembership.Direct, (IEnumerable<string>) null);
      if (identitiesWithGroupMembership.Contains((Microsoft.VisualStudio.Services.Identity.Identity) null))
      {
        List<Microsoft.VisualStudio.Services.Identity.Identity> list = identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (id => !identitiesWithGroupMembership.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (iwgm => iwgm?.Descriptor?.Identifier == id.Descriptor.Identifier)))).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
        identityList.AddRange((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list);
        this.WriteIdentityCI(requestContext, service1, service2, "NotIsMember-" + (source ?? "AdHoc"), string.Join<Microsoft.VisualStudio.Services.Identity.Identity>(";", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList));
        return false;
      }
      identitiesWithGroupMembership.ForEach<Microsoft.VisualStudio.Services.Identity.Identity>((Action<Microsoft.VisualStudio.Services.Identity.Identity>) (identity => IdentityConstantsNormalizer.NormalizeIdentity(identity, instanceId, collectionId)));
      this.WriteIdentityCI(requestContext, service1, service2, "SyncIdentity-" + (source ?? "AdHoc"), string.Join<Microsoft.VisualStudio.Services.Identity.Identity>(";", (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identitiesWithGroupMembership));
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.SyncIdentities((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identitiesWithGroupMembership);
      return true;
    }

    public virtual IEnumerable<ConstantsSearchRecord> SearchConstantsRecords(
      IVssRequestContext requestContext,
      IEnumerable<string> searchValues,
      IEnumerable<Guid> tfIds,
      bool includeInactiveIdentities)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(searchValues, nameof (searchValues));
      if (!searchValues.Any<string>() && !tfIds.Any<Guid>())
        return Enumerable.Empty<ConstantsSearchRecord>();
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.SearchConstantsRecords(searchValues, tfIds, includeInactiveIdentities, requestContext.ExecutionEnvironment.IsHostedDeployment);
    }

    public virtual IEnumerable<ConstantRecord> GetNonIdentityConstants(
      IVssRequestContext requestContext,
      IEnumerable<string> displayTextValues,
      bool includeInactiveConstants = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(displayTextValues, nameof (displayTextValues));
      bool flag = true;
      WorkItemTrackingConstantsCache constantsCache = this.GetConstantsCache(requestContext);
      IEnumerable<ConstantRecord> constantRecords;
      if (!constantsCache.TryGetConstants(requestContext, displayTextValues, flag, out constantRecords))
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          constantRecords = component.GetNonIdentityConstants(displayTextValues, includeInactiveConstants);
        constantsCache.AddConstants(requestContext, constantRecords, flag, true);
      }
      return constantRecords;
    }

    public virtual IEnumerable<GroupMembershipRecord> GetRuleDependentGroups(
      IVssRequestContext requestContext)
    {
      return requestContext.TraceBlock<IEnumerable<GroupMembershipRecord>>(900449, 900450, "Services", "MetadataService", nameof (GetRuleDependentGroups), (Func<IEnumerable<GroupMembershipRecord>>) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return component.GetRuleDependentGroups();
      }));
    }

    public virtual IEnumerable<ForNotRuleGroupRecord> GetForNotRuleGroups(
      IVssRequestContext requestContext)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.GetForNotRuleGroups();
    }

    public virtual void SaveRuleSetRecordToSets(
      IVssRequestContext requestContext,
      IEnumerable<RuleSetRecord> ruleSetRecords)
    {
      requestContext.TraceBlock(900450, 900451, "Services", "MetadataService", nameof (SaveRuleSetRecordToSets), (Action) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          component.SaveConstantSets(ruleSetRecords);
      }));
    }

    public IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IVssRequestContext requestContext,
      IEnumerable<ConstantSetReference> setReferences)
    {
      List<ConstantSetReference> distinctSetReferences = setReferences.Distinct<ConstantSetReference>().ToList<ConstantSetReference>();
      return requestContext.TraceBlock<IDictionary<ConstantSetReference, SetRecord[]>>(900458, 900459, "Services", "MetadataService", "GetConstantSets " + string.Join<int>(",", distinctSetReferences.Select<ConstantSetReference, int>((Func<ConstantSetReference, int>) (s => s.Id))), (Func<IDictionary<ConstantSetReference, SetRecord[]>>) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return component.GetConstantSets((IEnumerable<ConstantSetReference>) distinctSetReferences);
      }));
    }

    public IDictionary<ConstantSetReference, SetRecord[]> GetDirectConstantSets(
      IVssRequestContext requestContext,
      IEnumerable<ConstantSetReference> setReferences)
    {
      List<ConstantSetReference> distinctSetReferences = setReferences.Distinct<ConstantSetReference>().ToList<ConstantSetReference>();
      return requestContext.TraceBlock<IDictionary<ConstantSetReference, SetRecord[]>>(900458, 900459, "Services", "MetadataService", "GetDirectConstantSets " + string.Join<int>(",", distinctSetReferences.Select<ConstantSetReference, int>((Func<ConstantSetReference, int>) (s => s.Id))), (Func<IDictionary<ConstantSetReference, SetRecord[]>>) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return component.GetDirectConstantSets((IEnumerable<ConstantSetReference>) distinctSetReferences);
      }));
    }

    public IEnumerable<Guid> GetForceProcessADObjects(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(909303, "Identity", "MetadataService", nameof (GetForceProcessADObjects));
      Enumerable.Empty<Guid>();
      try
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return component.GetForceProcessADObjects();
      }
      finally
      {
        requestContext.TraceLeave(909304, "Identity", "MetadataService", nameof (GetForceProcessADObjects));
      }
    }

    public void ForceSyncADObjects(
      IVssRequestContext requestContext,
      IEnumerable<ImsSyncIdentity> identities)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<ImsSyncIdentity>>(identities, nameof (identities));
      requestContext.TraceEnter(909305, "Identity", "MetadataService", nameof (ForceSyncADObjects));
      try
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          component.ForceSyncADObjects(identities);
      }
      finally
      {
        requestContext.TraceLeave(909306, "Identity", "MetadataService", nameof (ForceSyncADObjects));
      }
    }

    public IEnumerable<ConstantAuditEntry> GetDuplicateIdentityConstants(
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<IEnumerable<ConstantAuditEntry>>(909309, 909310, "Identity", "MetadataService", nameof (GetDuplicateIdentityConstants), (Func<IEnumerable<ConstantAuditEntry>>) (() =>
      {
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          return component.GetDuplicateIdentityConstants();
      }));
    }

    internal IEnumerable<WorkItemTypeAction> GetWorkItemTypeActions(
      IVssRequestContext requestContext,
      string projectName,
      string workItemTypeName)
    {
      if (WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext))
      {
        ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, projectName);
        IEnumerable<WorkItemTypeAction> actions;
        if (this.TryGetWorkItemTypeActionsFromProjectProcess(requestContext, project.Id, workItemTypeName, out actions))
          return actions;
      }
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        return component.GetWorkItemTypeActions(projectName, workItemTypeName);
    }

    internal virtual IEnumerable<RuleRecord> GetRules(
      IVssRequestContext requestContext,
      int projectId,
      string workItemTypeName,
      WitReadReplicaContext? readReplicaContext)
    {
      using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(readReplicaContext))
        return replicaAwareComponent.GetRules(projectId, workItemTypeName);
    }

    public virtual void SetIdentityFieldBit(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(909301, "Services", "MetadataService", nameof (SetIdentityFieldBit));
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.SetIdentityFieldBit();
      requestContext.TraceLeave(909302, "Services", "MetadataService", nameof (SetIdentityFieldBit));
    }

    public void CleanupConstants(
      IVssRequestContext requestContext,
      IReadOnlyCollection<int> usedIds,
      long constantsMetadataStamp)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.CleanupConstants(usedIds, constantsMetadataStamp);
    }

    public bool TrySyncIdentitiesToConstants(
      IVssRequestContext requestContext,
      IList<string> missingIdentities,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> resolvedMissingIdentities)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IList<string>>(missingIdentities, nameof (missingIdentities));
      if (resolvedMissingIdentities != null)
        ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) resolvedMissingIdentities, nameof (resolvedMissingIdentities));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!missingIdentities.All<string>(TeamFoundationWorkItemTrackingMetadataService.\u003C\u003EO.\u003C0\u003E__IsDistinctNameIdentity ?? (TeamFoundationWorkItemTrackingMetadataService.\u003C\u003EO.\u003C0\u003E__IsDistinctNameIdentity = new Func<string, bool>(IdentityUtilities.IsDistinctNameIdentity))))
        throw new ArgumentException(nameof (missingIdentities));
      return requestContext.TraceBlock<bool>(909307, 909308, "Services", "MetadataService", nameof (TrySyncIdentitiesToConstants), (Func<bool>) (() =>
      {
        if (missingIdentities.Count <= 0 && (resolvedMissingIdentities == null || resolvedMissingIdentities.Count <= 0))
          return false;
        Microsoft.VisualStudio.Services.Identity.Identity[] source1 = this.ReadMissingIdentities(requestContext, missingIdentities.ToArray<string>());
        bool flag = false;
        bool constants = false;
        List<Microsoft.VisualStudio.Services.Identity.Identity> source2 = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
        if (source1 != null && ((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source1).All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x != null)))
        {
          flag = true;
          IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source1;
          if (resolvedMissingIdentities != null)
            identities = identities.Union<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) resolvedMissingIdentities);
          source2 = identities.Distinct<Microsoft.VisualStudio.Services.Identity.Identity>((IEqualityComparer<Microsoft.VisualStudio.Services.Identity.Identity>) IdentityComparer.Instance).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (this.SyncIdentities(requestContext, (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) source2, nameof (TrySyncIdentitiesToConstants)))
            constants = true;
        }
        Guid[] guidArray1;
        if (source2 == null)
        {
          guidArray1 = (Guid[]) null;
        }
        else
        {
          IEnumerable<Guid> source3 = source2.Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (x => x.Id));
          guidArray1 = source3 != null ? source3.ToArray<Guid>() : (Guid[]) null;
        }
        if (guidArray1 == null)
          guidArray1 = Array.Empty<Guid>();
        Guid[] guidArray2 = guidArray1;
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(requestContext, WorkItemIdentityEagerSyncTelemetry.Feature, (object) guidArray2, (object) flag, (object) constants);
        return constants;
      }));
    }

    public void ForceRefreshClientMetadata(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.StampDb();
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Services", "MetadataService", nameof (ForceRefreshClientMetadata), true);
    }

    private Microsoft.VisualStudio.Services.Identity.Identity[] ReadMissingIdentities(
      IVssRequestContext requestContext,
      string[] missingIdentities)
    {
      try
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        List<Microsoft.VisualStudio.Services.Identity.Identity> source = new List<Microsoft.VisualStudio.Services.Identity.Identity>(missingIdentities.Length);
        foreach (string missingIdentity in missingIdentities)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(requestContext, IdentitySearchFilter.DisplayName, missingIdentity, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          source.Add(identity);
        }
        if (source.Any<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)))
        {
          for (int index = 0; index < source.Count; ++index)
          {
            if (source[index] == null)
            {
              IdentityDisplayName disambiguatedSearchTerm = IdentityDisplayName.GetDisambiguatedSearchTerm(missingIdentities[index]);
              if (disambiguatedSearchTerm.Type == SearchTermType.DomainAndAccountName)
              {
                IdentityDescriptor identityDescriptor = new IdentityDescriptor("System:ServicePrincipal", disambiguatedSearchTerm.AccountName);
                Microsoft.VisualStudio.Services.Identity.Identity identity = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
                {
                  identityDescriptor
                }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
                if (identity != null)
                  source[index] = identity;
              }
            }
          }
        }
        return source.ToArray();
      }
      catch (GroupScopeDoesNotExistException ex)
      {
        requestContext.TraceException(909311, "Identity", "MetadataService", (Exception) ex);
        return (Microsoft.VisualStudio.Services.Identity.Identity[]) null;
      }
    }

    public virtual IDictionary<string, IList<GroupMemberEntry>> FindGroupMembersOfType(
      IVssRequestContext requestContext,
      IEnumerable<string> groupNames,
      GroupType groupType,
      int maxMemberForEachGroup = 100,
      int maxIteration = 50,
      WitReadReplicaContext? readReplicaContext = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IEnumerable<string>>(groupNames, nameof (groupNames));
      Dictionary<string, IList<GroupMemberEntry>> groupMembersOfType = new Dictionary<string, IList<GroupMemberEntry>>((IEqualityComparer<string>) VssStringComparer.GroupName);
      using (new PerformanceScenarioHelper(requestContext, "Identity", "MetadataService").Measure(nameof (FindGroupMembersOfType)))
      {
        using (WorkItemTrackingMetadataComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<WorkItemTrackingMetadataComponent>(readReplicaContext))
        {
          foreach (GroupMemberEntry groupMemberEntry in replicaAwareComponent.FindGroupMembersOfType(groupNames, groupType, maxMemberForEachGroup, maxIteration))
          {
            if (!groupMembersOfType.ContainsKey(groupMemberEntry.Group))
              groupMembersOfType.Add(groupMemberEntry.Group, (IList<GroupMemberEntry>) new List<GroupMemberEntry>());
            groupMembersOfType[groupMemberEntry.Group].Add(groupMemberEntry);
          }
          return (IDictionary<string, IList<GroupMemberEntry>>) groupMembersOfType;
        }
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      if (!changedEntries.Any<RegistryEntry>())
        return;
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      int configWebLayoutVersion = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/ConfigWebLayoutVersion", true, 0);
      int num = service.GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/CollectionWebLayoutVersion", false, 0);
      if (configWebLayoutVersion <= 0 || configWebLayoutVersion <= num)
        return;
      this.UpdateStampsForWebLayoutChanged(requestContext, configWebLayoutVersion);
    }

    private void UpdateStampsForWebLayoutChanged(
      IVssRequestContext requestContext,
      int configWebLayoutVersion)
    {
      requestContext.GetService<IVssRegistryService>().SetValue<int>(requestContext, "/Service/WorkItemTracking/Settings/CollectionWebLayoutVersion", configWebLayoutVersion);
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>(connectionType: new DatabaseConnectionType?(DatabaseConnectionType.Dbo)))
        component.SetCollectionWebLayoutVersion2(configWebLayoutVersion);
    }

    private WorkItemTrackingConstantsCache GetConstantsCache(IVssRequestContext requestContext)
    {
      if (this.m_constantsCache == null)
        this.m_constantsCache = new WorkItemTrackingConstantsCache(requestContext);
      return this.m_constantsCache;
    }

    public void ResetSequenceId(
      IVssRequestContext requestContext,
      int identitySequenceId,
      int groupSequenceId)
    {
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        component.ResetSequenceId(identitySequenceId, groupSequenceId);
    }

    private bool TryGetWorkItemTypeActionsFromProjectProcess(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemTypeName,
      out IEnumerable<WorkItemTypeAction> actions)
    {
      ProcessDescriptor processDescriptor;
      if (requestContext.GetService<WorkItemTrackingProcessService>().TryGetProjectProcessDescriptor(requestContext, projectId, out processDescriptor) && !processDescriptor.IsCustom)
      {
        Guid processTypeId = processDescriptor.IsDerived ? processDescriptor.Inherits : processDescriptor.TypeId;
        ProcessWorkItemTypeDefinition itemTypeDefinition = requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processTypeId).WorkItemTypeDefinitions.Where<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (w => TFStringComparer.WorkItemTypeName.Equals(w.Name, workItemTypeName))).FirstOrDefault<ProcessWorkItemTypeDefinition>();
        if (itemTypeDefinition != null)
        {
          actions = itemTypeDefinition.Actions.Select<ProcessWorkItemTypeActionDefinition, WorkItemTypeAction>((Func<ProcessWorkItemTypeActionDefinition, WorkItemTypeAction>) (a => new WorkItemTypeAction(a)));
          return true;
        }
      }
      actions = (IEnumerable<WorkItemTypeAction>) null;
      return false;
    }
  }
}
