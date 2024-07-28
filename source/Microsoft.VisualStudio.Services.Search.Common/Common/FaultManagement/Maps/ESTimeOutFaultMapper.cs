// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps.ESTimeOutFaultMapper
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Net;

namespace Microsoft.VisualStudio.Services.Search.Common.FaultManagement.Maps
{
  public class ESTimeOutFaultMapper : FaultMapper
  {
    public ESTimeOutFaultMapper()
      : base("ESTimeOut", IndexerFaultSource.ElasticSearch)
    {
    }

    public override bool IsMatch(Exception ex)
    {
      if (ex == null || !ex.GetType().IsEquivalentTo(typeof (SearchPlatformException)))
        return false;
      Exception innerException1 = ex.InnerException;
      if (innerException1 == null)
        return false;
      if ("The underlying connection was closed: A connection that was expected to be kept alive was closed by the server.".Equals(innerException1.Message, StringComparison.OrdinalIgnoreCase))
        return true;
      WebException webException = (WebException) null;
      if (innerException1.GetType().IsAssignableFrom(typeof (WebException)))
        webException = (WebException) innerException1;
      else if (innerException1.GetType().IsAssignableFrom(typeof (AggregateException)))
      {
        foreach (Exception innerException2 in (innerException1 as AggregateException).InnerExceptions)
        {
          if (innerException2.GetType().IsAssignableFrom(typeof (WebException)))
          {
            webException = (WebException) innerException2;
            break;
          }
        }
      }
      if (webException == null)
        return false;
      if (webException.Status == WebExceptionStatus.Timeout)
        return true;
      if (webException.Status != WebExceptionStatus.ReceiveFailure && webException.Status != WebExceptionStatus.KeepAliveFailure)
        return false;
      string str = "A connection attempt failed because the connected party did not properly respond after a period of time, or established connection failed because connected host has failed to respond.";
      return webException.InnerException?.InnerException != null && str.Equals(webException.InnerException.InnerException.Message, StringComparison.OrdinalIgnoreCase);
    }
  }
}
