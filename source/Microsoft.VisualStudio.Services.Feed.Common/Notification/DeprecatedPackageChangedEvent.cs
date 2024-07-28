// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.Notification.DeprecatedPackageChangedEvent
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Feed.WebApi.Internal;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Feed.Common.Notification
{
  [DataContract]
  [ServiceEventObject]
  [NotificationEventBindings(EventSerializerType.Json, "ms.feed.packagemanagement-packagechanged-event")]
  public class DeprecatedPackageChangedEvent : PackageChangedEvent
  {
    public DeprecatedPackageChangedEvent()
    {
    }

    public DeprecatedPackageChangedEvent(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      PackageIndexEntry package,
      PackageIndexEntryResponse response,
      PackageChangeType publishType = PackageChangeType.Publish)
      : base(feed, package, response, publishType)
    {
    }

    public DeprecatedPackageChangedEvent(
      Microsoft.VisualStudio.Services.Feed.WebApi.Feed feed,
      IPackageInfo package,
      Microsoft.VisualStudio.Services.Feed.WebApi.PackageVersion version,
      PackageChangeType publishType = PackageChangeType.Promote)
      : base(feed, package, version, publishType)
    {
    }
  }
}
