// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.Trigger
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using Microsoft.Rest;
using Newtonsoft.Json;

namespace Microsoft.Azure.KeyVault.Models
{
  public class Trigger
  {
    public Trigger()
    {
    }

    public Trigger(int? lifetimePercentage = null, int? daysBeforeExpiry = null)
    {
      this.LifetimePercentage = lifetimePercentage;
      this.DaysBeforeExpiry = daysBeforeExpiry;
    }

    [JsonProperty(PropertyName = "lifetime_percentage")]
    public int? LifetimePercentage { get; set; }

    [JsonProperty(PropertyName = "days_before_expiry")]
    public int? DaysBeforeExpiry { get; set; }

    public virtual void Validate()
    {
      int? lifetimePercentage1 = this.LifetimePercentage;
      int num1 = 99;
      if (lifetimePercentage1.GetValueOrDefault() > num1 & lifetimePercentage1.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMaximum, "LifetimePercentage", (object) 99);
      int? lifetimePercentage2 = this.LifetimePercentage;
      int num2 = 1;
      if (lifetimePercentage2.GetValueOrDefault() < num2 & lifetimePercentage2.HasValue)
        throw new ValidationException(ValidationRules.InclusiveMinimum, "LifetimePercentage", (object) 1);
    }
  }
}
