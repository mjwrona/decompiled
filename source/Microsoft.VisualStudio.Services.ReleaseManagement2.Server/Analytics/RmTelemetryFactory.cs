// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics.RmTelemetryFactory
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Analytics
{
  public static class RmTelemetryFactory
  {
    public static IRmTelemetryLogger GetLogger(IVssRequestContext requestContext) => requestContext != null && !requestContext.ExecutionEnvironment.IsHostedDeployment ? (IRmTelemetryLogger) RmOnPremTelemetryLogger.Instance : (IRmTelemetryLogger) RmHostedTelemetryLogger.Instance;
  }
}
