// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssTracingHttpRetryMessageHandler
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class VssTracingHttpRetryMessageHandler : VssHttpRetryMessageHandler
  {
    private readonly string m_traceArea = "Framework";
    public readonly string m_traceLayer = "VssHttpRetryMessageHandler";

    public VssTracingHttpRetryMessageHandler(int maxRetries, string clientName)
      : base(maxRetries, clientName)
    {
    }

    protected override void TraceRaw(
      HttpRequestMessage request,
      int tracepoint,
      TraceLevel level,
      string message,
      params object[] args)
    {
      string orchestrationId;
      request.Headers.TryGetSingleValue("X-VSS-OrchestrationId", out orchestrationId);
      Guid e2eId;
      HttpRequestMessageExtensions.TryGetHeaderGuid((HttpHeaders) request.Headers, "X-VSS-E2EID", out e2eId);
      Guid uniqueIdentifier;
      HttpRequestMessageExtensions.TryGetHeaderGuid((HttpHeaders) request.Headers, "X-TFS-Session", out uniqueIdentifier);
      TeamFoundationTracingService.TraceRaw(tracepoint, level, this.m_traceArea, this.m_traceLayer, e2eId, orchestrationId, uniqueIdentifier, message, args);
    }
  }
}
