// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ODataHttpContentExtensions
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class ODataHttpContentExtensions
  {
    public static Task<ODataMessageReader> GetODataMessageReaderAsync(
      this HttpContent content,
      IServiceProvider requestContainer)
    {
      return content.GetODataMessageReaderAsync(requestContainer, CancellationToken.None);
    }

    public static async Task<ODataMessageReader> GetODataMessageReaderAsync(
      this HttpContent content,
      IServiceProvider requestContainer,
      CancellationToken cancellationToken)
    {
      if (content == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (content));
      cancellationToken.ThrowIfCancellationRequested();
      return new ODataMessageReader((IODataRequestMessage) ODataMessageWrapperHelper.Create(await content.ReadAsStreamAsync(), content.Headers, requestContainer), ServiceProviderServiceExtensions.GetRequiredService<ODataMessageReaderSettings>(requestContainer));
    }
  }
}
