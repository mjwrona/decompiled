// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectOperationColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectOperationColumns : ObjectBinder<ProjectOperation>
  {
    private SqlColumnBinder idColumn = new SqlColumnBinder("ProjectId");
    private SqlColumnBinder revisionColumn = new SqlColumnBinder("Revision");
    private SqlColumnBinder operationColumn = new SqlColumnBinder("OperationId");

    protected override ProjectOperation Bind() => new ProjectOperation()
    {
      ProjectId = this.idColumn.GetGuid((IDataReader) this.Reader, false),
      Revision = this.revisionColumn.GetInt64((IDataReader) this.Reader),
      OperationId = this.operationColumn.GetGuid((IDataReader) this.Reader, false)
    };
  }
}
