// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationServiceHostBinderBase`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationServiceHostBinderBase<T> : ObjectBinder<T>
  {
    protected override T Bind() => default (T);

    protected void ProcessServicingDetails(
      TeamFoundationServiceHostProperties hostProperties,
      string servicingDetails)
    {
      if (string.IsNullOrEmpty(servicingDetails))
        return;
      hostProperties.ServicingDetails = new List<ServicingJobDetail>((IEnumerable<ServicingJobDetail>) TeamFoundationSerializationUtility.Deserialize<ServicingJobDetail[]>(servicingDetails, new XmlRootAttribute("ArrayOfServicingJobDetail")));
      foreach (ServicingJobDetail servicingDetail in hostProperties.ServicingDetails)
      {
        DateTime dateTime1;
        if (servicingDetail.QueueTime != DateTime.MinValue)
        {
          ServicingJobDetail servicingJobDetail = servicingDetail;
          dateTime1 = servicingDetail.QueueTime;
          DateTime dateTime2 = new DateTime(dateTime1.Ticks, DateTimeKind.Utc);
          servicingJobDetail.QueueTime = dateTime2;
        }
        if (servicingDetail.StartTime != DateTime.MinValue)
        {
          ServicingJobDetail servicingJobDetail = servicingDetail;
          dateTime1 = servicingDetail.StartTime;
          DateTime dateTime3 = new DateTime(dateTime1.Ticks, DateTimeKind.Utc);
          servicingJobDetail.StartTime = dateTime3;
        }
        if (servicingDetail.EndTime != DateTime.MinValue)
        {
          ServicingJobDetail servicingJobDetail = servicingDetail;
          dateTime1 = servicingDetail.EndTime;
          DateTime dateTime4 = new DateTime(dateTime1.Ticks, DateTimeKind.Utc);
          servicingJobDetail.EndTime = dateTime4;
        }
        if (!string.IsNullOrEmpty(servicingDetail.OperationString))
          servicingDetail.Operations = servicingDetail.OperationString.Split(new char[1]
          {
            ','
          }, StringSplitOptions.RemoveEmptyEntries);
      }
    }
  }
}
