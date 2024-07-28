// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.ChartsDeletedNotification
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class ChartsDeletedNotification
  {
    public static readonly Guid SqlEventId = new Guid("E1C551DA-14EF-4224-BAED-B31005E983B7");
    private const string s_payloadName = "chart-deletion-notification";
    public List<Guid> DeletedChartIds;

    internal static void SendNotification(
      IVssRequestContext requestContext,
      IEnumerable<Guid> deletedChartIds)
    {
      string eventData = TeamFoundationSerializationUtility.SerializeToString<ChartsDeletedNotification>(new ChartsDeletedNotification()
      {
        DeletedChartIds = deletedChartIds.ToList<Guid>()
      }, new XmlRootAttribute("chart-deletion-notification"));
      requestContext.GetService<TeamFoundationSqlNotificationService>().SendNotification(requestContext, ChartsDeletedNotification.SqlEventId, eventData);
    }

    public static ChartsDeletedNotification Deserialize(string serializedPayload) => TeamFoundationSerializationUtility.Deserialize<ChartsDeletedNotification>(serializedPayload, new XmlRootAttribute("chart-deletion-notification"));
  }
}
