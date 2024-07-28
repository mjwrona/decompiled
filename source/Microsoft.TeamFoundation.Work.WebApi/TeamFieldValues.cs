// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.TeamFieldValues
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class TeamFieldValues : TeamSettingsDataContractBase
  {
    [DataMember(Order = 10)]
    public FieldReference Field { get; set; }

    [DataMember(Order = 20)]
    public string DefaultValue { get; set; }

    [DataMember(Order = 30)]
    public IEnumerable<TeamFieldValue> Values { get; set; }
  }
}
