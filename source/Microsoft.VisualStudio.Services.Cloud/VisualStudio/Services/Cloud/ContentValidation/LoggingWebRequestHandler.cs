// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ContentValidation.LoggingWebRequestHandler
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Cloud.ContentValidation
{
  internal class LoggingWebRequestHandler : WebRequestHandler
  {
    private readonly string m_logName;

    public static AsyncLocal<Guid> E2EID { get; } = new AsyncLocal<Guid>();

    public static AsyncLocal<Guid> ActivityId { get; } = new AsyncLocal<Guid>();

    public LoggingWebRequestHandler(string logName) => this.m_logName = logName;

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage request,
      CancellationToken cancellationToken)
    {
      DateTime startTime = DateTime.UtcNow;
      HttpResponseMessage response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
      int msTaken = (int) (DateTime.UtcNow - startTime).TotalMilliseconds;
      try
      {
        string errorMessage = string.Empty;
        if (!response.IsSuccessStatusCode)
        {
          string str1 = response.ReasonPhrase;
          string str2 = Environment.NewLine;
          errorMessage = str1 + str2 + await response.Content.ReadAsStringAsync().ConfigureAwait(false);
          str1 = (string) null;
          str2 = (string) null;
        }
        using (new VssActivityScope(LoggingWebRequestHandler.ActivityId.Value))
          TeamFoundationTracingService.TraceHttpOutgoingRequest(startTime, msTaken, this.m_logName, request.Method.Method, request.RequestUri.Host, request.RequestUri.AbsolutePath, (int) response.StatusCode, errorMessage, LoggingWebRequestHandler.E2EID.Value, string.Empty, string.Empty, Guid.Empty, string.Empty, string.Empty);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(15289017, nameof (LoggingWebRequestHandler), nameof (SendAsync), ex);
      }
      HttpResponseMessage httpResponseMessage = response;
      response = (HttpResponseMessage) null;
      return httpResponseMessage;
    }
  }
}
