// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.DataAccess.PermissionLevelDefinitionBinder
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using System.Data;

namespace Microsoft.VisualStudio.Services.PermissionLevel.DataAccess
{
  public class PermissionLevelDefinitionBinder : ObjectBinder<PermissionLevelDefinition>
  {
    protected SqlColumnBinder m_IdColumn = new SqlColumnBinder("Id");
    protected SqlColumnBinder m_NameColumn = new SqlColumnBinder("Name");
    protected SqlColumnBinder m_DescriptionColumn = new SqlColumnBinder("Description");
    protected SqlColumnBinder m_TypeColumn = new SqlColumnBinder("Type");
    protected SqlColumnBinder m_ScopeIdColumn = new SqlColumnBinder("ScopeId");
    protected SqlColumnBinder m_IsActiveColumn = new SqlColumnBinder("IsActive");
    protected SqlColumnBinder m_CreationDateColumn = new SqlColumnBinder("CreationDate");
    protected SqlColumnBinder m_LastUpdatedColumn = new SqlColumnBinder("LastUpdated");

    protected override PermissionLevelDefinition Bind() => PermissionLevelDefinitionFactory.Create(this.m_IdColumn.GetGuid((IDataReader) this.Reader, false), this.m_NameColumn.GetString((IDataReader) this.Reader, true), this.m_DescriptionColumn.GetString((IDataReader) this.Reader, true), (PermissionLevelDefinitionType) this.m_TypeColumn.GetInt16((IDataReader) this.Reader), (PermissionLevelDefinitionScope) this.m_ScopeIdColumn.GetInt16((IDataReader) this.Reader), this.m_IsActiveColumn.GetBoolean((IDataReader) this.Reader), this.m_CreationDateColumn.GetDateTime((IDataReader) this.Reader), this.m_LastUpdatedColumn.GetDateTime((IDataReader) this.Reader));
  }
}
