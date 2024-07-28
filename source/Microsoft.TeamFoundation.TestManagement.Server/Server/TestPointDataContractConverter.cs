// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPointDataContractConverter
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPointDataContractConverter
  {
    public Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint ConvertTestPointToDataContract(
      TestPoint testPoint,
      TeamProjectReference projectReference,
      List<string> workItemFields,
      string url)
    {
      string name = projectReference.Name;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint dataContract = this.ConvertBasicTestPointToDataContract(testPoint, workItemFields);
      if (dataContract == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint) null;
      dataContract.AssignedTo = new IdentityRef()
      {
        Id = testPoint.AssignedTo.ToString(),
        DisplayName = testPoint.AssignedToName
      };
      dataContract.Configuration = new ShallowReference()
      {
        Id = testPoint.ConfigurationId.ToString(),
        Name = testPoint.ConfigurationName
      };
      dataContract.LastTestRun = new ShallowReference()
      {
        Id = testPoint.LastTestRunId.ToString()
      };
      dataContract.LastResult = new ShallowReference()
      {
        Id = testPoint.LastTestResultId.ToString()
      };
      dataContract.Url = url;
      dataContract.LastRunBuildNumber = testPoint.LastRunBuildNumber;
      dataContract.TestPlan = new ShallowReference()
      {
        Id = testPoint.PlanId.ToString(),
        Name = testPoint.PlanName
      };
      if (testPoint.SuiteId > 0)
        dataContract.Suite = new ShallowReference()
        {
          Id = testPoint.SuiteId.ToString(),
          Name = testPoint.SuiteName
        };
      return dataContract;
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint ConvertBasicTestPointToDataContract(
      TestPoint testPoint,
      List<string> workItemFields)
    {
      if (testPoint == null)
        return (Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint) null;
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestPoint()
      {
        Id = testPoint.PointId,
        TestCase = new WorkItemReference()
        {
          Id = testPoint.TestCaseId.ToString()
        },
        Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) testPoint.LastResultOutcome),
        WorkItemProperties = (object[]) this.ConstructWorkItemProperties(testPoint, workItemFields),
        State = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestPointState), (object) testPoint.State),
        LastResultState = Enum.GetName(typeof (TestResultState), (object) testPoint.LastResultState)
      };
    }

    private PointWorkItemProperty[] ConstructWorkItemProperties(
      TestPoint testPoint,
      List<string> workItemFields)
    {
      List<PointWorkItemProperty> workItemPropertyList = new List<PointWorkItemProperty>();
      if (testPoint.WorkItemProperties != null)
      {
        for (int index = 0; index < workItemFields.Count; ++index)
        {
          PointWorkItemProperty workItemProperty = new PointWorkItemProperty()
          {
            WorkItem = new KeyValuePair<string, object>(workItemFields[index], testPoint.WorkItemProperties[index])
          };
          workItemPropertyList.Add(workItemProperty);
        }
      }
      return workItemPropertyList.ToArray();
    }
  }
}
