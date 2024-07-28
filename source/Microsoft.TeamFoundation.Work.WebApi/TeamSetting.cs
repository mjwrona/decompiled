// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.TeamSetting
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class TeamSetting : TeamSettingsDataContractBase
  {
    [DataMember(IsRequired = true, Order = 30)]
    public TeamSettingsIteration BacklogIteration { get; set; }

    [DataMember(IsRequired = true, Order = 40)]
    public BugsBehavior BugsBehavior { get; set; }

    [DataMember(IsRequired = true, Order = 50)]
    public DayOfWeek[] WorkingDays { get; set; }

    [DataMember(IsRequired = true, Order = 60)]
    public IDictionary<string, bool> BacklogVisibilities { get; set; }

    [DataMember(IsRequired = false, Order = 70)]
    public TeamSettingsIteration DefaultIteration { get; set; }

    [DataMember(IsRequired = false, Order = 80)]
    public string DefaultIterationMacro { get; set; }
  }
}
