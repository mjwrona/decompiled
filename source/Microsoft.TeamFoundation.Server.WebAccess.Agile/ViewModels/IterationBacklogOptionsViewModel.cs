// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.IterationBacklogOptionsViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class IterationBacklogOptionsViewModel
  {
    [DataMember(Name = "maxItemsCount", IsRequired = true)]
    public int MaxItemsCount { get; set; }

    [DataMember(Name = "unlimitedItemsCount", IsRequired = true)]
    public int UnlimitedItemsCount { get; set; }

    [DataMember(Name = "childWorkItemTypes", EmitDefaultValue = false)]
    public IEnumerable<string> ChildWorkItemTypes { get; set; }

    public IterationBacklogOptionsViewModel(
      int maxItemsCount,
      IEnumerable<string> childWorkItemTypes = null)
    {
      this.MaxItemsCount = maxItemsCount;
      this.UnlimitedItemsCount = -1;
      this.ChildWorkItemTypes = childWorkItemTypes;
    }
  }
}
