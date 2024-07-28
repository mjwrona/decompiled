// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event.Marketplace_purchase
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Event
{
  [DataContract]
  public class Marketplace_purchase
  {
    [DataMember]
    public Account Account { get; set; }

    [DataMember]
    public string Billing_cycle { get; set; }

    [DataMember]
    public int Unit_count { get; set; }

    [DataMember]
    public bool On_free_trial { get; set; }

    [DataMember]
    public string Free_trial_ends_on { get; set; }

    [DataMember]
    public string Next_billing_date { get; set; }

    [DataMember]
    public Plan Plan { get; set; }
  }
}
