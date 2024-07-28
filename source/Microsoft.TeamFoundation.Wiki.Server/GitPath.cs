// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.GitPath
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class GitPath
  {
    private GitPath(string path) => this.Path = !string.IsNullOrEmpty(path) ? path : throw new InvalidArgumentValueException(nameof (path), "cannot be null or empty");

    public static GitPath GitMdPathFromWikiPagePath(WikiPagePath wikiPagePath, string mappedPath)
    {
      if (wikiPagePath == null)
        throw new InvalidArgumentValueException(nameof (wikiPagePath), "cannot be null");
      if (string.IsNullOrEmpty(mappedPath))
        throw new InvalidArgumentValueException(nameof (mappedPath), "cannot be null or empty");
      return wikiPagePath.IsWikiRootPage() ? new GitPath(PathHelper.GetPageFilePath(wikiPagePath.Path, mappedPath)) : new GitPath(PathHelper.GetPageFilePath(wikiPagePath.Path, mappedPath).GetPagePathWithExtension());
    }

    public static GitPath FromMappedPath(string mappedPath) => !string.IsNullOrEmpty(mappedPath) ? new GitPath(mappedPath) : throw new InvalidArgumentValueException(nameof (mappedPath), "cannot be null or empty");

    public static GitPath FromGitItem(GitItem item) => item != null ? new GitPath(item.Path) : throw new InvalidArgumentValueException(nameof (item), "cannot be null");

    public static GitPath ForParentOrderFile(WikiPagePath wikiPagePath, string mappedPath)
    {
      if (wikiPagePath == null)
        throw new InvalidArgumentValueException(nameof (wikiPagePath), "cannot be null");
      if (string.IsNullOrEmpty(mappedPath))
        throw new InvalidArgumentValueException(nameof (mappedPath), "cannot be null or empty");
      return new GitPath(PathHelper.GetParentOrderFilePath(PathHelper.GetPageFilePath(wikiPagePath.Path, mappedPath)));
    }

    public GitPath GitEquivalentFolderPath() => !this.Path.IsMdFile() ? this : new GitPath(System.IO.Path.ChangeExtension(this.Path, (string) null));

    public override string ToString() => this.Path;

    public override bool Equals(object obj) => obj is GitPath gitPath ? this.Path.Equals(gitPath.Path) : throw new InvalidArgumentValueException(nameof (obj), "equals expects only GitPath");

    public override int GetHashCode() => this.Path.GetHashCode();

    public string Path { get; }
  }
}
