// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Favorites.Server.DataAccess.FavoriteSqlResourceComponent
// Assembly: Microsoft.VisualStudio.Services.Favorites.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EC45EF48-58C1-4A3C-8054-12C79AD0CC5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Favorites.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Favorites.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Favorites.Server.DataAccess
{
  internal class FavoriteSqlResourceComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[2]
    {
      (IComponentCreator) new ComponentCreator<FavoriteSqlResourceComponent>(1),
      (IComponentCreator) new ComponentCreator<FavoriteSqlResourceComponent2>(2)
    }, "Favorite");
    private static readonly SqlMetaData[] typ_ArtifactTypeTable = new SqlMetaData[1]
    {
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 128L)
    };
    private static readonly SqlMetaData[] typ_FavoriteTable = new SqlMetaData[8]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier),
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("ArtifactType", SqlDbType.NVarChar, 128L),
      new SqlMetaData("ArtifactId", SqlDbType.NVarChar, 256L),
      new SqlMetaData("CreationDate", SqlDbType.DateTime),
      new SqlMetaData("ArtifactName", SqlDbType.NVarChar, 256L),
      new SqlMetaData("ArtifactProperties", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ArtifactIsDeleted", SqlDbType.Bit)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        1730001,
        new SqlExceptionFactory(typeof (FavoritesOwnerIdConflictException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new FavoritesOwnerIdConflictException()))
      }
    };

    public FavoriteSqlResourceComponent()
    {
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
      this.ContainerErrorCode = 50000;
    }

    protected virtual SqlParameter BindFavoriteTable(string parameterName, Favorite row) => this.BindFavoriteTable(parameterName, (IEnumerable<Favorite>) new Favorite[1]
    {
      row
    });

    protected virtual SqlParameter BindFavoriteTable(
      string parameterName,
      IEnumerable<Favorite> rows)
    {
      rows = rows ?? Enumerable.Empty<Favorite>();
      System.Func<Favorite, SqlDataRecord> selector = (System.Func<Favorite, SqlDataRecord>) (favorite =>
      {
        ArgumentUtility.CheckForNull<Favorite>(favorite, nameof (favorite));
        favorite.Validate();
        int? nullable1 = this.IdentifyDataspace(favorite.ArtifactScope);
        SqlDataRecord sqlDataRecord1 = new SqlDataRecord(FavoriteSqlResourceComponent.typ_FavoriteTable);
        int num1 = 0;
        SqlDataRecord record1 = sqlDataRecord1;
        int ordinal1 = num1;
        int num2 = ordinal1 + 1;
        Guid? id = favorite.Id;
        record1.SetNullableGuid(ordinal1, id);
        SqlDataRecord record2 = sqlDataRecord1;
        int ordinal2 = num2;
        int num3 = ordinal2 + 1;
        int? nullable2 = nullable1;
        record2.SetNullableInt32(ordinal2, nullable2);
        SqlDataRecord sqlDataRecord2 = sqlDataRecord1;
        int ordinal3 = num3;
        int num4 = ordinal3 + 1;
        string artifactType = favorite.ArtifactType;
        sqlDataRecord2.SetString(ordinal3, artifactType);
        SqlDataRecord sqlDataRecord3 = sqlDataRecord1;
        int ordinal4 = num4;
        int num5 = ordinal4 + 1;
        string str1 = favorite.ArtifactId ?? string.Empty;
        sqlDataRecord3.SetString(ordinal4, str1);
        SqlDataRecord record3 = sqlDataRecord1;
        int ordinal5 = num5;
        int num6 = ordinal5 + 1;
        DateTime? creationDate = favorite.CreationDate;
        record3.SetNullableDateTime(ordinal5, creationDate);
        SqlDataRecord sqlDataRecord4 = sqlDataRecord1;
        int ordinal6 = num6;
        int num7 = ordinal6 + 1;
        string str2 = favorite.ArtifactName ?? string.Empty;
        sqlDataRecord4.SetString(ordinal6, str2);
        bool flag = true;
        if (favorite.ArtifactProperties != null)
        {
          string artifactProperties = favorite.ArtifactProperties.Serialize<ArtifactProperties>();
          if (!string.IsNullOrEmpty(artifactProperties))
          {
            Favorite.VerifyArtifactPropertyString(artifactProperties);
            sqlDataRecord1.SetString(num7++, artifactProperties);
            flag = false;
          }
        }
        if (flag)
          sqlDataRecord1.SetDBNull(num7++);
        SqlDataRecord sqlDataRecord5 = sqlDataRecord1;
        int ordinal7 = num7;
        int num8 = ordinal7 + 1;
        int num9 = favorite.ArtifactIsDeleted ? 1 : 0;
        sqlDataRecord5.SetBoolean(ordinal7, num9 != 0);
        return sqlDataRecord1;
      });
      return this.BindTable(parameterName, "Favorites.typ_FavoriteTable", rows.Select<Favorite, SqlDataRecord>(selector));
    }

    private int? IdentifyDataspace(ArtifactScope artifactScope)
    {
      int dataspaceId;
      if (artifactScope.Type == "Project")
      {
        dataspaceId = this.GetDataspaceId(Guid.Parse(artifactScope.Id), true);
      }
      else
      {
        if (!(artifactScope.Type == "Collection") && !(artifactScope.Type == "Organization"))
          throw new NotSupportedException();
        dataspaceId = this.GetDataspaceId(Guid.Empty);
      }
      return new int?(dataspaceId);
    }

    protected virtual SqlParameter BindArtifactTypeTable(
      string parameterName,
      IEnumerable<string> types)
    {
      types = types ?? Enumerable.Empty<string>();
      System.Func<string, SqlDataRecord> selector = (System.Func<string, SqlDataRecord>) (type =>
      {
        ArgumentUtility.CheckForNull<string>(type, nameof (type));
        SqlDataRecord sqlDataRecord = new SqlDataRecord(FavoriteSqlResourceComponent.typ_ArtifactTypeTable);
        sqlDataRecord.SetString(0, type);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "Favorites.typ_ArtifactTypeTable", types.Select<string, SqlDataRecord>(selector));
    }

    public virtual List<Favorite> CreateUpdateFavorites(
      Guid ownerIdentity,
      bool ownerIsTeam,
      IEnumerable<Favorite> entries)
    {
      if (ownerIsTeam)
        throw new NotSupportedException();
      this.PrepareStoredProcedure("Favorites.prc_CreateUpdateFavorites");
      this.BindGuid("@ownerIdentity", ownerIdentity);
      this.BindFavoriteTable("@favorites", entries);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Favorite>((ObjectBinder<Favorite>) new FavoriteBinder());
      return resultCollection.GetCurrent<Favorite>().Items;
    }

    public virtual void DeleteFavoriteById(Guid ownerIdentity, Guid id)
    {
      this.PrepareStoredProcedure("Favorites.prc_DeleteFavoriteById");
      this.BindGuid("@ownerIdentity", ownerIdentity);
      this.BindGuid("@id", id);
      this.ExecuteNonQuery();
    }

    public virtual List<Favorite> GetFavoritesByOwner(Guid ownerIdentity)
    {
      this.PrepareStoredProcedure("Favorites.prc_GetFavoritesByOwner");
      this.BindGuid("@ownerIdentity", ownerIdentity);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Favorite>((ObjectBinder<Favorite>) new FavoriteBinder());
      return resultCollection.GetCurrent<Favorite>().Items;
    }

    public virtual List<Favorite> GetFavoritesByOwnerScopeType(
      Guid ownerIdentity,
      string storageScopeType,
      string storageScopeId,
      IEnumerable<string> types)
    {
      this.PrepareStoredProcedure("Favorites.prc_GetFavoritesByProjectAndType");
      this.BindGuid("@ownerIdentity", ownerIdentity);
      this.BindArtifactTypeTable("@artifactTypesTable", types);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Favorite>((ObjectBinder<Favorite>) new FavoriteBinder());
      return resultCollection.GetCurrent<Favorite>().Items;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) FavoriteSqlResourceComponent.s_sqlExceptionFactories;
  }
}
