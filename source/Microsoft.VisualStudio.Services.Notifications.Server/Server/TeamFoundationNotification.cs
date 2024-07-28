// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationNotification
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.VisualStudio.Services.Notifications.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class TeamFoundationNotification : ICloneable
  {
    public int Id { get; set; }

    public TeamFoundationEvent Event { get; set; }

    public int Attempts { get; set; }

    public string Channel { get; set; }

    public string Metadata { get; set; }

    public string ProcessQueue { get; set; }

    public NotificationDeliveryDetails DeliveryDetails { get; set; }

    public DateTime CreatedTime { get; set; }

    public DateTime ProcessedTime { get; set; }

    public DateTime NextProcessTime { get; set; }

    public DateTime ExpirationTime { get; set; }

    public string EventData => this.Event?.EventData;

    public IFieldContainer EventFieldContainer => this.Event.GetFieldContainer();

    public bool IsEmailDelivery => NotificationFrameworkConstants.EmailTargetDeliveryChannels.Contains(this.Channel);

    public bool IsHtmlDelivery => NotificationFrameworkConstants.HtmlEmailTargetDeliveryChannels.Contains(this.Channel);

    public SendNotificationState SendNotificationState { get; set; }

    public string Result { get; set; }

    public string ResultDetail { get; set; }

    internal Guid SingleSubscriberOverrideId { get; set; }

    internal DiagnosticNotification DiagnosticNotification { get; set; }

    internal NotificationStopwatch DeliveryStopwatch { get; set; }

    internal int BisEvent { get; set; } = int.MinValue;

    public override string ToString() => CoreRes.NotificationFields((object) this.Event.EventType, (object) this.Channel);

    public object Clone()
    {
      TeamFoundationNotification foundationNotification = new TeamFoundationNotification();
      if (this.Event != null)
        foundationNotification.Event = (TeamFoundationEvent) this.Event.Clone();
      foundationNotification.Id = this.Id;
      foundationNotification.Attempts = this.Attempts;
      foundationNotification.Channel = this.Channel;
      foundationNotification.Metadata = this.Metadata;
      foundationNotification.ProcessedTime = this.ProcessedTime;
      foundationNotification.CreatedTime = this.CreatedTime;
      foundationNotification.NextProcessTime = this.NextProcessTime;
      foundationNotification.ExpirationTime = this.ExpirationTime;
      foundationNotification.ProcessQueue = this.ProcessQueue;
      foundationNotification.SendNotificationState = this.SendNotificationState;
      foundationNotification.DeliveryDetails = (NotificationDeliveryDetails) this.DeliveryDetails?.Clone();
      foundationNotification.Result = this.Result;
      foundationNotification.ResultDetail = this.ResultDetail;
      return (object) foundationNotification;
    }
  }
}
