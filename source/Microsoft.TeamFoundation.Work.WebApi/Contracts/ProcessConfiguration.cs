// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Contracts.ProcessConfiguration
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi.Contracts
{
  [DataContract]
  public class ProcessConfiguration
  {
    [DataMember]
    public IDictionary<string, WorkItemFieldReference> TypeFields { get; set; }

    [DataMember]
    public CategoryConfiguration TaskBacklog { get; set; }

    [DataMember]
    public CategoryConfiguration RequirementBacklog { get; set; }

    [DataMember]
    public CategoryConfiguration[] PortfolioBacklogs { get; set; }

    [DataMember]
    public CategoryConfiguration BugWorkItems { get; set; }

    [DataMember]
    public string Url { get; set; }
  }
}
