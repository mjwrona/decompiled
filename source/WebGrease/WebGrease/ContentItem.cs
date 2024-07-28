// Decompiled with JetBrains decompiler
// Type: WebGrease.ContentItem
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System;
using System.Collections.Generic;
using System.IO;
using WebGrease.Configuration;

namespace WebGrease
{
  public class ContentItem
  {
    private string contentHash;
    private string content;

    private ContentItem()
    {
    }

    public string RelativeContentPath { get; private set; }

    public IEnumerable<ResourcePivotKey> ResourcePivotKeys { get; private set; }

    public string RelativeHashedContentPath { get; private set; }

    public string Content => this.ContentItemType != ContentItemType.Path ? this.ContentValue : this.ContentFromDisk();

    public bool IsFromDisk => this.ContentItemType == ContentItemType.Path;

    public string AbsoluteDiskPath => !this.IsFromDisk ? (string) null : this.AbsoluteContentPath;

    private string ContentValue { get; set; }

    private string AbsoluteContentPath { get; set; }

    private ContentItemType ContentItemType { get; set; }

    public static ContentItem FromCacheResult(
      CacheResult cacheResult,
      params ResourcePivotKey[] resourcePivotKeys)
    {
      return new ContentItem()
      {
        ContentItemType = ContentItemType.Path,
        AbsoluteContentPath = cacheResult.CachedFilePath,
        RelativeContentPath = cacheResult.RelativeContentPath,
        RelativeHashedContentPath = cacheResult.RelativeHashedContentPath,
        ResourcePivotKeys = (IEnumerable<ResourcePivotKey>) resourcePivotKeys
      };
    }

    public static ContentItem FromCacheResult(
      CacheResult cacheResult,
      string relativeContentPath = null,
      string relativeHashedContentPath = null,
      params ResourcePivotKey[] resourcePivotKeys)
    {
      return new ContentItem()
      {
        ContentItemType = ContentItemType.Path,
        AbsoluteContentPath = cacheResult.CachedFilePath,
        RelativeContentPath = relativeContentPath ?? cacheResult.RelativeContentPath,
        RelativeHashedContentPath = relativeHashedContentPath ?? cacheResult.RelativeHashedContentPath,
        ResourcePivotKeys = (IEnumerable<ResourcePivotKey>) resourcePivotKeys
      };
    }

    public static ContentItem FromFile(
      string absoluteContentPath,
      string relativeContentPath = null,
      string relativeHashedContentPath = null,
      params ResourcePivotKey[] resourcePivotKeys)
    {
      return new ContentItem()
      {
        ContentItemType = ContentItemType.Path,
        AbsoluteContentPath = absoluteContentPath,
        RelativeContentPath = relativeContentPath ?? absoluteContentPath,
        RelativeHashedContentPath = relativeHashedContentPath,
        ResourcePivotKeys = (IEnumerable<ResourcePivotKey>) resourcePivotKeys
      };
    }

    public static ContentItem FromContentItem(
      ContentItem contentItem,
      string relativeContentPath = null,
      string relativeHashedContentPath = null)
    {
      return new ContentItem()
      {
        RelativeHashedContentPath = relativeHashedContentPath ?? contentItem.RelativeHashedContentPath,
        RelativeContentPath = relativeContentPath ?? contentItem.RelativeContentPath,
        AbsoluteContentPath = contentItem.AbsoluteContentPath,
        ContentItemType = contentItem.ContentItemType,
        ContentValue = contentItem.ContentValue,
        ResourcePivotKeys = contentItem.ResourcePivotKeys,
        contentHash = contentItem.contentHash
      };
    }

    public static ContentItem FromContent(
      string content,
      params ResourcePivotKey[] resourcePivotKeys)
    {
      return new ContentItem()
      {
        ContentItemType = ContentItemType.Value,
        ContentValue = content,
        ResourcePivotKeys = (IEnumerable<ResourcePivotKey>) resourcePivotKeys
      };
    }

    public static ContentItem FromContent(
      string content,
      string relativeContentPath,
      string relativeHashedContentPath = null,
      params ResourcePivotKey[] resourcePivotKeys)
    {
      return new ContentItem()
      {
        ContentItemType = ContentItemType.Value,
        ContentValue = content,
        ResourcePivotKeys = (IEnumerable<ResourcePivotKey>) resourcePivotKeys,
        RelativeContentPath = relativeContentPath,
        RelativeHashedContentPath = relativeHashedContentPath
      };
    }

    public static ContentItem FromContent(
      string content,
      ContentItem contentItem,
      params ResourcePivotKey[] resourcePivotKeys)
    {
      ContentItem contentItem1 = new ContentItem();
      contentItem1.ContentItemType = ContentItemType.Value;
      contentItem1.ContentValue = content;
      contentItem1.RelativeContentPath = contentItem.RelativeContentPath;
      contentItem1.RelativeHashedContentPath = contentItem.RelativeHashedContentPath;
      ContentItem contentItem2 = contentItem1;
      ResourcePivotKey[] resourcePivotKeyArray = resourcePivotKeys;
      IEnumerable<ResourcePivotKey> resourcePivotKeys1 = resourcePivotKeyArray != null ? (IEnumerable<ResourcePivotKey>) resourcePivotKeyArray : contentItem.ResourcePivotKeys;
      contentItem2.ResourcePivotKeys = resourcePivotKeys1;
      return contentItem1;
    }

    internal string GetContentHash(IWebGreaseContext context) => this.contentHash ?? (this.contentHash = this.ContentItemType == ContentItemType.Value ? context.GetValueHash(this.Content) : context.GetFileHash(this.AbsoluteContentPath));

    internal void WriteToRelativeHashedPath(string destinationDirectory, bool overwrite = false) => this.WriteTo(Path.Combine(destinationDirectory ?? string.Empty, this.RelativeHashedContentPath), overwrite);

    internal void WriteToContentPath(string destinationDirectory, bool overwrite = false) => this.WriteTo(Path.Combine(destinationDirectory ?? string.Empty, this.RelativeContentPath), overwrite);

    internal void WriteTo(string fullPath, bool overwrite = false)
    {
      FileInfo absolutePath = new FileInfo(fullPath);
      Safe.FileLock((FileSystemInfo) absolutePath, (Action) (() =>
      {
        if (absolutePath.Exists && !overwrite)
          return;
        if (absolutePath.Directory != null && !absolutePath.Directory.Exists)
          absolutePath.Directory.Create();
        if (this.ContentItemType == ContentItemType.Path)
          File.Copy(this.AbsoluteContentPath, absolutePath.FullName, overwrite);
        else
          File.WriteAllText(absolutePath.FullName, this.Content);
      }));
    }

    private string ContentFromDisk() => this.content ?? (this.content = File.ReadAllText(this.AbsoluteContentPath));
  }
}
