// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.CapacityOptionsViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class CapacityOptionsViewModel
  {
    [DataMember(Name = "accountCurrentDate", EmitDefaultValue = true)]
    public DateTime AccountCurrentDate { get; set; }

    [DataMember(Name = "activityFieldName", EmitDefaultValue = true)]
    public string ActivityFieldName { get; set; }

    [DataMember(Name = "allowedActivities", EmitDefaultValue = true)]
    public IEnumerable<string> AllowedActivityValues { get; set; }

    [DataMember(Name = "assignedToFieldDisplayName", EmitDefaultValue = true)]
    public string AssignedToDisplayName { get; set; }

    [DataMember(Name = "activityFieldDisplayName", EmitDefaultValue = true)]
    public string ActivityFieldDisplayName { get; set; }

    [DataMember(Name = "suffixFormat", EmitDefaultValue = true)]
    public string SuffixFormat { get; set; }

    [DataMember(Name = "childWorkItemTypes", EmitDefaultValue = true)]
    public IEnumerable<string> ChildWorkItemTypes { get; set; }

    [DataMember(Name = "inline", EmitDefaultValue = true)]
    public bool Inline { get; set; }

    [DataMember(Name = "iterationId", EmitDefaultValue = true)]
    public Guid IterationId { get; set; }

    [DataMember(Name = "isEmpty", EmitDefaultValue = true)]
    public bool IsEmpty { get; set; }

    [DataMember(Name = "isFirstIteration", EmitDefaultValue = true)]
    public bool IsFirstIteration { get; set; }
  }
}
