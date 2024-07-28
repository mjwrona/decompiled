// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions.WorkItemLooseFileStorageEndpointHelpers
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Common.StorageEndpoint;
using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.ReSearch.Core.Stores.StorageEndpoint.Extensions
{
  public static class WorkItemLooseFileStorageEndpointHelpers
  {
    public const char MultiContentHashDelimiter = '_';

    public static string GetObjectPath(string path, ContentId contentId)
    {
      string path2 = (int.Parse(contentId.ItemId, (IFormatProvider) CultureInfo.InvariantCulture) / 1000).ToString((IFormatProvider) CultureInfo.InvariantCulture);
      string hexHash = contentId.ObjectId.HexHash;
      string contentKey = contentId.ContentKey;
      return Path.Combine(path, path2, hexHash, contentKey);
    }

    public static IEnumerable<ContentId> OrderFiles(IEnumerable<string> files) => WorkItemLooseFileStorageEndpointHelpers.OrderHexContentKeyStrings(files.Select<string, string>(WorkItemLooseFileStorageEndpointHelpers.\u003C\u003EO.\u003C0\u003E__GetFileName ?? (WorkItemLooseFileStorageEndpointHelpers.\u003C\u003EO.\u003C0\u003E__GetFileName = new Func<string, string>(Path.GetFileName))));

    public static IEnumerable<string> OrderFolders(IEnumerable<IDirectory> folders) => WorkItemLooseFileStorageEndpointHelpers.OrderHexStrings(folders.Select<IDirectory, string>((Func<IDirectory, string>) (f => Path.GetFileName(f.FullPath)))).Select<Hash, string>((Func<Hash, string>) (id => id.HexHash));

    public static IEnumerable<Hash> OrderHexStrings(IEnumerable<string> strings) => (IEnumerable<Hash>) strings.Select<string, Hash>((Func<string, Hash>) (s => new Hash(s))).OrderBy<Hash, Hash>((Func<Hash, Hash>) (id => id));

    public static IEnumerable<ContentId> OrderHexContentKeyStrings(IEnumerable<string> strings)
    {
      List<ContentId> source = new List<ContentId>();
      foreach (string hexHash in strings)
      {
        int length = hexHash.IndexOf('_');
        source.Add(length != -1 ? new ContentId(new Hash(hexHash.Substring(0, length)), hexHash.Substring(length + 1)) : new ContentId(new Hash(hexHash)));
      }
      return (IEnumerable<ContentId>) source.OrderBy<ContentId, Hash>((Func<ContentId, Hash>) (id => id.ObjectId));
    }
  }
}
