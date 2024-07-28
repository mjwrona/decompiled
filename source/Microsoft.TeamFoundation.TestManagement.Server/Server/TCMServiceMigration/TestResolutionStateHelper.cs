// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration.TestResolutionStateHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.TCMServiceMigration
{
  internal class TestResolutionStateHelper : TfsRestApiHelper
  {
    internal TestResolutionStateHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    internal List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState> QueryTestResolutionStates(
      string projectName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName), this.RequestContext.ServiceName);
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>>("TestResolutionStateHelper.QueryTestResolutionStates", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>>) (() =>
      {
        ShallowReference projectRepresentation = this.ProjectServiceHelper.GetProjectRepresentation(projectName);
        List<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState> testResolutionStateList1 = Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState.Query(this.TestManagementRequestContext, 0, projectName);
        List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState> testResolutionStateList2 = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState>(testResolutionStateList1.Count);
        foreach (Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState testResolutionState in testResolutionStateList1)
          testResolutionStateList2.Add(this.ConvertTestResolutionToDataContract(testResolutionState, projectRepresentation));
        return testResolutionStateList2;
      }), 1015073, "TestManagement");
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState ConvertTestResolutionToDataContract(
      Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState testResolutionState,
      ShallowReference projectReference)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestResolutionState()
      {
        Id = testResolutionState.Id,
        Name = testResolutionState.Name,
        project = projectReference
      };
    }
  }
}
