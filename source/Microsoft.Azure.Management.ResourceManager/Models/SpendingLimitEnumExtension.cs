// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.SpendingLimitEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class SpendingLimitEnumExtension
  {
    internal static string ToSerializedValue(this SpendingLimit? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this SpendingLimit value)
    {
      switch (value)
      {
        case SpendingLimit.On:
          return "On";
        case SpendingLimit.Off:
          return "Off";
        case SpendingLimit.CurrentPeriodOff:
          return "CurrentPeriodOff";
        default:
          return (string) null;
      }
    }

    internal static SpendingLimit? ParseSpendingLimit(this string value)
    {
      switch (value)
      {
        case "On":
          return new SpendingLimit?(SpendingLimit.On);
        case "Off":
          return new SpendingLimit?(SpendingLimit.Off);
        case "CurrentPeriodOff":
          return new SpendingLimit?(SpendingLimit.CurrentPeriodOff);
        default:
          return new SpendingLimit?();
      }
    }
  }
}
