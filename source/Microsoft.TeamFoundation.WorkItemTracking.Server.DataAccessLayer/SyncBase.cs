// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.SyncBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Tracing;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class SyncBase
  {
    private const int c_processRetryCountMax = 2;
    private const string c_waterMarkPath = "/Service/WorkItemTracking/LastIdentitySequenceIds/";
    private const string c_waterMarkGroupPath = "/Service/WorkItemTracking/LastGroupSequenceIds/";
    private const string c_delayKey = "/Service/WorkItemTracking/Settings/SyncJobDelay";
    private const int c_defaultDelay = 60;
    private const string c_accountStatePath = "/Service/WorkItemTracking/DataAccessLayer/IsAADBackedAccount";
    private const string c_accountStateMigratedPath = "/Service/WorkItemTracking/DataAccessLayer/IsMigrated";
    private static readonly Guid s_syncJobId = new Guid("69AD5827-6346-4B08-B29D-2EE8BE361F85");
    private IVssRequestContext m_requestContext;
    private SequenceIds m_identitySequenceIds;
    private Guid m_instanceId = Guid.NewGuid();
    private bool m_queueSync;

    public SyncBase(IVssRequestContext requestContext)
    {
      this.m_requestContext = requestContext;
      this.GetSequenceIds();
    }

    private SequenceIds HandleStaleIdentityManagementService_OnPremOnly()
    {
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      List<SyncBase.Membership> members = new List<SyncBase.Membership>();
      string instanceId = this.m_requestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToString("D");
      string collectionId = this.m_requestContext.ServiceHost.InstanceId.ToString("D");
      SequenceIds sequenceIds = (SequenceIds) null;
      using (TeamFoundationDataReader changesFromNewService = this.GetIdentityChangesFromNewService(this.m_requestContext, 0, 0, 0))
      {
        Tuple<int, int, int> tuple = changesFromNewService.Current<Tuple<int, int, int>>();
        sequenceIds = new SequenceIds()
        {
          IdentitySequenceId = tuple.Item1,
          IdentityGroupSequenceId = tuple.Item2,
          IdentityOrganizationSequenceId = tuple.Item3
        };
        changesFromNewService.MoveNext();
        foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in changesFromNewService.Current<StreamingCollection<Microsoft.VisualStudio.Services.Identity.Identity>>())
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = identity1;
          if (IdentityConstantsNormalizer.CanSyncIdentity(identity))
          {
            IdentityConstantsNormalizer.NormalizeIdentity(identity, instanceId, collectionId);
            identityList.Add(identity);
            if (identity.Members != null && identity.Members.Count > 0)
              members.AddRange(identity.Members.Select<IdentityDescriptor, SyncBase.Membership>((Func<IdentityDescriptor, SyncBase.Membership>) (item => new SyncBase.Membership()
              {
                Owner = identity.Descriptor.Identifier,
                Member = item.Identifier
              })));
          }
        }
      }
      using (DalSqlResourceComponent component = DalSqlResourceComponent.CreateComponent(this.m_requestContext))
        component.ResyncIdentities1((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identityList, (IEnumerable<SyncBase.Membership>) members, new Tuple<int, int>(sequenceIds.IdentitySequenceId, sequenceIds.IdentityGroupSequenceId));
      return sequenceIds;
    }

    private bool CheckAndHandleStaleIdentityManagementService()
    {
      if (this.m_requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Application);
      string watermarkPathKey = this.watermarkPathKey;
      string watermarkGroupPathKey = this.watermarkGroupPathKey;
      SqlRegistryService service = vssRequestContext.GetService<SqlRegistryService>();
      if (this.m_identitySequenceIds.HasSequenceIdHigherThan(this.ReadSequenceIdFromRegistry(vssRequestContext, service, watermarkPathKey, watermarkGroupPathKey)))
      {
        using (this.m_requestContext.GetService<ITeamFoundationLockingService>().AcquireLock(this.m_requestContext, TeamFoundationLockMode.Exclusive, SpecialGuids.ProcessChangesLock))
        {
          SequenceIds baseSeqId = this.ReadSequenceIdFromRegistry(vssRequestContext, service, watermarkPathKey, watermarkGroupPathKey);
          this.GetSequenceIds();
          if (this.m_identitySequenceIds.HasSequenceIdHigherThan(baseSeqId))
          {
            this.m_requestContext.TraceAlways(900704, TraceLevel.Warning, "Sync", nameof (SyncBase), string.Format("Resync identities. WIT: {0}; Registry: {1}", (object) this.m_identitySequenceIds, (object) baseSeqId));
            this.m_identitySequenceIds = this.HandleStaleIdentityManagementService_OnPremOnly();
            this.WriteSequenceIdToRegistry(vssRequestContext, service, watermarkPathKey, watermarkGroupPathKey, this.m_identitySequenceIds);
          }
        }
      }
      return true;
    }

    private SequenceIds ReadSequenceIdFromRegistry(
      IVssRequestContext applicationContext,
      SqlRegistryService registryService,
      string watermarkKey,
      string watermarkGroupKey)
    {
      int num1 = registryService.GetValue<int>(applicationContext, (RegistryQuery) watermarkKey, true, -1);
      int num2 = registryService.GetValue<int>(applicationContext, (RegistryQuery) watermarkGroupKey, true, -1);
      if (num2 == -1)
        num2 = num1;
      return new SequenceIds()
      {
        IdentitySequenceId = num1,
        IdentityGroupSequenceId = num2
      };
    }

    private void WriteSequenceIdToRegistry(
      IVssRequestContext applicationContext,
      SqlRegistryService registryService,
      string watermarkKey,
      string watermarkGroupKey,
      SequenceIds newSequenceIds)
    {
      for (int index = 0; index < 2; ++index)
      {
        try
        {
          List<RegistryEntry> registryEntryList = new List<RegistryEntry>()
          {
            new RegistryEntry(watermarkKey, newSequenceIds.IdentitySequenceId.ToString((IFormatProvider) CultureInfo.InvariantCulture)),
            new RegistryEntry(watermarkGroupKey, newSequenceIds.IdentityGroupSequenceId.ToString((IFormatProvider) CultureInfo.InvariantCulture))
          };
          registryService.WriteEntries(applicationContext, (IEnumerable<RegistryEntry>) registryEntryList);
          break;
        }
        catch (Exception ex)
        {
          this.m_requestContext.Trace(900516, TraceLevel.Warning, "Sync", nameof (SyncBase), "Failed to set sequence Id to TeamFoundation Registry");
          this.m_requestContext.TraceException(900533, "Sync", nameof (SyncBase), ex);
          if (index >= 2)
            throw;
        }
      }
    }

    public void ProcessIdentityChanges() => this.Retry((Action) (() =>
    {
      this.ProcessIdentityChangesInternal();
      if (!this.m_queueSync)
        return;
      int intervalInSeconds = this.m_requestContext.WitContext().ServerSettings.MinimalImsSyncIntervalInSeconds;
      SyncBase.QueueSyncJob(this.m_requestContext, intervalInSeconds);
      CustomerIntelligenceService service = this.m_requestContext.GetService<CustomerIntelligenceService>();
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("SequenceIds", (object) this.m_identitySequenceIds);
      intelligenceData.Add("AdditionalDelay", (double) intervalInSeconds);
      IVssRequestContext requestContext = this.m_requestContext;
      CustomerIntelligenceData properties = intelligenceData;
      service.Publish(requestContext, "Sync", "PageIdentities", properties);
    }));

    private bool ProcessIdentityChangesInternal()
    {
      this.m_requestContext.TraceEnter(900297, "Sync", nameof (SyncBase), string.Format("Instance: {0} - ProcessIdentityChanges({1})", (object) this.m_instanceId, (object) this.m_identitySequenceIds));
      this.CheckAndHandleStaleIdentityManagementService();
      this.CheckAndHandleAADAccountState();
      this.LogDuplicateIdentitiesInConstants();
      ITeamFoundationLockingService service = this.m_requestContext.GetService<ITeamFoundationLockingService>();
      SequenceIds sequenceIds = (SequenceIds) null;
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      if (this.m_requestContext.ExecutionEnvironment.IsHostedDeployment)
        this.ReadIdentityChanges(out identities, out sequenceIds);
      IVssRequestContext requestContext = this.m_requestContext;
      string processChangesLock = SpecialGuids.ProcessChangesLock;
      using (service.AcquireLock(requestContext, TeamFoundationLockMode.Exclusive, processChangesLock))
      {
        bool flag = false;
        if (identities != null)
        {
          SequenceIds identitySequenceIds = this.m_identitySequenceIds;
          this.GetSequenceIds();
          if (this.m_identitySequenceIds.HasSequenceIdHigherThan(identitySequenceIds))
          {
            this.m_requestContext.Trace(900763, TraceLevel.Verbose, "Sync", nameof (SyncBase), "Instance: {0} - Initial sequence changed", (object) this.m_instanceId);
            if (this.m_identitySequenceIds.HasSequenceIdHigherThan(sequenceIds))
            {
              flag = true;
              this.m_requestContext.Trace(900299, TraceLevel.Verbose, "Sync", nameof (SyncBase), "Instance: {0} - Skipping", (object) this.m_instanceId);
            }
          }
        }
        if (!flag)
        {
          if (identities == null)
            this.ReadIdentityChanges(out identities, out sequenceIds);
          this.ProcessIdentityChangesInternal(identities, sequenceIds, true);
        }
      }
      this.m_requestContext.TraceLeave(900523, "Sync", nameof (SyncBase), string.Format("Instance: {0} - ProcessIdentityChanges", (object) this.m_instanceId));
      return true;
    }

    private void ReadIdentityChanges(
      out IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      out SequenceIds sequenceIds)
    {
      using (TeamFoundationDataReader changesFromNewService = this.GetIdentityChangesFromNewService(this.m_requestContext, this.m_identitySequenceIds.IdentitySequenceId, this.m_identitySequenceIds.IdentityGroupSequenceId, this.m_identitySequenceIds.IdentityOrganizationSequenceId, new int?(this.m_requestContext.WitContext().ServerSettings.GetIdentityChangesPageSize)))
      {
        Tuple<int, int, int, bool> tuple = changesFromNewService.Current<Tuple<int, int, int, bool>>();
        sequenceIds = new SequenceIds()
        {
          IdentitySequenceId = tuple.Item1,
          IdentityGroupSequenceId = tuple.Item2,
          IdentityOrganizationSequenceId = tuple.Item3
        };
        if (tuple.Item4)
          this.m_queueSync = true;
        changesFromNewService.MoveNext();
        identities = (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) changesFromNewService.Current<StreamingCollection<Microsoft.VisualStudio.Services.Identity.Identity>>();
        this.m_requestContext.Trace(900298, TraceLevel.Verbose, "Sync", nameof (SyncBase), string.Format("Instance: {0} - GetIdentityChanges: Found ({1})", (object) this.m_instanceId, (object) sequenceIds));
      }
    }

    private void LogDuplicateIdentitiesInConstants()
    {
      IEnumerable<ConstantAuditEntry> identityConstants = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetDuplicateIdentityConstants(this.m_requestContext);
      if (identityConstants.Count<ConstantAuditEntry>() > 0)
        SyncBase.SendDuplicateItentitiesTrace(this.m_requestContext, identityConstants);
      else
        this.m_requestContext.Trace(981297, TraceLevel.Verbose, "Sync", nameof (SyncBase), "No duplicate identities with same TeamFoundationId/SID found, expected");
    }

    private static void SendDuplicateItentitiesTrace(
      IVssRequestContext requestContext,
      IEnumerable<ConstantAuditEntry> duplicateIdentities)
    {
      string str = "Following are identities with duplicate TeamFoundationId or SID in Constants table\r\n" + string.Join("\r\n", duplicateIdentities.Select<ConstantAuditEntry, string>((Func<ConstantAuditEntry, string>) (i => string.Format("TeamFoundationId={0},", (object) i.TeamFoundationId) + string.Format("ConstId={0},", (object) i.ConstId) + "DomainPart=" + i.DomainPart + ",NamePart=" + i.NamePart + ",DisplayPart=" + i.DisplayPart + "," + string.Format("ChangerId={0},", (object) i.ChangerId) + string.Format("AddedDate={0},", (object) i.AddedDate) + string.Format("RemovedDate={0},", (object) i.RemovedDate) + "SID=" + i.SID)));
      requestContext.TraceAlways(981296, TraceLevel.Warning, "Sync", nameof (SyncBase), str);
      requestContext.GetService<EuiiTracingService>().TraceEuii(requestContext, "Sync", nameof (duplicateIdentities), "LogDuplicateIdentitiesInConstants", str);
    }

    private void ProcessIdentityChangesInternal(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> changedIdentities,
      SequenceIds newSequenceIds,
      bool processMissingIdentitiesAndUpdateSequenceIds)
    {
      this.m_requestContext.Trace(981295, TraceLevel.Verbose, "Sync", nameof (SyncBase), "Instance: {0} - ProcessIdentityChangesInternal(worker, processMissingIdentitiesAndUpdateSequenceIds:{1}) SequenceId ({2})", (object) this.m_instanceId, (object) processMissingIdentitiesAndUpdateSequenceIds, (object) this.m_identitySequenceIds);
      IdentityEventMessage identityEventMessage = new IdentityEventMessage(this.m_requestContext, changedIdentities, newSequenceIds);
      Exception identityProcessingException = (Exception) null;
      if (!this.TryProcessIdentityChanges(identityEventMessage, newSequenceIds, processMissingIdentitiesAndUpdateSequenceIds, out identityProcessingException) && !string.IsNullOrEmpty(identityProcessingException.Message) && (identityProcessingException.Message.Contains("IX_Constants__String_RemovedDate") || identityProcessingException.Message.Contains("Cannot insert duplicate key row in object")))
      {
        ITeamFoundationWorkItemTrackingMetadataService service1 = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
        Guid[] array = service1.GetForceProcessADObjects(this.m_requestContext).ToArray<Guid>();
        if (((IEnumerable<Guid>) array).Any<Guid>())
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.m_requestContext.GetService<IdentityService>().ReadIdentities(this.m_requestContext, (IList<Guid>) array, QueryMembership.None, (IEnumerable<string>) null);
          List<ImsSyncIdentity> identities = new List<ImsSyncIdentity>();
          string instanceId = this.m_requestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToString("D");
          string collectionId = this.m_requestContext.ServiceHost.InstanceId.ToString("D");
          CustomerIntelligenceService service2 = this.m_requestContext.GetService<CustomerIntelligenceService>();
          service2.Publish(this.m_requestContext, "Sync", "ForceSync", "ForceSyncIds", string.Join<Guid>(",", (IEnumerable<Guid>) array));
          List<Guid> values1 = new List<Guid>();
          List<Guid> values2 = new List<Guid>();
          List<Guid> values3 = new List<Guid>();
          List<Guid> values4 = new List<Guid>();
          for (int index = 0; index < identityList.Count; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity1 = identityList[index];
            ImsSyncIdentity imsSyncIdentity;
            if (identity1 != null && array[index] == identity1.Id)
            {
              Microsoft.VisualStudio.Services.Identity.Identity identity2 = IdentityConstantsNormalizer.NormalizeIdentity(identity1, instanceId, collectionId);
              imsSyncIdentity = new ImsSyncIdentity()
              {
                AccountName = identity2.GetProperty<string>("Account", (string) null),
                DomainName = identity2.GetProperty<string>("Domain", (string) null),
                Deleted = !identity2.IsActive,
                DisplayName = identity2.DisplayName,
                Sid = identity2.Descriptor.Identifier,
                TeamFoundationId = identity2.Id
              };
              values1.Add(imsSyncIdentity.TeamFoundationId);
            }
            else
            {
              imsSyncIdentity = new ImsSyncIdentity()
              {
                TeamFoundationId = array[index]
              };
              values2.Add(imsSyncIdentity.TeamFoundationId);
              if (identity1 != null)
                values3.Add(imsSyncIdentity.TeamFoundationId);
              else
                values4.Add(array[index]);
            }
            identities.Add(imsSyncIdentity);
          }
          service2.Publish(this.m_requestContext, "Sync", "ForceSync", "ForceSyncIdNull", string.Join<Guid>(",", (IEnumerable<Guid>) values4));
          service2.Publish(this.m_requestContext, "Sync", "ForceSync", "ForceSyncIdMismatch", string.Join<Guid>(",", (IEnumerable<Guid>) values3));
          service2.Publish(this.m_requestContext, "Sync", "ForceSync", "ForceSyncIdUpdate", string.Join<Guid>(",", (IEnumerable<Guid>) values1));
          service2.Publish(this.m_requestContext, "Sync", "ForceSync", "ForceSyncIdDelete", string.Join<Guid>(",", (IEnumerable<Guid>) values2));
          try
          {
            service1.ForceSyncADObjects(this.m_requestContext, (IEnumerable<ImsSyncIdentity>) identities);
          }
          catch (Exception ex)
          {
            this.m_requestContext.TraceException(904000, "Sync", nameof (SyncBase), ex);
            identityProcessingException = ex;
          }
        }
      }
      if (processMissingIdentitiesAndUpdateSequenceIds)
      {
        if (identityProcessingException == null)
          this.m_identitySequenceIds = identityEventMessage.SequenceIds;
        else
          this.GetSequenceIds();
        this.m_requestContext.Trace(900522, TraceLevel.Info, "Sync", nameof (SyncBase), string.Format("Instance: {0} - Updating in memory seqId. ", (object) this.m_instanceId) + Environment.NewLine + "\tm_identitySeqId = " + this.m_identitySequenceIds.ToString());
        if (!this.m_requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          IVssRequestContext vssRequestContext = this.m_requestContext.To(TeamFoundationHostType.Application);
          CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
          this.WriteSequenceIdToRegistry(vssRequestContext, (SqlRegistryService) service, this.watermarkPathKey, this.watermarkGroupPathKey, this.m_identitySequenceIds);
        }
      }
      if (identityProcessingException != null)
        throw identityProcessingException;
    }

    private bool TryProcessIdentityChanges(
      IdentityEventMessage identityEventMessage,
      SequenceIds newSequenceIds,
      bool processMissingIdentitiesAndUpdateSequenceIds,
      out Exception identityProcessingException)
    {
      identityProcessingException = (Exception) null;
      bool flag = true;
      try
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> processedGroups;
        identityEventMessage.Process(out processedGroups);
        if (processMissingIdentitiesAndUpdateSequenceIds)
        {
          if (processedGroups.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
            this.ProcessMissingIdentitiesForGroups(processedGroups, newSequenceIds);
        }
      }
      catch (Exception ex)
      {
        flag = false;
        identityProcessingException = ex;
        this.m_requestContext.Trace(900518, TraceLevel.Warning, "Sync", nameof (SyncBase), "Exception was thrown while processing identity changes.");
        this.m_requestContext.TraceException(900534, "Sync", nameof (SyncBase), ex);
      }
      return flag;
    }

    internal void CheckAndHandleAADAccountState() => this.m_requestContext.TraceBlock(900792, 900797, "Sync", nameof (SyncBase), nameof (CheckAndHandleAADAccountState), (Action) (() =>
    {
      bool aadBackedAccount = this.m_requestContext.WitContext().IsAadBackedAccount;
      IVssRegistryService service = this.m_requestContext.GetService<IVssRegistryService>();
      int num1 = service.GetValue<int>(this.m_requestContext, (RegistryQuery) "/Service/WorkItemTracking/DataAccessLayer/IsAADBackedAccount", -1);
      if (num1 == -1)
      {
        num1 = aadBackedAccount ? 1 : 0;
        service.SetValue<int>(this.m_requestContext, "/Service/WorkItemTracking/DataAccessLayer/IsAADBackedAccount", num1);
      }
      int num2 = service.GetValue<int>(this.m_requestContext, (RegistryQuery) "/Service/WorkItemTracking/DataAccessLayer/IsMigrated", -1);
      if (num2 == -1)
      {
        num2 = 0;
        service.SetValue<int>(this.m_requestContext, "/Service/WorkItemTracking/DataAccessLayer/IsMigrated", num2);
      }
      bool flag = num1 == 1;
      this.m_requestContext.Trace(900798, TraceLevel.Verbose, "Sync", nameof (SyncBase), string.Format("CheckAndHandleAADAccountState: previousIsAccountAadValue = {0}, currentIsAccountAadValue = {1}, isMigrated = {2}", (object) flag, (object) aadBackedAccount, (object) num2));
      if (aadBackedAccount && (!flag || num2 == 0))
      {
        this.InsertSVRulesAndUpdateCacheStamp();
        service.SetValue<int>(this.m_requestContext, "/Service/WorkItemTracking/DataAccessLayer/IsMigrated", 1);
        service.SetValue<int>(this.m_requestContext, "/Service/WorkItemTracking/DataAccessLayer/IsAADBackedAccount", 1);
      }
      else
      {
        if (!flag || aadBackedAccount)
          return;
        this.DeleteSVRulesAndUpdateCacheStamp();
        service.SetValue<int>(this.m_requestContext, "/Service/WorkItemTracking/DataAccessLayer/IsAADBackedAccount", 0);
      }
    }));

    private void InsertSVRulesAndUpdateCacheStamp() => this.m_requestContext.TraceBlock(900793, 900794, "Sync", nameof (SyncBase), "CheckAndHandleAADAccountState-InsertSVRules", (Action) (() =>
    {
      using (DalSqlResourceComponent component = this.m_requestContext.CreateComponent<DalSqlResourceComponent>())
        component.InsertDomainAccountsSVRulesAndUpdateCacheStamp();
    }));

    private void DeleteSVRulesAndUpdateCacheStamp() => this.m_requestContext.TraceBlock(900795, 900796, "Sync", nameof (SyncBase), "CheckAndHandleAADAccountState-DeleteSVRules", (Action) (() =>
    {
      using (DalSqlResourceComponent component = this.m_requestContext.CreateComponent<DalSqlResourceComponent>())
        component.DeleteDomainAccountsSVRulesAndUpdateCacheStamp();
    }));

    public bool SyncIdentity(IdentityDescriptor descriptor)
    {
      IdentityService service1 = this.m_requestContext.GetService<IdentityService>();
      ITeamFoundationWorkItemTrackingMetadataService service2 = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>();
      IVssRequestContext requestContext = this.m_requestContext;
      List<IdentityDescriptor> list = ((IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
      {
        descriptor
      }).ToList<IdentityDescriptor>();
      Microsoft.VisualStudio.Services.Identity.Identity identity = service1.ReadIdentities(requestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
      return service2.SyncIdentity(this.m_requestContext, identity, nameof (SyncIdentity));
    }

    internal virtual void GetSequenceIds()
    {
      PayloadTable sequenceIds = new DataAccessLayerImpl(this.m_requestContext).GetSequenceIds();
      PayloadTable.PayloadRow row = sequenceIds.Rows[0];
      int int32 = Convert.ToInt32(row["IdentitySeqId"], (IFormatProvider) CultureInfo.InvariantCulture);
      int num1 = -1;
      if (sequenceIds.Columns.Contains("GroupSeqId"))
        num1 = Convert.ToInt32(row["GroupSeqId"], (IFormatProvider) CultureInfo.InvariantCulture);
      int num2 = num1 == -1 ? int32 : num1;
      int num3 = 0;
      if (sequenceIds.Columns.Contains("OrganizationSeqId"))
        num3 = Convert.ToInt32(row["OrganizationSeqId"], (IFormatProvider) CultureInfo.InvariantCulture);
      this.m_identitySequenceIds = new SequenceIds()
      {
        IdentitySequenceId = int32,
        IdentityGroupSequenceId = num2,
        IdentityOrganizationSequenceId = num3
      };
      this.m_requestContext.Trace(900300, TraceLevel.Verbose, "Sync", nameof (SyncBase), "Instance {0} - IdentitySeqId = {1}", (object) this.m_instanceId, (object) this.m_identitySequenceIds);
    }

    internal IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> GetMissingIdentities(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups)
    {
      DataAccessLayerImpl dataAccessLayerImpl = new DataAccessLayerImpl(this.m_requestContext);
      IdentityService service = this.m_requestContext.GetService<IdentityService>();
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups1 = groups;
      IdentityDescriptor[] array = dataAccessLayerImpl.GetMissingIdentities(groups1).Select<string, IdentityDescriptor>((Func<string, IdentityDescriptor>) (sid => IdentityHelper.CreateDescriptorFromSid(sid))).ToArray<IdentityDescriptor>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = service.ReadIdentities(this.m_requestContext, (IList<IdentityDescriptor>) array, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (array.Length != 0)
        WorkItemTrackingTelemetry.TraceCustomerIntelligence(this.m_requestContext, ImsSyncTelemetry.Feature, (object) array.Length, (object) list.Count);
      return (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) list;
    }

    internal void ProcessMissingIdentitiesForGroups(
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> groups,
      SequenceIds newSequenceIds)
    {
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> missingIdentities = this.GetMissingIdentities(groups);
      if (!missingIdentities.Any<Microsoft.VisualStudio.Services.Identity.Identity>())
        return;
      this.m_requestContext.Trace(900301, TraceLevel.Verbose, "Sync", nameof (SyncBase), "Instance {0} - Found {1} missing identities", (object) this.m_instanceId, (object) missingIdentities.Count<Microsoft.VisualStudio.Services.Identity.Identity>());
      this.ProcessIdentityChangesInternal(missingIdentities, newSequenceIds, false);
    }

    private void Retry(Action action)
    {
      for (int index = 0; index < 2; ++index)
      {
        try
        {
          if (index > 0)
            this.GetSequenceIds();
          action();
          break;
        }
        catch (Exception ex)
        {
          this.m_requestContext.Trace(900517, TraceLevel.Warning, "Sync", nameof (SyncBase), ex.ToString());
          switch (ex)
          {
            case LegacyValidationException _:
            case SqlException _:
            case ProjectDoesNotExistException _:
              if (index != 1)
                continue;
              break;
          }
          throw;
        }
      }
    }

    private TeamFoundationDataReader GetIdentityChangesFromNewService(
      IVssRequestContext requestContext,
      int identitySequenceId,
      int identityGroupSequenceId,
      int identityOrganizationSequenceId,
      int? pageSize = null)
    {
      IIdentityServiceInternal identityServiceInternal = this.m_requestContext.GetService<IdentityService>().IdentityServiceInternal();
      int pageSize1 = pageSize.HasValue ? pageSize.Value : 0;
      IVssRequestContext requestContext1 = requestContext;
      ChangedIdentitiesContext sequenceContext = new ChangedIdentitiesContext(identitySequenceId, identityGroupSequenceId, identityOrganizationSequenceId, pageSize1);
      ChangedIdentities identityChanges = identityServiceInternal.GetIdentityChanges(requestContext1, sequenceContext);
      IVssRequestContext requestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
      IdentityDomain hostDomain = new IdentityDomain(requestContext, new IdentityDomain(requestContext2, (IdentityDomain) null));
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities = identityChanges.Identities;
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in identities.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)))
      {
        identity1.Descriptor = hostDomain.MapToWellKnownIdentifier(identity1.Descriptor);
        Microsoft.VisualStudio.Services.Identity.Identity identity2 = identity1;
        ICollection<IdentityDescriptor> members = identity1.Members;
        List<IdentityDescriptor> list1 = members != null ? members.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (m => hostDomain.MapToWellKnownIdentifier(m))).ToList<IdentityDescriptor>() : (List<IdentityDescriptor>) null;
        identity2.Members = (ICollection<IdentityDescriptor>) list1;
        Microsoft.VisualStudio.Services.Identity.Identity identity3 = identity1;
        ICollection<IdentityDescriptor> memberOf = identity1.MemberOf;
        List<IdentityDescriptor> list2 = memberOf != null ? memberOf.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (m => hostDomain.MapToWellKnownIdentifier(m))).ToList<IdentityDescriptor>() : (List<IdentityDescriptor>) null;
        identity3.MemberOf = (ICollection<IdentityDescriptor>) list2;
      }
      return pageSize.HasValue ? new TeamFoundationDataReader(new object[2]
      {
        (object) new Tuple<int, int, int, bool>(identityChanges.SequenceContext.IdentitySequenceId, identityChanges.SequenceContext.GroupSequenceId, identityChanges.SequenceContext.OrganizationIdentitySequenceId, identityChanges.MoreData),
        (object) new StreamingCollection<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      }) : new TeamFoundationDataReader(new object[2]
      {
        (object) new Tuple<int, int, int>(identityChanges.SequenceContext.IdentitySequenceId, identityChanges.SequenceContext.GroupSequenceId, identityChanges.SequenceContext.OrganizationIdentitySequenceId),
        (object) new StreamingCollection<Microsoft.VisualStudio.Services.Identity.Identity>((IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) identities)
      });
    }

    internal static void QueueSyncJob(
      IVssRequestContext requestContext,
      int additionalDelayInSeconds = 0)
    {
      if (requestContext.IsTracing(900705, TraceLevel.Verbose, "Sync", nameof (SyncBase)))
        requestContext.Trace(900705, TraceLevel.Verbose, "Sync", nameof (SyncBase), Environment.StackTrace);
      int maxDelaySeconds = SyncBase.ComputeSyncJobDelay(requestContext) + additionalDelayInSeconds;
      TeamFoundationJobReference foundationJobReference = new TeamFoundationJobReference(SyncBase.s_syncJobId, JobPriorityClass.High);
      requestContext.GetService<TeamFoundationJobService>().QueueDelayedJobs(requestContext, (IEnumerable<TeamFoundationJobReference>) new TeamFoundationJobReference[1]
      {
        foundationJobReference
      }, maxDelaySeconds, JobPriorityLevel.Highest);
    }

    internal static int ComputeSyncJobDelay(IVssRequestContext requestContext)
    {
      IEnumerable<ProjectInfo> projects = requestContext.GetService<IProjectService>().GetProjects(requestContext.Elevate(), ProjectState.New);
      int syncJobDelay = 0;
      if (projects.Any<ProjectInfo>())
      {
        double totalSeconds = (DateTime.UtcNow - projects.Max<ProjectInfo, DateTime>((Func<ProjectInfo, DateTime>) (x => x.LastUpdateTime))).TotalSeconds;
        double num1 = totalSeconds > 0.0 ? totalSeconds : 0.0;
        int num2 = requestContext.GetService<CachedRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/WorkItemTracking/Settings/SyncJobDelay", true, 60);
        if (num1 < (double) num2)
          syncJobDelay = num2 - (int) num1;
      }
      return syncJobDelay;
    }

    private string watermarkPathKey => "/Service/WorkItemTracking/LastIdentitySequenceIds/" + this.m_requestContext.ServiceHost.InstanceId.ToString("D");

    private string watermarkGroupPathKey => "/Service/WorkItemTracking/LastGroupSequenceIds/" + this.m_requestContext.ServiceHost.InstanceId.ToString("D");

    internal struct Membership
    {
      public string Owner;
      public string Member;
    }
  }
}
