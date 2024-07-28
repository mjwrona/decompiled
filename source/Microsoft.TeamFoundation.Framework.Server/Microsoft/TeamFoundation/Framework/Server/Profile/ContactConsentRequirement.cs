// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Profile.ContactConsentRequirement
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Profile
{
  public class ContactConsentRequirement
  {
    private static readonly string[] DefaultOptOutRegions = new string[43]
    {
      "HK",
      "MX",
      "CL",
      "GT",
      "PA",
      "ID",
      "TH",
      "CY",
      "CZ",
      "FI",
      "FR",
      "IE",
      "LV",
      "LT",
      "MT",
      "NL",
      "PT",
      "RO",
      "SG",
      "ES",
      "SE",
      "HR",
      "IS",
      "KZ",
      "LI",
      "MK",
      "MC",
      "CH",
      "DZ",
      "CD",
      "EG",
      "ET",
      "GW",
      "IR",
      "IQ",
      "JO",
      "LB",
      "LR",
      "NA",
      "NG",
      "SN",
      "ZA",
      "SZ"
    };
    private static readonly string[] DefaultNoticeRegions = new string[46]
    {
      "US",
      "BO",
      "BR",
      "DO",
      "EC",
      "SV",
      "HN",
      "JM",
      "PY",
      "TT",
      "MO",
      "BH",
      "BW",
      "BI",
      "CM",
      "CF",
      "TD",
      "CI",
      "GQ",
      "ER",
      "GA",
      "GM",
      "GN",
      "IN",
      "KE",
      "LS",
      "LY",
      "MW",
      "ML",
      "MR",
      "NE",
      "OM",
      "QA",
      "CG",
      "RW",
      "SA",
      "SC",
      "SD",
      "SY",
      "TZ",
      "TG",
      "TM",
      "UG",
      "YE",
      "ZM",
      "ZW"
    };
    private static readonly string OptOutRegionsRegistryPath = "/Configuration/MarketingConsentRequirements/OptOutCountries";
    private static readonly string NoticeRegionsRegistryPath = "/Configuration/MarketingConsentRequirements/NoticeCountries";

    public string[] GetOptOutCountries(IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) ContactConsentRequirement.OptOutRegionsRegistryPath, false, (string) null);
      if (string.IsNullOrEmpty(str))
        return ContactConsentRequirement.DefaultOptOutRegions;
      return str.Replace(" ", "").Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public string[] GetNoticeCountries(IVssRequestContext requestContext)
    {
      string str = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) ContactConsentRequirement.NoticeRegionsRegistryPath, false, (string) null);
      if (string.IsNullOrEmpty(str))
        return ContactConsentRequirement.DefaultNoticeRegions;
      return str.Replace(" ", "").Split(new char[1]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
    }

    public ContactConsent GetContactConsent(string countryCode, IVssRequestContext requestContext)
    {
      if (((IEnumerable<string>) this.GetOptOutCountries(requestContext)).Contains<string>(countryCode))
        return ContactConsent.OptOut;
      return ((IEnumerable<string>) this.GetNoticeCountries(requestContext)).Contains<string>(countryCode) ? ContactConsent.Notice : ContactConsent.OptIn;
    }
  }
}
