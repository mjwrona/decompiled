// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventNotificationComponent1313
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  internal class EventNotificationComponent1313 : EventNotificationComponent1310
  {
    public override DateTime GetNextNotificationProcessTime(
      IEnumerable<string> channels,
      IEnumerable<string> processQueues)
    {
      DateTime notificationProcessTime = DateTime.MaxValue;
      this.PrepareStoredProcedure("prc_GetPendingNotificationLastProcessedTime");
      this.BindStringTable("@channels", channels, true, 25, false);
      this.BindStringTable("@processQueues", processQueues, true, 100, false);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder processedTimeBinder = new SqlColumnBinder("ProcessedTime");
        resultCollection.AddBinder<DateTime>((ObjectBinder<DateTime>) new SimpleObjectBinder<DateTime>((System.Func<IDataReader, DateTime>) (reader => processedTimeBinder.GetDateTime(reader))));
        List<DateTime> items = resultCollection.GetCurrent<DateTime>().Items;
        if (items.Any<DateTime>())
          notificationProcessTime = items.First<DateTime>().AddMinutes(5.0);
      }
      return notificationProcessTime;
    }
  }
}
