// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.SignalRDefaultSettings
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Cloud;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public static class SignalRDefaultSettings
  {
    public const int DefaultSignalRTopicCountForTest = 1;
    public const int DefaultSignalRTopicCount = 16;
    public static readonly object SignalRRouteRegistrationLock = new object();

    public static int GetDefaultSignalRTopicCount(
      IVssRequestContext requestContext,
      string serviceBusNamespace)
    {
      return !ServiceBusWellKnownNamespaces.IsTestNamespace(requestContext, serviceBusNamespace) ? 16 : 1;
    }
  }
}
