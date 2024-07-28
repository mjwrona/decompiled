// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITestManagementTestPlanConfigurationsCacheService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DefaultServiceImplementation(typeof (TestPlanConfigurationsCacheService))]
  internal interface ITestManagementTestPlanConfigurationsCacheService : IVssFrameworkService
  {
    bool TryUpdateTestPlanConfigurationsCache(
      IVssRequestContext requestContext,
      int testPlanId,
      Dictionary<int, TestConfiguration> configurations);

    bool TryGetCachedTestPlanConfigurationsData(
      IVssRequestContext requestContext,
      int testPlanId,
      out Dictionary<int, TestConfiguration> configurations);
  }
}
