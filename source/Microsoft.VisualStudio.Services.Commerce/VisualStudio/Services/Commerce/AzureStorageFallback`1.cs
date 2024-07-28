// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureStorageFallback`1
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AzureStorageFallback<T> where T : class
  {
    private const string Layer = "AzureStorageFallback";

    public TableResult ReportFallback(IVssRequestContext requestContext, T eventData)
    {
      requestContext.TraceEnter(5108413, "Commerce", nameof (AzureStorageFallback<T>), nameof (ReportFallback));
      try
      {
        if ((object) eventData == null)
        {
          requestContext.Trace(5108410, TraceLevel.Info, "Commerce", nameof (AzureStorageFallback<T>), "No event data available");
          return new TableResult() { HttpStatusCode = 500 };
        }
        XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) eventData);
        ITeamFoundationJobService service = requestContext.GetService<ITeamFoundationJobService>();
        int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Commerce/AzureStorageFallback/SchedulingTimeSpanHours", 4);
        TimeSpan timeSpan = new TimeSpan(num == 0 ? 4 : num, 0, 0);
        IVssRequestContext requestContext1 = requestContext;
        XmlNode jobData = xml;
        TimeSpan startOffset = timeSpan;
        Guid guid = service.QueueOneTimeJob(requestContext1, "UsageMeteringStorageFallbackJobVSOCommerce", "Microsoft.VisualStudio.Services.Commerce.UsageMeteringStorageFallbackJobExtension", jobData, startOffset);
        requestContext.Trace(5108419, TraceLevel.Info, "Commerce", nameof (AzureStorageFallback<T>), string.Format("Fallback job with Id {0} scheduled", (object) guid));
        return new TableResult() { HttpStatusCode = 202 };
      }
      finally
      {
        requestContext.TraceLeave(5108414, "Commerce", nameof (AzureStorageFallback<T>), nameof (ReportFallback));
      }
    }

    [DebuggerStepThrough]
    public TableResult ReportEventFallback(IVssRequestContext requestContext, T eventData) => this.ReportFallback(requestContext, eventData);
  }
}
