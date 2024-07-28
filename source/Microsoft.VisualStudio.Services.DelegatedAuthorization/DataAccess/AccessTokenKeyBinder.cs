// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AccessTokenKeyBinder
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class AccessTokenKeyBinder : ObjectBinder<AccessTokenKey>
  {
    private SqlColumnBinder AccessIdColumn = new SqlColumnBinder("AccessId");
    private SqlColumnBinder AuthorizationIdColumn = new SqlColumnBinder("AuthorizationId");
    private SqlColumnBinder ValidFromColumn = new SqlColumnBinder("ValidFrom");
    private SqlColumnBinder ValidToColumn = new SqlColumnBinder("ValidTo");
    private SqlColumnBinder DisplayNameColumn = new SqlColumnBinder("DisplayName");
    private SqlColumnBinder ScopeColumn = new SqlColumnBinder("Scope");
    private SqlColumnBinder IsValidColumn = new SqlColumnBinder("IsValid");

    protected override AccessTokenKey Bind()
    {
      Guid guid1 = this.AccessIdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid2 = this.AuthorizationIdColumn.GetGuid((IDataReader) this.Reader);
      DateTimeOffset dateTimeOffset1 = this.ValidFromColumn.GetDateTimeOffset(this.Reader);
      DateTimeOffset dateTimeOffset2 = this.ValidToColumn.GetDateTimeOffset(this.Reader);
      string str1 = this.DisplayNameColumn.GetString((IDataReader) this.Reader, true);
      string str2 = this.ScopeColumn.GetString((IDataReader) this.Reader, true);
      bool boolean = this.IsValidColumn.GetBoolean((IDataReader) this.Reader);
      return new AccessTokenKey()
      {
        AccessId = guid1,
        AuthorizationId = guid2,
        ValidFrom = dateTimeOffset1.UtcDateTime,
        ValidTo = dateTimeOffset2.UtcDateTime,
        DisplayName = str1,
        Scope = str2,
        IsValid = boolean
      };
    }
  }
}
