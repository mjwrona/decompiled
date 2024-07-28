// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LockHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class LockHelper
  {
    private const string c_lockKeyFormatForPlan = "TCM_TestPlan_<{0}>";
    private const string lockKeyFormatForProject = "TCM_TestProject_<{0}>";

    public static string ConstructLockKeyForPlan(int planId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TCM_TestPlan_<{0}>", (object) planId);

    public static string ConstructLockKeyForProject(string projectName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "TCM_TestProject_<{0}>", (object) projectName);
  }
}
