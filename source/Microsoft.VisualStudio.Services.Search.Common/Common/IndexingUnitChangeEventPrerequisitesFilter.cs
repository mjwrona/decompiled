// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IndexingUnitChangeEventPrerequisitesFilter
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  public class IndexingUnitChangeEventPrerequisitesFilter : IExtensibleDataObject
  {
    private ExtensionDataObject m_extensioDataValue;

    public ExtensionDataObject ExtensionData
    {
      get => this.m_extensioDataValue;
      set => this.m_extensioDataValue = value;
    }

    [DataMember(Order = 0)]
    public long Id { get; set; }

    [DataMember(Order = 1)]
    public IndexingUnitChangeEventFilterOperator Operator { get; set; }

    [DataMember(Order = 2)]
    public List<IndexingUnitChangeEventState> PossibleStates { get; set; }
  }
}
