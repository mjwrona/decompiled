// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent16
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent16 : DelegatedAuthorizationComponent15
  {
    public override void UpdateAudienceAndAccessHashForSSHKey(
      Guid authorizationId,
      string audience = null,
      string accessHash = null)
    {
      try
      {
        this.TraceEnter(1048567, nameof (UpdateAudienceAndAccessHashForSSHKey));
        this.PrepareStoredProcedure("prc_UpdateAudienceAndAccessHashForSSHKey");
        this.BindGuid("@authorizationId", authorizationId);
        this.BindString("@audience", audience, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@accessHash", accessHash, 500, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1048568, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048569, nameof (UpdateAudienceAndAccessHashForSSHKey));
      }
    }
  }
}
