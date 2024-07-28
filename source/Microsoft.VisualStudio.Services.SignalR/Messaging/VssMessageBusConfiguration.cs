// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.Messaging.VssMessageBusConfiguration
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.SignalR.Messaging
{
  public class VssMessageBusConfiguration : VssScaleoutConfiguration
  {
    public static readonly TimeSpan SubscriptionIdleTimeout = TimeSpan.FromMinutes(10.0);
    public static readonly TimeSpan SubscriptionMessageTimeToLive = TimeSpan.FromMinutes(1.0);
    private const string c_baseTopicName = "Microsoft.VisualStudio.Services.SignalR";

    public VssMessageBusConfiguration() => this.TopicCount = 1;

    public string Namespace { get; set; }

    public int TopicCount { get; set; }

    public string TopicSuffix { get; set; }

    public IList<string> GetTopicNames() => (IList<string>) Enumerable.Range(0, this.TopicCount).Select<int, string>((Func<int, string>) (x => this.FormatTopicName(x))).ToList<string>();

    private string FormatTopicName(int topicIndex) => string.IsNullOrEmpty(this.TopicSuffix) ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}", (object) "Microsoft.VisualStudio.Services.SignalR", (object) topicIndex) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-{1}-{2}", (object) "Microsoft.VisualStudio.Services.SignalR", (object) this.TopicSuffix, (object) topicIndex);
  }
}
