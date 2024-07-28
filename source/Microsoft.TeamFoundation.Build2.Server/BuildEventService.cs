// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEventService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildEventService : IBuildEventService, IVssFrameworkService
  {
    private const string c_refreshIntervalPath = "/Service/Build2/BuildEvent/RefreshIntervalSeconds";
    private const int c_refreshIntervalSecondsDefault = 60;
    private const string c_queueHealthyLimitPath = "/Service/Build2/BuildEvent/QueueHealthyLimit";
    private const int c_queueHealthyLimitDefault = 10000;
    private int m_queueHealthyLimit;
    private VssRefreshCache<BuildEventService.EventQueueData> m_eventQueueData;

    public BuildChangeEvent AddBuildEvent(
      IVssRequestContext requestContext,
      Guid projectId,
      int buildId,
      BuildEventType buildEventType)
    {
      using (requestContext.TraceScope(nameof (BuildEventService), nameof (AddBuildEvent)))
      {
        ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId), "Build2");
        ArgumentUtility.CheckForNonnegativeInt(buildId, nameof (buildId));
        BuildChangeEvent buildChangeEvent = (BuildChangeEvent) null;
        using (BuildEventComponent component = requestContext.CreateComponent<BuildEventComponent>())
          buildChangeEvent = component.AddBuildEvent(projectId, buildId, buildEventType);
        return buildChangeEvent;
      }
    }

    public BuildEventResults GetBuildEvents(
      IVssRequestContext requestContext,
      BuildEventStatus buildEventStatus,
      int? maxCount)
    {
      using (requestContext.TraceScope(nameof (BuildEventService), nameof (GetBuildEvents)))
      {
        using (BuildEventComponent component = requestContext.CreateComponent<BuildEventComponent>())
          return component.GetBuildEvents(buildEventStatus, maxCount);
      }
    }

    public bool IsQueueHealthy(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope(nameof (BuildEventService), nameof (IsQueueHealthy)))
      {
        BuildEventService.EventQueueData eventQueueData = this.m_eventQueueData.Get(requestContext);
        if (eventQueueData.EventCount > (long) this.m_queueHealthyLimit)
        {
          requestContext.Trace(12030268, TraceLevel.Verbose, "Build2", nameof (BuildEventService), "BuildEventQueue is not healthy. Unprocessed event count: {0}, Event count limit: {1}.  Some event listeners may limit their data to speed up processing of the event queue. (Control event count limit with registry key {2})", (object) eventQueueData.EventCount, (object) this.m_queueHealthyLimit, (object) "/Service/Build2/BuildEvent/QueueHealthyLimit");
          return false;
        }
      }
      return true;
    }

    public List<BuildChangeEvent> UpdateBuildEventsStatus(
      IVssRequestContext requestContext,
      List<BuildChangeEvent> buildEvents)
    {
      using (requestContext.TraceScope(nameof (BuildEventService), nameof (UpdateBuildEventsStatus)))
      {
        if (buildEvents == null || buildEvents.Count == 0)
          return buildEvents;
        ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) buildEvents, nameof (buildEvents));
        List<BuildChangeEvent> buildChangeEventList = buildEvents;
        using (BuildEventComponent component = requestContext.CreateComponent<BuildEventComponent>())
          buildChangeEventList = component.UpdateBuildEventsStatus((IEnumerable<BuildChangeEvent>) buildEvents);
        return buildChangeEventList;
      }
    }

    public void DeleteBuildEvents(
      IVssRequestContext requestContext,
      BuildEventStatus buildEventStatus,
      DateTime minCreatedTime,
      int? batchSize)
    {
      using (requestContext.TraceScope(nameof (BuildEventService), nameof (DeleteBuildEvents)))
      {
        ArgumentUtility.CheckForDateTimeRange(minCreatedTime, nameof (minCreatedTime), (DateTime) SqlDateTime.MinValue, (DateTime) SqlDateTime.MaxValue);
        using (BuildEventComponent component = requestContext.CreateComponent<BuildEventComponent>())
          component.DeleteBuildEvents(buildEventStatus, minCreatedTime, batchSize);
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      IVssRegistryService vssRegistryService = systemRequestContext.IsSystemContext ? systemRequestContext.GetService<IVssRegistryService>() : throw new BuildEventPermissionException(BuildServerResources.BuildEventPermissionError());
      IVssRegistryService registryService1 = vssRegistryService;
      IVssRequestContext requestContext1 = systemRequestContext;
      RegistryQuery registryQuery = (RegistryQuery) "/Service/Build2/BuildEvent/RefreshIntervalSeconds";
      ref RegistryQuery local1 = ref registryQuery;
      int num = registryService1.GetValue<int>(requestContext1, in local1, 60);
      IVssRegistryService registryService2 = vssRegistryService;
      IVssRequestContext requestContext2 = systemRequestContext;
      registryQuery = (RegistryQuery) "/Service/Build2/BuildEvent/QueueHealthyLimit";
      ref RegistryQuery local2 = ref registryQuery;
      this.m_queueHealthyLimit = registryService2.GetValue<int>(requestContext2, in local2, 10000);
      this.m_eventQueueData = new VssRefreshCache<BuildEventService.EventQueueData>(TimeSpan.FromSeconds((double) num), new Func<IVssRequestContext, BuildEventService.EventQueueData>(this.GetQueueData), true);
    }

    private BuildEventService.EventQueueData GetQueueData(IVssRequestContext requestContext)
    {
      long buildEventQueueData;
      using (BuildEventComponent component = requestContext.CreateComponent<BuildEventComponent>())
        buildEventQueueData = component.GetBuildEventQueueData();
      return new BuildEventService.EventQueueData()
      {
        EventCount = buildEventQueueData
      };
    }

    private class EventQueueData
    {
      public long EventCount { get; set; }
    }
  }
}
