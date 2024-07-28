// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.AccountSettingsModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class AccountSettingsModel
  {
    public AccountSettingsModel(
      string timeZoneId,
      Guid accountOwnerId,
      string accountName,
      string accountUrl,
      TeamFoundationIdentity ownerIdentity,
      string region,
      string privacyUrl,
      Guid? subscriptionId,
      bool? devopsDomainUrls,
      string targetAccountUrl,
      IDictionary<string, PolicyModel> policies)
    {
      this.TimeZoneId = timeZoneId;
      this.AccountOwnerId = accountOwnerId;
      this.OwnerIdentity = ownerIdentity;
      this.AccountUrl = accountUrl;
      this.AccountName = accountName;
      this.AccountRegion = region;
      this.PrivacyUrl = privacyUrl;
      this.SubscriptionId = subscriptionId;
      this.DevOpsDomainUrls = devopsDomainUrls;
      this.TargetAccountUrl = targetAccountUrl;
      this.Policies = policies;
    }

    public string TimeZoneId { get; set; }

    public Guid AccountOwnerId { get; set; }

    public string AccountUrl { get; set; }

    public string AccountName { get; set; }

    public string AccountRegion { get; set; }

    public string PrivacyUrl { get; set; }

    public TeamFoundationIdentity OwnerIdentity { get; set; }

    public Guid? SubscriptionId { get; set; }

    public bool? DevOpsDomainUrls { get; set; }

    public string TargetAccountUrl { get; set; }

    public IDictionary<string, PolicyModel> Policies { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["timeZoneId"] = (object) this.TimeZoneId;
      List<TimeZoneInfoModel> timeZoneInfoModelList = new List<TimeZoneInfoModel>()
      {
        (TimeZoneInfoModel) new DefaultTimeZoneInfoModel()
      };
      timeZoneInfoModelList.AddRange((IEnumerable<TimeZoneInfoModel>) CommonUtility.AllTimeZones);
      json["allTimeZones"] = (object) timeZoneInfoModelList;
      json["accountOwnerId"] = (object) this.AccountOwnerId;
      json["ownerIdentity"] = (object) new UserIdentityViewModel(this.OwnerIdentity).ToJson();
      json["accountUrl"] = (object) this.AccountUrl;
      json["accountName"] = (object) this.AccountName;
      json["accountRegion"] = (object) this.AccountRegion;
      json["privacyUrl"] = (object) this.PrivacyUrl;
      json["subscriptionId"] = (object) this.SubscriptionId;
      json["codexDomainUrls"] = (object) this.DevOpsDomainUrls;
      json["targetAccountUrl"] = (object) this.TargetAccountUrl;
      json["policies"] = (object) this.Policies.ToDictionary<KeyValuePair<string, PolicyModel>, string, JsObject>((Func<KeyValuePair<string, PolicyModel>, string>) (x => x.Key), (Func<KeyValuePair<string, PolicyModel>, JsObject>) (x => x.Value.ToJson()));
      return json;
    }
  }
}
