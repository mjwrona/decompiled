// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification.ExtensionTrialStartEmailFormatter
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Commerce.Common.Models.TrialEmailNotification
{
  public class ExtensionTrialStartEmailFormatter : TrialEmailBaseFormatter
  {
    private readonly string OfferMeterName;

    public ExtensionTrialStartEmailFormatter(OfferMeter offerMeter, string offerMeterName)
    {
      this.OfferMeter = offerMeter;
      this.OfferMeterName = offerMeterName;
    }

    public override void SetEmailBody(IVssRequestContext collectionContext) => this.Body = HostingResources.VSOExtensionTrialStartEmailTemplate();

    public override void SetEmailSubject(IVssRequestContext collectionContext) => this.Subject = HostingResources.VSOExtensionTrialStartEmailSubject((object) this.OfferMeter.Name);

    public override void SetEmailAttributes(IVssRequestContext collectionContext)
    {
      this.ExtensionName = this.GetOfferMeter(collectionContext, this.OfferMeterName).Name;
      this.AccountName = CollectionHelper.GetCollectionName(collectionContext);
      this.PopulateEmailLinks(collectionContext, this.OfferMeter);
    }

    private OfferMeter OfferMeter { get; set; }
  }
}
