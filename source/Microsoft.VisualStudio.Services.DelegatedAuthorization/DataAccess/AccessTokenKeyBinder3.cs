// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AccessTokenKeyBinder3
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class AccessTokenKeyBinder3 : AccessTokenKeyBinder2
  {
    private SqlColumnBinder UserIdColumn = new SqlColumnBinder("UserId");
    private SqlColumnBinder AccessHashColumn = new SqlColumnBinder("AccessHash");

    protected override AccessTokenKey Bind()
    {
      AccessTokenKey accessTokenKey = base.Bind();
      accessTokenKey.UserId = this.UserIdColumn.GetGuid((IDataReader) this.Reader);
      if (this.AccessHashColumn.ColumnExists((IDataReader) this.Reader))
        accessTokenKey.AccessHash = this.AccessHashColumn.GetString((IDataReader) this.Reader, true);
      return accessTokenKey;
    }
  }
}
