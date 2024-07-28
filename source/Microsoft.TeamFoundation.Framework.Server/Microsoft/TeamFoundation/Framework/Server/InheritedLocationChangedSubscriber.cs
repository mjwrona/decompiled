// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.InheritedLocationChangedSubscriber
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class InheritedLocationChangedSubscriber : ISubscriber
  {
    private static readonly Type[] s_types = new Type[1]
    {
      typeof (List<ServiceDefinition>)
    };
    private const string c_area = "LocationService";
    private const string c_layer = "InheritedLocationChangedSubscriber";

    public string Name => nameof (InheritedLocationChangedSubscriber);

    public SubscriberPriority Priority => SubscriberPriority.Normal;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      requestContext.TraceEnter(92000, "LocationService", nameof (InheritedLocationChangedSubscriber), nameof (ProcessEvent));
      try
      {
        statusCode = 0;
        statusMessage = (string) null;
        properties = (ExceptionPropertyCollection) null;
        if (notificationType == NotificationType.Notification && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          requestContext.GetService<InheritedLocationDataService>().OnLocationDataChanged(requestContext, LocationDataKind.Local);
        return EventNotificationStatus.ActionPermitted;
      }
      finally
      {
        requestContext.TraceLeave(92001, "LocationService", nameof (InheritedLocationChangedSubscriber), nameof (ProcessEvent));
      }
    }

    public Type[] SubscribedTypes() => InheritedLocationChangedSubscriber.s_types;
  }
}
