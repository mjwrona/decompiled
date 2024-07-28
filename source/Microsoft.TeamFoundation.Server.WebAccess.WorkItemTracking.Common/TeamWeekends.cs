// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.TeamWeekends
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [JsonConverter(typeof (TeamWeekendsJsonConverter))]
  [DataContract]
  public class TeamWeekends : ITeamWeekends
  {
    [DataMember(Name = "days", EmitDefaultValue = false)]
    public DayOfWeek[] Days { get; set; }

    [DataMember(Name = "canEditWeekends", EmitDefaultValue = false)]
    public bool CanEditWeekends { get; set; }

    public void UpdateDays(int[] weekends)
    {
      List<DayOfWeek> dayOfWeekList = new List<DayOfWeek>();
      foreach (int weekend in weekends)
      {
        switch (weekend)
        {
          case 0:
          case 1:
          case 2:
          case 3:
          case 4:
          case 5:
          case 6:
            dayOfWeekList.Add((DayOfWeek) weekend);
            break;
        }
      }
      this.Days = dayOfWeekList.ToArray();
    }
  }
}
