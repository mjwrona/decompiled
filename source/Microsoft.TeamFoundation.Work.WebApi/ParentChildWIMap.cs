// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.ParentChildWIMap
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class ParentChildWIMap
  {
    public ParentChildWIMap() => this.ChildWorkItemIds = (IList<int>) new List<int>();

    [DataMember(Order = 1, Name = "id", IsRequired = false, EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(Order = 2, Name = "title", IsRequired = false, EmitDefaultValue = false)]
    public string Title { get; set; }

    [DataMember(Order = 3, Name = "childWorkItemIds", IsRequired = false, EmitDefaultValue = false)]
    public IList<int> ChildWorkItemIds { get; set; }

    [DataMember(Order = 4, Name = "workItemTypeName", IsRequired = false, EmitDefaultValue = false)]
    public string WorkItemTypeName { get; set; }
  }
}
