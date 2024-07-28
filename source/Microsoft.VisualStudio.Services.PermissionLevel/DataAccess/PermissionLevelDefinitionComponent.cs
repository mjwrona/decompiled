// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PermissionLevel.DataAccess.PermissionLevelDefinitionComponent
// Assembly: Microsoft.VisualStudio.Services.PermissionLevel, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 43771064-3FEF-4CA1-8A8B-671AEDB99122
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PermissionLevel.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.PermissionLevel.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.PermissionLevel.DataAccess
{
  public class PermissionLevelDefinitionComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[1]
    {
      (IComponentCreator) new ComponentCreator<PermissionLevelDefinitionComponent>(1)
    }, "PermissionLevel");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        3890001,
        new SqlExceptionFactory(typeof (PermissionLevelDefinitionAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelDefinitionAlreadyExistsException(Resources.PermissionLevelDefinitionAlreadyExists((object) "Id", (object) sqEr.ExtractString("Id")))))
      },
      {
        3890002,
        new SqlExceptionFactory(typeof (PermissionLevelDefinitionAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelDefinitionAlreadyExistsException(Resources.PermissionLevelDefinitionAlreadyExists((object) "Name", (object) sqEr.ExtractString("Name")))))
      },
      {
        3890003,
        new SqlExceptionFactory(typeof (PermissionLevelDefinitionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelDefinitionNotFoundException(Resources.PermissionLevelDefinitionNotFound((object) "Id", (object) sqEr.ExtractString("Id")))))
      },
      {
        3890004,
        new SqlExceptionFactory(typeof (PermissionLevelDefinitionNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new PermissionLevelDefinitionNotFoundException(Resources.PermissionLevelDefinitionNotFound((object) "Scope", (object) sqEr.ExtractString("Id")))))
      }
    };
    private const string c_area = "PermissionLevel";
    private const string c_layer = "PermissionLevelDefinitionComponent";
    private const string c_serviceName = "PermissionLevel";
    private const int c_maxNameLength = 256;
    private const int c_maxDescriptionLength = 1024;

    public PermissionLevelDefinitionComponent() => this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;

    public virtual PermissionLevelDefinition CreatePermissionLevelDefinition(
      Guid id,
      string name,
      string description,
      PermissionLevelDefinitionType type,
      PermissionLevelDefinitionScope scope,
      DateTime creationDate)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      PermissionLevelDefinitionComponent.ValidatePermissionLevelDefinitionName(name);
      PermissionLevelDefinitionComponent.ValidatePermissionLevelDefinitionType(type);
      this.PrepareStoredProcedure("PermissionLevel.prc_CreatePermissionLevelDefinition");
      this.BindGuid("@id", id);
      this.BindString("@name", name, 256, false, SqlDbType.NVarChar);
      this.BindString("@description", description, 1024, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
      this.BindInt("@scopeId", (int) scope);
      this.BindInt("@type", (int) type);
      this.BindDateTime("@creationDate", creationDate);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelDefinition>(this.CreatePermissionLevelDefinitionColumns());
        return resultCollection.GetCurrent<PermissionLevelDefinition>().FirstOrDefault<PermissionLevelDefinition>();
      }
    }

    public virtual IList<PermissionLevelDefinition> QueryPermissionLevelDefinitionsByIds(
      IEnumerable<Guid> definitionIds)
    {
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelDefinitionsByIds");
      this.BindSortedGuidTable("@ids", definitionIds);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelDefinition>(this.CreatePermissionLevelDefinitionColumns());
        return (IList<PermissionLevelDefinition>) resultCollection.GetCurrent<PermissionLevelDefinition>().ToList<PermissionLevelDefinition>();
      }
    }

    public virtual IList<PermissionLevelDefinition> QueryPermissionLevelDefinitionsByScope(
      PermissionLevelDefinitionScope scope,
      PermissionLevelDefinitionType type)
    {
      this.PrepareStoredProcedure("PermissionLevel.prc_QueryPermissionLevelDefinitionsByScope");
      this.BindInt("@scopeId", (int) scope);
      this.BindInt("@type", (int) type);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelDefinition>(this.CreatePermissionLevelDefinitionColumns());
        return (IList<PermissionLevelDefinition>) resultCollection.GetCurrent<PermissionLevelDefinition>().ToList<PermissionLevelDefinition>();
      }
    }

    public virtual PermissionLevelDefinition UpdatePermissionLevelDefinitionName(
      Guid id,
      string newName)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      PermissionLevelDefinitionComponent.ValidatePermissionLevelDefinitionName(newName);
      this.PrepareStoredProcedure("PermissionLevel.prc_UpdatePermissionLevelDefinition");
      this.BindGuid("@id", id);
      this.BindString("@newName", newName, 256, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindBoolean("@setName", true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelDefinition>(this.CreatePermissionLevelDefinitionColumns());
        return resultCollection.GetCurrent<PermissionLevelDefinition>().FirstOrDefault<PermissionLevelDefinition>();
      }
    }

    public virtual PermissionLevelDefinition UpdatePermissionLevelDefinitionDescription(
      Guid id,
      string newDescription)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      this.PrepareStoredProcedure("PermissionLevel.prc_UpdatePermissionLevelDefinition");
      this.BindGuid("@id", id);
      this.BindString("@newDescription", newDescription, 1024, BindStringBehavior.Unchanged, SqlDbType.VarChar);
      this.BindBoolean("@setDescription", true);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<PermissionLevelDefinition>(this.CreatePermissionLevelDefinitionColumns());
        return resultCollection.GetCurrent<PermissionLevelDefinition>().FirstOrDefault<PermissionLevelDefinition>();
      }
    }

    public virtual void DeletePermissionLevelDefinition(Guid id)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      this.PrepareStoredProcedure("PermissionLevel.prc_DeletePermissionLevelDefinition");
      this.BindGuid("@id", id);
      this.ExecuteNonQuery();
    }

    public virtual void RestorePermissionLevelDefinition(Guid id)
    {
      ArgumentUtility.CheckForEmptyGuid(id, nameof (id));
      this.PrepareStoredProcedure("PermissionLevel.prc_RestorePermissionLevelDefinition");
      this.BindGuid("@id", id);
      this.ExecuteNonQuery();
    }

    protected virtual ObjectBinder<PermissionLevelDefinition> CreatePermissionLevelDefinitionColumns() => (ObjectBinder<PermissionLevelDefinition>) new PermissionLevelDefinitionBinder();

    private static void ValidatePermissionLevelDefinitionName(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckForOutOfRange(name.Length, nameof (name), 1, 256);
    }

    private static void ValidatePermissionLevelDefinitionType(PermissionLevelDefinitionType type)
    {
      if ((PermissionLevelDefinitionType.All & type) != type)
        throw new ArgumentException(string.Format("Invalid input value for the PermissionLevelDefinition Enum flag: {0}", (object) type));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PermissionLevelDefinitionComponent.s_sqlExceptionFactories;
  }
}
