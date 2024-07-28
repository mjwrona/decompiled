// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.FileCoverageController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "filecoverage", ResourceVersion = 1)]
  public class FileCoverageController : TcmControllerBase
  {
    private CodeCoverageHelper m_codeCoverageHelper;

    [HttpPost]
    [ActionName("GetFileLevelCodeCoverage")]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetFileLevelCodeCoverage", MediaType = "text/plain")]
    [ClientLocationId("4A6D0C46-51CA-45AA-9163-249CEE3289B7")]
    public async Task<HttpResponseMessage> GetFileLevelCodeCoverage(
      FileCoverageRequest fileCoverageRequest)
    {
      FileCoverageController coverageController = this;
      if (!coverageController.TestManagementRequestContext.IsFeatureEnabled("TestManagement.Server.FileLevelCoverageReport"))
        return coverageController.Request.CreateResponse<int>(204);
      HttpResponseMessage response = coverageController.Request.CreateResponse(HttpStatusCode.OK);
      MemoryStream contentStream = new MemoryStream();
      try
      {
        // ISSUE: explicit non-virtual call
        await coverageController.CodeCovHelper.GetFileLevelCoverageAsync(fileCoverageRequest, __nonvirtual (coverageController.ProjectInfo), (Stream) contentStream).ConfigureAwait(false);
        response.Content = (HttpContent) new PushStreamContent((Func<Stream, HttpContent, TransportContext, Task>) (async (outputStream, httpContent, transportContext) =>
        {
          try
          {
            this.TfsRequestContext.UpdateTimeToFirstPage();
            if (contentStream == null)
              return;
            using (contentStream)
            {
              contentStream.Position = 0L;
              await contentStream.CopyToAsync(outputStream);
            }
          }
          catch (Exception ex)
          {
            this.TfsRequestContext.TraceError("RestLayer", "FileCoverageController.GetFileLevelCodeCoverage: Error: " + ex.ToString());
          }
          finally
          {
            outputStream?.Dispose();
          }
        }), new MediaTypeHeaderValue("text/plain"));
      }
      catch (Exception ex)
      {
        coverageController.TfsRequestContext.TraceError("RestLayer", "FileCoverageController.GetFileLevelCodeCoverage: Error: " + ex.ToString());
        response.StatusCode = HttpStatusCode.InternalServerError;
      }
      return response;
    }

    private CodeCoverageHelper CodeCovHelper
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
