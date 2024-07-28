// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementVariableService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementVariableService))]
  public interface ITeamFoundationTestManagementVariableService : IVssFrameworkService
  {
    TestVariable GetTestVariableById(
      TestManagementRequestContext requestContext,
      int testVariableId,
      string projectName);

    IEnumerable<TestVariable> GetTestVariables(
      TestManagementRequestContext requestContext,
      string projectName,
      int skip,
      int topToFetch,
      int watermark);

    TestVariable CreateTestVariable(
      TestManagementRequestContext requestContext,
      TestVariable testVariable,
      string projectName);

    void DeleteTestVariable(
      TestManagementRequestContext requestContext,
      int testVariableId,
      string projectName);

    TestVariable UpdateTestVariable(
      TestManagementRequestContext requestContext,
      int testVariableId,
      TestVariable updatedTestVariable,
      string projectName);
  }
}
