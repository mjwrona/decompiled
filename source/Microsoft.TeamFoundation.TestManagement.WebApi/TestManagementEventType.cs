// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.TestManagementEventType
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  public static class TestManagementEventType
  {
    public static readonly string TestRunCreated = "testmanagement.testrun.created";
    public static readonly string TestRunStarted = "testmanagement.testrun.started";
    public static readonly string TestRunCanceled = "testmanagement.testrun.canceled";
    public static readonly string TestResultCreated = "testmanagement.testresult.created";
    public static readonly string TestRunWithDtlEnv = "testmanagement.testrun.testrunwithdtlenv";
  }
}
