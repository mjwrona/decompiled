// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.ShardsSizeData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class ShardsSizeData : HealthData
  {
    private readonly Dictionary<string, IEnumerable<KeyValuePair<string, double>>> m_indexShardsSizeDictionary;

    public ShardsSizeData(
      Dictionary<string, IEnumerable<KeyValuePair<string, double>>> elasticsearchIndexDetails,
      DataType dataType)
      : base(dataType)
    {
      this.m_indexShardsSizeDictionary = elasticsearchIndexDetails;
    }

    public Dictionary<string, IEnumerable<KeyValuePair<string, double>>> GetIndexShardsSizes() => this.m_indexShardsSizeDictionary;
  }
}
