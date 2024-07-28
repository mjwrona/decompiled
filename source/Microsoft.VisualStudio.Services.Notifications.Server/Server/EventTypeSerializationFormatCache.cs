// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.EventTypeSerializationFormatCache
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class EventTypeSerializationFormatCache : VssMemoryCacheService<string, EventSerializerType>
  {
    public EventTypeSerializationFormatCache()
      : base((IEqualityComparer<string>) EqualityComparer<string>.Default, EventTypeSerializationFormatCache.GetConfiguration())
    {
    }

    private static MemoryCacheConfiguration<string, EventSerializerType> GetConfiguration() => new MemoryCacheConfiguration<string, EventSerializerType>().WithCleanupInterval(TimeSpan.FromHours(1.0)).WithExpiryInterval(TimeSpan.FromMinutes(30.0));
  }
}
