// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.AccessTokenBinder
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class AccessTokenBinder : ObjectBinder<AccessToken>
  {
    private SqlColumnBinder AccessIdColumn = new SqlColumnBinder("AccessId");
    private SqlColumnBinder AuthorizationIdColumn = new SqlColumnBinder("AuthorizationId");
    private SqlColumnBinder ValidFromColumn = new SqlColumnBinder("ValidFrom");
    private SqlColumnBinder ValidToColumn = new SqlColumnBinder("ValidTo");
    private SqlColumnBinder RefreshedColumn = new SqlColumnBinder("Refreshed");
    private SqlColumnBinder IsRefreshColumn = new SqlColumnBinder("IsRefresh");
    private SqlColumnBinder IsValidColumn = new SqlColumnBinder("IsValid");

    protected override AccessToken Bind()
    {
      Guid guid1 = this.AccessIdColumn.GetGuid((IDataReader) this.Reader);
      Guid guid2 = this.AuthorizationIdColumn.GetGuid((IDataReader) this.Reader);
      DateTimeOffset dateTimeOffset1 = this.ValidFromColumn.GetDateTimeOffset(this.Reader);
      DateTimeOffset dateTimeOffset2 = this.ValidToColumn.GetDateTimeOffset(this.Reader);
      DateTimeOffset dateTimeOffset3 = this.RefreshedColumn.GetDateTimeOffset(this.Reader);
      bool boolean1 = this.IsRefreshColumn.GetBoolean((IDataReader) this.Reader);
      bool boolean2 = this.IsValidColumn.GetBoolean((IDataReader) this.Reader);
      return new AccessToken()
      {
        AccessId = guid1,
        AuthorizationId = guid2,
        ValidFrom = dateTimeOffset1,
        ValidTo = dateTimeOffset2,
        Refreshed = dateTimeOffset3,
        IsRefresh = boolean1,
        IsValid = boolean2
      };
    }
  }
}
