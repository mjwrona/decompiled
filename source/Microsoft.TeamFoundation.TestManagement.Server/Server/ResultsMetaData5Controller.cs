// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ResultsMetaData5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ClientInternalUseOnly(true)]
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "ResultMetaData", ResourceVersion = 1)]
  public class ResultsMetaData5Controller : TestResultsControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [ClientLocationId("AFA7830E-67A7-4336-8090-2B448CA80295")]
    public List<TestResultMetaData> GetTestResultsMetaData([ClientParameterAsIEnumerable(typeof (int), ',')] string testReferenceIds) => throw new NotSupportedException("GetTestResultsMetaData Api version 5.0 Not Supported");

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
