// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.ESContext
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class ESContext : ProviderContext
  {
    public List<Tuple<string, string>> SearchText { get; set; }

    public Dictionary<string, List<string>> SearchFilters { get; set; }

    public DocumentContractType ContractType { get; set; }

    public List<string> Indices { get; set; }

    public IEntityType EntityType { get; set; }

    public string QueryConnectionString { get; set; }

    public string SearchPlatformSettings { get; set; }
  }
}
