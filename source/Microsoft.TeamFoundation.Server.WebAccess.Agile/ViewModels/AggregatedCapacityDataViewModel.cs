// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.AggregatedCapacityDataViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Server.WebAccess.Agile.Models;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class AggregatedCapacityDataViewModel
  {
    [DataMember(Name = "remainingWorkField", IsRequired = true)]
    public string RemainingWorkField { get; set; }

    [DataMember(Name = "aggregatedCapacity", IsRequired = true)]
    public AggregateFieldValues AggregatedCapacity { get; set; }

    [DataMember(Name = "previousValueData", EmitDefaultValue = false)]
    public PreviousFieldValues PreviousValueData { get; set; }

    [DataMember(Name = "aggregatedCapacityLimitExceeded", IsRequired = true)]
    public bool AggregatedCapacityLimitExceeded { get; set; }
  }
}
