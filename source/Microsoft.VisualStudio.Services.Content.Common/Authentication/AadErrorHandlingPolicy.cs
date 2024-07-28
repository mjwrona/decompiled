// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.Authentication.AadErrorHandlingPolicy
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.Identity.Client;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common.Authentication
{
  internal class AadErrorHandlingPolicy : IAadErrorHandlingPolicy
  {
    private int aadRetryCount;
    private TimeSpan aadRetryDelay;
    private LogCallback log;
    private bool logStarted;

    internal AadErrorHandlingPolicy(
      int aadRetryCount,
      TimeSpan aadRetryDelay,
      LogCallback logCallback)
    {
      this.aadRetryCount = aadRetryCount;
      this.aadRetryDelay = aadRetryDelay;
      this.log = logCallback != null ? logCallback : throw new ArgumentNullException(nameof (logCallback));
    }

    public int AadRetryCount => this.aadRetryCount;

    public TimeSpan AadRetryDelay => this.aadRetryDelay;

    public void Log(Microsoft.Identity.Client.LogLevel level, string message)
    {
      if (!this.logStarted)
        return;
      this.log(level, message, false);
    }

    public void StartAadLogging() => this.logStarted = true;

    public void StopAadLogging() => this.logStarted = false;
  }
}
