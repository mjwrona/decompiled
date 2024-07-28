// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification.TrialEmailBaseFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Commerce.Common.Models.EmailFormatters;
using Microsoft.VisualStudio.Services.EmailNotification;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification
{
  public abstract class TrialEmailBaseFormatter : CommerceEmailBaseFormatter
  {
    protected const string Area = "Commerce";
    protected const string Layer = "TrialEmailBaseFormatter";

    public TrialEmailBaseFormatter()
    {
      this.HeaderType = EmailTemplateHeaderType.TrialNotification;
      this.FooterType = EmailTemplateFooterType.TrialNotification;
      this.IsHtml = true;
    }

    public abstract void SetEmailBody(IVssRequestContext collectionContext);

    public abstract void SetEmailSubject(IVssRequestContext collectionContext);

    public abstract void SetEmailAttributes(IVssRequestContext collectionContext);

    protected OfferMeter GetOfferMeter(IVssRequestContext collectionContext, string offerMeterName)
    {
      IVssRequestContext vssRequestContext = collectionContext.To(TeamFoundationHostType.Deployment);
      return (OfferMeter) vssRequestContext.GetService<IOfferMeterService>().GetOfferMeter(vssRequestContext, offerMeterName);
    }

    public static bool IsOfferMeterCategoryExtension(OfferMeter offerMeter) => offerMeter.Category == MeterCategory.Extension;

    protected void PopulateEmailLinks(
      IVssRequestContext collectionContext,
      OfferMeter offerMeter,
      string buyExtensionPath = null)
    {
      this.ExtensionUrl = string.Format("https://marketplace.visualstudio.com/items?itemName={0}", (object) offerMeter.GalleryId);
      if (buyExtensionPath != null)
      {
        IVssRequestContext requestContext = collectionContext.To(TeamFoundationHostType.Deployment);
        string str = string.Format(buyExtensionPath, (object) offerMeter.GalleryId, (object) collectionContext.ServiceHost.InstanceId.ToString());
        this.BuyLink = this.GetLocationServiceUrl(requestContext, CommerceConstants.GalleryGuid, "https://marketplace.visualstudio.com/") + str;
      }
      this.PopulateAccountAndManageExtensionUsersLinks(collectionContext, offerMeter);
    }

    protected string ExtensionName
    {
      set => this.Attributes[nameof (ExtensionName)] = value;
    }

    protected string AccountName
    {
      set => this.Attributes[nameof (AccountName)] = value;
    }

    protected string ExtensionUrl
    {
      set => this.Attributes[nameof (ExtensionUrl)] = value;
    }

    protected string IncludedQuantity
    {
      set => this.Attributes[nameof (IncludedQuantity)] = value;
    }

    protected string BuyLink
    {
      set => this.Attributes[nameof (BuyLink)] = value;
    }

    public override string EmailType() => "VSOTrial";
  }
}
