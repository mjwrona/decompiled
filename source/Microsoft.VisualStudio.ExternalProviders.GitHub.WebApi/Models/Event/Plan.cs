// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.Plan
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event
{
  [DataContract]
  public class Plan
  {
    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public int Monthly_price_in_cents { get; set; }

    [DataMember]
    public int Yearly_price_in_cents { get; set; }

    [DataMember]
    public string Price_model { get; set; }

    [DataMember]
    public bool Has_free_trial { get; set; }

    [DataMember]
    public string Unit_name { get; set; }

    [DataMember]
    public string[] Bullet { get; set; }
  }
}
