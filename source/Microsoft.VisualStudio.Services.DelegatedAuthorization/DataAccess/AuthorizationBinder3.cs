// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AuthorizationBinder3
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class AuthorizationBinder3 : AuthorizationBinder2
  {
    private SqlColumnBinder SourceColumn = new SqlColumnBinder("Source");

    protected override Authorization Bind()
    {
      Authorization authorization = base.Bind();
      authorization.Source = this.SourceColumn.GetString((IDataReader) this.Reader, true);
      return authorization;
    }
  }
}
