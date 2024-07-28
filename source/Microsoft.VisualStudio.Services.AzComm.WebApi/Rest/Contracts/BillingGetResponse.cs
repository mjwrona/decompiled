// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.AzComm.Rest.Contracts.BillingGetResponse
// Assembly: Microsoft.VisualStudio.Services.AzComm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B1B69FBB-72A0-4C7F-B8FC-E0B0311A8184
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.AzComm.WebApi.dll

using Microsoft.VisualStudio.Services.AzComm.Migration.Rest.Contracts;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.AzComm.Rest.Contracts
{
  [DataContract]
  public class BillingGetResponse
  {
    [DataMember(EmitDefaultValue = false)]
    public Guid SubscriptionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime UpdatedDateTime { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid UpdatedBy { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public BillingOperations AllowedBillingOperations { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsEnterpriseBillingEnabled { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public bool IsAssignmentBillingEnabled { get; set; }

    [IgnoreDataMember]
    public bool IsSuspendedBannerBillingEnabled { get; set; }

    [DataMember(EmitDefaultValue = true)]
    public SubscriptionStatus SubscriptionStatus { get; set; }
  }
}
