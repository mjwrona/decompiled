// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdMappingColumns
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityIdMappingColumns : ObjectBinder<IdentityIdMapping>
  {
    private readonly bool m_allowNullMasterIds;
    private SqlColumnBinder m_localIdColumn = new SqlColumnBinder("localId");
    private SqlColumnBinder m_masterIdColumn = new SqlColumnBinder("masterId");

    public IdentityIdMappingColumns(bool allowNullMasterIds = false) => this.m_allowNullMasterIds = allowNullMasterIds;

    protected override IdentityIdMapping Bind() => new IdentityIdMapping()
    {
      LocalId = this.m_localIdColumn.GetGuid((IDataReader) this.Reader),
      MasterId = this.m_masterIdColumn.GetGuid((IDataReader) this.Reader, this.m_allowNullMasterIds)
    };
  }
}
