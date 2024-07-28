// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore.BasicMetaDataStoreReader
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Microsoft.VisualStudio.Services.Search.Server.Storage.FileSystem.Definitions;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Stores.TreeStore
{
  public abstract class BasicMetaDataStoreReader
  {
    public static IMetaDataStore Create(string path, IFileSystem fileStore)
    {
      if (string.IsNullOrWhiteSpace(path))
        throw new ArgumentException("Path can not be null or empty.", nameof (path));
      if (fileStore == null)
        throw new ArgumentNullException(nameof (fileStore));
      return fileStore.FileExists(path) ? BasicMetaDataStoreReader.CreateInternal(fileStore, path) : throw new FileNotFoundException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Path [{0}] doesn't exist at the time of MetaDataStore creation.", (object) path));
    }

    private static IMetaDataStore CreateInternal(IFileSystem fileStore, string input)
    {
      IMetaDataStore metaDataStore = (IMetaDataStore) null;
      string str1;
      string str2;
      using (IFileReader fileReader = fileStore.GetDirectory(Path.GetDirectoryName(input) ?? "\\", false).GetFileReader(Path.GetFileName(input)))
      {
        str1 = fileReader.ReadString();
        str2 = fileReader.ReadString();
      }
      if (str1 == typeof (BasicMetaDataStoreItem).Name && str2 == typeof (BasicMetaDataStoreItemEncoder).Name)
        metaDataStore = (IMetaDataStore) new BasicMetaDataStoreReader<BasicMetaDataStoreItem, BasicMetaDataStoreItemEncoder>(input, fileStore);
      else if (str1 == typeof (WikiMetaDataStoreItem).Name && str2 == typeof (WikiMetaDataStoreItemEncoder).Name)
        metaDataStore = (IMetaDataStore) new BasicMetaDataStoreReader<WikiMetaDataStoreItem, WikiMetaDataStoreItemEncoder>(input, fileStore);
      else if (str1 == typeof (BasicMetaDataStoreItem).Name && str2 == typeof (WorkItemMetaDataStoreItemEncoder).Name)
        metaDataStore = (IMetaDataStore) new BasicMetaDataStoreReader<BasicMetaDataStoreItem, WorkItemMetaDataStoreItemEncoder>(input, fileStore);
      return metaDataStore;
    }
  }
}
