// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Client.ObjectTypes
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

namespace Microsoft.TeamFoundation.TestManagement.Client
{
  public enum ObjectTypes
  {
    None = 0,
    TestRun = 1,
    TestConfiguration = 2,
    TestPlan = 3,
    TestPoint = 5,
    TestResult = 6,
    TestVariable = 7,
    TestResolutionState = 8,
    TestSettings = 9,
    Attachment = 10, // 0x0000000A
    TestSuite = 11, // 0x0000000B
    TestSuiteEntry = 12, // 0x0000000C
    TeamProject = 13, // 0x0000000D
    TestVariableValue = 14, // 0x0000000E
    TestConfigurationVariable = 15, // 0x0000000F
    BugFieldMapping = 16, // 0x00000010
    Session = 17, // 0x00000011
    TestController = 18, // 0x00000012
    DataCollector = 19, // 0x00000013
    TestCase = 20, // 0x00000014
    SharedSteps = 21, // 0x00000015
    TestFailureType = 22, // 0x00000016
    SharedParameterDataSet = 23, // 0x00000017
    CustomTestField = 24, // 0x00000018
    ResultRetentionSettings = 25, // 0x00000019
    Definition = 26, // 0x0000001A
    DefinitionRun = 27, // 0x0000001B
    AssociatedBuild = 28, // 0x0000001C
    LogStoreStorageAccount = 29, // 0x0000001D
    LogStoreContainer = 30, // 0x0000001E
    AssociatedRelease = 31, // 0x0000001F
    TestPlanClone = 32, // 0x00000020
    TestSuiteClone = 33, // 0x00000021
    Build = 34, // 0x00000022
    Other = 1000, // 0x000003E8
  }
}
