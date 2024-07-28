// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.CodeCoverageController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.FileContainer;
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

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "CodeCoverage", ResourceVersion = 1)]
  [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
  public class CodeCoverageController : TestManagementController
  {
    private CodeCoverageHelper m_codeCoverageHelper;

    [HttpGet]
    [ActionName("CodeCoverage")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>), null, null)]
    [ClientLocationId("9629116F-3B89-4ED8-B358-D4694EFDA160")]
    [ClientExample("GET__test_runs__runId__codeCoverage_flags-7.json", null, null, null)]
    public HttpResponseMessage GetTestRunCodeCoverage(int runId, int flags) => !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>) this.CodeCovHelper.GetTestRunCodeCoverage(this.ProjectId.ToString(), runId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags)) : this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>) TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestRunCoverage>>) (() => this.TestResultsHttpClient.GetTestRunCodeCoverageAsync(this.ProjectId, runId, flags)?.Result)));

    [HttpGet]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (IList<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>), null, null)]
    [ClientLocationId("77560E8A-4E8C-4D59-894E-A5F264C24444")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET__test_codeCoverage_buildId-_buildIdParam__flags-7.json", null, null, null)]
    public HttpResponseMessage GetBuildCodeCoverage(int buildId, int flags) => !this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS") ? this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) this.CodeCovHelper.GetBuildCodeCoverage(this.ProjectId.ToString(), buildId, (Microsoft.TeamFoundation.TestManagement.Client.CoverageQueryFlags) flags), (ISecuredObject) new CodeCoverageSecuredObject(this.ProjectId)) : this.GenerateResponse<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>) TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>>) (() =>
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage> result = this.TestResultsHttpClient.GetBuildCodeCoverageAsync(this.ProjectId, buildId, flags)?.Result;
      if (result != null && result.Any<Microsoft.TeamFoundation.TestManagement.WebApi.BuildCoverage>())
      {
        foreach (TestManagementBaseSecuredObject baseSecuredObject in result)
          baseSecuredObject.InitializeSecureObject((ISecuredObject) new CodeCoverageSecuredObject(this.ProjectId));
      }
      return result;
    })), (ISecuredObject) new CodeCoverageSecuredObject(this.ProjectId));

    [HttpGet]
    [ActionName("BuildCodeCoverage")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary), null, null)]
    [ClientLocationId("77560E8A-4E8C-4D59-894E-A5F264C24444")]
    [PublicProjectRequestRestrictions]
    [ClientInternalUseOnly(false)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.CodeCoverageSummary GetCodeCoverageSummary(
      int buildId,
      int deltaBuildId = -1)
    {
      return this.CodeCovHelper.GetCodeCoverageSummary(this.ProjectId.ToString(), buildId, deltaBuildId);
    }

    [HttpPost]
    [ActionName("BuildCodeCoverage")]
    [ClientLocationId("77560E8A-4E8C-4D59-894E-A5F264C24444")]
    [ClientInternalUseOnly(false)]
    public void UpdateCodeCoverageSummary(int buildId, CodeCoverageData coverageData)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        this.CodeCovHelper.UpdateCodeCoverageSummary(this.ProjectId.ToString(), buildId, coverageData, CoverageSummaryStatus.Completed);
      else
        this.TestResultsHttpClient.UpdateCodeCoverageSummaryAsync(coverageData, this.ProjectId, buildId)?.Wait();
    }

    [HttpGet]
    [ActionName("BrowseCodeCoverage")]
    [ClientLocationId("5A37D0E4-C49D-4B18-9EC1-E7CAE9914E71")]
    [ClientIgnore]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetCoverageView(long containerId, [ClientQueryParameter] string filePath)
    {
      try
      {
        HttpResponseMessage downloadResponse = this.CreateDownloadResponse(this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationFileContainerService>().QueryItems(this.TestManagementRequestContext.RequestContext, containerId, filePath, this.ProjectId).Single<FileContainerItem>(), (string) null);
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
        response.Content = (HttpContent) new VssServerStringContent(SafeHtmlWrapper.MakeSafe(new StreamContent(stream).ReadAsStringAsync().SyncResult<string>()), (object) new CodeCoverageSecuredObject(this.ProjectId));
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
