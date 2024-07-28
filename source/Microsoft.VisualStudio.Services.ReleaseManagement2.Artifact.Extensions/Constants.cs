// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Constants
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions
{
  public static class Constants
  {
    public const int ArtifactNameLengthMin = 0;
    public const int ArtifactNameLengthMax = 260;
    public const string ArtifactsInputId = "artifacts";
    public const string ArtifactItemsInputId = "artifactItems";
    public const string ArtifactItemPath = "itemPath";
    public const string ArtifactItemType = "itemType";
    public const string ArtifactItemFolderType = "folder";
    public const string ArtifactItemFileType = "file";
    public const string ArtifactResourceType = "type";
    public const string ArtifactResourceDownloadUrl = "downloadUrl";
    public const string IsMultiDefinitionTypeKey = "isMultiDefinitionType";
    public const string DefinitionIdKey = "definitionId";
    public const string DefinitionNameKey = "definitionName";
    public const string SourceBranchKey = "branch";
    public const string SourceVersionKey = "sourceVersion";
    public const string RepositoryIdKey = "repositoryId";
    public const string RepositoryTypeKey = "repositoryType";
    public const string BuildTagsKey = "tags";
    public const string BuildDefinitionPathKey = "folderPath";
    public const string SourceControlGitEnabled = "SourceControlGitEnabled";
    public const string SourceControlTfvcEnabled = "SourceControlTfvcEnabled";
    public const char ForwardSlash = '/';
    public const char BackwardSlash = '\\';
    public const char Dollar = '$';
    public const char WhiteSpace = ' ';
    public const string ChangesetPrefix = "Changeset";
    public const string ArtifactItemContent = "artifactItemContent";
    public const string ContainerId = "ContainerId";
    public const string FileSize = "FileSize";
    public const long MaxFileSizeSupported = 2097152;
    public const long MaxFileSizeSupportedForBuild = 4194304;
    public const string IntegratedInReleaseEnvironment = "Integrated in release environment";
    public const string CommitMessage = "commitMessage";
    public const string ContinuationToken = "continuationToken";
    public const string CallbackRequired = "callbackRequired";
    public const string SearchText = "name";
    public const string GitRepositoryProvider = "Git";
    public const string GitHubRepositoryProvider = "GitHub";
  }
}
