// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.TokenExpirationComponent2
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class TokenExpirationComponent2 : TokenExpirationComponent
  {
    internal override List<ExpiringToken> GetExpiringTokens(DateTime setToExpireOn)
    {
      try
      {
        this.TraceEnter(1048576, nameof (GetExpiringTokens));
        this.PrepareStoredProcedure("prc_GetExpiringPATs");
        this.BindDate("@setToExpireOn", setToExpireOn);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<ExpiringToken>((ObjectBinder<ExpiringToken>) new TokenExpirationComponent.ExpiringTokenBinder());
          return resultCollection.GetCurrent<ExpiringToken>().Items;
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
    }
  }
}
