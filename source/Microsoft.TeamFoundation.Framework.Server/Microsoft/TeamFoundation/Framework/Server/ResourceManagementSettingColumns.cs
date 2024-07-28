// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ResourceManagementSettingColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class ResourceManagementSettingColumns : ObjectBinder<ResourceManagementSetting>
  {
    private SqlColumnBinder m_nameColumn = new SqlColumnBinder("Name");
    private SqlColumnBinder m_valueColumn = new SqlColumnBinder("Value");

    protected override ResourceManagementSetting Bind() => new ResourceManagementSetting()
    {
      Name = this.m_nameColumn.GetString((IDataReader) this.Reader, false),
      Value = this.m_valueColumn.GetString((IDataReader) this.Reader, true)
    };
  }
}
