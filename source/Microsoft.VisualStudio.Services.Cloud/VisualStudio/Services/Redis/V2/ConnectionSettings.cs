// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Redis.V2.ConnectionSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using StackExchange.Redis;
using System;

namespace Microsoft.VisualStudio.Services.Redis.V2
{
  public class ConnectionSettings
  {
    public ConfigurationOptions ConfigurationOptions { get; private set; }

    public int MaxMessageSize { get; private set; }

    public int RetryCount { get; private set; }

    public int MaxFailuresPerRequest { get; private set; }

    public double BatchingRadixBackoff { get; private set; }

    public ConnectionSettings(
      ConfigurationOptions configurationOptions,
      int maxMessageSize,
      int retryCount,
      int maxFailuresPerRequest,
      double batchingRadixBackoff)
    {
      this.ConfigurationOptions = configurationOptions;
      this.MaxMessageSize = maxMessageSize;
      this.RetryCount = retryCount;
      this.MaxFailuresPerRequest = maxFailuresPerRequest;
      this.BatchingRadixBackoff = batchingRadixBackoff;
    }

    public override bool Equals(object obj)
    {
      ConnectionSettings connectionSettings = obj as ConnectionSettings;
      return string.Equals(this.AsString(), connectionSettings?.AsString(), StringComparison.Ordinal);
    }

    public override int GetHashCode() => this.AsString().GetHashCode();

    private string AsString() => string.Format("{0},maxMessageSize={1},retryCount={2},maxFailures={3},batchingRadix={4}", (object) this.ConfigurationOptions, (object) this.MaxMessageSize, (object) this.RetryCount, (object) this.MaxFailuresPerRequest, (object) this.BatchingRadixBackoff);
  }
}
