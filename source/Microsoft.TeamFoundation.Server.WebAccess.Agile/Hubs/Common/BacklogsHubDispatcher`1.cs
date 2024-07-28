// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common.BacklogsHubDispatcher`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.AspNet.SignalR;
using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Agile.Web.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Hubs.Common
{
  public abstract class BacklogsHubDispatcher<T> : IBacklogsHubDispatcher where T : IBacklogsHubClient
  {
    private const string PublishAutorefreshUpdateEventCI = "/Service/Agile/Settings/PublishAutorefreshUpdateEventCI";
    protected IHubContext<T> m_hubContext;

    public void NotifyWorkItemsChanged(
      IVssRequestContext requestContext,
      WorkItemsChangedWithExtensionsBatchEvent workItemChangedBatchEvent)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<WorkItemsChangedWithExtensionsBatchEvent>(workItemChangedBatchEvent, nameof (workItemChangedBatchEvent));
      try
      {
        if (this.m_hubContext != null)
        {
          Dictionary<string, List<WorkItemUpdateEventForExtension>> groupToUpdateEventsMap = new Dictionary<string, List<WorkItemUpdateEventForExtension>>();
          Dictionary<string, int> typeCountDictionary = this.CreateEventChangeTypeCountDictionary();
          foreach (WorkItemChangedEventExtended change in workItemChangedBatchEvent.Changes)
          {
            HashSet<string> oldGroupNames = this.GetOldGroupNames(requestContext, change);
            HashSet<string> newGroupNames = this.GetNewGroupNames(requestContext, change);
            int intField1 = WorkItemChangedHelper.GetIntField(change.LegacyChangedEvent, "System.Id");
            ChangeTypes changeType1 = WorkItemChangedHelper.GetChangeType(change.LegacyChangedEvent);
            int intField2 = WorkItemChangedHelper.GetIntField(change.LegacyChangedEvent, "System.Rev");
            string associatedWithTheChange = WorkItemChangedHelper.GetStackRankAssociatedWithTheChange(change.LegacyChangedEvent);
            bool flag1 = WorkItemChangedHelper.HasChangedChildren(change.LegacyChangedEvent);
            bool flag2 = WorkItemChangedHelper.HasChangedTypeOrTitle(change.LegacyChangedEvent);
            int revision = intField2;
            int changeType2 = (int) changeType1;
            string stackRank = associatedWithTheChange;
            int num1 = flag1 ? 1 : 0;
            int num2 = flag2 ? 1 : 0;
            WorkItemUpdateEventForExtension eventForExtension = new WorkItemUpdateEventForExtension(intField1, revision, (ChangeTypes) changeType2, stackRank, num1 != 0, num2 != 0);
            switch (changeType1)
            {
              case ChangeTypes.New:
                using (HashSet<string>.Enumerator enumerator = newGroupNames.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string current = enumerator.Current;
                    this.AddUpdateEventData(groupToUpdateEventsMap, eventForExtension, current);
                    ++typeCountDictionary["Created"];
                  }
                  continue;
                }
              case ChangeTypes.Change:
                IEnumerable<string> strings1 = oldGroupNames.Except<string>((IEnumerable<string>) newGroupNames);
                WorkItemUpdateEventForExtension updateEvent1 = new WorkItemUpdateEventForExtension(eventForExtension);
                updateEvent1.ChangeType = "Removed";
                foreach (string groupName in strings1)
                {
                  this.AddUpdateEventData(groupToUpdateEventsMap, updateEvent1, groupName);
                  ++typeCountDictionary["Removed"];
                }
                IEnumerable<string> strings2 = newGroupNames.Except<string>((IEnumerable<string>) oldGroupNames);
                WorkItemUpdateEventForExtension updateEvent2 = new WorkItemUpdateEventForExtension(eventForExtension);
                updateEvent2.ChangeType = "Added";
                foreach (string groupName in strings2)
                {
                  this.AddUpdateEventData(groupToUpdateEventsMap, updateEvent2, groupName);
                  ++typeCountDictionary["Added"];
                }
                using (IEnumerator<string> enumerator = newGroupNames.Intersect<string>((IEnumerable<string>) oldGroupNames).GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string current = enumerator.Current;
                    this.AddUpdateEventData(groupToUpdateEventsMap, eventForExtension, current);
                    ++typeCountDictionary["Updated"];
                  }
                  continue;
                }
              case ChangeTypes.Delete:
                using (HashSet<string>.Enumerator enumerator = oldGroupNames.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string current = enumerator.Current;
                    this.AddUpdateEventData(groupToUpdateEventsMap, eventForExtension, current);
                    ++typeCountDictionary["Deleted"];
                  }
                  continue;
                }
              case ChangeTypes.Restore:
                using (HashSet<string>.Enumerator enumerator = newGroupNames.GetEnumerator())
                {
                  while (enumerator.MoveNext())
                  {
                    string current = enumerator.Current;
                    this.AddUpdateEventData(groupToUpdateEventsMap, eventForExtension, current);
                    ++typeCountDictionary["Restored"];
                  }
                  continue;
                }
              default:
                requestContext.Trace(290536, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "Unsupported work item change type");
                continue;
            }
          }
          this.DispatchEventsToClients(requestContext, groupToUpdateEventsMap);
          this.PublishUpdateEventData(requestContext, workItemChangedBatchEvent.Changes.Count, groupToUpdateEventsMap.Keys.Count, typeCountDictionary);
        }
        else
          requestContext.Trace(290537, TraceLevel.Error, "Agile", TfsTraceLayers.BusinessLogic, "Null reference to hub context in BacklogsHubDispatcher");
      }
      catch (Exception ex)
      {
        requestContext.TraceException(290534, "Agile", TfsTraceLayers.BusinessLogic, ex);
      }
    }

    private void DispatchEventsToClients(
      IVssRequestContext requestContext,
      Dictionary<string, List<WorkItemUpdateEventForExtension>> groupToUpdateEventsMap)
    {
      foreach (KeyValuePair<string, List<WorkItemUpdateEventForExtension>> groupToUpdateEvents in groupToUpdateEventsMap)
        this.m_hubContext.Clients.Group(groupToUpdateEvents.Key).OnWorkItemsUpdated(requestContext, groupToUpdateEvents.Value);
    }

    private Dictionary<string, int> CreateEventChangeTypeCountDictionary() => new Dictionary<string, int>()
    {
      ["Created"] = 0,
      ["Added"] = 0,
      ["Updated"] = 0,
      ["Deleted"] = 0,
      ["Removed"] = 0,
      ["Restored"] = 0
    };

    private void PublishUpdateEventData(
      IVssRequestContext requestContext,
      int updateCount,
      int extensionsCount,
      Dictionary<string, int> changeTypeToEventsCount)
    {
      try
      {
        if (!requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/Agile/Settings/PublishAutorefreshUpdateEventCI", true, true))
          return;
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(AgileCustomerIntelligencePropertyName.EventsBatchSizeCount, (double) updateCount);
        properties.Add(AgileCustomerIntelligencePropertyName.ExtensionsCount, (double) extensionsCount);
        int num = 0;
        foreach (KeyValuePair<string, int> keyValuePair in changeTypeToEventsCount)
        {
          properties.Add(keyValuePair.Key, (double) keyValuePair.Value);
          num += keyValuePair.Value;
        }
        properties.Add(AgileCustomerIntelligencePropertyName.EventsToClientsCount, (double) num);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, AgileCustomerIntelligenceArea.Agile, AgileCustomerIntelligenceFeature.KanbanBoardAutoRefresh, properties);
      }
      catch (Exception ex)
      {
      }
    }

    private void AddUpdateEventData(
      Dictionary<string, List<WorkItemUpdateEventForExtension>> groupToUpdateEventsMap,
      WorkItemUpdateEventForExtension updateEvent,
      string groupName)
    {
      List<WorkItemUpdateEventForExtension> eventForExtensionList;
      if (!groupToUpdateEventsMap.TryGetValue(groupName, out eventForExtensionList))
      {
        eventForExtensionList = new List<WorkItemUpdateEventForExtension>();
        groupToUpdateEventsMap[groupName] = eventForExtensionList;
      }
      eventForExtensionList.Add(updateEvent);
    }

    protected abstract HashSet<string> GetNewGroupNames(
      IVssRequestContext requestContext,
      WorkItemChangedEventExtended workItemChangedEvent);

    protected abstract HashSet<string> GetOldGroupNames(
      IVssRequestContext requestContext,
      WorkItemChangedEventExtended workItemChangedEvent);
  }
}
