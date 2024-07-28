// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AzureQuotaId
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public static class AzureQuotaId
  {
    private static readonly Dictionary<string, AzureQuotaId.Mapping> QuotaIdMappings = new Dictionary<string, AzureQuotaId.Mapping>()
    {
      {
        "Default_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "PayAsYouGo_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable, AzureOfferType.Standard)
      },
      {
        "AzureInOpen_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable, AzureOfferType.Standard)
      },
      {
        "FreeTrial_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Unsupported)
      },
      {
        "MSDNDevTest_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Msdn)
      },
      {
        "MSDN_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Msdn)
      },
      {
        "EnterpriseAgreement_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPrePaidFundWarningRequired, AzureOfferType.Ea)
      },
      {
        "Internal_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable, AzureOfferType.Standard)
      },
      {
        "BizSpark_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "BizSparkPlus_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "LegacyMonetaryCommitment_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable, AzureOfferType.Standard)
      },
      {
        "MPN_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "AzurePass_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "BackupStorage_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Unsupported)
      },
      {
        "AzureDynamics_2014-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Unsupported)
      },
      {
        "DreamSpark_2015-02-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "CSP_2015-05-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Csp)
      },
      {
        "CSP_MG_2017-12-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Csp)
      },
      {
        "MonetaryCommitment_2015-05-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable, AzureOfferType.Standard)
      },
      {
        "AAD_2015-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Unsupported)
      },
      {
        "DevEssentials_2016-01-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.IsPricingAvailable | AzureSubscriptionAttributes.IsMonetaryCheckRequired, AzureOfferType.Standard)
      },
      {
        "Sponsored_2016-01-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Standard)
      },
      {
        "LightweightTrial_2016-09-01",
        new AzureQuotaId.Mapping(AzureSubscriptionAttributes.None, AzureOfferType.Unsupported)
      }
    };

    public static bool TryMap(string quotaId, out AzureQuotaId.Mapping mapping)
    {
      if (!string.IsNullOrEmpty(quotaId))
        return AzureQuotaId.QuotaIdMappings.TryGetValue(quotaId, out mapping);
      mapping = (AzureQuotaId.Mapping) null;
      return false;
    }

    public static AzureOfferType? ToOfferType(string quotaId)
    {
      AzureQuotaId.Mapping mapping;
      return AzureQuotaId.TryMap(quotaId, out mapping) ? new AzureOfferType?(mapping.OfferType) : new AzureOfferType?();
    }

    public static AzureSubscriptionAttributes? ToAttributes(string quotaId)
    {
      AzureQuotaId.Mapping mapping;
      return AzureQuotaId.TryMap(quotaId, out mapping) ? new AzureSubscriptionAttributes?(mapping.Attributes) : new AzureSubscriptionAttributes?();
    }

    public class Mapping
    {
      public Mapping(AzureSubscriptionAttributes attributes, AzureOfferType offerType)
      {
        this.Attributes = attributes;
        this.OfferType = offerType;
      }

      public AzureSubscriptionAttributes Attributes { get; set; }

      public AzureOfferType OfferType { get; set; }
    }
  }
}
