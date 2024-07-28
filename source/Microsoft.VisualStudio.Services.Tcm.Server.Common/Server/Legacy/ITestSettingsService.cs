// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.ITestSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  [DefaultServiceImplementation(typeof (TestSettingsService))]
  internal interface ITestSettingsService : IVssFrameworkService
  {
    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties CreateTestSettings(
      TestManagementRequestContext requestContext,
      Guid projectId,
      LegacyTestSettings legacyTestSettings);

    Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestSettings(
      TestManagementRequestContext requestContext,
      Guid projectId,
      LegacyTestSettings legacyTestSettings);

    LegacyTestSettings GetTestSettingsById(
      TestManagementRequestContext requestContext,
      Guid projectId,
      int testSettingsId);

    List<LegacyTestSettings> QueryTestSettings(
      TestManagementRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      bool omitSettings);

    int QueryTestSettingsCount(TestManagementRequestContext requestContext, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query);
  }
}
