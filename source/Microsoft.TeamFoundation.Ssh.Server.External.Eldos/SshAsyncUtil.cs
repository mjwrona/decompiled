// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.SshAsyncUtil
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  internal static class SshAsyncUtil
  {
    internal static void RethrowNonCanceled(
      ITraceRequest tracer,
      int tracepoint,
      string layer,
      AggregateException ex)
    {
      if (tracer.IsTracing(tracepoint, TraceLevel.Verbose, "Ssh", layer))
      {
        try
        {
          ex.Handle((Func<Exception, bool>) (innerEx => !(innerEx is System.OperationCanceledException)));
        }
        catch (AggregateException ex1)
        {
          tracer.TraceException(tracepoint, TraceLevel.Verbose, "Ssh", layer, (Exception) ex1);
        }
      }
      ex.Handle((Func<Exception, bool>) (innerEx => innerEx is System.OperationCanceledException));
    }
  }
}
