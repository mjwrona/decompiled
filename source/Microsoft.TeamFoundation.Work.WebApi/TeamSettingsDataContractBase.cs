// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.TeamSettingsDataContractBase
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public abstract class TeamSettingsDataContractBase
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false, Order = 998)]
    public string Url { get; set; }

    [DataMember(EmitDefaultValue = false, Name = "_links", Order = 999)]
    public ReferenceLinks Links { get; set; }
  }
}
