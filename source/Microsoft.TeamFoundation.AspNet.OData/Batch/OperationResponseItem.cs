// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.OperationResponseItem
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  public class OperationResponseItem : ODataBatchResponseItem
  {
    public OperationResponseItem(HttpResponseMessage response) => this.Response = response != null ? response : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (response));

    public HttpResponseMessage Response { get; private set; }

    public override Task WriteResponseAsync(
      ODataBatchWriter writer,
      CancellationToken cancellationToken)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      return ODataBatchResponseItem.WriteMessageAsync(writer, this.Response, cancellationToken);
    }

    internal override bool IsResponseSuccessful() => this.Response.IsSuccessStatusCode;

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Response.Dispose();
    }
  }
}
