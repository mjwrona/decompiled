// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingDeploymentInfoColumns
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingDeploymentInfoColumns : ObjectBinder<ServicingDeploymentInfo>
  {
    private SqlColumnBinder m_serviceLevels = new SqlColumnBinder("ServiceLevel");
    private SqlColumnBinder m_startTime = new SqlColumnBinder("StartTime");
    private SqlColumnBinder m_endTime = new SqlColumnBinder("EndTime");

    protected override ServicingDeploymentInfo Bind()
    {
      string serviceLevels = this.m_serviceLevels.GetString((IDataReader) this.Reader, false);
      DateTime dateTime = this.m_startTime.GetDateTime((IDataReader) this.Reader);
      DateTime? nullable = new DateTime?();
      if (!this.m_endTime.IsNull((IDataReader) this.Reader))
        nullable = new DateTime?(this.m_endTime.GetDateTime((IDataReader) this.Reader));
      DateTime startTime = dateTime;
      DateTime? endTime = nullable;
      return new ServicingDeploymentInfo(serviceLevels, startTime, endTime);
    }
  }
}
