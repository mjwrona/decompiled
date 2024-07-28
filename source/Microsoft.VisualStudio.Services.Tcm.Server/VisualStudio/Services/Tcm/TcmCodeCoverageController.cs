// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmCodeCoverageController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

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
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "codecoverage", ResourceVersion = 1)]
  [AccessReadConsistencyFilter("TestManagement.Server.EnableSqlReadReplicaUsageInTcm")]
  public class TcmCodeCoverageController : TcmControllerBase
  {
    private CodeCoverageHelper m_codeCoverageHelper;

    [HttpGet]
    [ActionName("codecoverage")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>), null, null)]
    [ClientLocationId("5641EFBC-6F9B-401A-BAEB-D3DA22489E5E")]
    public HttpResponseMessage GetTestRunCodeCoverage(int runId, int flags) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>) this.CodeCovHelper.GetTestRunCodeCoverage(this.ProjectId.ToString(), runId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags));

    [HttpGet]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>), null, null)]
    [ClientLocationId("9B3E1ECE-C6AB-4FBB-8167-8A32A0C92216")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetBuildCodeCoverage(int buildId, int flags) => this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) this.CodeCovHelper.GetBuildCodeCoverage(this.ProjectId.ToString(), buildId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags), (ISecuredObject) new CodeCoverageSecuredObject(this.ProjectId));

    [HttpGet]
    [ActionName("FetchSourceCodeCoverageReport")]
    [ClientResponseType(typeof (IList<SourceViewBuildCoverage>), null, null)]
    [ClientLocationId("a459e10b-d703-4193-b3c1-60f2287918b3")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage FetchSourceCodeCoverageReport(int buildId) => this.GenerateResponse<SourceViewBuildCoverage>((IEnumerable<SourceViewBuildCoverage>) this.CodeCovHelper.FetchFolderViewCodeCoverageReport(this.ProjectId, buildId), (ISecuredObject) new CodeCoverageSecuredObject(this.ProjectId));

    [HttpGet]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary), null, null)]
    [ClientLocationId("9B3E1ECE-C6AB-4FBB-8167-8A32A0C92216")]
    [PublicProjectRequestRestrictions]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary GetCodeCoverageSummary(
      int buildId,
      int deltaBuildId = -1)
    {
      return this.CodeCovHelper.GetCodeCoverageSummary(this.ProjectId.ToString(), buildId, deltaBuildId);
    }

    [HttpPost]
    [ActionName("BuildCodeCoverage")]
    [ClientLocationId("9B3E1ECE-C6AB-4FBB-8167-8A32A0C92216")]
    public void UpdateCodeCoverageSummary(int buildId, CodeCoverageData coverageData) => this.CodeCovHelper.UpdateCodeCoverageSummary(this.ProjectId.ToString(), buildId, coverageData, CoverageSummaryStatus.Completed);

    [HttpPatch]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary), null, null)]
    [ClientLocationId("9B3E1ECE-C6AB-4FBB-8167-8A32A0C92216")]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary UpdateCodeCoverageSummary(
      int buildId)
    {
      return this.CodeCovHelper.QueueInvokerJobForPatchSummaryRequest(this.ProjectId, buildId);
    }

    [HttpGet]
    [ActionName("BrowseCodeCoverage")]
    [ClientLocationId("05F8E25B-1CD1-4572-BF73-2C04D719CF3D")]
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
    [ClientLocationId("2A021320-F757-4BDF-9A72-8ECA0974A4BA")]
    [ClientIgnore]
    [PublicProjectRequestRestrictions]
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
        Stream stream = this.TfsRequestContext.GetService<TeamFoundationFileContainerService>().RetrieveFile(this.TfsRequestContext, item.ContainerId, new Guid?(), item, true, out compressionType);
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
