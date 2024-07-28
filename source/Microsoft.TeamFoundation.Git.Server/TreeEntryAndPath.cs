// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.TreeEntryAndPath
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public sealed class TreeEntryAndPath
  {
    private readonly string m_basePath;
    private string m_relativePath;

    public TreeEntryAndPath(string basePath, TfsGitTreeEntry entry)
    {
      this.m_basePath = basePath;
      this.Entry = entry;
    }

    public TfsGitTreeEntry Entry { get; }

    public string RelativePath
    {
      get
      {
        if (this.m_relativePath == null)
          this.m_relativePath = TreeEntryAndPath.PrependBasePath(this.m_basePath, this.Entry.Name);
        return this.m_relativePath;
      }
    }

    private static string PrependBasePath(string basePath, string relativePath) => basePath.EndsWith("/", StringComparison.Ordinal) ? basePath + relativePath : basePath + "/" + relativePath;
  }
}
