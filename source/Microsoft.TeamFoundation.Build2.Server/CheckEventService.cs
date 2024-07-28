// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.CheckEventService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class CheckEventService : ICheckEventService, IVssFrameworkService
  {
    public BuildCheckEvent AddCheckEvent(
      IVssRequestContext requestContext,
      BuildCheckEvent checkEvent)
    {
      using (requestContext.TraceScope(nameof (CheckEventService), nameof (AddCheckEvent)))
      {
        ArgumentUtility.CheckForNull<BuildCheckEvent>(checkEvent, nameof (checkEvent), "Build2");
        bool securityFixEnabled = requestContext.IsFeatureEnabled("Build2.EnableSecurityFixForCheckEventSerializer");
        BuildCheckEvent buildCheckEvent;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          buildCheckEvent = component.AddCheckEvent(checkEvent, securityFixEnabled);
        requestContext.TraceAlways(12030291, TraceLevel.Info, "Build2", nameof (CheckEventService), string.Format("Created check event. Type:{0} Build:{1} CheckEventId: {2}", (object) checkEvent.EventType, (object) checkEvent.BuildId, (object) checkEvent.CheckEventId));
        requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
        {
          JobExtensionIds.BuildCheckEventDeliveryJob
        }, false);
        return buildCheckEvent;
      }
    }

    public List<BuildCheckEvent> AddCheckEvents(
      IVssRequestContext requestContext,
      List<BuildCheckEvent> checkEvents)
    {
      using (requestContext.TraceScope(nameof (CheckEventService), nameof (AddCheckEvents)))
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) checkEvents, nameof (checkEvents), "Build2");
        bool securityFixEnabled = requestContext.IsFeatureEnabled("Build2.EnableSecurityFixForCheckEventSerializer");
        List<BuildCheckEvent> source;
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          source = component.AddCheckEvents(checkEvents, securityFixEnabled);
        if (source != null && source.Any<BuildCheckEvent>())
        {
          requestContext.TraceAlways(12030291, TraceLevel.Info, "Build2", nameof (CheckEventService), string.Join("\n", source.Select<BuildCheckEvent, string>((Func<BuildCheckEvent, string>) (x => string.Format("Created check event. Type:{0} Build:{1} CheckEventId: {2}", (object) x.EventType, (object) x.BuildId, (object) x.CheckEventId)))));
          requestContext.GetService<ITeamFoundationJobService>().QueueJobsNow(requestContext, (IEnumerable<Guid>) new Guid[1]
          {
            JobExtensionIds.BuildCheckEventDeliveryJob
          }, false);
        }
        return source;
      }
    }

    public CheckEventResults GetCheckEvents(IVssRequestContext requestContext, int? maxCount)
    {
      using (requestContext.TraceScope(nameof (CheckEventService), nameof (GetCheckEvents)))
      {
        bool securityFixEnabled = requestContext.IsFeatureEnabled("Build2.EnableSecurityFixForCheckEventSerializer");
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.GetCheckEvents(maxCount, securityFixEnabled);
      }
    }

    public List<BuildCheckEvent> UpateCheckEventStatus(
      IVssRequestContext requestContext,
      List<BuildCheckEvent> checkEvents)
    {
      using (requestContext.TraceScope(nameof (CheckEventService), nameof (UpateCheckEventStatus)))
      {
        ArgumentUtility.CheckForNull<List<BuildCheckEvent>>(checkEvents, nameof (checkEvents), "Build2");
        bool securityFixEnabled = requestContext.IsFeatureEnabled("Build2.EnableSecurityFixForCheckEventSerializer");
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
          return component.UpdateCheckEvents((IEnumerable<BuildCheckEvent>) checkEvents, securityFixEnabled);
      }
    }

    public void CleanupCheckEvents(IVssRequestContext requestContext)
    {
      using (requestContext.TraceScope(nameof (CheckEventService), nameof (CleanupCheckEvents)))
      {
        IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
        IVssRequestContext requestContext1 = requestContext;
        RegistryQuery registryQuery = (RegistryQuery) "/Service/Build/Settings/CheckEvents/FiredDaysToKeep";
        ref RegistryQuery local1 = ref registryQuery;
        int firedCheckEvents = BuildConstants.DefaultDaysToKeepFiredCheckEvents;
        int num1 = service.GetValue<int>(requestContext1, in local1, true, firedCheckEvents);
        IVssRequestContext requestContext2 = requestContext;
        registryQuery = (RegistryQuery) "/Service/Build/Settings/CheckEvents/FailedDaysToKeep";
        ref RegistryQuery local2 = ref registryQuery;
        int failedCheckEvents = BuildConstants.DefaultDaysToKeepFailedCheckEvents;
        int num2 = service.GetValue<int>(requestContext2, in local2, true, failedCheckEvents);
        DateTime minCreatedTime1 = DateTime.UtcNow.Subtract(TimeSpan.FromDays((double) num1));
        DateTime minCreatedTime2 = DateTime.UtcNow.Subtract(TimeSpan.FromDays((double) num2));
        using (Build2Component component = requestContext.CreateComponent<Build2Component>())
        {
          component.DeleteCheckEvents(CheckEventStatus.Fired, minCreatedTime1, new int?(1000));
          component.DeleteCheckEvents(CheckEventStatus.Failed, minCreatedTime2, new int?(1000));
        }
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
