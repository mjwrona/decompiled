// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.ESDeploymentContext
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CDF1FDEE-D833-4D57-965E-D6F8F75FE22F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManagerAPI;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.HealthManager.Data
{
  public class ESDeploymentContext : ProviderContext
  {
    private IEntityType m_entityType;

    public string SearchPlatformConnectionString { get; set; }

    public string SearchPlatformSettings { get; set; }

    public IEntityType EntityType
    {
      get => this.m_entityType == null ? (IEntityType) AllEntityType.GetInstance() : this.m_entityType;
      set => this.m_entityType = value;
    }

    public IEnumerable<string> Indices { get; set; }
  }
}
