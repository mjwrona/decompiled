// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CvsFileDownloadController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [VersionedApiControllerCustomName(Area = "CvsFileDownload", ResourceName = "CvsFileDownload")]
  [ControllerApiVersion(5.0)]
  public class CvsFileDownloadController : TfsApiController
  {
    private const string c_area = "CvsFileDownload";
    private const string c_layer = "CvsFileDownloadController";
    private static readonly Guid s_signingKey = ProxyConstants.ProxySigningKey;

    public override string TraceArea => "CvsFileDownload";

    public override string ActivityLogArea => "CvsFileDownload";

    [HttpGet]
    [ClientResponseType(typeof (Stream), "CvsDownloadFile", "application/octet-stream")]
    public HttpResponseMessage DownloadFile()
    {
      this.TfsRequestContext.CheckProjectCollectionOrOrganizationRequestContext();
      IVssRequestContext vssRequestContext = this.TfsRequestContext.Elevate();
      using (ISigner signer = vssRequestContext.GetService<ITeamFoundationSigningService>().GetSigner(vssRequestContext, CvsFileDownloadController.s_signingKey))
      {
        DownloadContext downloadContext = new DownloadContext(this.Request.GetQueryNameValuePairs());
        downloadContext.RequireTicketType("rsa");
        downloadContext.DemandValidSignature(signer, DateTime.UtcNow);
        this.ValidateRequest(downloadContext);
        ITeamFoundationFileService service = this.TfsRequestContext.GetService<ITeamFoundationFileService>();
        FileStatistics fileStatistics = service.GetFileStatistics(this.TfsRequestContext, (long) downloadContext.FileId);
        this.ValidateFileAccess(downloadContext, fileStatistics);
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        long contentLength;
        CompressionType compressionType;
        Stream content = service.RetrieveFile(this.TfsRequestContext, (long) downloadContext.FileId, true, out byte[] _, out contentLength, out compressionType);
        response.Content = (HttpContent) new StreamContent(content);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        if (compressionType == CompressionType.GZip)
          response.Content.Headers.ContentEncoding.Add("gzip");
        response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = fileStatistics.FileName
        };
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_CVSFD_TotalDownloads").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_CVSFD_DownloadsPerSec").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_CVSFD_TotalBytesDownloaded").IncrementBy(contentLength);
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_CVSFD_BytesDownloadedPerSec").IncrementBy(contentLength);
        this.TfsRequestContext.UpdateTimeToFirstPage();
        return response;
      }
    }

    internal virtual void ValidateRequest(DownloadContext downloadContext)
    {
    }

    internal virtual void ValidateFileAccess(
      DownloadContext downloadContext,
      FileStatistics fileStatistics)
    {
    }

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<FileIdNotFoundException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<DownloadTicketValidationException>(HttpStatusCode.Forbidden);
    }
  }
}
