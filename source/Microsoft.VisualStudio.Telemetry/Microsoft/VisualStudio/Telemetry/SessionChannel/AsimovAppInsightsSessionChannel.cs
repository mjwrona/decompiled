// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.SessionChannel.AsimovAppInsightsSessionChannel
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.ApplicationInsights.DataContracts;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.IO;

namespace Microsoft.VisualStudio.Telemetry.SessionChannel
{
  internal sealed class AsimovAppInsightsSessionChannel : BaseAppInsightsSessionChannel
  {
    private const string DefaultAsimovIKey = "AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70";
    private const string DefaultAppInsightsIKey = "f144292e-e3b2-4011-ac90-20e5c03fbce5";
    private const string AppInsightsPersistencePath = "Microsoft\\VSApplicationInsights";
    private readonly string channelId;
    private readonly bool isUtcEnabled;
    private readonly TelemetrySession hostTelemetrySession;
    private readonly IStorageBuilder storageBuilder;
    private readonly IProcessLockFactory processLockFactory;

    protected override string FolderNameSuffix => this.InstrumentationKey;

    internal override string IKey => this.InstrumentationKey;

    public override string ChannelId => this.channelId;

    public AsimovAppInsightsSessionChannel(
      string channelId,
      bool isUtcEnabled,
      string instrumentationKey,
      string userId,
      ChannelProperties channelProperties,
      TelemetrySession hostTelemetrySession,
      IStorageBuilder storageBuilder,
      IProcessLockFactory processLockFactory)
      : base(instrumentationKey, userId, defaultChannelProperties: channelProperties)
    {
      channelId.RequiresArgumentNotNullAndNotEmpty(nameof (channelId));
      this.channelId = channelId;
      this.isUtcEnabled = isUtcEnabled;
      this.hostTelemetrySession = hostTelemetrySession;
      this.storageBuilder = storageBuilder;
      this.processLockFactory = processLockFactory;
    }

    internal void CheckPendingEventsAndStartChannel(string sessionId)
    {
      string[] strArray = new string[3]
      {
        "LOCALAPPDATA",
        "TEMP",
        "ProgramData"
      };
      int index = 0;
      while (index < strArray.Length && !this.TryUploadPendingFiles(strArray[index], sessionId))
        ++index;
    }

    protected override IAppInsightsClientWrapper CreateAppInsightsClientWrapper() => (IAppInsightsClientWrapper) new AsimovAppInsightsClientWrapper(this.isUtcEnabled, this.InstrumentationKey, this.hostTelemetrySession, this.storageBuilder.Create(this.PersistenceFolderName), this.processLockFactory);

    private bool TryUploadPendingFiles(string environmentFolderName, string sessionId)
    {
      try
      {
        string environmentVariable = Environment.GetEnvironmentVariable(environmentFolderName);
        if (!string.IsNullOrEmpty(environmentVariable))
        {
          if (Directory.GetFiles(System.IO.Path.Combine(System.IO.Path.Combine(environmentVariable, "Microsoft\\VSApplicationInsights"), this.PersistenceFolderName), "*.trn", SearchOption.TopDirectoryOnly).Length != 0)
            this.Start(sessionId);
          return true;
        }
      }
      catch (Exception ex)
      {
      }
      return false;
    }

    internal override void AppendCommonSchemaVersion(EventTelemetry eventTelemetry) => eventTelemetry.CommonSchemaVersion = 2;
  }
}
