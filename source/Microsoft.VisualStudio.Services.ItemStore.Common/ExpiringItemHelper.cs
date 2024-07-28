// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.ExpiringItemHelper
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public static class ExpiringItemHelper
  {
    private const string ExpirationNeverConstant = "never";

    public static string DescribeExpirationTime(IExpiringItem drop)
    {
      DateTime? expirationTime;
      if (!drop.TryGetExpirationTime(out expirationTime))
        return "NoData";
      return !expirationTime.HasValue ? "never" : expirationTime.Value.ToString("o");
    }

    public static string GetExpirationTimeStringToSetTo(DateTime? newDateTime) => !newDateTime.HasValue ? "never" : newDateTime.Value.ToUniversalTime().ToString("o");

    public static bool TryGetExpirationTime(string existingVal, out DateTime? expirationTime)
    {
      bool expirationTime1 = existingVal != null;
      expirationTime = expirationTime1 ? (string.Equals(existingVal, "never", StringComparison.OrdinalIgnoreCase) ? new DateTime?() : new DateTime?(DateTime.Parse(existingVal).ToUniversalTime())) : new DateTime?();
      return expirationTime1;
    }

    public static bool IsRetainedForever(string existingVal)
    {
      DateTime? expirationTime;
      return ExpiringItemHelper.TryGetExpirationTime(existingVal, out expirationTime) && !expirationTime.HasValue;
    }

    public static bool SameExpirationValues(this IExpiringItem a, IExpiringItem b)
    {
      DateTime? expirationTime1;
      DateTime? expirationTime2;
      if (a.TryGetExpirationTime(out expirationTime1) != b.TryGetExpirationTime(out expirationTime2))
        return false;
      DateTime? nullable1 = expirationTime1;
      DateTime? nullable2 = expirationTime2;
      if (nullable1.HasValue != nullable2.HasValue)
        return false;
      return !nullable1.HasValue || nullable1.GetValueOrDefault() == nullable2.GetValueOrDefault();
    }

    public static bool IsFiniteExpiration(this IExpiringItem a, out DateTime? expirationTime) => a.TryGetExpirationTime(out expirationTime) && expirationTime.HasValue;
  }
}
