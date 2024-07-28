// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.ResultsMetaDataController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "ResultMetaData", ResourceVersion = 1)]
  public class ResultsMetaDataController : TcmControllerBase
  {
    private ResultsHelper m_resultsHelper;

    [HttpGet]
    [ClientLocationId("B72FF4C0-4341-4213-BA27-F517CF341C95")]
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
