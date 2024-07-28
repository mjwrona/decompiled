// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Handlers.DiagnosticsHandler
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Handler;
using Microsoft.Azure.Cosmos.Tracing;
using Microsoft.Azure.Cosmos.Tracing.TraceData;
using Microsoft.Azure.Documents.Rntbd;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Handlers
{
  internal class DiagnosticsHandler : RequestHandler
  {
    public override async Task<ResponseMessage> SendAsync(
      RequestMessage request,
      CancellationToken cancellationToken)
    {
      ResponseMessage responseMessage = await base.SendAsync(request, cancellationToken);
      SystemUsageHistory diagnosticsSystemHistory = DiagnosticsHandlerHelper.Instance.GetDiagnosticsSystemHistory();
      if (diagnosticsSystemHistory != null)
        request.Trace.AddDatum("System Info", (TraceDatum) new CpuHistoryTraceDatum(diagnosticsSystemHistory));
      return responseMessage;
    }
  }
}
