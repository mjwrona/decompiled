// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestRunStateConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public static class TestRunStateConverter
  {
    public static Dictionary<int, string> GetEnumToDisplayNamesMap() => new Dictionary<int, string>()
    {
      {
        4,
        TestManagementResources.TestRunStateAborted
      },
      {
        3,
        TestManagementResources.TestRunStateCompleted
      },
      {
        2,
        TestManagementResources.TestRunStateInProgress
      },
      {
        6,
        TestManagementResources.TestRunStateNeedsInvestigation
      },
      {
        1,
        TestManagementResources.TestRunStateNotStarted
      },
      {
        0,
        TestManagementResources.TestRunStateUnspecified
      },
      {
        5,
        TestManagementResources.TestRunStateWaiting
      }
    };
  }
}
