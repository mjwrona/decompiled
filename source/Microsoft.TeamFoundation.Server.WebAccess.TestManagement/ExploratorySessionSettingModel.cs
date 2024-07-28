// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.ExploratorySessionSettingModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class ExploratorySessionSettingModel
  {
    [DataMember(Name = "detailPaneState")]
    public bool DetailPaneState { get; set; }

    [DataMember(Name = "teamFilter")]
    public string TeamFilter { get; set; }

    [DataMember(Name = "ownerFilter")]
    public string OwnerFilter { get; set; }

    [DataMember(Name = "periodFilter")]
    public string PeriodFilter { get; set; }

    [DataMember(Name = "queryFilterName")]
    public string QueryFilterName { get; set; }

    [DataMember(Name = "queryFilterValue")]
    public string QueryFilterValue { get; set; }

    [DataMember(Name = "groupBySetting")]
    public string GroupBySetting { get; set; }

    [DataMember(Name = "filterBySetting")]
    public string FilterBySetting { get; set; }
  }
}
