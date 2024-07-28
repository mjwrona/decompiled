// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TestFailureTypeHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  internal class TestFailureTypeHelper : TfsRestApiHelper
  {
    internal TestFailureTypeHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    internal List<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType> QueryTestFailureTypes(
      string projectName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>>("TestFailureTypeHelper.QueryTestFailureTypes", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestFailureType> testFailureTypeList1 = Microsoft.TeamFoundation.TestManagement.Server.TestFailureType.Query(this.TestManagementRequestContext, -1, projectName);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType> testFailureTypeList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType>(testFailureTypeList1.Count);
        foreach (Microsoft.TeamFoundation.TestManagement.Server.TestFailureType failureType in testFailureTypeList1)
          testFailureTypeList2.Add(this.ConvertTestResolutionToDataContract(failureType, projectRepresentation));
        return testFailureTypeList2;
      }), 1015074, "TestManagement");
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType ConvertTestResolutionToDataContract(
      Microsoft.TeamFoundation.TestManagement.Server.TestFailureType failureType,
      ShallowReference projectReference)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestFailureType()
      {
        Id = failureType.Id,
        Name = failureType.Name,
        project = projectReference
      };
    }
  }
}
