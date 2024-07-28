// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.DataAccess.PermissionLevelAssignmentBinder
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.PermissionLevel.DataAccess
{
  public class PermissionLevelAssignmentBinder : ObjectBinder<PermissionLevelAssignmentStoreItem>
  {
    protected SqlColumnBinder m_HostIdColumn = new SqlColumnBinder("HostId");
    protected SqlColumnBinder m_PermissionLevelDefinitionIdColumn = new SqlColumnBinder("PermissionLevelDefinitionId");
    protected SqlColumnBinder m_ResourceIdColumn = new SqlColumnBinder("ResourceId");
    protected SqlColumnBinder m_SubjectIdColumn = new SqlColumnBinder("SubjectId");
    protected SqlColumnBinder m_ScopeIdColumn = new SqlColumnBinder("ScopeId");

    protected override PermissionLevelAssignmentStoreItem Bind() => new PermissionLevelAssignmentStoreItem(this.m_HostIdColumn.GetGuid((IDataReader) this.Reader, false), this.m_PermissionLevelDefinitionIdColumn.GetGuid((IDataReader) this.Reader, false), this.m_ResourceIdColumn.GetString((IDataReader) this.Reader, false), this.m_SubjectIdColumn.GetGuid((IDataReader) this.Reader, false), this.m_ScopeIdColumn.GetInt32((IDataReader) this.Reader, -1));
  }
}
