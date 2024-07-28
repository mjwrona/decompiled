// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlNotificationComponent2
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SqlNotificationComponent2 : SqlNotificationComponent
  {
    public override long SendNotification(
      Guid eventClass,
      string eventData,
      Guid eventAuthor,
      Guid hostId)
    {
      this.PrepareStoredProcedure("prc_SendNotification");
      this.BindGuid("@eventClass", eventClass);
      this.BindString("@eventData", eventData, int.MaxValue, true, SqlDbType.NVarChar);
      this.BindNullableGuid("@eventAuthor", eventAuthor);
      this.BindGuid("@hostId", hostId);
      return (long) this.ExecuteScalar();
    }
  }
}
