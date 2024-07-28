// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.SubscriptionStatus
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public enum SubscriptionStatus : short
  {
    [EnumMember] JailedByNotificationsVolume = -200, // 0xFF38
    [EnumMember] PendingDeletion = -100, // 0xFF9C
    [EnumMember] DisabledArgumentException = -12, // 0xFFF4
    [EnumMember] DisabledProjectInvalid = -11, // 0xFFF5
    [EnumMember] DisabledMissingPermissions = -10, // 0xFFF6
    [EnumMember] DisabledFromProbation = -9, // 0xFFF7
    [EnumMember] DisabledInactiveIdentity = -8, // 0xFFF8
    [EnumMember] DisabledMessageQueueNotSupported = -7, // 0xFFF9
    [EnumMember] DisabledMissingIdentity = -6, // 0xFFFA
    [EnumMember] DisabledInvalidRoleExpression = -5, // 0xFFFB
    [EnumMember] DisabledInvalidPathClause = -4, // 0xFFFC
    [EnumMember] DisabledAsDuplicateOfDefault = -3, // 0xFFFD
    [EnumMember] DisabledByAdmin = -2, // 0xFFFE
    [EnumMember] Disabled = -1, // 0xFFFF
    [EnumMember] Enabled = 0,
    [EnumMember] EnabledOnProbation = 1,
  }
}
