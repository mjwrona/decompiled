// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsStateInvalidTransitionException
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Analytics
{
  public class AnalyticsStateInvalidTransitionException : VssServiceException
  {
    public AnalyticsState current;
    public AnalyticsState target;

    public AnalyticsStateInvalidTransitionException(string message)
      : base(message)
    {
    }

    public AnalyticsStateInvalidTransitionException(
      string message,
      AnalyticsState current,
      AnalyticsState target)
      : base(message)
    {
      this.current = current;
      this.target = target;
    }
  }
}
