// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts.SubscriptionGetResponse
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using Microsoft.VisualStudio.Services.Commerce;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.AzComm.WebApi.Contracts
{
  [DataContract]
  public class SubscriptionGetResponse
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string OfferCode { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts.SubscriptionStatus Status { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime UpdatedDateTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid TenantId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsMultiOrgSubscription { get; set; }

    [IgnoreDataMember]
    [DataMember(EmitDefaultValue = false)]
    public SubscriptionSpendingLimit? SpendingLimit { get; set; }

    [IgnoreDataMember]
    [DataMember(EmitDefaultValue = false)]
    public DateTime? GracePeriodEnd { get; set; }
  }
}
