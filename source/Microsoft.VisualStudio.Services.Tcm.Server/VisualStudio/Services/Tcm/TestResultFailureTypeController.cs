// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TestResultFailureTypeController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "testfailuretype", ResourceVersion = 1)]
  public class TestResultFailureTypeController : TcmControllerBase
  {
    private ITestResultFailureTypeService m_testResultFailureTypeService;

    [HttpPost]
    [ClientLocationId("C4AC0486-830C-4A2A-9EF9-E8A1791A70FD")]
    [ClientResponseType(typeof (Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType), null, null)]
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType CreateFailureType(
      TestResultFailureTypeRequestModel testResultFailureType)
    {
      ArgumentUtility.CheckForNull<TestResultFailureTypeRequestModel>(testResultFailureType, nameof (testResultFailureType), "Test Results");
      Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType resultFailureType = new Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType();
      HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
      if (string.IsNullOrEmpty(testResultFailureType?.Name))
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
          Content = (HttpContent) new StringContent("Failure type name value cannot be empty or null")
        });
      if (testResultFailureType != null && testResultFailureType.Name.Length > 256)
        throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = (HttpContent) new StringContent("Failure type name value shouldn't exceed 256 characters")
        });
      return this.TestResultFailureTypeService.CreateTestResultFailureType(this.TestManagementRequestContext, testResultFailureType.Name, this.ProjectName);
    }

    [HttpGet]
    [ClientLocationId("C4AC0486-830C-4A2A-9EF9-E8A1791A70FD")]
    [ClientResponseType(typeof (List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType>), null, null)]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType> GetFailureTypes() => this.TestResultFailureTypeService.GetTestResultFailureType(this.TestManagementRequestContext, this.ProjectName);

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    [ClientLocationId("C4AC0486-830C-4A2A-9EF9-E8A1791A70FD")]
    public HttpResponseMessage DeleteFailureType(int failureTypeId)
    {
      HttpResponseMessage httpResponseMessage1 = new HttpResponseMessage();
      HttpResponseMessage httpResponseMessage2;
      if (failureTypeId == 5 || failureTypeId == 0)
        httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.InternalServerError)
        {
          Content = (HttpContent) new StringContent(string.Format("The failure type with Id {0} cannot be deleted.", (object) failureTypeId))
        };
      else if (!this.TestResultFailureTypeService.DeleteTestResultFailureType(this.TestManagementRequestContext, failureTypeId, this.ProjectName))
        httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
          Content = (HttpContent) new StringContent(string.Format("The failure type with Id {0} cannot be found.", (object) failureTypeId))
        };
      else
        httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = (HttpContent) new StringContent("Successful")
        };
      return httpResponseMessage2;
    }

    internal ITestResultFailureTypeService TestResultFailureTypeService
    {
      get
      {
        if (this.m_testResultFailureTypeService == null)
          this.m_testResultFailureTypeService = this.TestManagementRequestContext.RequestContext.GetService<ITestResultFailureTypeService>();
        return this.m_testResultFailureTypeService;
      }
    }
  }
}
