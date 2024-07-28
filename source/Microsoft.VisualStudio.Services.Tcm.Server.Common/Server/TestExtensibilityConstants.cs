// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestExtensibilityConstants
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  public static class TestExtensibilityConstants
  {
    public static readonly string[] TestResultSystemFieldNames = new string[1]
    {
      nameof (StackTrace)
    };
    public static readonly string[] TestRunSystemFieldNames = new string[0];
    public const string StackTrace = "StackTrace";
    public const string FailingSince = "FailingSince";
    public const string Comment = "Comment";
    public const string ErrorMessage = "ErrorMessage";
    public const string OutcomeConfidence = "OutcomeConfidence";
    public const string TestRunSystem = "TestRunSystem";
    public const string AttemptId = "AttemptId";
    internal const string UnsanitizedTestCaseTitle = "UnsanitizedTestCaseTitle";
    internal const string UnsanitizedAutomatedTestName = "UnsanitizedAutomatedTestName";
    public const string TestResultGroupType = "TestResultGroupType";
    public const string MaxReservedSubResultId = "MaxReservedSubResultId";
    public const string IsTestResultFlaky = "IsTestResultFlaky";
    public const string TestResultFlakyState = "TestResultFlakyState";
    public const string LogStoreContainerState = "LogStoreContainerState";
    public const string LogStoreContainerSize = "LogStoreContainerSize";
    public const string NewTestEntry = "NewTestEntry";
    internal static readonly HashSet<string> TestResultExtensionUpdateFields = new HashSet<string>()
    {
      nameof (OutcomeConfidence)
    };
  }
}
