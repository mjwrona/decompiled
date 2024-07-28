// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent15
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent15 : DelegatedAuthorizationComponent14
  {
    public override IList<Guid> GetAuthorizationIdsByPublicData(string publicData)
    {
      try
      {
        this.TraceEnter(1048564, nameof (GetAuthorizationIdsByPublicData));
        this.PrepareStoredProcedure("prc_GetAuthorizationIdsByPublicData");
        this.BindString("@PublicData", publicData, 4000, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new PublicAuthorizationIdBinder());
          return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048565, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048566, nameof (GetAuthorizationIdsByPublicData));
      }
    }
  }
}
