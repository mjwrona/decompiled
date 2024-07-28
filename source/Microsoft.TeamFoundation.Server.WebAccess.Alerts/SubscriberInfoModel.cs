// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.SubscriberInfoModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server.Core;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  [DataContract]
  internal class SubscriberInfoModel
  {
    public SubscriberInfoModel()
    {
    }

    public SubscriberInfoModel(TeamFoundationIdentity identity)
    {
      this.TeamFoundationId = identity.TeamFoundationId;
      this.DisplayName = identity.DisplayName;
      this.IsGroup = identity.IsContainer;
      this.Email = SubscriberInfoModel.GetEmailAddress(identity);
    }

    public static string GetEmailAddress(TeamFoundationIdentity identity)
    {
      object obj = (object) null;
      return identity.TryGetProperty(IdentityPropertyScope.Global, "CustomNotificationAddresses", out obj) && obj is string str ? str : identity.GetAttribute("Mail", string.Empty);
    }

    [DataMember(Name = "teamFoundationId")]
    public Guid TeamFoundationId { get; set; }

    [DataMember(Name = "identityType")]
    public string IdentityType { get; set; }

    [DataMember(Name = "displayName")]
    public string DisplayName { get; set; }

    [DataMember(Name = "isGroup")]
    public bool IsGroup { get; set; }

    [DataMember(Name = "email")]
    public string Email { get; set; }
  }
}
