// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureBillableEventContract
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DataContract]
  public class AzureBillableEventContract
  {
    [DataMember]
    public string SubscriptionId { set; get; }

    [DataMember]
    public Guid EventId { set; get; }

    [DataMember]
    public DateTime EventDateTime { set; get; }

    [DataMember]
    public double Quantity { set; get; }

    [DataMember]
    public string MeterId { set; get; }

    [DataMember]
    public string ResourceUri { set; get; }

    [DataMember(IsRequired = false)]
    public string Tags { set; get; }

    [DataMember]
    public string Location { set; get; }

    [DataMember(IsRequired = false)]
    public string PartNumber { set; get; }

    [DataMember(IsRequired = false)]
    public string OrderNumber { set; get; }

    [DataMember(IsRequired = false)]
    public object AdditionalInfo { set; get; }
  }
}
