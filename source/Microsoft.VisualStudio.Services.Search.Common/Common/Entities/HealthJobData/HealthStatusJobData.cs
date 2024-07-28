// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData.HealthStatusJobData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.HealthJobData
{
  [DataContract]
  public class HealthStatusJobData : IExtensibleDataObject
  {
    public ExtensionDataObject ExtensionData { get; set; }

    [DataMember(Order = 0)]
    public List<Scenario> Scenarios { get; set; }

    [DataMember(Order = 1)]
    public List<Tuple<string, string>> SearchText { get; set; }

    [DataMember(Order = 2)]
    public Dictionary<string, List<string>> SearchFilters { get; set; }

    private IEntityType m_entityType { get; set; }

    [DataMember(Order = 3)]
    public IEntityType EntityType
    {
      get => this.m_entityType;
      set => this.m_entityType = value;
    }

    [DataMember(Order = 4)]
    public List<string> Indices { get; set; }
  }
}
