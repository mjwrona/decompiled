// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementConfigurationService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementConfigurationService))]
  public interface ITeamFoundationTestManagementConfigurationService : IVssFrameworkService
  {
    TestConfiguration GetTestConfigurationById(
      TestManagementRequestContext requestContext,
      int testConfigurationId,
      string projectName);

    IEnumerable<TestConfiguration> GetTestConfigurations(
      TestManagementRequestContext requestContext,
      string projectName);

    IEnumerable<TestConfiguration> GetTestConfigurationsWithPaging(
      TestManagementRequestContext requestContext,
      string projectName,
      int skip,
      int top,
      int watermark);

    TestConfiguration CreateTestConfiguration(
      TestManagementRequestContext requestContext,
      TestConfiguration testConfiguration,
      string projectName);

    void DeleteTestConfiguration(
      TestManagementRequestContext requestContext,
      int testConfigurationId,
      string projectName);

    TestConfiguration UpdateTestConfiguration(
      TestManagementRequestContext requestContext,
      int testConfigurationId,
      TestConfiguration updatedTestConfiguration,
      string projectName);

    List<TestConfigurationRecord> QueryTestConfigurationsByChangedDate(
      TestManagementRequestContext requestContext,
      int projectId,
      int batchSize,
      DateTime fromDate,
      out DateTime toDate,
      TestArtifactSource dataSource);
  }
}
