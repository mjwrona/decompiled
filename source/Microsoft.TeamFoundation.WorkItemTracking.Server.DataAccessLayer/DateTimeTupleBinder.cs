// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DateTimeTupleBinder
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DateTimeTupleBinder : ObjectBinder<Tuple<DateTime, DateTime>>
  {
    private SqlColumnBinder maxCreatedDate = new SqlColumnBinder("MaxCreatedDate");
    private SqlColumnBinder maxRemovedDate = new SqlColumnBinder("MaxRemovedDate");

    protected override Tuple<DateTime, DateTime> Bind() => new Tuple<DateTime, DateTime>(this.maxCreatedDate.GetDateTime((IDataReader) this.Reader), this.maxRemovedDate.GetDateTime((IDataReader) this.Reader));
  }
}
