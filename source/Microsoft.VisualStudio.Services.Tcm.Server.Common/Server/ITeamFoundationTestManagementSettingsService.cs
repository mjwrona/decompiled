// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementSettingsService))]
  public interface ITeamFoundationTestManagementSettingsService : IVssFrameworkService
  {
    IList<TestSettings2> GetTestSettings(
      TestManagementRequestContext requestContext,
      string projectName,
      ref int top,
      int continuationTokenId);

    TestSettings GetTestSettingsById(
      TestManagementRequestContext requestContext,
      int testSettingsId,
      TeamProjectReference teamProject);

    int CreateTestSettings(
      TestManagementRequestContext requestContext,
      TestSettings testSettings,
      TeamProjectReference teamProject);

    void DeleteTestSettings(
      TestManagementRequestContext requestContext,
      int testSettingsId,
      TeamProjectReference teamProject);
  }
}
