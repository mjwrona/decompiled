// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.TestSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  internal class TestSettingsService : 
    TeamFoundationTestManagementService,
    ITestSettingsService,
    IVssFrameworkService
  {
    public TestSettingsService()
    {
    }

    public TestSettingsService(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties CreateTestSettings(
      TestManagementRequestContext requestContext,
      Guid projectId,
      LegacyTestSettings legacyTestSettings)
    {
      TestSettings testSettings = TestSettingsContractConverter.Convert(legacyTestSettings);
      TeamProjectReference projectReference = this.GetProjectReference(requestContext, projectId);
      TestManagementRequestContext context = requestContext;
      string name = projectReference.Name;
      return UpdatedPropertiesConverter.Convert(testSettings.Create(context, name));
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.UpdatedProperties UpdateTestSettings(
      TestManagementRequestContext requestContext,
      Guid projectId,
      LegacyTestSettings legacyTestSettings)
    {
      TestSettings testSettings = TestSettingsContractConverter.Convert(legacyTestSettings);
      TeamProjectReference projectReference = this.GetProjectReference(requestContext, projectId);
      TestManagementRequestContext context = requestContext;
      string name = projectReference.Name;
      return UpdatedPropertiesConverter.Convert(testSettings.Update(context, name));
    }

    public LegacyTestSettings GetTestSettingsById(
      TestManagementRequestContext requestContext,
      Guid projectId,
      int testSettingsId)
    {
      TeamProjectReference projectReference = this.GetProjectReference(requestContext, projectId);
      return TestSettingsContractConverter.Convert(TestSettings.QueryById(requestContext, testSettingsId, projectReference.Name));
    }

    public List<LegacyTestSettings> QueryTestSettings(
      TestManagementRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query,
      bool omitSettings)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return TestSettings.Query(requestContext, ResultsStoreQueryContractConverter.Convert(query), omitSettings).Select<TestSettings, LegacyTestSettings>(TestSettingsService.\u003C\u003EO.\u003C0\u003E__Convert ?? (TestSettingsService.\u003C\u003EO.\u003C0\u003E__Convert = new Func<TestSettings, LegacyTestSettings>(TestSettingsContractConverter.Convert))).ToList<LegacyTestSettings>();
    }

    public int QueryTestSettingsCount(
      TestManagementRequestContext requestContext,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      return TestSettings.QueryCount(requestContext, ResultsStoreQueryContractConverter.Convert(query));
    }

    private TeamProjectReference GetProjectReference(
      TestManagementRequestContext requestContext,
      Guid projectId)
    {
      ProjectInfo projectFromGuid = requestContext.ProjectServiceHelper.GetProjectFromGuid(projectId);
      if (projectFromGuid == null)
        return (TeamProjectReference) null;
      return new TeamProjectReference()
      {
        Id = projectFromGuid.Id,
        Name = projectFromGuid.Name
      };
    }
  }
}
