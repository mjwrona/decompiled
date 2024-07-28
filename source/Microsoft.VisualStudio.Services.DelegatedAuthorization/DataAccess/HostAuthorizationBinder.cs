// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.HostAuthorizationBinder
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class HostAuthorizationBinder : ObjectBinder<HostAuthorization>
  {
    private SqlColumnBinder IdColumn = new SqlColumnBinder("Id");
    private SqlColumnBinder RegistrationIdColumn = new SqlColumnBinder("RegistrationId");
    private SqlColumnBinder HostIdColumn = new SqlColumnBinder("HostId");
    private SqlColumnBinder IsValidColumn = new SqlColumnBinder("IsValid");

    protected override HostAuthorization Bind()
    {
      Guid guid1 = this.IdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid2 = this.RegistrationIdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid3 = this.HostIdColumn.GetGuid((IDataReader) this.Reader);
      bool boolean = this.IsValidColumn.GetBoolean((IDataReader) this.Reader);
      return new HostAuthorization()
      {
        Id = guid1,
        RegistrationId = guid2,
        HostId = guid3,
        IsValid = boolean
      };
    }
  }
}
