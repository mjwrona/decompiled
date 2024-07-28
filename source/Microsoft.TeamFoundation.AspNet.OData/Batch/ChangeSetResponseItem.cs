// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Batch.ChangeSetResponseItem
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.OData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.AspNet.OData.Batch
{
  public class ChangeSetResponseItem : ODataBatchResponseItem
  {
    public ChangeSetResponseItem(IEnumerable<HttpResponseMessage> responses) => this.Responses = responses != null ? responses : throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (responses));

    public IEnumerable<HttpResponseMessage> Responses { get; private set; }

    public override async Task WriteResponseAsync(
      ODataBatchWriter writer,
      CancellationToken cancellationToken)
    {
      if (writer == null)
        throw Microsoft.AspNet.OData.Common.Error.ArgumentNull(nameof (writer));
      writer.WriteStartChangeset();
      foreach (HttpResponseMessage response in this.Responses)
        await ODataBatchResponseItem.WriteMessageAsync(writer, response, cancellationToken);
      writer.WriteEndChangeset();
    }

    internal override bool IsResponseSuccessful() => this.Responses.All<HttpResponseMessage>((Func<HttpResponseMessage, bool>) (r => r.IsSuccessStatusCode));

    protected override void Dispose(bool disposing)
    {
      if (!disposing)
        return;
      foreach (HttpResponseMessage response in this.Responses)
        response?.Dispose();
    }
  }
}
