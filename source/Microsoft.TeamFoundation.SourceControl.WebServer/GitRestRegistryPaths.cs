// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitRestRegistryPaths
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  internal static class GitRestRegistryPaths
  {
    public const string GitRestServiceRoot = "/Service/GitRest";
    public const string MaxGitRestFilePathsSetting = "/Service/GitRest/Settings/maxFilePaths";
    public const string PushesControllerMaxPostBodyBytes = "/Service/GitRest/Settings/PushesControllerMaxPostBodyBytes";
    public const string PushesControllerExtendedMaxPostBodyBytes = "/Service/GitRest/Settings/PushesControllerExtendedMaxPostBodyBytes";
    public const string PushesControllerExtendedMaxPostBodyBytesAppIds = "/Service/GitRest/Settings/PushesControllerExtendedMaxPostBodyBytesAppIds";
    public const string FileDiffsControllerMaxFileDiffBytes = "/Service/GitRest/Settings/FileDiffsControllerMaxFileDiffBytes";
    public const string FileDiffsControllerMaxFileDiffs = "/Service/GitRest/Settings/FileDiffsControllerMaxFileDiffs";
    public const string GitCommitsBatchControllerMaxTop = "/Service/GitRest/Settings/GitCommitsBatchControllerMaxTop";
    public const string GitCommitsBatchControllerMaxSkip = "/Service/GitRest/Settings/GitCommitsBatchControllerMaxSkip";
    public const string GitHfsFileViewEnabled = "/Service/GitRest/Settings/GitHfsFileViewEnabled";
  }
}
