// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ICommerceEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public interface ICommerceEvent
  {
    string EventId { get; set; }

    DateTime EventTime { get; set; }

    string EventName { get; set; }

    Guid OrganizationId { get; set; }

    string OrganizationName { get; set; }

    Guid CollectionId { get; set; }

    string CollectionName { get; set; }

    Guid SubscriptionId { get; set; }

    string MeterName { get; set; }

    string GalleryId { get; set; }

    int CommittedQuantity { get; set; }

    int CurrentQuantity { get; set; }

    int PreviousQuantity { get; set; }

    double BilledQuantity { get; set; }

    int IncludedQuantity { get; set; }

    int? PreviousIncludedQuantity { get; set; }

    int MaxQuantity { get; set; }

    int? PreviousMaxQuantity { get; set; }

    string RenewalGroup { get; set; }

    string EventSource { get; set; }

    string Environment { get; set; }

    Guid UserIdentity { get; set; }

    Guid ServiceIdentity { get; set; }

    DateTime? TrialStartDate { get; set; }

    DateTime? TrialEndDate { get; set; }

    DateTime? EffectiveDate { get; set; }

    string Version { get; set; }
  }
}
