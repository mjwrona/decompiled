// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.DefaultAppInsightsSessionChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.DataContracts;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal sealed class DefaultAppInsightsSessionChannel : BaseAppInsightsSessionChannel
  {
    private readonly IStorageBuilder storageBuilder;
    private readonly IProcessLockFactory processLockFactory;

    public override string ChannelId => "ai";

    internal override string IKey => this.InstrumentationKey;

    protected override string FolderNameSuffix => this.InstrumentationKey;

    public DefaultAppInsightsSessionChannel(string instrumentationKey, string userId)
      : this(instrumentationKey, userId, (IStorageBuilder) new WindowsStorageBuilder(), (IProcessLockFactory) new WindowsProcessLockFactory())
    {
    }

    public DefaultAppInsightsSessionChannel(
      string instrumentationKey,
      string userId,
      IStorageBuilder storageBuilder,
      IProcessLockFactory processLockFactory)
      : base(instrumentationKey, userId)
    {
      this.storageBuilder = storageBuilder;
      this.processLockFactory = processLockFactory;
    }

    protected override IAppInsightsClientWrapper CreateAppInsightsClientWrapper() => (IAppInsightsClientWrapper) new DefaultAppInsightsClientWrapper(this.InstrumentationKey, this.storageBuilder.Create(this.PersistenceFolderName), this.processLockFactory);

    internal override void AppendCommonSchemaVersion(EventTelemetry eventTelemetry) => eventTelemetry.CommonSchemaVersion = 2;
  }
}
