// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.DataAccess.WidgetsETagBinder
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Dashboards.DataAccess
{
  internal class WidgetsETagBinder : ObjectBinder<string>
  {
    private SqlColumnBinder eTagColumn = new SqlColumnBinder("ETag");

    protected override string Bind() => this.eTagColumn.GetString((IDataReader) this.Reader, true);
  }
}
