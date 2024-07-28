// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.MsdnEntitlement
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Licensing
{
  public class MsdnEntitlement : ICloneable
  {
    public string EntitlementCode { get; set; }

    public string EntitlementName { get; set; }

    public string EntitlementType { get; set; }

    public bool IsEntitlementAvailable { get; set; }

    public bool? IsActivated { get; set; }

    public DateTimeOffset SubscriptionExpirationDate { get; set; }

    public string SubscriptionId { get; set; }

    public string SubscriptionLevelCode { get; set; }

    public string SubscriptionLevelName { get; set; }

    public string SubscriptionStatus { get; set; }

    public string SubscriptionChannel { get; set; }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(string.Format("[EntitlementCode: {0}; ", (object) this.EntitlementCode));
      stringBuilder.Append(string.Format("EntitlementType: {0}; ", (object) this.EntitlementType));
      stringBuilder.Append(string.Format("EntitlementName: {0}; ", (object) this.EntitlementName));
      stringBuilder.Append(string.Format("IsEntitlementAvailable: {0}; ", (object) this.IsEntitlementAvailable));
      stringBuilder.Append(string.Format("SubscriptionExpirationDate: {0}; ", (object) this.SubscriptionExpirationDate.ToString()));
      stringBuilder.Append(string.Format("SubscriptionId: {0}; ", (object) this.SubscriptionId));
      stringBuilder.Append(string.Format("SubscriptionLevelCode: {0}; ", (object) this.SubscriptionLevelCode));
      stringBuilder.Append(string.Format("SubscriptionLevelName: {0}; ", (object) this.SubscriptionLevelName));
      stringBuilder.Append(string.Format("SubscriptionStatus: {0}; ", (object) this.SubscriptionStatus));
      stringBuilder.Append(string.Format("SubscriptionChannel: {0}] ", (object) this.SubscriptionChannel));
      return stringBuilder.ToString();
    }

    public MsdnEntitlement Clone() => (MsdnEntitlement) this.MemberwiseClone();

    object ICloneable.Clone() => this.MemberwiseClone();
  }
}
