// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityMapComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityMapComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      [400039] = new SqlExceptionFactory(typeof (IdentityMapReadOnlyException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new IdentityMapReadOnlyException()))
    };
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<IdentityMapComponent>(1, true),
      (IComponentCreator) new ComponentCreator<IdentityMapComponent2>(2),
      (IComponentCreator) new ComponentCreator<IdentityMapComponent3>(3),
      (IComponentCreator) new ComponentCreator<IdentityMapComponent4>(4),
      (IComponentCreator) new ComponentCreator<IdentityMapComponent5>(5),
      (IComponentCreator) new ComponentCreator<IdentityMapComponent6>(6)
    }, "IdentityMap");

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) IdentityMapComponent.s_sqlExceptionFactories;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "IdentityMapComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    internal virtual ResultCollection ReadMappings()
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return (ResultCollection) null;
      this.PrepareStoredProcedure("prc_ReadIdentityMappings");
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<IdentityIdMapping>((ObjectBinder<IdentityIdMapping>) new IdentityIdMappingColumns());
      return resultCollection;
    }

    internal virtual Guid MapIdentity(Guid masterId)
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return masterId;
      this.PrepareStoredProcedure("prc_MapIdentity");
      this.BindGuid("@masterId", masterId);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new IdentityMapComponent.IdentityLocalIdColumns().Bind(reader) : throw new UnexpectedDatabaseResultException(this.ProcedureName);
    }

    internal virtual Guid ReadMapping(Guid localId)
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return localId;
      this.PrepareStoredProcedure("prc_ReadIdentityMapping");
      this.BindGuid("@localId", localId);
      SqlDataReader reader = this.ExecuteReader();
      return !reader.Read() ? Guid.Empty : new IdentityMapComponent.IdentityMasterIdColumns().Bind(reader);
    }

    internal virtual void UpdateIdentityMappings2(IEnumerable<KeyValuePair<Guid, Guid>> mappings) => throw new NotImplementedException();

    internal void LockIdentityMap()
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.PrepareStoredProcedure("prc_LockIdentityMap", false);
      this.ExecuteNonQuery();
    }

    internal void UnlockIdentityMap()
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.PrepareStoredProcedure("prc_UnlockIdentityMap", false);
      this.ExecuteNonQuery();
    }

    internal void CommitSnapshot()
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.PrepareStoredProcedure("prc_CommitIdentitySnapshot", true);
      this.ExecuteNonQuery();
    }

    internal void DeleteIdentityMap()
    {
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.PrepareStoredProcedure("prc_DeleteIdentityMap", true);
      this.ExecuteNonQuery();
    }

    internal virtual bool IdentityMapLocked() => false;

    protected class IdentityLocalIdColumns
    {
      private SqlColumnBinder idColumn = new SqlColumnBinder("localId");

      internal Guid Bind(SqlDataReader reader) => this.idColumn.GetGuid((IDataReader) reader);
    }

    protected class IdentityMasterIdColumns
    {
      private SqlColumnBinder idColumn = new SqlColumnBinder("masterId");

      internal Guid Bind(SqlDataReader reader) => this.idColumn.GetGuid((IDataReader) reader, true);
    }
  }
}
