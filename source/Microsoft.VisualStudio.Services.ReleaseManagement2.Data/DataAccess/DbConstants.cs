// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess.DbConstants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.DataAccess
{
  public static class DbConstants
  {
    public const long MaxNVarCharSize = -1;
    public const long ReleaseStepCommentLength = 1024;
    public const long ReleaseStepLogsLength = 4000;
    public const int NVarCharColumnLength = 4000;
    public const int ControlOptionsColumnLength = 4000;
    public const int MaxNameLength = 256;
    public const int MaxFolderNameLength = 400;
    public const int ReleaseDefinitionMaxNameLength = 256;
    public const int MaxSourceIdFilterLength = 256;
    public const int MaxQueueNameLength = 260;
    public const int MaxDescriptionLength = 256;
    public const int MaxTypeFilterLength = 256;
    public const int MaxSourceBranchFilterLength = 400;
    public const int MaxVersionIdFilterLength = 256;
    public const int MaxSourceDataLength = 2048;
    public const int ExecutionPoliciesColumnLength = 2048;
    public const string PreApprovalOptions = "pre";
    public const string PostApprovalOptions = "post";
    public const int MaxArtifactVersionIdLength = 256;
    public const int MaxArtifactTypeLength = 256;
    public const int MaxKnownSmallStringLength = 32;
    public const int MaxGitCommitIdLength = 40;
    public const int MaxNormalizedArtifactDefinitionIdentifierLength = 2048;
  }
}
