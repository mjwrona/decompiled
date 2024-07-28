// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ProjectNameMappingsColumns
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class ProjectNameMappingsColumns : VersionControlObjectBinder<KeyValuePair<Guid, string>>
  {
    private SqlColumnBinder dataspaceIdColumn = new SqlColumnBinder("DataspaceId");
    private SqlColumnBinder projectNameColumn = new SqlColumnBinder("ProjectName");

    public ProjectNameMappingsColumns(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override KeyValuePair<Guid, string> Bind() => new KeyValuePair<Guid, string>(this.m_component.GetDataspaceIdentifier(this.dataspaceIdColumn.GetInt32((IDataReader) this.Reader)), Microsoft.TeamFoundation.Framework.Server.DBPath.DatabaseToUserPath(this.projectNameColumn.GetString((IDataReader) this.Reader, false)));
  }
}
