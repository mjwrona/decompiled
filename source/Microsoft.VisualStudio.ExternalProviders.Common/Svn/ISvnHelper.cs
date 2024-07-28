// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.Svn.ISvnHelper
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

namespace Microsoft.VisualStudio.ExternalProviders.Common.Svn
{
  public interface ISvnHelper
  {
    bool GetLogs(
      string repositoryUri,
      string sourceBranch,
      string lastVersionBuilt,
      string username,
      string password,
      string acceptUntrustedCerts,
      string timeoutSeconds,
      string exePath,
      out string logItems);

    bool GetLastRevision(
      string repositoryUri,
      string sourceBranch,
      string username,
      string password,
      string acceptUntrustedCerts,
      string timeoutSeconds,
      string exePath,
      out string lastChangeRevision);

    bool GetAuthor(
      string repositoryUri,
      string username,
      string password,
      string acceptUntrustedCerts,
      string timeoutSeconds,
      string version,
      string exePath,
      out string author);
  }
}
