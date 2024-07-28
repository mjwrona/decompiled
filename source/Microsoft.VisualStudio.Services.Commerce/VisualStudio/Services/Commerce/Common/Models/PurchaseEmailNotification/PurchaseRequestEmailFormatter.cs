// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification.PurchaseRequestEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.PurchaseEmailNotification
{
  internal abstract class PurchaseRequestEmailFormatter : PurchaseEmailBaseFormatter
  {
    internal PurchaseRequestEmailFormatter(PurchaseRequest purchaseRequest) => this.PurchaseRequest = purchaseRequest;

    protected string AcceptRequestLink
    {
      set => this.Attributes[nameof (AcceptRequestLink)] = value;
    }

    protected string RequestorEmail
    {
      set => this.Attributes[nameof (RequestorEmail)] = value;
    }

    protected string RequestMessage
    {
      set => this.Attributes[nameof (RequestMessage)] = value;
    }

    public abstract override void SetEmailBody(ResourceRenewalGroup renewalGroup);

    public abstract override void SetEmailAttributes(
      IVssRequestContext collectionContext,
      string offerMeterName,
      string galleryId,
      Guid azureSubscriptionId,
      ResourceRenewalGroup renewalGroup,
      int quantity,
      string offerCode,
      DateTime renewalDate,
      Guid? tenantId,
      Guid? objectId);

    public override void SetEmailSubject() => this.Subject = HostingResources.PurchaseRequestEmailSubject();

    protected PurchaseRequest PurchaseRequest { get; }
  }
}
