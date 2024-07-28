// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.ForNotGroupMembershipService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public class ForNotGroupMembershipService : BaseTeamFoundationWorkItemTrackingService
  {
    private static readonly MetadataTable[] s_metadataStampTables = new MetadataTable[1]
    {
      MetadataTable.Rules
    };
    protected MetadataDBStamps m_currentMetadataStamps;
    protected List<ForNotRuleGroupRecord> m_forNotGroups;
    protected List<Microsoft.VisualStudio.Services.Identity.Identity> m_forNotGroupIdentities;
    protected ILockName m_lock;

    protected override void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      this.m_forNotGroups = (List<ForNotRuleGroupRecord>) null;
      this.m_forNotGroupIdentities = (List<Microsoft.VisualStudio.Services.Identity.Identity>) null;
      this.m_currentMetadataStamps = (MetadataDBStamps) null;
      base.ServiceEnd(systemRequestContext);
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.m_lock = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.ServiceHost.CreateLockName("ForNotGroupCacheServiceLock") : throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
    }

    public bool[] GetForNotGroupMembershipForPhase2(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      IWorkItemTrackingProcessService service1 = requestContext.GetService<IWorkItemTrackingProcessService>();
      ProcessDescriptor processDescriptor;
      if (!WorkItemTrackingFeatureFlags.IsProcessCustomizationEnabled(requestContext) || !service1.TryGetLatestProjectProcessDescriptor(requestContext, projectId, out processDescriptor) || !processDescriptor.IsDerived)
        return Array.Empty<bool>();
      IProcessWorkItemTypeService service2 = requestContext.GetService<IProcessWorkItemTypeService>();
      PerformanceScenarioHelper performanceScenarioHelper = new PerformanceScenarioHelper(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (GetForNotGroupMembershipForPhase2));
      HashSet<Guid> source = new HashSet<Guid>();
      using (performanceScenarioHelper.Measure("GetForNotGroupsPhase2"))
      {
        foreach (ProcessWorkItemType typelet in (IEnumerable<ProcessWorkItemType>) service2.GetTypelets<ProcessWorkItemType>(requestContext, processDescriptor.TypeId))
        {
          if (typelet != null)
            source.UnionWith(typelet.ForNotGroups);
        }
      }
      IRuleMembershipFilter filter = requestContext.WitContext().RuleMembershipFilter;
      bool[] array;
      using (performanceScenarioHelper.Measure("GetGroupMembershipPhase2"))
        array = source.OrderBy<Guid, Guid>((Func<Guid, Guid>) (id => id)).Select<Guid, bool>((Func<Guid, bool>) (id => filter.IsCurrentUserMemberOfGroup(id))).ToArray<bool>();
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, nameof (GetForNotGroupMembershipForPhase2), "NoOfGroups", (double) array.Length);
      return array;
    }

    public bool[] GetForNotGroupMembership(IVssRequestContext requestContext)
    {
      CustomerIntelligenceData intelligenceData = new CustomerIntelligenceData();
      intelligenceData.Add("Action", nameof (GetForNotGroupMembership));
      PerformanceScenarioHelper perfScenario = new PerformanceScenarioHelper(requestContext, CustomerIntelligenceArea.WorkItemTracking, "GetCollectionForNotGroupMembership");
      WorkItemTrackingRequestContext witContext = requestContext.WitContext();
      MetadataDBStamps metadataStamps = this.GetMetadataStamps(witContext);
      bool flag;
      using (requestContext.AcquireReaderLock(this.m_lock))
        flag = this.IsStampFresh(metadataStamps);
      if (!flag)
      {
        using (perfScenario.Measure("HandleMetadataStampsChange"))
          this.HandleMetadataStampsChange(requestContext, metadataStamps, intelligenceData, perfScenario);
      }
      bool[] currentUserMemberships;
      using (perfScenario.Measure("GetGroupMembership"))
        currentUserMemberships = this.GetCurrentUserMemberships(witContext);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, CustomerIntelligenceArea.WorkItemTracking, "ForNotGroupsMembership", intelligenceData);
      perfScenario.EndScenario();
      return currentUserMemberships;
    }

    internal virtual MetadataDBStamps GetMetadataStamps(WorkItemTrackingRequestContext witContext) => witContext.MetadataDbStamps((IEnumerable<MetadataTable>) ForNotGroupMembershipService.s_metadataStampTables);

    internal virtual bool[] GetCurrentUserMemberships(WorkItemTrackingRequestContext witContext)
    {
      IRuleMembershipFilter membershipFilter = witContext.RuleMembershipFilter;
      return this.m_forNotGroupIdentities.Select<Microsoft.VisualStudio.Services.Identity.Identity, bool>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (g => g != null && membershipFilter.IsCurrentUserMemberOfGroup(g.Id))).ToArray<bool>();
    }

    protected bool TryParseMetadataStamps(string token, out long[] stamps)
    {
      if (string.IsNullOrEmpty(token))
      {
        stamps = (long[]) null;
        return false;
      }
      string[] strArray = token.Split(',');
      stamps = new long[strArray.Length];
      for (int index = 0; index < strArray.Length; ++index)
      {
        long result;
        if (!long.TryParse(strArray[index], out result))
          return false;
        stamps[index] = result;
      }
      return true;
    }

    private bool IsStampFresh(MetadataDBStamps newMetaDataStamp) => this.m_currentMetadataStamps.IsFresh(newMetaDataStamp);

    protected virtual List<ForNotRuleGroupRecord> GetForNotGroups(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetForNotRuleGroups(requestContext).ToList<ForNotRuleGroupRecord>();

    private bool AreGroupsEqual(List<ForNotRuleGroupRecord> newForNotGroups) => this.CompareList<ForNotRuleGroupRecord>(this.m_forNotGroups, newForNotGroups);

    private bool CompareList<T>(List<T> list1, List<T> list2)
    {
      if (list1 == null && list2 == null)
        return true;
      return (list1 != null || list2 == null) && (list2 != null || list1 == null) && !list1.Except<T>((IEnumerable<T>) list2).Any<T>() && !list2.Except<T>((IEnumerable<T>) list1).Any<T>();
    }

    internal virtual void HandleMetadataStampsChange(
      IVssRequestContext requestContext,
      MetadataDBStamps newStamps,
      CustomerIntelligenceData ci,
      PerformanceScenarioHelper perfScenario)
    {
      List<ForNotRuleGroupRecord> newForNotGroups = this.RecordPerfScenario<List<ForNotRuleGroupRecord>>(perfScenario, "GetForNotGroupNames", (Func<List<ForNotRuleGroupRecord>>) (() => this.GetForNotGroups(requestContext)));
      bool flag = false;
      using (requestContext.AcquireReaderLock(this.m_lock))
      {
        if (!this.AreGroupsEqual(newForNotGroups))
          flag = true;
      }
      if (flag)
      {
        List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.RecordPerfScenario<List<Microsoft.VisualStudio.Services.Identity.Identity>>(perfScenario, "ConvertGroupNameToGroupIdentity", (Func<List<Microsoft.VisualStudio.Services.Identity.Identity>>) (() => this.GetNewGroupIdentities(requestContext, newForNotGroups)));
        using (requestContext.AcquireWriterLock(this.m_lock))
        {
          this.m_forNotGroups = newForNotGroups;
          this.m_forNotGroupIdentities = identityList;
          this.m_currentMetadataStamps = newStamps;
        }
      }
      else
        this.m_currentMetadataStamps = newStamps;
      ci.Add("CacheCleared", flag);
    }

    private T RecordPerfScenario<T>(
      PerformanceScenarioHelper perfScenario,
      string scenarioName,
      Func<T> action)
    {
      if (perfScenario == null)
        return action();
      using (perfScenario.Measure(scenarioName))
        return action();
    }

    protected internal virtual List<Microsoft.VisualStudio.Services.Identity.Identity> GetNewGroupIdentities(
      IVssRequestContext requestContext,
      List<ForNotRuleGroupRecord> newForNotGroups)
    {
      IRuleMembershipFilter membershipFilter = requestContext.WitContext().RuleMembershipFilter;
      IdentityService service = requestContext.GetService<IdentityService>();
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList;
      if (newForNotGroups.Any<ForNotRuleGroupRecord>((Func<ForNotRuleGroupRecord, bool>) (g => !Guid.Empty.Equals(g.TeamFoundationId))))
      {
        identityList = service.ReadIdentities(requestContext, (IList<Guid>) newForNotGroups.Select<ForNotRuleGroupRecord, Guid>((Func<ForNotRuleGroupRecord, Guid>) (g => g.TeamFoundationId)).ToArray<Guid>(), QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (group => group != null)).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      }
      else
      {
        IEnumerable<string> strings = newForNotGroups.Select<ForNotRuleGroupRecord, string>((Func<ForNotRuleGroupRecord, string>) (g => membershipFilter.ConvertGroupToSearchFactor(requestContext, g.StringValue)));
        identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
        foreach (string factorValue in strings)
          identityList.AddRange(service.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, factorValue, QueryMembership.None, (IEnumerable<string>) null).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (i => i != null)));
      }
      List<Microsoft.VisualStudio.Services.Identity.Identity> newGroupIdentities = new List<Microsoft.VisualStudio.Services.Identity.Identity>();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identityList)
        newGroupIdentities.Add(identity);
      return newGroupIdentities;
    }
  }
}
