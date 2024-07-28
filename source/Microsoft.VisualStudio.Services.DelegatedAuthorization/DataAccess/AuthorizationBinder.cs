// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AuthorizationBinder
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class AuthorizationBinder : ObjectBinder<Authorization>
  {
    private SqlColumnBinder AuthorizationIdColumn = new SqlColumnBinder("AuthorizationId");
    private SqlColumnBinder RegistrationIdColumn = new SqlColumnBinder("RegistrationId");
    private SqlColumnBinder IdentityIdColumn = new SqlColumnBinder("IdentityId");
    private SqlColumnBinder ScopeColumn = new SqlColumnBinder("Scope");
    private SqlColumnBinder ValidFromColumn = new SqlColumnBinder("ValidFrom");
    private SqlColumnBinder ValidToColumn = new SqlColumnBinder("ValidTo");
    private SqlColumnBinder AccessIssuedColumn = new SqlColumnBinder("AccessIssued");
    private SqlColumnBinder IsAccessIssuedColumn = new SqlColumnBinder("IsAccessIssued");
    private SqlColumnBinder IsValidColumn = new SqlColumnBinder("IsValid");
    private SqlColumnBinder RedirectLocationColumn = new SqlColumnBinder("Location");

    protected override Authorization Bind()
    {
      Guid guid1 = this.AuthorizationIdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid2 = this.RegistrationIdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid3 = this.IdentityIdColumn.GetGuid((IDataReader) this.Reader);
      string str = this.ScopeColumn.GetString((IDataReader) this.Reader, true);
      DateTimeOffset dateTimeOffset1 = this.ValidFromColumn.GetDateTimeOffset(this.Reader);
      DateTimeOffset dateTimeOffset2 = this.ValidToColumn.GetDateTimeOffset(this.Reader);
      DateTimeOffset dateTimeOffset3 = this.AccessIssuedColumn.GetDateTimeOffset(this.Reader);
      bool boolean1 = this.IsAccessIssuedColumn.GetBoolean((IDataReader) this.Reader);
      bool boolean2 = this.IsValidColumn.GetBoolean((IDataReader) this.Reader);
      string uriString = this.RedirectLocationColumn.GetString((IDataReader) this.Reader, true);
      Authorization authorization = new Authorization()
      {
        AuthorizationId = guid1,
        RegistrationId = guid2,
        IdentityId = guid3,
        Scopes = str,
        ValidFrom = dateTimeOffset1,
        ValidTo = dateTimeOffset2,
        AccessIssued = dateTimeOffset3,
        IsAccessUsed = boolean1,
        IsValid = boolean2
      };
      if (!string.IsNullOrEmpty(uriString))
        authorization.RedirectUri = new Uri(uriString);
      return authorization;
    }
  }
}
