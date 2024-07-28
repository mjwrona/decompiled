// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.StorageId
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public static class StorageId
  {
    public static IStorageId Parse(string input)
    {
      string[] strArray1 = input.Split(new char[1]{ ':' }, 2, StringSplitOptions.None);
      if (strArray1.Length == 1)
        return (IStorageId) new BlobStorageId(BlobIdentifier.Deserialize(input));
      string str = strArray1[0];
      if (str != null)
      {
        switch (str.Length)
        {
          case 4:
            switch (str[0])
            {
              case 'b':
                if (str == "blob")
                  return (IStorageId) new BlobStorageId(BlobIdentifier.Deserialize(strArray1[1]));
                break;
              case 'd':
                if (str == "drop")
                  return (IStorageId) new DropStorageId(strArray1[1]);
                break;
            }
            break;
          case 5:
            if (str == "dedup")
            {
              string[] strArray2 = strArray1[1].Split(new char[1]
              {
                ':'
              }, 2, StringSplitOptions.None);
              return (IStorageId) new DedupStoreStorageId(DedupIdentifier.Create(strArray2[0]), DedupIdentifier.Create(strArray2[1]));
            }
            break;
          case 7:
            if (str == "literal")
              return (IStorageId) new LiteralStringStorageId(strArray1[1]);
            break;
          case 8:
            if (str == "upstream")
              return (IStorageId) new UpstreamStorageId(JsonUtilities.Deserialize<UpstreamSourceInfo>(strArray1[1]));
            break;
          case 9:
            if (str == "upstreams")
              return (IStorageId) new TryAllUpstreamsStorageId();
            break;
          case 11:
            if (str == "zip-extract")
              return (IStorageId) ExtractFileFromZipStorageId.Parse(strArray1[1]);
            break;
        }
      }
      throw new InvalidOperationException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UnknownStorageType());
    }
  }
}
