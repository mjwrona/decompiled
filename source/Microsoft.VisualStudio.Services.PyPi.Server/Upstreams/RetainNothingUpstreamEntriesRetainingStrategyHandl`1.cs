// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.RetainNothingUpstreamEntriesRetainingStrategyHandler`1
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class RetainNothingUpstreamEntriesRetainingStrategyHandler<TMetadataEntry> : 
    IAsyncHandler<MetadataDocument<TMetadataEntry>, List<TMetadataEntry>>,
    IHaveInputType<MetadataDocument<TMetadataEntry>>,
    IHaveOutputType<List<TMetadataEntry>>
    where TMetadataEntry : class, IMetadataEntry
  {
    public Task<List<TMetadataEntry>> Handle(MetadataDocument<TMetadataEntry> request) => Task.FromResult<List<TMetadataEntry>>(new List<TMetadataEntry>());
  }
}
