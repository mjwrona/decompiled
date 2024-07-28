// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultFailureTypeHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestResultFailureTypeHelper : ITestResultFailureTypeService, IVssFrameworkService
  {
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType> GetTestResultFailureType(
      TestManagementRequestContext context,
      string ProjectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResultFailureTypeHelper.GetTestResultFailureType"))
      {
        context.RequestContext.Trace(1015932, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestResultFailureTypeHelper.GetTestResultFailureType. ProjectName: {0}", (object) ProjectName);
        context.TraceEnter("BusinessLayer", "TestResultFailureTypeHelper.GetTestResultFailureType");
        this.PublishTelemetryDataForGetTestResultFailureType(Validator.CheckAndGetProjectFromName(context, ProjectName).ToString(), context);
        List<TestResultFailureType> failureTypes = TestResultFailureType.QueryTestResultFailureType(context, -1, ProjectName);
        context.TraceLeave("BusinessLayer", "TestResultFailureTypeHelper.GetTestResultFailureType");
        return this.GetTestResultFailureTypeContract(failureTypes);
      }
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType CreateTestResultFailureType(
      TestManagementRequestContext context,
      string failureTypeName,
      string ProjectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResultFailureTypeHelper.CreateTestResultFailureType"))
      {
        context.RequestContext.Trace(1015933, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestResultFailureTypeHelper.CreateTestResultFailureType. ProjectName: {0}, FailureTypeName: {1}", (object) ProjectName, (object) failureTypeName);
        context.TraceEnter("BusinessLayer", "TestResultFailureTypeHelper.CreateTestResultFailureType");
        this.PublishTelemetryDataForCreateTestResultFailureType(Validator.CheckAndGetProjectFromName(context, ProjectName).ToString(), failureTypeName, context);
        TestResultFailureType resultFailureType = TestResultFailureType.CreateTestResultFailureType(context, failureTypeName, ProjectName);
        context.TraceLeave("BusinessLayer", "TestResultFailureTypeHelper.CreateTestResultFailureType");
        return this.TestResultFailureTypeModelToContract(resultFailureType);
      }
    }

    public bool DeleteTestResultFailureType(
      TestManagementRequestContext context,
      int failureTypeId,
      string ProjectName)
    {
      using (PerfManager.Measure(context.RequestContext, "BusinessLayer", "TestResultFailureTypeHelper.DeleteTestResultFailureType"))
      {
        context.RequestContext.Trace(1015934, TraceLevel.Info, "TestManagement", "BusinessLayer", "TestResultFailureTypeHelper.DeleteTestResultFailureType. ProjectName: {0}, FailureTypeId: {1}", (object) ProjectName, (object) failureTypeId);
        context.TraceEnter("BusinessLayer", "TestResultFailureTypeHelper.DeleteTestResultFailureType");
        this.PublishTelemetryDataForDeleteTestResultFailureType(Validator.CheckAndGetProjectFromName(context, ProjectName).ToString(), failureTypeId, context);
        context.TraceLeave("BusinessLayer", "TestResultFailureTypeHelper.DeleteTestResultFailureType");
        return TestResultFailureType.DeleteTestResultFailureType(context, failureTypeId, ProjectName);
      }
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    internal void PublishTelemetryDataForGetTestResultFailureType(
      string projectId,
      TestManagementRequestContext context)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      new TelemetryLogger().PublishData(context.RequestContext, "GetTestResultFailureType", cid);
    }

    private void PublishTelemetryDataForCreateTestResultFailureType(
      string projectId,
      string failureTypeName,
      TestManagementRequestContext context)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      cid.Add("TestResultFailureType", failureTypeName);
      new TelemetryLogger().PublishData(context.RequestContext, " CreateTestResultFailureType", cid);
    }

    private void PublishTelemetryDataForDeleteTestResultFailureType(
      string projectId,
      int failureTypeId,
      TestManagementRequestContext context)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      cid.Add("TestResultFailureTypeId", (double) failureTypeId);
      new TelemetryLogger().PublishData(context.RequestContext, "DeleteTestResultFailureType", cid);
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType TestResultFailureTypeModelToContract(
      TestResultFailureType failureType)
    {
      if (failureType == null)
        return new Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType();
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType()
      {
        Id = failureType.Id,
        Name = failureType.Name
      };
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType> GetTestResultFailureTypeContract(
      List<TestResultFailureType> failureTypes)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType> failureTypeContract = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultFailureType>();
      if (failureTypes == null)
        return failureTypeContract;
      foreach (TestResultFailureType failureType in failureTypes)
        failureTypeContract.Add(this.TestResultFailureTypeModelToContract(failureType));
      return failureTypeContract;
    }
  }
}
