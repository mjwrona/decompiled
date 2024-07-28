// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmResultsDocumentController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "ResultDocument", ResourceVersion = 1)]
  public class TcmResultsDocumentController : TcmControllerBase
  {
    private ResultsHelper _resultsHelper;

    [HttpPost]
    [ClientLocationId("74838649-B038-42F1-A0E7-6DEB3973BF14")]
    public TestResultDocument PublishTestResultDocument(int runId, TestResultDocument document) => this.ResultsHelper.PublishTestResultDocument(this.ProjectInfo, runId, document);

    internal ResultsHelper ResultsHelper
    {
      get => this._resultsHelper ?? new ResultsHelper(this.TestManagementRequestContext);
      set => this._resultsHelper = value;
    }
  }
}
