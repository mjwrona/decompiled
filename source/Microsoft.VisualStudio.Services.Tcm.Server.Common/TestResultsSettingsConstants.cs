// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.TestResultsSettingsConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement
{
  public static class TestResultsSettingsConstants
  {
    public const string SettingsKeys = "/TestResults/Settings";
    public const string FlakySettingsKey = "FlakySettings";
    public const string NewTestResultSettingsKey = "NewTestResultSettings";
    public const string TestResultsSettingsArtifactGuid = "98CE0696-08C6-4f5D-AD99-23843056FFDE";
    public const string TestResultsSettingsMoniker = "TestResults.TestSettings";
    public const int DeleteFlakyTestDataSprocDeletionBatchSize = 10000;
    public const int DeleteFlakyTestDataSprocExecTimeOutInSec = 30;
  }
}
