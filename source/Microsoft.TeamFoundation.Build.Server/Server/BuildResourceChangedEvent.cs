// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildResourceChangedEvent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Build.Server
{
  [NotificationEventBindings(EventSerializerType.Xml, "ms.vss-build.build-resource-changed-event")]
  public sealed class BuildResourceChangedEvent : ChangedEvent
  {
    private HashSet<string> PublishedAgentProperties = new HashSet<string>((IEnumerable<string>) new string[10]
    {
      "Name",
      "BuildDirectory",
      "ControllerUri",
      "Description",
      "Enabled",
      "ReservedForBuild",
      nameof (ServiceHostUri),
      "Status",
      "StatusMessage",
      "Tags"
    });
    private HashSet<string> PublishedControllerProperties = new HashSet<string>((IEnumerable<string>) new string[9]
    {
      "Name",
      "Description",
      nameof (ServiceHostUri),
      "Enabled",
      "Status",
      "StatusMessage",
      "CustomAssemblyPath",
      "MaxConcurrentBuilds",
      "QueueCount"
    });

    public BuildResourceChangedEvent() => this.PropertyChanges = new List<PropertyChange>();

    internal BuildResourceChangedEvent(
      IVssRequestContext requestContext,
      BuildAgent oldAgent,
      BuildAgentUpdateOptions update)
      : this()
    {
      this.ServiceHostUri = oldAgent.ServiceHostUri;
      this.SendNotificationToBuildMachine = (update.Fields & (BuildAgentUpdate.ControllerUri | BuildAgentUpdate.BuildDirectory)) != 0;
      EventHelper.FillChangedEventFromUpdate<BuildAgent, BuildAgentUpdateOptions>(requestContext, (ChangedEvent) this, oldAgent, update);
      foreach (PropertyChange propertyChange in EventHelper.CommonPropertiesDeltaFromUpdate<BuildAgent, BuildAgentUpdateOptions>(oldAgent, update, this.PublishedAgentProperties))
      {
        this.PropertyChanges.Add(propertyChange);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertyChange.Name, (object) propertyChange.OldValue, (object) propertyChange.NewValue);
      }
    }

    internal BuildResourceChangedEvent(
      IVssRequestContext requestContext,
      BuildController oldController,
      BuildControllerUpdateOptions update)
      : this()
    {
      this.ServiceHostUri = oldController.ServiceHostUri;
      this.SendNotificationToBuildMachine = (update.Fields & BuildControllerUpdate.CustomAssemblyPath) == BuildControllerUpdate.CustomAssemblyPath;
      EventHelper.FillChangedEventFromUpdate<BuildController, BuildControllerUpdateOptions>(requestContext, (ChangedEvent) this, oldController, update);
      foreach (PropertyChange propertyChange in EventHelper.CommonPropertiesDeltaFromUpdate<BuildController, BuildControllerUpdateOptions>(oldController, update, this.PublishedControllerProperties))
      {
        this.PropertyChanges.Add(propertyChange);
        requestContext.Trace(0, TraceLevel.Info, "Build", "Notification", "Property changed: Name={0} OldValue={1} NewValue={2}", (object) propertyChange.Name, (object) propertyChange.OldValue, (object) propertyChange.NewValue);
      }
    }

    internal BuildResourceChangedEvent(
      IVssRequestContext requestContext,
      ChangedType action,
      BuildAgent agent)
      : this()
    {
      this.ServiceHostUri = agent.ServiceHostUri;
      this.SendNotificationToBuildMachine = true;
      EventHelper.FillAddedOrDeletedEvent<BuildAgent>(requestContext, action, (ChangedEvent) this, agent, this.PublishedAgentProperties);
    }

    internal BuildResourceChangedEvent(
      IVssRequestContext requestContext,
      ChangedType action,
      BuildController controller)
      : this()
    {
      this.ServiceHostUri = controller.ServiceHostUri;
      this.SendNotificationToBuildMachine = true;
      EventHelper.FillAddedOrDeletedEvent<BuildController>(requestContext, action, (ChangedEvent) this, controller, this.PublishedControllerProperties);
    }

    internal string ServiceHostUri { get; set; }

    internal bool SendNotificationToBuildMachine { get; set; }
  }
}
