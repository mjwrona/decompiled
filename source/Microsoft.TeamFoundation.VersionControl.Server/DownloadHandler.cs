// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.DownloadHandler
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  public class DownloadHandler : VersionControlHttpHandler
  {
    protected override string Layer => "VCDownloadHandler";

    protected override MethodType MethodType => MethodType.Normal;

    protected override EstimatedMethodCost EstimatedMethodCost => EstimatedMethodCost.VeryLow;

    protected override void Execute()
    {
      try
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentFileDownloads").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentFileDownloadsPerSec").Increment();
        MethodInformation methodInformation = this.GetMethodInformation();
        HttpRequest request = HttpContext.Current.Request;
        HttpResponse response1 = HttpContext.Current.Response;
        DownloadContext downloadContext1 = new DownloadContext(request.QueryString);
        methodInformation.AddParameter("fileId", (object) downloadContext1.FileId);
        this.EnterMethod(methodInformation);
        GenericDownloadHandler genericDownloadHandler = new GenericDownloadHandler();
        this.RequestContext.UpdateTimeToFirstPage();
        IVssRequestContext requestContext = this.RequestContext;
        DownloadContext downloadContext2 = downloadContext1;
        HttpContextBase handlerHttpContext = this.HandlerHttpContext;
        HttpResponseWrapper response2 = new HttpResponseWrapper(response1);
        GenericDownloadHandler.HandleErrorDelegate errorDelegate = (GenericDownloadHandler.HandleErrorDelegate) ((exception, exceptionHeader, statusCode, responseStarted) => this.HandleException(exception, exceptionHeader, statusCode, responseStarted));
        bool? useCache = new bool?();
        genericDownloadHandler.DownloadFile(requestContext, downloadContext2, handlerHttpContext, (HttpResponseBase) response2, errorDelegate, useCache);
      }
      catch (Exception ex)
      {
        this.HandleException(ex, "X-VersionControl-Exception", 500, false);
      }
      finally
      {
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.VersionControl.Server.PerformanceCounters.CurrentFileDownloads").Decrement();
      }
    }
  }
}
