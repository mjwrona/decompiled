// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.VssRequestPumpProcessorTableExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Cosmos.Table;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class VssRequestPumpProcessorTableExtensions
  {
    internal static OperationContext CreateTableContext(
      this VssRequestPump.Processor processor,
      OperationContext context = null,
      bool traceAzureStorageSdkResponseExceptions = false)
    {
      context = context ?? new OperationContext();
      if (processor.E2EId != Guid.Empty || processor.X_TFS_Session != Guid.Empty)
        context.ClientRequestID = string.Format("e2eid={0:D}, session={1:D}", (object) processor.E2EId, (object) processor.X_TFS_Session);
      if (traceAzureStorageSdkResponseExceptions)
        context.ResponseReceived += (EventHandler<RequestEventArgs>) ((sender, args) =>
        {
          if (!(sender is OperationContext) || args.RequestInformation == null || args.RequestInformation.Exception == null)
            return;
          processor.ExecuteWorkAsync((Action<IVssRequestContext>) (vssContext =>
          {
            if (args.Response != null && args.Request != null)
              vssContext.TraceAlways(5700900, TraceLevel.Error, "BlobStore", "Service", "Azure Storage SDK request failed: StatusCode={0}, Url={1}", (object) (int) args.Response.StatusCode, (object) args.Request.RequestUri.ToString());
            vssContext.TraceException(5700901, "BlobStore", "Service", args.RequestInformation.Exception);
          }));
        });
      if (TableHttpRequestTracingEnabler.TraceTableCallsToHttpOutgoingRequests)
        context.RequestCompleted += (EventHandler<RequestEventArgs>) ((_, args) =>
        {
          using (new VssActivityScope(processor.ActivityId))
          {
            string afdRefInfo = string.Empty;
            int responseCode = -1;
            string errorMessage = (string) null;
            if (args.Response != null)
            {
              IEnumerable<string> values;
              if (args.Response.Headers.TryGetValues("X-MSEdge-Ref", out values))
                afdRefInfo = values.First<string>();
              responseCode = (int) args.Response.StatusCode;
              errorMessage = responseCode == 404 ? "ErrorMessage: " + args.RequestInformation.ExtendedErrorInformation?.ErrorMessage + "\r\nRequestID: " + args.RequestInformation.ServiceRequestID : args.RequestInformation.Exception?.ToString();
            }
            TimeSpan timeSpan = args.RequestInformation.EndTime - args.RequestInformation.StartTime;
            TeamFoundationTracingService.TraceHttpOutgoingRequest(args.RequestInformation.StartTime.UtcDateTime, (int) timeSpan.TotalMilliseconds, "Artifacts.ItemStore", args.Request.Method.ToString(), args.Request.RequestUri.Host, args.Request.RequestUri.AbsolutePath, responseCode, errorMessage, processor.E2EId, afdRefInfo, "", Guid.Empty, "", "");
          }
        });
      return context;
    }
  }
}
