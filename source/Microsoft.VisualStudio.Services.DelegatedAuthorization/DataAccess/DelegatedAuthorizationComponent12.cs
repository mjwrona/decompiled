// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent12
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent12 : DelegatedAuthorizationComponent11
  {
    public override string RemoveAccessTokenKey(Guid authorizationId)
    {
      try
      {
        this.TraceEnter(1048810, nameof (RemoveAccessTokenKey));
        this.PrepareStoredProcedure("prc_RemoveAccessTokenKey");
        this.BindGuid("@authorizationId", authorizationId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<string>((ObjectBinder<string>) new AccessTokenHashStringBinder());
          return resultCollection.GetCurrent<string>().FirstOrDefault<string>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048811, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048812, nameof (RemoveAccessTokenKey));
      }
    }
  }
}
