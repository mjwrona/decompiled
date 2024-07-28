// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.Common.Svn.SvnHelper
// Assembly: Microsoft.VisualStudio.ExternalProviders.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7E34B318-B0E9-49BD-88C0-4A425E8D0753
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Diagnostics;
using System.Text;

namespace Microsoft.VisualStudio.ExternalProviders.Common.Svn
{
  public class SvnHelper : ISvnHelper
  {
    public bool GetAuthor(
      string repositoryUri,
      string username,
      string password,
      string acceptUntrustedCerts,
      string timeoutSeconds,
      string version,
      string exePath,
      out string author)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("-get-author").Append(" ").Append(repositoryUri).Append(" ").Append(acceptUntrustedCerts).Append(" ").Append(timeoutSeconds).Append(" ").Append(version);
      return this.ExecuteSvnCommand(exePath, stringBuilder.ToString(), username, password, out author);
    }

    public bool GetLastRevision(
      string repositoryUri,
      string sourceBranch,
      string username,
      string password,
      string acceptUntrustedCerts,
      string timeoutSeconds,
      string exePath,
      out string lastChangeRevision)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("-get-last-revision").Append(" ").Append(repositoryUri).Append(" ").Append(sourceBranch).Append(" ").Append(acceptUntrustedCerts).Append(" ").Append(timeoutSeconds);
      return this.ExecuteSvnCommand(exePath, stringBuilder.ToString(), username, password, out lastChangeRevision);
    }

    public bool GetLogs(
      string repositoryUri,
      string sourceBranch,
      string lastVersionBuilt,
      string username,
      string password,
      string acceptUntrustedCerts,
      string timeoutSeconds,
      string exePath,
      out string logItems)
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("-get-logs").Append(" ").Append(repositoryUri).Append(" ").Append(sourceBranch).Append(" ").Append(acceptUntrustedCerts).Append(" ").Append(timeoutSeconds).Append(" ").Append(lastVersionBuilt);
      return this.ExecuteSvnCommand(exePath, stringBuilder.ToString(), username, password, out logItems);
    }

    private bool ExecuteSvnCommand(
      string exePath,
      string args,
      string username,
      string password,
      out string result)
    {
      result = (string) null;
      ProcessOutput processOutput = ProcessHandler.RunExe(new ProcessStartInfo()
      {
        FileName = exePath,
        Arguments = args
      }, args, this.GetBase64EncodedCreds(username, password), (ITFLogger) new TraceLogger());
      if (processOutput.ExitCode != 0 || !string.IsNullOrEmpty(processOutput.StdErr))
      {
        result = processOutput.StdErr;
        if (processOutput.ExitCode == 3)
          throw new SvnAuthenticationException(result);
        if (processOutput.ExitCode == 4)
          throw new SvnRepositoryIOException(result);
        if (processOutput.ExitCode == 5)
          throw new SvnSystemException(result);
        if (processOutput.ExitCode == 6)
          throw new SvnException(result);
        if (processOutput.ExitCode == 10)
          throw new Exception(result);
        return false;
      }
      if (!string.IsNullOrEmpty(processOutput.StdOut))
        result = processOutput.StdOut.Trim();
      return true;
    }

    private string GetBase64EncodedCreds(string username, string password) => Convert.ToBase64String(Encoding.UTF8.GetBytes(username + "\0" + password));
  }
}
