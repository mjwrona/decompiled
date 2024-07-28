// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityIdTranslationComponent
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
  public class IdentityIdTranslationComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[3]
    {
      (IComponentCreator) new ComponentCreator<IdentityIdTranslationComponent>(1),
      (IComponentCreator) new ComponentCreator<IdentityIdTranslationComponent>(2),
      (IComponentCreator) new ComponentCreator<IdentityIdTranslationComponent>(3)
    }, "IdentityIdTranslation");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      [400101] = new SqlExceptionFactory(typeof (InvalidIdentityIdTranslationException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new InvalidIdentityIdTranslationException(sqEr.ToString(), (Exception) sqEx))),
      [400102] = new SqlExceptionFactory(typeof (IdTranslationsAreMigratedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new IdTranslationsAreMigratedException(sqEr.ToString(), (Exception) sqEx)))
    };

    public IdentityIdTranslationComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    protected override void SetupComponentCircuitBreaker(
      IVssRequestContext requestContext,
      string databaseName,
      ApplicationIntent applicationIntent)
    {
      string key = "IdentityIdTranslationComponent-" + databaseName;
      this.ComponentLevelCommandSetter.AndCommandKey((Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key);
      this.ComponentLevelCircuitBreakerProperties = (ICommandProperties) new CommandPropertiesRegistry(requestContext, (Microsoft.VisualStudio.Services.CircuitBreaker.CommandKey) key, this.ComponentLevelCommandSetter.CommandPropertiesDefaults);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) IdentityIdTranslationComponent.s_sqlExceptionFactories;

    internal virtual IList<IdentityIdTranslation> GetTranslations()
    {
      this.PrepareStoredProcedure("prc_GetIdentityIdTranslations");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<IdentityIdTranslation>((ObjectBinder<IdentityIdTranslation>) new IdentityIdTranslationBinder());
        return (IList<IdentityIdTranslation>) resultCollection.GetCurrent<IdentityIdTranslation>().Items;
      }
    }

    internal virtual Guid? GetTranslationById(Guid id)
    {
      this.PrepareStoredProcedure("prc_GetIdentityIdTranslationById");
      this.BindGuid("@id", id);
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
      {
        Guid guid = new IdentityIdTranslationComponent.IdentityMasterIdColumnBinder().Bind(reader);
        if (guid != Guid.Empty)
          return new Guid?(guid);
      }
      return new Guid?();
    }

    internal virtual Guid? GetTranslationByMasterId(Guid masterId)
    {
      this.PrepareStoredProcedure("prc_GetIdentityIdTranslationByMasterId");
      this.BindGuid("@masterId", masterId);
      SqlDataReader reader = this.ExecuteReader();
      if (reader.Read())
      {
        Guid guid = new IdentityIdTranslationComponent.IdentityIdColumnBinder().Bind(reader);
        if (guid != Guid.Empty)
          return new Guid?(guid);
      }
      return new Guid?();
    }

    internal virtual void UpdateTranslations(IList<IdentityIdTranslation> translations)
    {
      this.PrepareStoredProcedure("prc_UpdateIdentityIdTranslations");
      this.BindIdentityIdTranslationTable("@translations", (IEnumerable<IdentityIdTranslation>) translations);
      this.ExecuteNonQuery();
    }

    protected class IdentityMasterIdColumnBinder
    {
      private SqlColumnBinder idColumn = new SqlColumnBinder("MasterId");

      internal Guid Bind(SqlDataReader reader) => this.idColumn.GetGuid((IDataReader) reader, true);
    }

    protected class IdentityIdColumnBinder
    {
      private SqlColumnBinder idColumn = new SqlColumnBinder("Id");

      internal Guid Bind(SqlDataReader reader) => this.idColumn.GetGuid((IDataReader) reader, true);
    }
  }
}
