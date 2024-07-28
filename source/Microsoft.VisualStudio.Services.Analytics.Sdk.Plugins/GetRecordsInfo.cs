// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.GetRecordsInfo
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public struct GetRecordsInfo
  {
    public GetRecordsInfo(
      int shardId,
      string shardLocator,
      string watermark,
      int batchSize,
      bool keysOnly,
      int stageVersion)
    {
      this.ShardId = shardId;
      this.ShardLocator = shardLocator;
      this.Watermark = watermark;
      this.BatchSize = batchSize;
      this.KeysOnly = keysOnly;
      this.StageVersion = stageVersion;
    }

    public int ShardId { get; }

    public string ShardLocator { get; }

    public string Watermark { get; }

    public int BatchSize { get; }

    public bool KeysOnly { get; }

    public int StageVersion { get; }
  }
}
