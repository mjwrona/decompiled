// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanning.TestCaseExportController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.Server.RestControllers.Helpers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestManagement.TestPlanning.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server.TestPlanning
{
  [ClientGroupByResource("Test Case Export")]
  [ControllerApiVersion(1.0)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "testplan", ResourceName = "TestCaseExport", ResourceVersion = 1)]
  public class TestCaseExportController : TestManagementController
  {
    private ITestCaseExportService m_testCaseExportService;

    [HttpPost]
    [ActionName("TestCaseExport")]
    [ClientLocationId("3b9d1c87-6b1a-4e7d-9e7d-1a8e543112bb")]
    [ClientResponseType(typeof (Stream), "ExportTestCases", "application/octet-stream")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage ExportTestCase(ExportTestCaseParams exportTestCaseRequestBody)
    {
      HttpResponseMessage response1;
      try
      {
        ArgumentUtility.CheckForNull<ExportTestCaseParams>(exportTestCaseRequestBody, nameof (exportTestCaseRequestBody), this.TfsRequestContext.ServiceName);
        ArgumentUtility.CheckType<int>((object) exportTestCaseRequestBody.testPlanId, "exportTestCaseRequestBody.testPlanId", "int");
        ArgumentUtility.CheckType<int>((object) exportTestCaseRequestBody.testSuiteId, "exportTestCaseRequestBody.testSuiteId", "int");
        ArgumentUtility.CheckType<List<int>>((object) exportTestCaseRequestBody.testCaseIds, "exportTestCaseRequestBody.testCaseIds", "List");
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) exportTestCaseRequestBody.testCaseIds, "exportTestCaseRequestBody.testCaseIds", this.TfsRequestContext.ServiceName);
        byte[] excel = this.TestCaseExportService.ExportSuiteTestCasesToExcel(this.TestManagementRequestContext, exportTestCaseRequestBody.testPlanId, exportTestCaseRequestBody.testSuiteId, exportTestCaseRequestBody.testCaseIds);
        response1 = this.Request.CreateResponse(HttpStatusCode.OK);
        response1.Content = (HttpContent) new ByteArrayContent(excel);
        response1.Content.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        response1.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
        {
          FileName = "TestCases.xlsx"
        };
      }
      catch (ArgumentException ex)
      {
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          Content = (HttpContent) new StringContent("Invalid/incomplete request parameters " + ex.Message)
        });
      }
      catch (Exception ex)
      {
        HttpResponseMessage response2 = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = (HttpContent) new StringContent(ex.Message)
        };
        this.TestManagementRequestContext.RequestContext.TraceError("RestLayer", "TestCaseExportController.ExportTestCase: TestCaseExportService threw exception. projectId = {0}, testPlanId = {1}, Exception = {2}", (object) this.ProjectId, (object) exportTestCaseRequestBody.testPlanId, (object) ex.Message);
        throw new HttpResponseException(response2);
      }
      return response1;
    }

    internal ITestCaseExportService TestCaseExportService
    {
      get
      {
        if (this.m_testCaseExportService == null)
          this.m_testCaseExportService = this.TestManagementRequestContext.RequestContext.GetService<ITestCaseExportService>();
        return this.m_testCaseExportService;
      }
    }
  }
}
