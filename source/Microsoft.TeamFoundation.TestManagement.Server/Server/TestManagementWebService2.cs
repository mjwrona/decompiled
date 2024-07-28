// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementWebService2
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Web.Services;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [WebService(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/07/TCM/TestManagement/01", Description = "Test Management Service", Name = "TestManagementWebService2")]
  [ClientService(ComponentName = "TestManagement", RegistrationName = "TestManagement", ServiceName = "TestManagementWebService2", CollectionServiceIdentifier = "48627723-8AC1-4E9F-BEE0-2179914AE783")]
  public class TestManagementWebService2 : BaseTestManagementWebService
  {
    private TfsTestManagementRequestContext m_tmRequestContext;

    public TestManagementWebService2()
    {
      this.RequestContext.ServiceName = "Test Management 2";
      this.m_tmRequestContext = new TfsTestManagementRequestContext(this.RequestContext);
    }

    [WebMethod]
    public int BeginCloneOperation(
      int fromSuiteId,
      int toSuiteId,
      string projectName,
      string targetProjectName,
      CloneOptions options)
    {
      try
      {
        MethodInformation methodInformation = new MethodInformation(nameof (BeginCloneOperation), MethodType.Normal, EstimatedMethodCost.Low);
        methodInformation.AddParameter(nameof (fromSuiteId), (object) fromSuiteId);
        methodInformation.AddParameter(nameof (toSuiteId), (object) toSuiteId);
        methodInformation.AddParameter(nameof (projectName), (object) projectName);
        methodInformation.AddParameter(nameof (targetProjectName), (object) targetProjectName);
        methodInformation.AddParameter(nameof (options), (object) options);
        this.EnterMethod(methodInformation);
        this.m_tmRequestContext.TraceVerbose("WebService", "BeginCloneOperation: {0}, {1}", (object) fromSuiteId, (object) toSuiteId);
        ArgumentUtility.CheckForNull<CloneOptions>(options, nameof (options), this.m_tmRequestContext.RequestContext.ServiceName);
        return ServerTestSuite.BeginCloneOperation((TestManagementRequestContext) this.m_tmRequestContext, projectName, fromSuiteId, targetProjectName, toSuiteId, options, true);
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
      }
    }
  }
}
