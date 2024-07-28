// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Search.ApiSearchController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Search, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CC5F1FD-9493-4B23-B40F-49E474A0E625
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Search.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Search
{
  [SupportedRouteArea("Api", NavigationContextLevels.All)]
  [DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  [OutputCache(CacheProfile = "NoCache")]
  public class ApiSearchController : TfsAreaController
  {
    private long connectionSetupTime;
    private const string TraceMessageFormat = "Value of Kpi {0} is {1}";
    private const int SearchControllerGetProjectRepositoriesTracePoint = 1080721;
    private const int SearchControllerGetFileContentTracePoint = 1080724;

    protected SearchHttpClient SearchClient
    {
      get
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        SearchHttpClient client = this.TfsRequestContext.GetClient<SearchHttpClient>();
        stopwatch.Stop();
        this.connectionSetupTime = stopwatch.ElapsedMilliseconds;
        return client;
      }
    }

    public override string AreaName => "Search";

    public override string TraceArea => "WebAccess.Search";

    [AcceptVerbs(HttpVerbs.Get)]
    public async Task<ActionResult> GetFileContent(
      string scope,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath)
    {
      ApiSearchController searchController = this;
      Stopwatch sw = Stopwatch.StartNew();
      string fileContent1 = await searchController.SearchClient.GetFileContent(projectName, repositoryName, branchName, filePath);
      sw.Stop();
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Value of Kpi {0} is {1}", (object) nameof (GetFileContent), (object) sw.ElapsedMilliseconds);
      searchController.TfsRequestContext.Trace(1080724, TraceLevel.Info, searchController.TraceArea, TfsTraceLayers.Controller, message);
      ActionResult fileContent2 = (ActionResult) searchController.Json((object) fileContent1, JsonRequestBehavior.AllowGet);
      sw = (Stopwatch) null;
      return fileContent2;
    }

    [AcceptVerbs(HttpVerbs.Get)]
    public FileResult GetFileDownload(
      string scope,
      string projectName,
      string repositoryName,
      string branchName,
      string filePath,
      string fileName,
      bool contentsOnly)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      string result = this.SearchClient.GetFileContent(projectName, repositoryName, branchName, filePath).Result;
      stopwatch.Stop();
      string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Value of Kpi {0} is {1}", (object) nameof (GetFileDownload), (object) stopwatch.ElapsedMilliseconds);
      this.TfsRequestContext.Trace(1080721, TraceLevel.Info, this.TraceArea, TfsTraceLayers.Controller, message);
      byte[] numArray = new byte[result.Length * 2];
      Buffer.BlockCopy((Array) result.ToCharArray(), 0, (Array) numArray, 0, numArray.Length);
      return contentsOnly ? (FileResult) this.File(numArray, MimeMapping.GetMimeMapping(fileName)) : (FileResult) this.File(numArray, MimeMapping.GetMimeMapping(fileName), fileName);
    }
  }
}
