// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.EventServiceNotificationPublisher
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class EventServiceNotificationPublisher : INotificationPublisher
  {
    private const string Area = "Commerce";
    private const string Layer = "EventServiceNotificationPublisher";

    public void Publish(IVssRequestContext requestContext, object notification)
    {
      requestContext.TraceEnter(5105901, "Commerce", nameof (EventServiceNotificationPublisher), nameof (Publish));
      try
      {
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, notification);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(5105909, "Commerce", nameof (EventServiceNotificationPublisher), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(5105950, "Commerce", nameof (EventServiceNotificationPublisher), nameof (Publish));
      }
    }
  }
}
