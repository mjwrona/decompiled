// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MessageBusPublisherCreateOptions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class MessageBusPublisherCreateOptions
  {
    public bool DeleteIfExists { get; set; }

    public string Namespace { get; set; }

    public bool EnableExpress { get; set; }

    public bool? EnablePartitioning { get; set; }

    public TimeSpan SubscriptionIdleTimeout { get; set; }

    public TimeSpan SubscriptionMessageTimeToLive { get; set; }

    public int SubscriptionPrefetchCount { get; set; }
  }
}
