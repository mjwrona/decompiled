// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TagStringBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class TagStringBinder : ObjectBinder<TagStringData>
  {
    private SqlColumnBinder m_stringId = new SqlColumnBinder("StringId");
    private SqlColumnBinder m_stringValue = new SqlColumnBinder("StringValue");

    protected override TagStringData Bind() => new TagStringData()
    {
      StringId = this.m_stringId.GetInt32((IDataReader) this.Reader),
      StringValue = this.m_stringValue.GetString((IDataReader) this.Reader, false)
    };
  }
}
