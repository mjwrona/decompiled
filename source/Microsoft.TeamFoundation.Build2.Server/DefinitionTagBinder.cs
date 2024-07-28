// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DefinitionTagBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class DefinitionTagBinder : ObjectBinder<DefinitionTagData>
  {
    private SqlColumnBinder m_definitionId = new SqlColumnBinder("DefinitionId");
    private SqlColumnBinder m_definitionVersion = new SqlColumnBinder("DefinitionVersion");
    private SqlColumnBinder m_tag = new SqlColumnBinder("Tag");

    protected override DefinitionTagData Bind() => new DefinitionTagData()
    {
      DefinitionId = this.m_definitionId.GetInt32((IDataReader) this.Reader),
      DefinitionVersion = this.m_definitionVersion.GetInt32((IDataReader) this.Reader),
      Tag = this.m_tag.GetString((IDataReader) this.Reader, false)
    };
  }
}
