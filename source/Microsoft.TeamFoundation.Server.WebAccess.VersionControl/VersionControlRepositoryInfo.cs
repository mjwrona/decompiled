// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.VersionControlRepositoryInfo
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.SourceControl.WebServer;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  public class VersionControlRepositoryInfo
  {
    protected VersionControlRepositoryInfo(
      VersionControlRepositoryType type,
      VersionControlProvider versionControlProvider)
    {
      this.RepositoryType = type;
      this.Provider = versionControlProvider;
    }

    public VersionControlRepositoryType RepositoryType { get; private set; }

    public VersionControlProvider Provider { get; private set; }
  }
}
