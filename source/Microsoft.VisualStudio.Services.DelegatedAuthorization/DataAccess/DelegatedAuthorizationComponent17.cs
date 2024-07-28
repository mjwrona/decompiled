// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent17
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent17 : DelegatedAuthorizationComponent16
  {
    public override void UpdateAudienceForPATsAndHostAuth(
      Guid orgHostId,
      Guid collectionHostId,
      string oldAudience = null,
      string newAudience = null)
    {
      try
      {
        this.TraceEnter(1048570, nameof (UpdateAudienceForPATsAndHostAuth));
        this.PrepareStoredProcedure("prc_UpdateAudienceForPATsAndHostAuth");
        this.BindGuid("@orgHostId", orgHostId);
        this.BindGuid("@collectionHostId", collectionHostId);
        this.BindString("@oldAudience", oldAudience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@newAudience", newAudience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1048571, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048572, nameof (UpdateAudienceForPATsAndHostAuth));
      }
    }

    public override List<AccessTokenKey> GetSSHKeysForCollection(string audience)
    {
      if (audience == null)
        throw new ArgumentNullException();
      List<AccessTokenKey> keysForCollection = new List<AccessTokenKey>();
      try
      {
        this.TraceEnter(1048573, nameof (GetSSHKeysForCollection));
        this.PrepareStoredProcedure("prc_GetSSHKeysForCollection");
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<AccessTokenKey>((ObjectBinder<AccessTokenKey>) new AccessTokenKeyBinder6());
          List<AccessTokenKey> items = resultCollection.GetCurrent<AccessTokenKey>().Items;
          keysForCollection.AddRange((IEnumerable<AccessTokenKey>) items);
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048574, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048575, nameof (GetSSHKeysForCollection));
      }
      return keysForCollection;
    }
  }
}
