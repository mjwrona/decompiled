// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsMetaData5_1Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(5.1)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultMetaData", ResourceVersion = 2)]
  public class ResultsMetaData5_1Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpPost]
    [ClientLocationId("AFA7830E-67A7-4336-8090-2B448CA80295")]
    public List<TestResultMetaData> QueryTestResultsMetaData([FromBody] List<string> testReferenceIds)
    {
      if (!this.TestManagementRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
        return this.ResultsHelper.GetTestResultsMetaData(this.ProjectInfo, testReferenceIds);
      if (testReferenceIds == null)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.InValidListOfIds, (object) nameof (testReferenceIds))));
      if (testReferenceIds.Count > 200)
        throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.GetTestResultsMetaDataApiMaxLimitError, (object) 200));
      List<int> testRefIdList = this.ResultsHelper.GetListOfRefIds(this.TestManagementRequestContext.RequestContext, testReferenceIds, nameof (testReferenceIds));
      return TestManagementController.InvokeAction<List<TestResultMetaData>>((Func<List<TestResultMetaData>>) (() =>
      {
        Task<List<TestResultMetaData>> task = this.TestResultsHttpClient.QueryTestResultsMetaDataAsync((IEnumerable<string>) testRefIdList.Select<int, string>((Func<int, string>) (x => x.ToString())).ToList<string>(), this.ProjectInfo.Id, new ResultMetaDataDetails?(ResultMetaDataDetails.None), (object) null, new CancellationToken());
        return task == null ? (List<TestResultMetaData>) null : task.Result.ToList<TestResultMetaData>();
      }));
    }

    internal ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new ResultsHelper(this.TestManagementRequestContext);
        return this.m_resultsHelper;
      }
    }
  }
}
