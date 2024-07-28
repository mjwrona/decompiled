// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPagePath
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPagePath
  {
    private WikiPagePath(string path) => this.Path = !string.IsNullOrEmpty(path) ? path : throw new InvalidArgumentValueException(nameof (path), "cannot be null or empty");

    public static WikiPagePath FromWikiPagePath(string wikiPagePath)
    {
      wikiPagePath = !string.IsNullOrEmpty(wikiPagePath) ? PathHelper.NormalizePath(wikiPagePath) : throw new InvalidArgumentValueException(nameof (wikiPagePath), "cannot be null or empty");
      return new WikiPagePath(wikiPagePath);
    }

    public static WikiPagePath FromWikiPageIdDetails(WikiPageIdDetails wikiPageIdDetails)
    {
      if (string.IsNullOrEmpty(wikiPageIdDetails?.GitFriendlyPagePath))
        throw new InvalidArgumentValueException(nameof (wikiPageIdDetails), "is null or invalid");
      return wikiPageIdDetails.GitFriendlyPagePath.IsMdFile() ? new WikiPagePath(PathHelper.GetPageReadablePathFromUnReadablePath(System.IO.Path.ChangeExtension(wikiPageIdDetails.GitFriendlyPagePath, (string) null))) : throw new InvalidArgumentValueException(nameof (wikiPageIdDetails), "Expecting WikiPageIdDetails to be a md file");
    }

    public static WikiPagePath FromGitItem(GitItem item, string mappedPath)
    {
      if (item == null)
        throw new InvalidArgumentValueException(nameof (item), "cannot be null");
      if (string.IsNullOrEmpty(mappedPath))
        throw new InvalidArgumentValueException(nameof (mappedPath), "cannot be null or empty");
      return new WikiPagePath(PathHelper.GetPageReadablePath(item.IsFolder ? item.Path : System.IO.Path.ChangeExtension(item.Path, (string) null), mappedPath));
    }

    public static WikiPageChange WikiPageMoveParametersToWikiPageChange(
      WikiPageMoveParameters pageMoveParameters,
      WikiChangeType changeType,
      string comment)
    {
      if (pageMoveParameters == null)
        throw new InvalidArgumentValueException(nameof (pageMoveParameters), "cannot be null ");
      WikiPageChange wikiPageChange = new WikiPageChange();
      wikiPageChange.Path = WikiPagePath.FromWikiPagePath(pageMoveParameters.Path);
      wikiPageChange.NewPath = !string.IsNullOrEmpty(pageMoveParameters.NewPath) ? WikiPagePath.FromWikiPagePath(pageMoveParameters.NewPath) : (WikiPagePath) null;
      wikiPageChange.NewOrder = pageMoveParameters.NewOrder;
      wikiPageChange.Content = (string) null;
      wikiPageChange.Comment = comment;
      wikiPageChange.ChangeType = changeType;
      return wikiPageChange;
    }

    public WikiPagePath GetParentPagePath()
    {
      string parentPath = PathHelper.GetParentPath(PathHelper.GetPageFilePath(this.Path, "/"));
      return !this.IsWikiRootPage() ? WikiPagePath.FromWikiPagePath(PathHelper.GetPageReadablePath(parentPath, "/")) : (WikiPagePath) null;
    }

    public override string ToString() => this.Path;

    public bool IsWikiRootPage() => this.Path.Equals("/");

    public string GetGitFileName() => this.IsWikiRootPage() ? (string) null : System.IO.Path.GetFileName(PathHelper.GetPageFilePath(this.Path, "/"));

    public string Path { get; }

    public override bool Equals(object obj) => obj is WikiPagePath wikiPagePath ? this.Path.Equals(wikiPagePath.Path) : throw new InvalidArgumentValueException(nameof (obj), "equals expects only WikiPagePath");

    public bool IsEquivalentTo(string relativeGitPath) => PathHelper.GetPageReadablePath(this.Path, "/").GetPagePathWithExtension().Equals(relativeGitPath);

    public override int GetHashCode() => this.Path.GetHashCode();
  }
}
