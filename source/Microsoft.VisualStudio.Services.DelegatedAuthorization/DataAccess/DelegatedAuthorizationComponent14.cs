// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess.DelegatedAuthorizationComponent14
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization.DataAccess
{
  internal class DelegatedAuthorizationComponent14 : DelegatedAuthorizationComponent13
  {
    public override void CreateAccessTokenKey(
      Guid accessId,
      string tokenHash,
      string displayName,
      Guid identityId,
      bool isPublic = false,
      string publicData = null)
    {
      try
      {
        this.TraceEnter(1048540, nameof (CreateAccessTokenKey));
        this.PrepareStoredProcedure("prc_CreateDelegatedAuthorizationAccessKey");
        this.BindGuid("@accessId", accessId);
        this.BindString("@accessHash", tokenHash, 100, BindStringBehavior.Unchanged, SqlDbType.VarChar);
        this.BindString("@displayName", displayName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindBoolean("@isPublic", isPublic);
        this.BindString("@publicData", publicData, 4000, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
        this.BindGuid("@identityId", identityId);
        this.ExecuteNonQuery();
      }
      finally
      {
        this.TraceLeave(1048541, nameof (CreateAccessTokenKey));
      }
    }

    public override IList<HostAuthorization> GetHostAuthorizations(Guid hostId)
    {
      List<HostAuthorization> hostAuthorizations = new List<HostAuthorization>();
      try
      {
        this.TraceEnter(1048700, nameof (GetHostAuthorizations));
        this.PrepareStoredProcedure("prc_GetDelegatedHostAuthorizations");
        this.BindGuid("@hostId", hostId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HostAuthorization>((ObjectBinder<HostAuthorization>) new HostAuthorizationBinder());
          hostAuthorizations.AddRange((IEnumerable<HostAuthorization>) resultCollection.GetCurrent<HostAuthorization>().Items);
        }
        return (IList<HostAuthorization>) hostAuthorizations;
      }
      finally
      {
        this.TraceLeave(1048701, nameof (GetHostAuthorizations));
      }
    }

    public override HostAuthorization InsertDelegatedHostAuthorization(
      Guid id,
      Guid clientId,
      Guid hostId)
    {
      try
      {
        this.TraceEnter(1048813, nameof (InsertDelegatedHostAuthorization));
        this.PrepareStoredProcedure("prc_InsertDelegatedHostAuthorization");
        this.BindGuid("@id", id);
        this.BindGuid("@registrationId", clientId);
        this.BindGuid("@hostId", hostId);
        using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
        {
          resultCollection.AddBinder<HostAuthorization>((ObjectBinder<HostAuthorization>) new HostAuthorizationBinder());
          return resultCollection.GetCurrent<HostAuthorization>().Items.FirstOrDefault<HostAuthorization>();
        }
      }
      catch (Exception ex)
      {
        this.TraceException(1048814, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1048815, nameof (InsertDelegatedHostAuthorization));
      }
    }
  }
}
