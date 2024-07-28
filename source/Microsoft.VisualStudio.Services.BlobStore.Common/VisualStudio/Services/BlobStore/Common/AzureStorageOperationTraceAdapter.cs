// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.AzureStorageOperationTraceAdapter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Content.Common.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  [CLSCompliant(false)]
  public class AzureStorageOperationTraceAdapter
  {
    private readonly IAppTraceSource tracer;

    public AzureStorageOperationTraceAdapter(IAppTraceSource tracer)
    {
      ArgumentUtility.CheckForNull<IAppTraceSource>(tracer, nameof (tracer));
      this.tracer = tracer;
    }

    public void AttachTracing(OperationContext context)
    {
      context.LogLevel = Microsoft.Azure.Storage.LogLevel.Verbose;
      context.RequestCompleted += new EventHandler<RequestEventArgs>(this.RequestCompleted);
      context.ResponseReceived += new EventHandler<RequestEventArgs>(this.ResponseReceived);
      context.Retrying += new EventHandler<RequestEventArgs>(this.Retrying);
      context.SendingRequest += new EventHandler<RequestEventArgs>(this.SendingRequest);
    }

    private void RequestCompleted(object sender, RequestEventArgs e)
    {
    }

    private void ResponseReceived(object sender, RequestEventArgs e) => this.TraceStorageOperation(nameof (ResponseReceived), sender, e, false);

    private void Retrying(object sender, RequestEventArgs e) => this.TraceStorageOperation(nameof (Retrying), sender, e, true);

    private void SendingRequest(object sender, RequestEventArgs e) => this.TraceStorageOperation(nameof (SendingRequest), sender, e, false);

    private void TraceStorageOperation(
      string label,
      object sender,
      RequestEventArgs e,
      bool infoLevel)
    {
      HttpRequestMessage request = e.Request;
      HttpResponseMessage response = e.Response;
      StringBuilder stringBuilder = new StringBuilder("AzureStorage ");
      stringBuilder.Append(label);
      stringBuilder.Append(", ");
      if (response != null)
      {
        string str = (string) null;
        IEnumerable<string> values;
        if (response.Headers.TryGetValues("x-ms-request-id", out values))
          str = values.FirstOrDefault<string>();
        stringBuilder.Append(string.Format("{0} {1}, ContentLength {2}", (object) "x-ms-request-id", (object) str, (object) response.Content.Headers.ContentLength));
      }
      else
      {
        string str = (string) null;
        IEnumerable<string> values;
        if (request.Headers.TryGetValues("x-ms-client-request-id", out values))
          str = values.FirstOrDefault<string>();
        stringBuilder.Append(string.Format("{0} {1}, {2}", (object) "x-ms-client-request-id", (object) str, (object) request.Method));
        RangeHeaderValue range = request.Headers.Range;
        if (range != null)
        {
          stringBuilder.Append(", ");
          stringBuilder.Append((object) range);
        }
        stringBuilder.Append(", ");
        stringBuilder.Append((object) request.RequestUri);
      }
      string format = stringBuilder.ToString();
      if (infoLevel)
        this.tracer.Info(format);
      else
        this.tracer.Verbose(format);
    }
  }
}
