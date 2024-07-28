// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.AssetEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class AssetEvent : TelemetryEvent
  {
    private const string AssetEventPropertyPrefixName = "DataModel.Asset.";
    private const string AssetIdPropertyName = "DataModel.Asset.AssetId";
    private const string AssetEventVersionPropertyName = "DataModel.Asset.SchemaVersion";

    public AssetEvent(string eventName, string assetId, int assetEventVersion)
      : this(eventName, assetId, assetEventVersion, new TelemetryEventCorrelation(Guid.NewGuid(), DataModelEventType.Asset))
    {
    }

    public AssetEvent(
      string eventName,
      string assetId,
      int assetEventVersion,
      TelemetryEventCorrelation correlation)
      : base(eventName, TelemetrySeverity.Normal, correlation)
    {
      if (correlation.EventType != DataModelEventType.Asset)
        throw new ArgumentException("Property EventType should be AssetEvent.", nameof (correlation));
      DataModelEventNameHelper.SetProductFeatureEntityName(this);
      this.AssetId = assetId;
      this.ReservedProperties.AddPrefixed("Reserved.DataModel.Asset.AssetId", (object) assetId);
      this.AssetEventVersion = assetEventVersion;
      this.ReservedProperties.AddPrefixed("Reserved.DataModel.Asset.SchemaVersion", (object) assetEventVersion);
    }

    public string AssetId { get; }

    public int AssetEventVersion { get; }
  }
}
