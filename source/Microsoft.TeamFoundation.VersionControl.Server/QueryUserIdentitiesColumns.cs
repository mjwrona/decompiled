// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.QueryUserIdentitiesColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class QueryUserIdentitiesColumns : VersionControlObjectBinder<ChangeSetOwner>
  {
    internal SqlColumnBinder teamFoundationId = new SqlColumnBinder("TeamFoundationId");
    internal SqlColumnBinder changeSetCount = new SqlColumnBinder("ChangeSetCount");
    internal SqlColumnBinder lastCheckinDate = new SqlColumnBinder("LastCheckinDate");

    protected override ChangeSetOwner Bind() => new ChangeSetOwner()
    {
      TeamFoundationId = this.teamFoundationId.GetGuid((IDataReader) this.Reader),
      NumChangesets = this.changeSetCount.GetInt32((IDataReader) this.Reader),
      LastCheckinDate = this.lastCheckinDate.GetDateTime((IDataReader) this.Reader)
    };
  }
}
