// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.MessageReceiverSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ServiceBus.Messaging;
using System;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class MessageReceiverSettings : IEquatable<MessageReceiverSettings>
  {
    public Uri Uri;
    public MessageBusCredentials Credentials;
    public int BatchFlushInterval;
    public int PrefetchCount;
    public int TraceDelay;
    public string FilterKey;
    public string FilterValue;
    public TransportType TransportType;
    public string TopicName;
    public string Namespace;
    public string SubscriberName;
    public int DeadLetterCleanupBatchSize;

    public bool Equals(MessageReceiverSettings other) => string.Equals(this.Uri.AbsoluteUri, other.Uri.AbsoluteUri, StringComparison.OrdinalIgnoreCase) && this.Credentials.Equals(other.Credentials) && this.BatchFlushInterval == other.BatchFlushInterval && this.PrefetchCount == other.PrefetchCount && this.TraceDelay == other.TraceDelay && this.FilterKey == other.FilterKey && this.FilterValue == other.FilterValue && this.TransportType == other.TransportType && string.Equals(this.TopicName, other.TopicName, StringComparison.OrdinalIgnoreCase) && string.Equals(this.Namespace, other.Namespace, StringComparison.OrdinalIgnoreCase) && string.Equals(this.SubscriberName, other.SubscriberName, StringComparison.OrdinalIgnoreCase) && this.DeadLetterCleanupBatchSize == other.DeadLetterCleanupBatchSize;

    public override string ToString() => this.Namespace != null ? string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}::{1}::{2}", (object) this.Namespace, (object) this.TopicName, (object) this.SubscriberName) : string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}::{1}", (object) this.TopicName, (object) this.SubscriberName);
  }
}
