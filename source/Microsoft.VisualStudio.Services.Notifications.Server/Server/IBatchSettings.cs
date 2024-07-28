// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.IBatchSettings
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public interface IBatchSettings
  {
    Dictionary<string, string> DynamicTargetMappings { get; }

    TimeSpan RegexTimeout { get; }

    int EventBatchSize { get; }

    bool RemoveBadSubscriptions { get; }

    int NotificationFlushTimeout { get; }

    int MaxTeamMembersForRoleExpansion { get; }

    DateTime EventsCreatedDate { get; set; }

    bool LoadContributedSubscriptions { get; set; }

    int CleanupNotificationAgeMins { get; }

    bool UseRegexForMatch { get; }
  }
}
