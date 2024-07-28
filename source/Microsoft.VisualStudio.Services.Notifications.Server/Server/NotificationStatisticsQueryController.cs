// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.NotificationStatisticsQueryController
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  [ControllerApiVersion(3.0)]
  [ClientInclude(RestClientLanguages.CSharp | RestClientLanguages.TypeScript)]
  public class NotificationStatisticsQueryController : NotificationControllerBase
  {
    [HttpPost]
    public List<NotificationStatistic> QueryStatistics(NotificationStatisticsQuery statisticsQuery)
    {
      ArgumentUtility.CheckForNull<NotificationStatisticsQuery>(statisticsQuery, nameof (statisticsQuery));
      ArgumentUtility.CheckForNull<List<NotificationStatisticsQueryConditions>>(statisticsQuery.Conditions, "statisticsQuery.Conditions");
      this.LoggableDiagnosticParameters[nameof (statisticsQuery)] = (object) statisticsQuery;
      if (statisticsQuery.Conditions.Count == 0)
        throw new ArgumentException(CoreRes.MustProvideAtLeastOneCondition());
      return this.TfsRequestContext.GetService<IEventNotificationServiceInternal>().QueryNotificationStatistics(this.TfsRequestContext, (IEnumerable<NotificationStatisticsQueryConditions>) statisticsQuery.Conditions);
    }
  }
}
