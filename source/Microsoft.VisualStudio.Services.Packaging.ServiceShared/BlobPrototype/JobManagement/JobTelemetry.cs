// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement.JobTelemetry
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JobManagement
{
  public class JobTelemetry
  {
    public string Message { get; set; }

    public string ExceptionType { get; set; }

    public string ExceptionMessage { get; set; }

    [JsonIgnore]
    public Exception Exception { get; set; }

    public void LogException(Exception ex)
    {
      this.Exception = ex;
      this.ExceptionType = ex?.GetType().FullName;
      this.ExceptionMessage = ex != null ? StackTraceCompressor.CompressStackTrace(ex.ToString()) : (string) null;
    }

    public static void SetRequestContextEtwTracingData(
      IVssRequestContext requestContext,
      string feedId,
      IProtocol protocol)
    {
      requestContext.Items["Packaging.FeedId"] = (object) feedId;
      requestContext.Items["Packaging.Protocol"] = (object) protocol;
    }
  }
}
