// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.Models.ActionTypeEnumExtension
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

namespace Microsoft.Azure.KeyVault.Models
{
  internal static class ActionTypeEnumExtension
  {
    internal static string ToSerializedValue(this ActionType? value) => value.HasValue ? value.Value.ToSerializedValue() : (string) null;

    internal static string ToSerializedValue(this ActionType value)
    {
      if (value == ActionType.EmailContacts)
        return "EmailContacts";
      return value == ActionType.AutoRenew ? "AutoRenew" : (string) null;
    }

    internal static ActionType? ParseActionType(this string value)
    {
      switch (value)
      {
        case "EmailContacts":
          return new ActionType?(ActionType.EmailContacts);
        case "AutoRenew":
          return new ActionType?(ActionType.AutoRenew);
        default:
          return new ActionType?();
      }
    }
  }
}
