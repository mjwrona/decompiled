// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.PurchaseQuantityDetails
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Web
{
  public class PurchaseQuantityDetails
  {
    [DataMember(Name = "currentQuantity")]
    public long CurrentQuantity { get; set; }

    [DataMember(Name = "includedQuantity")]
    public long IncludedQuantity { get; set; }

    [DataMember(Name = "maximumQuantity")]
    public long MaximumQuantity { get; set; }

    [DataMember(Name = "nextMonthQuantity")]
    public long NextMonthQuantity { get; set; }

    [DataMember(Name = "renewalDate")]
    public DateTime? RenewalDate { get; set; }

    [DataMember(Name = "trialEndDate")]
    public DateTime? TrialEndDate { get; set; }
  }
}
