// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements.Ev4Entitlement
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud.SubscriptionServiceEntitlements
{
  [DataContract(Name = "Entitlement")]
  [ExcludeFromCodeCoverage]
  public class Ev4Entitlement
  {
    [DataMember(EmitDefaultValue = false, IsRequired = false)]
    public Guid BenefitDetailGuid { get; set; }

    [DataMember]
    public string SubscriptionLevelCode { get; set; }

    [DataMember]
    public string SubscriptionLevelName { get; set; }

    [DataMember]
    public string SubscriptionStatus { get; set; }

    [DataMember]
    public DateTime SubscriptionExpirationDate { get; set; }

    [DataMember]
    public DateTime? SubscriptionEndedDate { get; set; }

    [DataMember]
    public string SubscriptionProgramCode { get; set; }

    [DataMember]
    public bool IsSubscriptionVolumeLicense { get; set; }

    [DataMember]
    public string EntitlementCode { get; set; }

    [DataMember]
    public string EntitlementName { get; set; }

    [DataMember]
    public string EntitlementType { get; set; }

    [DataMember]
    public string SubscriptionChannel { get; set; }

    [DataMember]
    public bool IsEntitlementAvailable { get; set; }
  }
}
