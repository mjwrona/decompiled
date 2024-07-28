// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.SubscriptionEvaluationJobData
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class SubscriptionEvaluationJobData
  {
    public Subscription SubscriptionToEvaluate { get; set; }

    public DateTime EventsCreatedDate { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendFormat("EventsCreatedDate:{0}", (object) this.EventsCreatedDate);
      stringBuilder.AppendFormat(" SubscriptionId:{0}", (object) this.SubscriptionToEvaluate.ID);
      stringBuilder.AppendFormat(" EventType:{0}", (object) this.SubscriptionToEvaluate.SubscriptionFilter.EventType);
      stringBuilder.AppendFormat(" Condition:{0}", (object) this.SubscriptionToEvaluate.Condition.ToString());
      return stringBuilder.ToString();
    }
  }
}
