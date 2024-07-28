// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.GitHubConnector.ConnectionInfo
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.GitHubConnector
{
  public class ConnectionInfo
  {
    public ConnectionInfo(
      Guid connectionId,
      string gitHubInstallationId,
      GitHubAccount gitHubAccount)
    {
      this.ConnectionId = connectionId;
      this.GitHubInstallationId = gitHubInstallationId;
      this.GitHubAccount = gitHubAccount;
    }

    public ConnectionInfo()
    {
    }

    public Guid ConnectionId { get; set; }

    public string GitHubInstallationId { get; set; }

    public GitHubAccount GitHubAccount { get; set; }
  }
}
