// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.ShardsSizeDataProvider
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F07E19E9-6199-4A9C-8D41-E26991BA8812
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.DataProviders
{
  public class ShardsSizeDataProvider : AbstractSearchDataProvider, IDataProvider
  {
    public ShardsSizeDataProvider()
      : base(Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.SearchPlatformFactory.GetInstance())
    {
    }

    [Info("InternalForTestPurpose")]
    internal ShardsSizeDataProvider(ISearchPlatformFactory searchPlatformFactory)
      : base(searchPlatformFactory)
    {
    }

    public List<HealthData> GetData(ProviderContext providerContext)
    {
      this.Initialize(providerContext);
      List<string> filteredIndices = this.GetFilteredIndices(providerContext);
      if (filteredIndices.Count == 0)
        return new List<HealthData>();
      return new List<HealthData>()
      {
        (HealthData) new ShardsSizeData(filteredIndices.Select<string, KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>>((Func<string, KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>>) (it => new KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>(it, this.SearchClusterStateService.GetIndexShardsSizeInBytes(this.ExecutionContext, it)))).ToDictionary<KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>, string, IEnumerable<KeyValuePair<string, double>>>((Func<KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>, string>) (it => it.Key), (Func<KeyValuePair<string, IEnumerable<KeyValuePair<string, double>>>, IEnumerable<KeyValuePair<string, double>>>) (it => it.Value)), DataType.ShardSizeData)
      };
    }
  }
}
