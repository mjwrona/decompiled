// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GetOperationColumns3
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class GetOperationColumns3 : GetOperationColumns
  {
    protected SqlColumnBinder propertyId = new SqlColumnBinder("PropertyId");

    public GetOperationColumns3()
    {
    }

    public GetOperationColumns3(VersionControlSqlResourceComponent component)
      : base(component)
    {
    }

    protected override GetOperation Bind()
    {
      GetOperation getOperation = base.Bind();
      getOperation.PropertyId = this.propertyId.GetInt32((IDataReader) this.Reader, -1);
      return getOperation;
    }
  }
}
