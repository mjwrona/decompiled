// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitTelemetryInfo
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

namespace Microsoft.TeamFoundation.Git.Server
{
  internal static class GitTelemetryInfo
  {
    public const string TelemetryEventBaseName = "TFS/Git/";
    public const string PushEvent = "TFS/Git/Push";
    public const string VCFilesChangedEvent = "TFS/Git/VCFilesChanged";
    public const string TelemetryPropertyBaseName = "TFS.SourceControl.";
    public const string KeyPushId = "TFS.SourceControl.PushId";
    public const string KeyRepositoryId = "TFS.SourceControl.RepositoryId";
    public const string KeyFileExtensionChangeCounts = "TFS.SourceControl.FileExtensionChangeCounts";
  }
}
