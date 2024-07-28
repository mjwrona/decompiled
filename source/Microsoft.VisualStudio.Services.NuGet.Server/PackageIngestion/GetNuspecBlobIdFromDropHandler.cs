// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion.GetNuspecBlobIdFromDropHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.PackageIngestion
{
  public class GetNuspecBlobIdFromDropHandler : 
    IAsyncHandler<string, BlobIdentifier>,
    IHaveInputType<string>,
    IHaveOutputType<BlobIdentifier>
  {
    private readonly IDropHttpClient dropHttpClient;

    public GetNuspecBlobIdFromDropHandler(IDropHttpClient dropHttpClient) => this.dropHttpClient = dropHttpClient;

    public async Task<BlobIdentifier> Handle(string dropName)
    {
      FileItem nuspecItem = (FileItem) null;
      IConcurrentIterator<IDictionary<Locator, FileItem>> fileListEnumerator = await this.dropHttpClient.GetDropFileListAsync(dropName, false);
label_10:
      bool flag = nuspecItem == null;
      if (flag)
        flag = await fileListEnumerator.MoveNextAsync(new CancellationToken());
      if (flag)
      {
        using (IEnumerator<KeyValuePair<Locator, FileItem>> enumerator = fileListEnumerator.Current.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<Locator, FileItem> current = enumerator.Current;
            Locator key = current.Key;
            FileItem fileItem = current.Value;
            if (key.PathSegmentCount == 1 && key.PathSegments[0].EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase))
            {
              nuspecItem = fileItem;
              break;
            }
          }
          goto label_10;
        }
      }
      else
      {
        BlobIdentifier blobIdentifier = nuspecItem?.BlobIdentifier;
        nuspecItem = (FileItem) null;
        fileListEnumerator = (IConcurrentIterator<IDictionary<Locator, FileItem>>) null;
        return blobIdentifier;
      }
    }
  }
}
