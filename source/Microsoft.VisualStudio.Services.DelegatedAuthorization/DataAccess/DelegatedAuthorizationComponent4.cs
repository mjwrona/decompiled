// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent4
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent4 : DelegatedAuthorizationComponent3
  {
    public override List<AccessTokenKey> ListAccessTokenKeys(
      Guid userId,
      Guid accessId,
      bool isPublic = false)
    {
      if (userId == Guid.Empty)
        throw new ArgumentNullException();
      List<AccessTokenKey> accessTokenKeyList = new List<AccessTokenKey>();
      try
      {
        this.TraceEnter(1048537, nameof (ListAccessTokenKeys));
        this.PrepareStoredProcedure("prc_GetAccessTokensByUserId");
        this.BindGuid("@userId", userId);
        this.BindGuid("@accessId", accessId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder());
          List<AccessTokenKey> items = resultCollection.GetCurrent<AccessTokenKey>().Items;
          accessTokenKeyList.AddRange((IEnumerable<AccessTokenKey>) items);
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048532, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048533, nameof (ListAccessTokenKeys));
      }
      return accessTokenKeyList;
    }
  }
}
