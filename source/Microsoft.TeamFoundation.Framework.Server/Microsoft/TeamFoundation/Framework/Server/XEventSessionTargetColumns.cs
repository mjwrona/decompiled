// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.XEventSessionTargetColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal sealed class XEventSessionTargetColumns : ObjectBinder<XEventTargetProperties>
  {
    private SqlColumnBinder m_targetData = new SqlColumnBinder("targetData");
    private SqlColumnBinder m_isRunningInReadScaleOut = new SqlColumnBinder("isRunningInReadScaleOut");

    protected override XEventTargetProperties Bind()
    {
      IDataReader baseReader = this.BaseReader;
      return new XEventTargetProperties()
      {
        TargetData = this.m_targetData.GetString(baseReader, false),
        IsRunningInReadScaleOut = this.m_isRunningInReadScaleOut.GetBoolean(baseReader, false)
      };
    }
  }
}
