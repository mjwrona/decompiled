// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.AzureBlobContainerFactoryFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Azure;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class AzureBlobContainerFactoryFacade : ICloudBlobContainerFactory
  {
    private readonly ICancellationFacade cancellation;
    private readonly AzureBlobContainerFactory azureBlobContainerFactory;

    public AzureBlobContainerFactoryFacade(
      AzureBlobContainerFactory azureBlobContainerFactory,
      ICancellationFacade cancellation)
    {
      this.cancellation = cancellation;
      this.azureBlobContainerFactory = azureBlobContainerFactory;
    }

    public ICloudBlobContainer Get(string input) => this.azureBlobContainerFactory.CreateContainerReference(input, false);

    public IEnumerable<ICloudBlobContainer> Get()
    {
      IConcurrentIterator<ICloudBlobContainer> ConcurrentIterator = this.azureBlobContainerFactory.GetAllContainerReferences(this.cancellation.Token);
      return (IEnumerable<ICloudBlobContainer>) AsyncPump.Run<List<ICloudBlobContainer>>((Func<Task<List<ICloudBlobContainer>>>) (() => ConcurrentIterator.ToListAsync<ICloudBlobContainer>(this.cancellation.Token)));
    }

    public IEnumerable<ICloudBlobContainer> GetByPrefix(string prefix)
    {
      IConcurrentIterator<ICloudBlobContainer> ConcurrentIterator = this.azureBlobContainerFactory.GetContainerReferences(prefix, this.cancellation.Token);
      return (IEnumerable<ICloudBlobContainer>) AsyncPump.Run<List<ICloudBlobContainer>>((Func<Task<List<ICloudBlobContainer>>>) (() => ConcurrentIterator.ToListAsync<ICloudBlobContainer>(this.cancellation.Token)));
    }
  }
}
