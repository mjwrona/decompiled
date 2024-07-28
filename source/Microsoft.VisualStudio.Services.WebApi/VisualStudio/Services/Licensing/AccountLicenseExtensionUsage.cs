// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.AccountLicenseExtensionUsage
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Commerce;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  [DataContract]
  public class AccountLicenseExtensionUsage
  {
    [DataMember]
    public string ExtensionName { get; set; }

    [DataMember]
    public string ExtensionId { get; set; }

    [DataMember]
    public int ProvisionedCount { get; set; }

    [DataMember]
    public int IncludedQuantity { get; set; }

    [DataMember]
    public int UsedCount { get; set; }

    [DataMember]
    public int MsdnUsedCount { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public bool IsTrial { get; set; }

    [DataMember(IsRequired = false)]
    public int RemainingTrialDays { get; set; }

    [DataMember]
    public MinimumRequiredServiceLevel MinimumLicenseRequired { get; set; }

    [DataMember(IsRequired = false)]
    public DateTime? TrialExpiryDate { get; set; }
  }
}
