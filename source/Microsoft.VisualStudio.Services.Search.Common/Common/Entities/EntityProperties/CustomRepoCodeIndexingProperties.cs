// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.CustomRepoCodeIndexingProperties
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties
{
  [Export(typeof (IndexingProperties))]
  public class CustomRepoCodeIndexingProperties : IndexingProperties
  {
    [DataMember(Order = 0)]
    public string LastIndexedRequestId { get; set; }

    [DataMember(Order = 1)]
    public DateTime LastIndexedChangeTime { get; set; }

    [DataMember(Order = 2)]
    public int RepositorySize { get; set; }

    [DataMember(Order = 3)]
    public Dictionary<string, Dictionary<string, Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties.DepotIndexInfo>> DepotIndexInfo { get; set; }
  }
}
