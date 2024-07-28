// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Notification.FeedChangeType
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.Common.Notification
{
  [DataContract]
  public enum FeedChangeType
  {
    [EnumMember] CreatedFeed = 1,
    [EnumMember] DeletedFeed = 2,
    [EnumMember] UpdatedFeed = 3,
    [EnumMember] FeedPermissionChanged = 4,
    [EnumMember] AddedView = 5,
    [EnumMember] DeletedView = 6,
    [EnumMember] UpdatedView = 7,
    [EnumMember] ViewPermissionChanged = 8,
    [EnumMember] RestoredFeed = 9,
    [EnumMember] PermanentDeletedFeed = 10, // 0x0000000A
  }
}
