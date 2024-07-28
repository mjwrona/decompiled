// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseClonerConstants
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestCaseClonerConstants
  {
    public const string XPathToS3InActions = "//compref/@ref";
    public const string XPathToS3InUITest = "//*/@SharedSetSetId";
    public const string CloneLinkPrefix = "TF237027";
    public const string TestedByLinkName = "Microsoft.VSTS.Common.TestedBy";
    public static readonly string TestedByForwardLinkName = "Microsoft.VSTS.Common.TestedBy-Forward";
    public static readonly string TestedByReverseLinkName = "Microsoft.VSTS.Common.TestedBy-Reverse";
    public const string ChildLinkName = "System.LinkTypes.Hierarchy";
    public const string ChildForwardLinkName = "System.LinkTypes.Hierarchy-Forward";
    public const string SharedStepLinkName = "Microsoft.VSTS.TestCase.SharedStepReferencedBy";
    public const string LinkTypeRelated = "System.LinkTypes.Related";
    public const string RegisteredLinkType_TestResult = "Test Result";
    public const string RegisteredLinkType_TestResultAttachment = "Result Attachment";
    public const string RegisteredLinkType_Session = "Session";
    public const string RegisteredLinkType_Test = "Test";
  }
}
