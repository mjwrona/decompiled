// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.TokenExpirationComponent
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class TokenExpirationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<TokenExpirationComponent>(1),
      (IComponentCreator) new ComponentCreator<TokenExpirationComponent2>(2)
    }, "TokenExpiration");

    public TokenExpirationComponent()
    {
      this.ContainerErrorCode = 50000;
      this.SelectedFeatures |= SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    internal virtual List<ExpiringToken> GetExpiringTokens(DateTime setToExpireOn, int offsetWindow)
    {
      List<ExpiringToken> expiringTokens = new List<ExpiringToken>();
      try
      {
        this.TraceEnter(1048576, nameof (GetExpiringTokens));
        this.PrepareStoredProcedure("prc_GetExpiringPATs");
        this.BindDate("@setToExpireOn", setToExpireOn);
        this.BindInt("@offsetWindow", offsetWindow);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ExpiringToken>((ObjectBinder<ExpiringToken>) new TokenExpirationComponent.ExpiringTokenBinder());
          List<ExpiringToken> items = resultCollection.GetCurrent<ExpiringToken>().Items;
          expiringTokens.AddRange((IEnumerable<ExpiringToken>) items);
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048577, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048578, nameof (GetExpiringTokens));
      }
      return expiringTokens;
    }

    internal virtual List<ExpiringToken> GetExpiringTokens(DateTime setToExpireOn) => throw new NotImplementedException();

    internal class ExpiringTokenBinder : ObjectBinder<ExpiringToken>
    {
      private SqlColumnBinder DisplayNameColumn = new SqlColumnBinder("DisplayName");
      private SqlColumnBinder AuthorizationIdColumn = new SqlColumnBinder("AuthorizationId");
      private SqlColumnBinder IdentityIdColumn = new SqlColumnBinder("UserId");
      private SqlColumnBinder ValidFromColumn = new SqlColumnBinder("ValidFrom");
      private SqlColumnBinder ValidToColumn = new SqlColumnBinder("ValidTo");
      private SqlColumnBinder ScopesColumn = new SqlColumnBinder("Scopes");
      private SqlColumnBinder AudienceColumn = new SqlColumnBinder("Audience");

      protected override ExpiringToken Bind()
      {
        string str1 = this.DisplayNameColumn.GetString((IDataReader) this.Reader, false);
        Guid guid1 = this.AuthorizationIdColumn.GetGuid((IDataReader) this.Reader);
        Guid guid2 = this.IdentityIdColumn.GetGuid((IDataReader) this.Reader);
        DateTimeOffset dateTimeOffset1 = this.ValidFromColumn.GetDateTimeOffset(this.Reader);
        DateTimeOffset dateTimeOffset2 = this.ValidToColumn.GetDateTimeOffset(this.Reader);
        string str2 = this.ScopesColumn.GetString((IDataReader) this.Reader, false);
        string str3 = this.AudienceColumn.GetString((IDataReader) this.Reader, true);
        return new ExpiringToken()
        {
          DisplayName = str1,
          AuthorizationId = guid1,
          UserId = guid2,
          ValidFrom = dateTimeOffset1.Date,
          ValidTo = dateTimeOffset2.Date,
          Scopes = str2,
          Audience = str3
        };
      }
    }
  }
}
