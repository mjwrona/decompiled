// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.ExternalIntegration.Utilities.GitHub.GitHubInstallationRedirectState
// Assembly: Microsoft.TeamFoundation.ExternalIntegration.Utilities, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6309B6D0-0EEE-4299-AA79-F0B62882E0B1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.ExternalIntegration.Utilities.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.ExternalIntegration.Utilities.GitHub
{
  [Serializable]
  public class GitHubInstallationRedirectState
  {
    public string RedirectUrl;
    public string Signature;
    public IEnumerable<string> RepoNodeIds;
    public Guid ExistingConnectionId;
  }
}
