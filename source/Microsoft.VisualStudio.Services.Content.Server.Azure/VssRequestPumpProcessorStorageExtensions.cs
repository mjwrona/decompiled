// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.VssRequestPumpProcessorStorageExtensions
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

using Microsoft.Azure.Storage;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public static class VssRequestPumpProcessorStorageExtensions
  {
    internal static OperationContext CreateStorageContext(
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
      return context;
    }
  }
}
