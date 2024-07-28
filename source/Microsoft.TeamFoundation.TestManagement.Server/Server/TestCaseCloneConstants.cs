// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestCaseCloneConstants
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal static class TestCaseCloneConstants
  {
    public const string CloneOperationJobExtensionName = "Microsoft.TeamFoundation.TestManagement.Server.Jobs.TestCaseCloneOperationJob";
    public const string CloneOperationDataXml = "<CloneOperation OpId=\"{0}\" IncludeAttachments=\"{1}\" IncludeLinks=\"{2}\"/>";
    public const string XPathToOpIdInCloneOperationData = "//@OpId";
    public const string XPathToIncludeAttachmentsInCloneOperationData = "//@IncludeAttachments";
    public const string XPathToIncludeLinksInCloneOperationData = "//@IncludeLinks";
    internal const int LinkCommentLength = 255;
    internal const int MaxStringLength = 1024;
  }
}
