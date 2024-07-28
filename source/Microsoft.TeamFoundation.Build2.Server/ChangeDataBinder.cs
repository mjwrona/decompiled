// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ChangeDataBinder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal sealed class ChangeDataBinder : BuildObjectBinder<ChangeData>
  {
    private SqlColumnBinder m_position = new SqlColumnBinder("Position");
    private SqlColumnBinder m_descriptor = new SqlColumnBinder("Descriptor");

    protected override ChangeData Bind() => new ChangeData()
    {
      Position = this.m_position.GetInt32((IDataReader) this.Reader),
      Descriptor = this.m_descriptor.GetString((IDataReader) this.Reader, false)
    };
  }
}
