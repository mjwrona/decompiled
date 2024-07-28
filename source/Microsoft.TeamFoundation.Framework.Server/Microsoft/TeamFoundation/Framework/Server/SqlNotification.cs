// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlNotification
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlNotification
  {
    public Guid EventClass;
    public long EventId;
    public string EventData;
    public Guid EventAuthor;
    public Guid HostId;
    public bool IsGlobal;
    internal Dictionary<Guid, List<NotificationProcessor.INotification>> m_notifications;
  }
}
