// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectHistoryColumns2
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectHistoryColumns2 : ProjectInfoColumns2
  {
    private SqlColumnBinder deltaColumn = new SqlColumnBinder("ProjectPropertiesDelta");

    protected override ProjectInfo Bind()
    {
      ProjectInfo projectInfo = base.Bind();
      int ordinal = this.deltaColumn.GetOrdinal((IDataReader) this.Reader);
      if (ordinal >= 0 && !this.deltaColumn.IsNull((IDataReader) this.Reader))
        ProjectHistoryColumns.ReadProperties(projectInfo, this.Reader.GetSqlXml(ordinal));
      return projectInfo;
    }
  }
}
