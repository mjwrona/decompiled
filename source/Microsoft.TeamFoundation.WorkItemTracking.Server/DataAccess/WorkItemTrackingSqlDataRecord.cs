// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess.WorkItemTrackingSqlDataRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess
{
  internal class WorkItemTrackingSqlDataRecord : SqlDataRecord
  {
    public WorkItemTrackingSqlDataRecord(params SqlMetaData[] metaData)
      : base(metaData)
    {
    }

    public override void SetDateTime(int ordinal, DateTime value)
    {
      if (value < SqlDateTime.MinValue.Value)
        value = SqlDateTime.MinValue.Value;
      base.SetDateTime(ordinal, value);
    }
  }
}
