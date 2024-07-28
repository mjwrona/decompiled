// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyCodeCoverageController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.FileContainer.Client;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "codecoverage", ResourceVersion = 1)]
  public class LegacyCodeCoverageController : TcmControllerBase
  {
    private CodeCoverageHelper m_codeCoverageHelper;

    [HttpGet]
    [ActionName("codecoverage")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>), null, null)]
    [ClientLocationId("B82BC30A-7228-4679-901C-0A59AC4B1252")]
    public HttpResponseMessage GetTestRunCodeCoverage(int runId, int flags) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>) this.CodeCovHelper.GetTestRunCodeCoverage(this.ProjectId.ToString(), runId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags));

    [HttpGet]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>), null, null)]
    [ClientLocationId("21DFFF92-0917-41BB-A4AF-E68922678713")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetBuildCodeCoverage(int buildId, int flags) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) this.CodeCovHelper.GetBuildCodeCoverage(this.ProjectId.ToString(), buildId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags), (ISecuredObject) new CodeCoverageSecuredObject(this.ProjectId));

    [HttpGet]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary), null, null)]
    [ClientLocationId("21DFFF92-0917-41BB-A4AF-E68922678713")]
    [PublicProjectRequestRestrictions]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary GetCodeCoverageSummary(
      int buildId,
      int deltaBuildId = -1)
    {
      return this.CodeCovHelper.GetCodeCoverageSummary(this.ProjectId.ToString(), buildId, deltaBuildId);
    }

    [HttpPost]
    [ActionName("BuildCodeCoverage")]
    [ClientLocationId("21DFFF92-0917-41BB-A4AF-E68922678713")]
    public void UpdateCodeCoverageSummary(int buildId, CodeCoverageData coverageData) => this.CodeCovHelper.UpdateCodeCoverageSummary(this.ProjectId.ToString(), buildId, coverageData, CoverageSummaryStatus.Completed);

    [HttpGet]
    [ActionName("BrowseCodeCoverage")]
    [ClientLocationId("7DA4458F-63AA-49FA-BA70-01600E50E1C9")]
    [ClientIgnore]
    public HttpResponseMessage GetCoverageView(long containerId, [ClientQueryParameter] string filePath)
    {
      try
      {
        HttpResponseMessage downloadResponse = this.CreateDownloadResponse(this.TestManagementRequestContext.RequestContext.GetClient<FileContainerHttpClient>(ServiceInstanceTypes.TFS).QueryContainerItemsAsync(containerId, this.ProjectId, filePath).SyncResult<List<FileContainerItem>>().Single<FileContainerItem>(), (string) null);
        HttpContentHeaders headers = downloadResponse.Content.Headers;
        long? contentLength = headers.ContentLength;
        long browsingContentSize = FrameworkServerConstants.MaxBrowsingContentSize;
        if (contentLength.GetValueOrDefault() > browsingContentSize & contentLength.HasValue)
          throw new ContainerItemContentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, FrameworkResources.MaximumBrowsingLimitExceeded((object) filePath)));
        if (headers.ContentDisposition != null)
          headers.ContentDisposition.DispositionType = "inline";
        else
          headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
          {
            FileName = this.CodeCovHelper.ExtractFilename(filePath)
          };
        headers.ContentType = new MediaTypeHeaderValue(this.CodeCovHelper.GetMimeTypeFromFileExtension(downloadResponse));
        return downloadResponse;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in getting code coverage view; Error = {0}, containerId = {1}, path = {2}", (object) ex.ToString(), (object) containerId, (object) filePath);
      }
      return new HttpResponseMessage(HttpStatusCode.BadRequest);
    }

    [HttpGet]
    [ActionName("DownloadCodeCoverage")]
    [ClientLocationId("4EB40B6B-B55E-4C51-8C59-C5FCC9E46994")]
    [ClientIgnore]
    public HttpResponseMessage DownloadCoverageReport(long containerId, [ClientQueryParameter] string filePath)
    {
      try
      {
        string downloadFileName = this.CodeCovHelper.GetDownloadFileName(containerId, filePath, string.Empty, false);
        CompressionType compressionType = CompressionType.None;
        Stream content = this.CodeCovHelper.DownloadCoverageReport(this.ProjectId, containerId, filePath);
        if (content == null)
          return this.Request.CreateResponse(HttpStatusCode.NotFound);
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
        {
          Project = new ShallowReference()
          {
            Id = this.ProjectId.ToString()
          }
        };
        response.Content = (HttpContent) new VssServerStreamContent(content, (object) securedObject);
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(AttachmentDownloadHelper.GetContentType(downloadFileName));
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(downloadFileName);
        if (compressionType == CompressionType.GZip)
          response.Content.Headers.ContentEncoding.Add("gzip");
        return response;
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceError("RestLayer", "Error occurred in downloading code coverage attachment; Error = {0}, containerId = {1}, path = {2}", (object) ex.ToString(), (object) containerId, (object) filePath);
      }
      return new HttpResponseMessage(HttpStatusCode.BadRequest);
    }

    private HttpResponseMessage CreateDownloadResponse(
      FileContainerItem item,
      string downloadFileName)
    {
      using (PerfManager.Measure(this.TfsRequestContext, "CrossService", TraceUtils.GetActionName(nameof (CreateDownloadResponse), "File")))
      {
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        CompressionType compressionType;
        Stream stream = this.TfsRequestContext.GetService<TeamFoundationFileService>().RetrieveFile(this.TfsRequestContext, (long) item.FileId, out compressionType);
        if (compressionType == CompressionType.GZip)
          stream = (Stream) new GZipStream(stream, CompressionMode.Decompress);
        response.Content = (HttpContent) new StringContent(SafeHtmlWrapper.MakeSafe(new StreamContent(stream).ReadAsStringAsync().SyncResult<string>()));
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(this.CodeCovHelper.GetDownloadFileName(item.ContainerId, item.Path, downloadFileName, false));
        return response;
      }
    }

    internal CodeCoverageHelper CodeCovHelper
    {
      get
      {
        if (this.m_codeCoverageHelper == null)
          this.m_codeCoverageHelper = new CodeCoverageHelper(this.TestManagementRequestContext);
        return this.m_codeCoverageHelper;
      }
    }
  }
}
