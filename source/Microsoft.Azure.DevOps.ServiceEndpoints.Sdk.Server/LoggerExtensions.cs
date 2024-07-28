// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.LoggerExtensions
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using System.Diagnostics;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public static class LoggerExtensions
  {
    public static void Info(this ILogger logger, string message, int tracepointId = 0) => logger?.Log(TraceLevel.Info, tracepointId, message);

    public static void Warning(this ILogger logger, string message, int tracepointId = 0) => logger?.Log(TraceLevel.Warning, tracepointId, message);

    public static void Error(this ILogger logger, string message, int tracepointId = 0) => logger?.Log(TraceLevel.Error, tracepointId, message);

    public static void Verbose(this ILogger logger, string message, int tracepointId = 0) => logger?.Log(TraceLevel.Verbose, tracepointId, message);
  }
}
