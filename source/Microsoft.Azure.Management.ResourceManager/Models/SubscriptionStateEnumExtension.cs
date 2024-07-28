// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.SubscriptionStateEnumExtension
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  internal static class SubscriptionStateEnumExtension
  {
    internal static string ToSerializedValue(this SubscriptionState? value) => !value.HasValue ? (string) null : value.Value.ToSerializedValue();

    internal static string ToSerializedValue(this SubscriptionState value)
    {
      switch (value)
      {
        case SubscriptionState.Enabled:
          return "Enabled";
        case SubscriptionState.Warned:
          return "Warned";
        case SubscriptionState.PastDue:
          return "PastDue";
        case SubscriptionState.Disabled:
          return "Disabled";
        case SubscriptionState.Deleted:
          return "Deleted";
        default:
          return (string) null;
      }
    }

    internal static SubscriptionState? ParseSubscriptionState(this string value)
    {
      switch (value)
      {
        case "Enabled":
          return new SubscriptionState?(SubscriptionState.Enabled);
        case "Warned":
          return new SubscriptionState?(SubscriptionState.Warned);
        case "PastDue":
          return new SubscriptionState?(SubscriptionState.PastDue);
        case "Disabled":
          return new SubscriptionState?(SubscriptionState.Disabled);
        case "Deleted":
          return new SubscriptionState?(SubscriptionState.Deleted);
        default:
          return new SubscriptionState?();
      }
    }
  }
}
