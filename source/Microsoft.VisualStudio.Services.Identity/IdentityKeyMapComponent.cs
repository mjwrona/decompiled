// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityKeyMapComponent
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Graph;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityKeyMapComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<IdentityKeyMapComponent>(1)
    }, "IdentityKeyMap");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      [400103] = new SqlExceptionFactory(typeof (InvalidIdentityKeyMapException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidIdentityKeyMapException(sqEr.ToString(), (Exception) sqEx))),
      [400104] = new SqlExceptionFactory(typeof (InvalidTypeIdForIdentityKeyMapException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidTypeIdForIdentityKeyMapException(sqEr.ToString(), (Exception) sqEx)))
    };
    private const string ComponentName = "IdentityKeyMap";

    public IdentityKeyMapComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "IdentityKeyMapComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) IdentityKeyMapComponent.s_sqlExceptionFactories;

    internal IdentityKeyMap GetIdentityKeyMapByCuid(Guid cuid)
    {
      this.PrepareStoredProcedure("prc_GetIdentityKeyMapByCuid");
      this.BindGuid("@cuid", cuid);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityKeyMap>((ObjectBinder<IdentityKeyMap>) new IdentityKeyMapComponent.IdentityKeyMapBinder());
        return resultCollection.GetCurrent<IdentityKeyMap>().Items.SingleOrDefault<IdentityKeyMap>();
      }
    }

    internal IdentityKeyMap GetIdentityKeyMapByStorageKey(Guid storageKey)
    {
      this.PrepareStoredProcedure("prc_GetIdentityKeyMapByStorageKey");
      this.BindGuid("@storageKey", storageKey);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityKeyMap>((ObjectBinder<IdentityKeyMap>) new IdentityKeyMapComponent.IdentityKeyMapBinder());
        return resultCollection.GetCurrent<IdentityKeyMap>().Items.SingleOrDefault<IdentityKeyMap>();
      }
    }

    public virtual IDictionary<Guid, IdentityKeyMap> GetIdentityKeyMapsByStorageKeys(
      IEnumerable<Guid> storageKeys)
    {
      List<Guid> list = storageKeys.Distinct<Guid>().ToList<Guid>();
      if (list.Count == 1)
      {
        Guid storageKey = list.Single<Guid>();
        IdentityKeyMap keyMapByStorageKey = this.GetIdentityKeyMapByStorageKey(storageKey);
        return (IDictionary<Guid, IdentityKeyMap>) new Dictionary<Guid, IdentityKeyMap>()
        {
          {
            storageKey,
            keyMapByStorageKey
          }
        };
      }
      this.PrepareStoredProcedure("prc_GetIdentityKeyMapsByStorageKeys");
      this.BindGuidTable("@storageKeys", (IEnumerable<Guid>) list);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityKeyMap>((ObjectBinder<IdentityKeyMap>) new IdentityKeyMapComponent.IdentityKeyMapBinder());
        return (IDictionary<Guid, IdentityKeyMap>) resultCollection.GetCurrent<IdentityKeyMap>().Items.ToDictionary<IdentityKeyMap, Guid, IdentityKeyMap>((System.Func<IdentityKeyMap, Guid>) (result => result.StorageKey), (System.Func<IdentityKeyMap, IdentityKeyMap>) (result => result));
      }
    }

    public virtual IDictionary<Guid, IdentityKeyMap> GetIdentityKeyMapsByCuids(
      IEnumerable<Guid> cuids)
    {
      List<Guid> list = cuids.Distinct<Guid>().ToList<Guid>();
      if (list.Count == 1)
      {
        Guid cuid = list.Single<Guid>();
        IdentityKeyMap identityKeyMapByCuid = this.GetIdentityKeyMapByCuid(cuid);
        return (IDictionary<Guid, IdentityKeyMap>) new Dictionary<Guid, IdentityKeyMap>()
        {
          {
            cuid,
            identityKeyMapByCuid
          }
        };
      }
      this.PrepareStoredProcedure("prc_GetIdentityKeyMapsByCuids");
      this.BindGuidTable("@cuids", (IEnumerable<Guid>) list);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityKeyMap>((ObjectBinder<IdentityKeyMap>) new IdentityKeyMapComponent.IdentityKeyMapBinder());
        return (IDictionary<Guid, IdentityKeyMap>) resultCollection.GetCurrent<IdentityKeyMap>().Items.ToDictionary<IdentityKeyMap, Guid, IdentityKeyMap>((System.Func<IdentityKeyMap, Guid>) (result => result.Cuid), (System.Func<IdentityKeyMap, IdentityKeyMap>) (result => result));
      }
    }

    internal virtual void UpdateIdentityKeyMaps(IList<IdentityKeyMap> keyMaps)
    {
      this.PrepareStoredProcedure("prc_UpdateIdentityKeyMaps");
      this.BindIdentityKeyMapTable("@keyMaps", (IEnumerable<IdentityKeyMap>) keyMaps);
      this.ExecuteNonQuery();
    }

    internal virtual void InsertIdentityKeyMaps(IList<IdentityKeyMap> keyMaps)
    {
      this.PrepareStoredProcedure("prc_InsertIdentityKeyMaps");
      this.BindIdentityKeyMapTable("@keyMaps", (IEnumerable<IdentityKeyMap>) keyMaps);
      this.ExecuteNonQuery();
    }

    internal IEnumerable<IdentityKeyMap> AssignIdentityKeyMaps(IList<IdentityKeyMap> keyMaps)
    {
      this.PrepareStoredProcedure("prc_AssignIdentityKeyMaps");
      this.BindIdentityKeyMapTable("@keyMaps", (IEnumerable<IdentityKeyMap>) keyMaps);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityKeyMap>((ObjectBinder<IdentityKeyMap>) new IdentityKeyMapComponent.IdentityKeyMapBinder());
        List<IdentityKeyMap> items = resultCollection.GetCurrent<IdentityKeyMap>().Items;
        if (keyMaps.Count != items.Count)
          throw new UnexpectedDatabaseResultException(this.ProcedureName);
        return (IEnumerable<IdentityKeyMap>) items;
      }
    }

    protected class IdentityKeyMapBinder : ObjectBinder<IdentityKeyMap>
    {
      private SqlColumnBinder StorageKeyColumn = new SqlColumnBinder("StorageKey");
      private SqlColumnBinder CuidColumn = new SqlColumnBinder("Cuid");
      private SqlColumnBinder TypeIdColumn = new SqlColumnBinder("TypeId");

      protected override IdentityKeyMap Bind()
      {
        Guid guid1 = this.StorageKeyColumn.GetGuid((IDataReader) this.Reader);
        Guid guid2 = this.CuidColumn.GetGuid((IDataReader) this.Reader);
        byte typeId = this.TypeIdColumn.GetByte((IDataReader) this.Reader);
        return new IdentityKeyMap()
        {
          StorageKey = guid1,
          Cuid = guid2,
          SubjectType = SubjectTypeMapper.Instance.GetTypeNameFromId(typeId)
        };
      }
    }
  }
}
