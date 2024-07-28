// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades.DropClientFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Drop.WebApi;
using Microsoft.VisualStudio.Services.ItemStore.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades
{
  public class DropClientFacade : IDropHttpClient
  {
    private readonly IVssRequestContext requestContext;

    public DropClientFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public async Task<DropItem> GetDropAsync(string dropName) => await this.requestContext.GetClient<DropHttpClient>().GetDropAsync(dropName, this.requestContext.CancellationToken);

    public async Task<HttpResponseMessage> DeleteDropAsync(DropItem drop, bool synchronous) => await this.requestContext.GetClient<DropHttpClient>().DeleteDropAsync(drop, synchronous, this.requestContext.CancellationToken);

    public async Task<IConcurrentIterator<IDictionary<Locator, FileItem>>> GetDropFileListAsync(
      string dropName,
      bool getDownloadUris)
    {
      return await this.requestContext.GetClient<DropHttpClient>().GetDropFileListAsync(dropName, getDownloadUris, this.requestContext.CancellationToken, (IEnumerable<string>) null, true);
    }

    public bool IgnoreDeleteAccessDeniedException => this.requestContext.IsFeatureEnabledWithLogging("Packaging.IgnoreDeleteDropAccessDeniedException");
  }
}
