// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FileContainerItemExtensions
// Assembly: Microsoft.TeamFoundation.FileContainer.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D69E508F-D51F-4EF2-B979-7E96E61DA19E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.FileContainer.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class FileContainerItemExtensions
  {
    private static readonly string s_area = "FileContainer";
    private static readonly string s_layer = nameof (FileContainerItemExtensions);

    internal static void Validate(
      this FileContainerItem fileContainerItem,
      IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fileContainerItem.Path, "Path");
      try
      {
        string a = FileContainerPathHelper.NormalizePath(fileContainerItem.Path);
        if (!string.Equals(a, fileContainerItem.Path, StringComparison.Ordinal))
          throw new InvalidPathException(FrameworkResources.InvalidPathNotCanonical((object) fileContainerItem.Path, (object) a));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1008400, FileContainerItemExtensions.s_area, FileContainerItemExtensions.s_layer, ex);
        throw;
      }
      switch (fileContainerItem.ItemType)
      {
        case ContainerItemType.Folder:
          break;
        case ContainerItemType.File:
          ArgumentUtility.CheckForOutOfRange(fileContainerItem.FileLength, "FileLength", 0L);
          ArgumentUtility.CheckForOutOfRange(fileContainerItem.FileEncoding, "FileEncoding", 0);
          ArgumentUtility.CheckForOutOfRange(fileContainerItem.FileType, "FileType", 0);
          break;
        default:
          throw new ArgumentException("ItemType");
      }
    }

    public static void GenerateContainerItemTickets(
      this IEnumerable<FileContainerItem> items,
      IVssRequestContext requestContext)
    {
      using (UrlSigner urlSigner = new UrlSigner(requestContext))
        urlSigner.SignObject((ISignable) new FileContainerItemExtensions.FileContainerSignable(items));
    }

    private class FileContainerSignable : ISignable
    {
      private List<FileContainerItem> m_items;

      public FileContainerSignable(IEnumerable<FileContainerItem> items) => this.m_items = items.Where<FileContainerItem>((Func<FileContainerItem, bool>) (x => x.ItemType == ContainerItemType.File)).ToList<FileContainerItem>();

      public int GetDownloadUrlCount() => this.m_items.Count;

      public int GetFileId(int index) => this.m_items[index].FileId;

      public byte[] GetHashValue(int index) => (byte[]) null;

      public void SetDownloadUrl(int index, string downloadUrl) => this.m_items[index].Ticket = downloadUrl;
    }
  }
}
