// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.DownloadHandler
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Specialized;
using System.Web;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class DownloadHandler : FileHttpHandler
  {
    public DownloadHandler()
    {
    }

    protected DownloadHandler(HttpContextBase httpContext)
      : base(httpContext)
    {
    }

    protected override void Execute()
    {
      try
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.CurrentFileDownloads").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.CurrentFileDownloadsPerSec").Increment();
        MethodInformation methodInformation = new MethodInformation("FileDownloadHandler", MethodType.Normal, EstimatedMethodCost.VeryLow, true, true);
        HttpRequestBase request = this.HandlerHttpContext.Request;
        HttpResponseBase response = this.HandlerHttpContext.Response;
        this.EnterMethod(methodInformation);
        this.RequestContext.UpdateTimeToFirstPage();
        if (this.TryRetrieveFromRemote(request.QueryString, response))
          return;
        DownloadContext downloadContext = new DownloadContext(request.QueryString);
        methodInformation.AddParameter("fileId", (object) downloadContext.FileId);
        new GenericDownloadHandler().DownloadFile(this.RequestContext, downloadContext, this.HandlerHttpContext, response, (GenericDownloadHandler.HandleErrorDelegate) ((exception, exceptionHeader, statusCode, responseStarted) => this.HandleException(exception, exceptionHeader, statusCode, responseStarted)));
      }
      catch (Exception ex)
      {
        this.HandleException(ex, "X-VersionControl-Exception", 500, false);
      }
      finally
      {
        this.LeaveMethod();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.PerformanceCounters.Proxy.CurrentFileDownloads").Decrement();
      }
    }

    private bool TryRetrieveFromRemote(NameValueCollection queryString, HttpResponseBase response) => new TcmAttachmentsDownloadHandler(this.RequestContext, queryString).TryDownloadAndCopyTo(response);
  }
}
