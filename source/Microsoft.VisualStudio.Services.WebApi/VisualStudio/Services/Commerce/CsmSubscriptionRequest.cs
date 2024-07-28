// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.CsmSubscriptionRequest
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class CsmSubscriptionRequest
  {
    private const string QuotaIdKey = "quotaId";
    private static readonly string[] FreeTierQuotaIdentifiers = new string[1]
    {
      "DreamSpark_2015-02-01"
    };

    public CsmSubscriptionState State { get; set; }

    public DateTime RegistrationDate { get; set; }

    public Guid SubscriptionId { get; set; }

    public Dictionary<string, object> Properties { get; set; }

    public string QuotaId { get; private set; }

    public SubscriptionSource Source { get; set; }

    public bool AdjustData()
    {
      object obj;
      if (this.Properties != null && this.Properties.TryGetValue("quotaId", out obj))
        this.QuotaId = obj.ToString().Trim() ?? string.Empty;
      if (!string.IsNullOrEmpty(this.QuotaId) && ((IEnumerable<string>) CsmSubscriptionRequest.FreeTierQuotaIdentifiers).Contains<string>(this.QuotaId, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) && this.State == CsmSubscriptionState.Registered)
        this.Source = SubscriptionSource.FreeTier;
      return true;
    }

    public bool IsValid() => this.State != CsmSubscriptionState.Unknown;

    public override string ToString() => string.Format("Subscription State:{0}; Registration Date:{1}: QuotaId: {2}", (object) this.State, (object) this.RegistrationDate, (object) this.QuotaId);
  }
}
