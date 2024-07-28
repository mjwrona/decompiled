// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.OperationRequestItem
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  public class OperationRequestItem : ODataBatchRequestItem
  {
    public OperationRequestItem(HttpRequestMessage request) => this.Request = request != null ? request : throw Error.ArgumentNull(nameof (request));

    public HttpRequestMessage Request { get; private set; }

    public override async Task<ODataBatchResponseItem> SendRequestAsync(
      HttpMessageInvoker invoker,
      CancellationToken cancellationToken)
    {
      if (invoker == null)
        throw Error.ArgumentNull(nameof (invoker));
      return (ODataBatchResponseItem) new OperationResponseItem(await ODataBatchRequestItem.SendMessageAsync(invoker, this.Request, cancellationToken, (Dictionary<string, string>) null));
    }

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      this.Request.Dispose();
    }

    public override IEnumerable<IDisposable> GetResourcesForDisposal() => this.Request.GetResourcesForDisposal();
  }
}
