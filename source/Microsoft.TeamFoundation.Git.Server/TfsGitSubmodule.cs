// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TfsGitSubmodule
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class TfsGitSubmodule
  {
    private const int c_maxGitmoduleFileCap = 200000;
    private const string c_layer = "TfsGitSubmodule";

    internal TfsGitSubmodule()
    {
    }

    internal TfsGitSubmodule(string name, string path, string repositoryUrl, string branch = null)
    {
      this.Name = name;
      this.Path = path;
      this.RepositoryUrl = repositoryUrl;
      this.Branch = branch;
    }

    public string Name { get; internal set; }

    public string Path { get; internal set; }

    public string RepositoryUrl { get; internal set; }

    public string Branch { get; internal set; }

    internal bool IsValid() => !string.IsNullOrEmpty(this.Name) && !string.IsNullOrEmpty(this.Path) && !string.IsNullOrEmpty(this.RepositoryUrl);

    internal IList<TfsGitSubmodule> ParseGitmodules(
      IVssRequestContext requestContext,
      TfsGitBlob gitModuleFileBlob)
    {
      ArgumentUtility.CheckForNull<TfsGitBlob>(gitModuleFileBlob, nameof (gitModuleFileBlob));
      using (Stream content = gitModuleFileBlob.GetContent())
        return this.ParseGitmodules(requestContext, content);
    }

    internal IList<TfsGitSubmodule> ParseGitmodules(
      IVssRequestContext requestContext,
      Stream gitModuleFileContentStream)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Git/Settings/GitmoduleFileSizeCap", 200000);
      IList<TfsGitSubmodule> gitmodules = (IList<TfsGitSubmodule>) new List<TfsGitSubmodule>();
      if (gitModuleFileContentStream != null)
      {
        if (gitModuleFileContentStream.Length > (long) num)
        {
          requestContext.Trace(1013324, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (TfsGitSubmodule), ".gitmodules file size {0} larger than file size cap {1}", (object) gitModuleFileContentStream.Length, (object) num);
          return gitmodules;
        }
        using (StreamReader streamReader = new StreamReader(gitModuleFileContentStream, Encoding.UTF8))
        {
          TfsGitSubmodule tfsGitSubmodule = (TfsGitSubmodule) null;
          int lineNumber = 0;
          string str1;
          while ((str1 = streamReader.ReadLine()) != null)
          {
            string line = str1.Trim();
            ++lineNumber;
            if (!string.IsNullOrEmpty(line) && !line.StartsWith(";", StringComparison.Ordinal) && !line.StartsWith("#", StringComparison.Ordinal))
            {
              string name = (string) null;
              if (this.IsSubmoduleSectionHeader(line, lineNumber, out name))
              {
                if (tfsGitSubmodule != null && tfsGitSubmodule.IsValid())
                  gitmodules.Add(tfsGitSubmodule);
                tfsGitSubmodule = new TfsGitSubmodule();
                tfsGitSubmodule.Name = name;
              }
              else if (tfsGitSubmodule != null)
              {
                int length = line.IndexOf('=');
                if (length < 3 || length == line.Length - 1)
                {
                  requestContext.Trace(1013325, TraceLevel.Warning, GitServerUtils.TraceArea, nameof (TfsGitSubmodule), "Failed to parse the .gitmodules file, the key value pair is malformatted");
                }
                else
                {
                  string str2 = line.Substring(0, length).Trim();
                  string str3 = line.Substring(length + 1).Trim();
                  switch (str2.ToLower())
                  {
                    case "path":
                      tfsGitSubmodule.Path = str3;
                      continue;
                    case "url":
                      tfsGitSubmodule.RepositoryUrl = str3;
                      continue;
                    case "branch":
                      tfsGitSubmodule.Branch = str3;
                      continue;
                    default:
                      continue;
                  }
                }
              }
            }
          }
          if (tfsGitSubmodule != null)
          {
            if (tfsGitSubmodule.IsValid())
              gitmodules.Add(tfsGitSubmodule);
          }
        }
      }
      return gitmodules;
    }

    private bool IsSubmoduleSectionHeader(string line, int lineNumber, out string name)
    {
      string str = "[submodule ";
      bool flag = false;
      name = (string) null;
      if (line != null)
      {
        if (line.StartsWith("[", StringComparison.Ordinal) && !line.StartsWith(str, StringComparison.Ordinal))
          throw new GitmoduleMalformedException(lineNumber);
        if (line.StartsWith(str, StringComparison.Ordinal))
        {
          if (!line.EndsWith("\"]", StringComparison.Ordinal))
            throw new GitmoduleMalformedException(lineNumber);
          int num1 = line.IndexOf('"', str.Length);
          int num2 = -1;
          if (num1 >= str.Length)
            num2 = line.LastIndexOf('"');
          if (num1 > -1 && num2 > -1 && num2 - num1 > 1)
          {
            name = line.Substring(num1 + 1, num2 - num1 - 1).Trim();
            if (name.IndexOf('"') > -1)
              throw new GitmoduleMalformedException(lineNumber);
            flag = name.Length > 0;
          }
        }
      }
      return flag;
    }
  }
}
