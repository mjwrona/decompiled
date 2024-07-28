// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AccessTokenKeyBinder5
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class AccessTokenKeyBinder5 : AccessTokenKeyBinder4
  {
    private SqlColumnBinder IsPublicColumn = new SqlColumnBinder("IsPublic");
    private SqlColumnBinder PublicDataColumn = new SqlColumnBinder("PublicData");

    protected override AccessTokenKey Bind()
    {
      AccessTokenKey accessTokenKey = base.Bind();
      accessTokenKey.IsPublic = this.IsPublicColumn.GetBoolean((IDataReader) this.Reader);
      accessTokenKey.PublicData = this.PublicDataColumn.GetString((IDataReader) this.Reader, true);
      return accessTokenKey;
    }
  }
}
