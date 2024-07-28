// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.AuthorizationComponent
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class AuthorizationComponent : IntegrationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<AuthorizationComponent>(1, true),
      (IComponentCreator) new ComponentCreator<AuthorizationComponent2>(2)
    }, "Authorization");

    public virtual ResultCollection SecurityGetChangedAcls(int startSequenceId)
    {
      try
      {
        this.PrepareStoredProcedure("prc_AuthorizationGetChangedACEs");
        this.BindInt("@startSequenceId", startSequenceId);
        ResultCollection changedAcls = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_AuthorizationGetChangedACEs", this.RequestContext);
        changedAcls.AddBinder<int>((ObjectBinder<int>) new ChangeIdColumn());
        changedAcls.AddBinder<SecurityChange>((ObjectBinder<SecurityChange>) new ChangedAclsColumns());
        return changedAcls;
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public ResultCollection QueryAuthorizationObjects(
      string specificObjectId,
      string specificSecurityToken)
    {
      this.PrepareStoredProcedure("prc_AuthorizationQueryData");
      this.BindString("@objectId", specificObjectId, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      this.BindString("@securityToken", specificSecurityToken, 1024, BindStringBehavior.EmptyStringToNull, SqlDbType.NVarChar);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_AuthorizationQueryData", this.RequestContext);
      resultCollection.AddBinder<SecurityObject>((ObjectBinder<SecurityObject>) new SecurityObjectColumns());
      return resultCollection;
    }

    public virtual ResultCollection RegisterObject(
      string objectId,
      string classId,
      Guid projectId,
      string parentObjectId,
      Guid userId)
    {
      this.PrepareStoredProcedure("prc_AuthorizationRegisterObject");
      this.BindString("@objectId", objectId, 1024, false, SqlDbType.NVarChar);
      this.BindString("@classId", classId, 64, false, SqlDbType.VarChar);
      this.BindGuid("@projectId", projectId);
      this.BindString("@parentObjectId", parentObjectId, 1024, true, SqlDbType.NVarChar);
      this.BindGuid("@userId", userId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_AuthorizationRegisterObject", this.RequestContext);
      resultCollection.AddBinder<string>((ObjectBinder<string>) new SecurityTokenColumns());
      return resultCollection;
    }

    public virtual void UnregisterObject(string objectId, string securityToken)
    {
      this.PrepareStoredProcedure("prc_AuthorizationUnregisterObject");
      this.BindString("@objectId", objectId, 1024, false, SqlDbType.NVarChar);
      this.BindString("@securityToken", securityToken, 256, false, SqlDbType.VarChar);
      this.ExecuteNonQuery();
    }

    public virtual ResultCollection ResetInheritance(
      string objectId,
      string parentObjectId,
      Guid namespaceGuid)
    {
      this.PrepareStoredProcedure("prc_AuthorizationResetInheritance");
      this.BindString("@objectId", objectId, 1024, false, SqlDbType.NVarChar);
      this.BindString("@parentObjectId", parentObjectId, 1024, false, SqlDbType.NVarChar);
      this.BindBoolean("@returnAffectedObjects", true);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_AuthorizationResetInheritance", this.RequestContext);
      resultCollection.AddBinder<SecurityObject>((ObjectBinder<SecurityObject>) new SecurityObjectColumns());
      return resultCollection;
    }
  }
}
