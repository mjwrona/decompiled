// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestOutcomeConverter
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public static class TestOutcomeConverter
  {
    public static Dictionary<int, string> GetEnumToDisplayNamesMap() => new Dictionary<int, string>()
    {
      {
        2,
        TestManagementResources.TestOutcome_Passed
      },
      {
        4,
        TestManagementResources.TestOutcome_Inconclusive
      },
      {
        11,
        TestManagementResources.TestOutcome_NotApplicable
      },
      {
        3,
        TestManagementResources.TestOutcome_Failed
      },
      {
        14,
        TestManagementResources.TestOutcome_NotImpacted
      },
      {
        10,
        TestManagementResources.TestOutcome_Error
      },
      {
        6,
        TestManagementResources.TestOutcome_Aborted
      },
      {
        5,
        TestManagementResources.TestOutcome_Timeout
      },
      {
        7,
        TestManagementResources.TestOutcome_Blocked
      },
      {
        9,
        TestManagementResources.TestOutcome_Warning
      },
      {
        8,
        TestManagementResources.TestOutcome_NotExecuted
      },
      {
        1,
        TestManagementResources.TestOutcome_None
      },
      {
        0,
        TestManagementResources.TestOutcome_Result_Unspecified
      },
      {
        13,
        TestManagementResources.TestOutcome_InProgress
      },
      {
        12,
        TestManagementResources.TestOutcome_Paused
      }
    };

    public static string GetLocalizedString(string outcome)
    {
      TestOutcome result;
      return Enum.TryParse<TestOutcome>(outcome, out result) ? TestOutcomeConverter.GetEnumToDisplayNamesMap()[(int) result] : outcome;
    }
  }
}
