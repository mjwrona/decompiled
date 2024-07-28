// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.Common.DefaultTraceEx
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Net.Sockets;

namespace Microsoft.Azure.Documents.Common
{
  internal static class DefaultTraceEx
  {
    public static void TraceException(Exception e)
    {
      if (e is AggregateException aggregateException)
      {
        foreach (Exception innerException in aggregateException.InnerExceptions)
          DefaultTraceEx.TraceExceptionInternal(innerException);
      }
      else
        DefaultTraceEx.TraceExceptionInternal(e);
    }

    private static void TraceExceptionInternal(Exception e)
    {
      for (; e != null; e = e.InnerException)
      {
        Uri uri = (Uri) null;
        if (e is DocumentClientException documentClientException)
          uri = documentClientException.RequestUri;
        if (e is SocketException socketException)
          DefaultTrace.TraceWarning("Exception {0}: RequesteUri: {1}, SocketErrorCode: {2}, {3}, {4}", (object) e.GetType(), (object) uri, (object) socketException.SocketErrorCode, (object) e.Message, (object) e.StackTrace);
        else
          DefaultTrace.TraceWarning("Exception {0}: RequestUri: {1}, {2}, {3}", (object) e.GetType(), (object) uri, (object) e.Message, (object) e.StackTrace);
      }
    }
  }
}
