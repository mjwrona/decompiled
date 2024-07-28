// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.SecurityObjectColumns
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class SecurityObjectColumns : ObjectBinder<SecurityObject>
  {
    private SqlColumnBinder instanceColumn = new SqlColumnBinder("Instance");
    private SqlColumnBinder objectIdColumn = new SqlColumnBinder("ObjectId");
    private SqlColumnBinder securityTokenColumn = new SqlColumnBinder("SecurityToken");
    private SqlColumnBinder classIdColumn = new SqlColumnBinder("ClassId");
    private SqlColumnBinder projectIdColumn = new SqlColumnBinder("ProjectInstance");

    protected override SecurityObject Bind() => new SecurityObject(this.instanceColumn.GetInt32((IDataReader) this.Reader), this.objectIdColumn.GetString((IDataReader) this.Reader, false), this.securityTokenColumn.GetString((IDataReader) this.Reader, false).TrimEnd(AuthorizationSecurityConstants.SeparatorChar), this.classIdColumn.GetString((IDataReader) this.Reader, false), this.projectIdColumn.GetGuid((IDataReader) this.Reader, false));
  }
}
