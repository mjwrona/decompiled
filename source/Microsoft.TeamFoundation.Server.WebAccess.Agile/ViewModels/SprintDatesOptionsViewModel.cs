// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels.SprintDatesOptionsViewModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.ViewModels
{
  [DataContract]
  public class SprintDatesOptionsViewModel
  {
    [DataMember(Name = "teamId")]
    public string TeamId { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "iterationPath")]
    public string IterationPath { get; set; }

    [DataMember(Name = "iterationId")]
    public string IterationId { get; set; }

    [DataMember(Name = "accountCurrentDate")]
    public DateTime AccountCurrentDate { get; set; }

    [DataMember(Name = "isCurrentDateInIteration")]
    public bool IsCurrentDateInIteration { get; set; }

    [DataMember(Name = "startDate")]
    public DateTime? StartDate { get; set; }

    [DataMember(Name = "finishDate")]
    public DateTime? FinishDate { get; set; }
  }
}
