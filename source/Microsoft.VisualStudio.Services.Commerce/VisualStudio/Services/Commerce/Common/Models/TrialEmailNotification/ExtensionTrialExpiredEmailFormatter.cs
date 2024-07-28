// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification.ExtensionTrialExpiredEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification
{
  public class ExtensionTrialExpiredEmailFormatter : TrialEmailBaseFormatter
  {
    private readonly string OfferMeterName;
    private readonly int OfferMeterIncludedQuantity;
    private const string buyTrialExpiredUrlPath = "items?itemName={0}&workflowId=marketplace&accountId={1}&wt.mc_id=TrialEndedEmail&install=true";

    public ExtensionTrialExpiredEmailFormatter(
      OfferMeter offerMeter,
      int includedQuantity,
      string offerMeterName)
    {
      this.OfferMeter = offerMeter;
      this.OfferMeterName = offerMeterName;
      this.OfferMeterIncludedQuantity = includedQuantity;
    }

    public override void SetEmailBody(IVssRequestContext collectionContext) => this.Body = this.OfferMeterIncludedQuantity > 0 ? HostingResources.VSOExtensionTrialExpiredWithIncludedQuantityEmailTemplate() : HostingResources.VSOExtensionTrialExpiredEmailTemplate();

    public override void SetEmailSubject(IVssRequestContext collectionContext) => this.Subject = HostingResources.VSOExtensionTrialExpiredEmailSubject((object) this.OfferMeter.Name);

    public override void SetEmailAttributes(IVssRequestContext collectionContext)
    {
      this.ExtensionName = this.GetOfferMeter(collectionContext, this.OfferMeterName).Name;
      this.AccountName = CollectionHelper.GetCollectionName(collectionContext);
      this.IncludedQuantity = this.OfferMeterIncludedQuantity.ToString();
      this.PopulateEmailLinks(collectionContext, this.OfferMeter, "items?itemName={0}&workflowId=marketplace&accountId={1}&wt.mc_id=TrialEndedEmail&install=true");
    }

    private OfferMeter OfferMeter { get; set; }
  }
}
