// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionFieldType
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public enum SubscriptionFieldType
  {
    [EnumMember] String = 1,
    [EnumMember] Integer = 2,
    [EnumMember] DateTime = 3,
    [EnumMember] PlainText = 5,
    [EnumMember] Html = 7,
    [EnumMember] TreePath = 8,
    [EnumMember] History = 9,
    [EnumMember] Double = 10, // 0x0000000A
    [EnumMember] Guid = 11, // 0x0000000B
    [EnumMember] Boolean = 12, // 0x0000000C
    [EnumMember] Identity = 13, // 0x0000000D
    [EnumMember] PicklistInteger = 14, // 0x0000000E
    [EnumMember] PicklistString = 15, // 0x0000000F
    [EnumMember] PicklistDouble = 16, // 0x00000010
    [EnumMember] TeamProject = 17, // 0x00000011
  }
}
