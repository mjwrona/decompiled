// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StrongBoxComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class StrongBoxComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[20]
    {
      (IComponentCreator) new ComponentCreator<StrongBoxComponent>(1, true),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent2>(2),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent2>(3),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent2>(4),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent2>(5),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent6>(6),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent7>(7),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent8>(8),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent9>(9),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent10>(10),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent11>(11),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent12>(12),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent13>(13),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent14>(14),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent15>(15),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent16>(16),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent17>(17),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent18>(18),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent19>(19),
      (IComponentCreator) new ComponentCreator<StrongBoxComponent20>(20)
    }, "StrongBox");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>()
    {
      {
        800096,
        new SqlExceptionFactory(typeof (StrongBoxDrawerExistsException))
      },
      {
        800115,
        new SqlExceptionFactory(typeof (StrongBoxSigningKeyRotatedException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, errNum, sqlException, sqlErr) => (Exception) new StrongBoxSigningKeyRotatedException()))
      },
      {
        800116,
        new SqlExceptionFactory(typeof (StrongBoxDrawerNotFoundException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, errNum, sqlException, sqlErr) => (Exception) new StrongBoxDrawerNotFoundException(Guid.Parse(sqlErr.ExtractString("id")))))
      }
    };

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) StrongBoxComponent.s_sqlExceptionFactories;

    public bool DrawerExists(string name) => this.GetDrawerInfo(name) != null;

    public virtual void CreateDrawer(string drawerName, Guid drawerId, Guid signingKeyId)
    {
      this.PrepareStoredProcedure("prc_CreateDrawer");
      this.BindString("@name", drawerName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      this.BindGuid("@drawerId", drawerId);
      this.ExecuteNonQuery();
    }

    [Obsolete("Use GetDrawerInfo instead.")]
    public virtual Guid GetDrawerId(string drawerName)
    {
      StrongBoxDrawerInfo drawerInfo = this.GetDrawerInfo(drawerName);
      return drawerInfo != null ? drawerInfo.DrawerId : Guid.Empty;
    }

    public virtual void AddStrongBoxItem(Guid drawerId, StrongBoxItemInfo item)
    {
      this.PrepareStoredProcedure("prc_AddStrongBoxItem");
      this.BindGuid("@drawerId", item.DrawerId);
      this.BindString("@lookupKey", item.LookupKey, 512, false, SqlDbType.NVarChar);
      this.BindByte("@itemKind", (byte) item.ItemKind);
      if (this.Version >= 3)
      {
        this.BindNullableDateTime("@expirationDate", item.ExpirationDate);
        this.BindString("@credentialName", item.CredentialName, 256, BindStringBehavior.Unchanged, SqlDbType.NVarChar);
        this.BindGuid("@signingKeyId", item.SigningKeyId);
      }
      if (this.Version >= 5)
      {
        if (item.EncryptedContent == null)
          this.BindNullValue("@encryptedContent", SqlDbType.VarBinary);
        else
          this.BindBinary("@encryptedContent", item.EncryptedContent, SqlDbType.VarBinary);
      }
      else
      {
        this.BindInt("@fileId", item.FileId);
        this.BindString("@value", item.Value, 512, true, SqlDbType.NVarChar);
      }
      this.ExecuteNonQuery();
    }

    public virtual void AddStrongBoxItems(Guid drawerId, IEnumerable<StrongBoxItemInfo> items)
    {
      foreach (StrongBoxItemInfo strongBoxItemInfo in items)
        this.AddStrongBoxItem(drawerId, strongBoxItemInfo);
    }

    public virtual List<StrongBoxItemInfo> ReadContents(Guid drawerId)
    {
      this.PrepareStoredProcedure("prc_ReadStrongBoxContents");
      this.BindGuid("@drawerId", drawerId);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        return resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
      }
    }

    public virtual List<StrongBoxItemInfo> ReadContentsWithPartialLookupKey(
      Guid drawerId,
      string partialLookupKey)
    {
      List<StrongBoxItemInfo> source = this.ReadContents(drawerId);
      return source.Count > 0 ? source.Where<StrongBoxItemInfo>((System.Func<StrongBoxItemInfo, bool>) (x => x.LookupKey.IndexOf(partialLookupKey, StringComparison.OrdinalIgnoreCase) >= 0)).ToList<StrongBoxItemInfo>() : source;
    }

    public virtual bool IsDrawerEmpty(Guid drawerId)
    {
      string sqlStatement = "SELECT CASE WHEN EXISTS(SELECT TOP (1) 1 FROM tbl_StrongBoxItem WHERE PartitionId = @partitionId AND DrawerId = @drawerId) THEN 1 ELSE 0 END";
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindGuid("@drawerId", drawerId);
      return (int) this.ExecuteScalar() == 0;
    }

    public virtual StrongBoxItemInfo GetItemInfo(Guid drawerId, string key)
    {
      this.PrepareStoredProcedure("prc_GetStrongBoxItemInfo");
      this.BindGuid("@drawerId", drawerId);
      this.BindString("@lookupKey", key, 512, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        List<StrongBoxItemInfo> items = resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
        return items != null && items.Count > 0 ? items[0] : (StrongBoxItemInfo) null;
      }
    }

    public virtual void RemoveItem(Guid drawerId, string key)
    {
      this.PrepareStoredProcedure("prc_DeleteStrongBoxItem");
      this.BindGuid("@drawerId", drawerId);
      this.BindString("@lookupKey", key, 512, false, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
    }

    public virtual bool ReplaceItem(
      StrongBoxItemInfo item,
      byte[] prevEncryptedContent,
      int prevFileId)
    {
      throw new ServiceVersionNotSupportedException(nameof (StrongBoxComponent), this.Version, 11);
    }

    public virtual void DeleteDrawerAndContents(Guid drawerId)
    {
      this.PrepareStoredProcedure("prc_DeleteStrongBoxDrawer");
      this.BindGuid("@drawerId", drawerId);
      this.ExecuteNonQuery();
    }

    public virtual List<string> QueryDrawerNames() => throw new ServiceVersionNotSupportedException(nameof (StrongBoxComponent), this.Version, 7);

    public virtual List<string> QueryDrawerNamesByType(SigningKeyType keyTypeFilter) => throw new ServiceVersionNotSupportedException(nameof (StrongBoxComponent), this.Version, 12);

    internal virtual ICollection<StrongBoxItemInfo> QueryItemsToReencrypt(Guid? signingKeyId)
    {
      if (this.Version < 3)
        return (ICollection<StrongBoxItemInfo>) new List<StrongBoxItemInfo>();
      if (!signingKeyId.HasValue)
        throw new ServiceVersionNotSupportedException(nameof (StrongBoxComponent), this.Version, 14);
      this.PrepareStoredProcedure("prc_QueryStrongBoxItemsToReencrypt");
      this.BindGuid("@signingKeyId", signingKeyId.Value);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        return (ICollection<StrongBoxItemInfo>) resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
      }
    }

    internal virtual ICollection<StrongBoxItemInfo> QueryItemsBatchToReencrypt(
      int batchSize = -1,
      int errorOffset = 0)
    {
      throw new NotImplementedException();
    }

    internal virtual ICollection<StrongBoxItemInfo> QueryItemsToReencryptMultisets(
      int batchSize,
      StrongBoxItemInfo lastAttemptedItem)
    {
      throw new NotImplementedException();
    }

    internal IList<StrongBoxItemInfo> QueryLegacyItems()
    {
      if (this.Version < 5)
        return (IList<StrongBoxItemInfo>) new List<StrongBoxItemInfo>();
      this.PrepareStoredProcedure("prc_QueryLegacyStrongBoxItems");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<StrongBoxItemInfo>((ObjectBinder<StrongBoxItemInfo>) new ItemBinder());
        return (IList<StrongBoxItemInfo>) resultCollection.GetCurrent<StrongBoxItemInfo>().Items;
      }
    }

    internal virtual IList<Guid> QueryStrongBoxSigningKeys()
    {
      if (this.Version < 4)
        return (IList<Guid>) Array.Empty<Guid>();
      this.PrepareStoredProcedure("prc_QueryStrongBoxSigningKeys");
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        SqlColumnBinder keyIdColumn = new SqlColumnBinder("SigningKeyId");
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new SimpleObjectBinder<Guid>((System.Func<IDataReader, Guid>) (reader => keyIdColumn.GetGuid(reader))));
        return (IList<Guid>) resultCollection.GetCurrent<Guid>().Items;
      }
    }

    internal virtual StrongBoxDrawerInfo GetDrawerInfo(string drawerName)
    {
      this.PrepareStoredProcedure("prc_GetDrawerId");
      this.BindString("@name", drawerName, (int) byte.MaxValue, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<Guid>((ObjectBinder<Guid>) new DrawerIdColumns());
        List<Guid> items = resultCollection.GetCurrent<Guid>().Items;
        return items != null && items.Count > 0 ? new StrongBoxDrawerInfo(drawerName, items[0], Guid.Empty, DateTime.MinValue) : (StrongBoxDrawerInfo) null;
      }
    }

    internal virtual StrongBoxDrawerInfo GetDrawerInfo(Guid drawerId) => new StrongBoxDrawerInfo("$Unknown", drawerId, Guid.Empty, DateTime.MinValue);

    internal virtual void UpdateDrawerSigningKey(Guid drawerId, Guid signingKey) => throw new NotImplementedException();

    internal virtual IList<Guid> QueryStrongBoxSigningKeysExcludingKeyType(
      SigningKeyType signingKeyType)
    {
      throw new ServiceVersionNotSupportedException(nameof (StrongBoxComponent), this.Version, 13);
    }

    public virtual void ReencryptMultipleItems(
      IEnumerable<TeamFoundationStrongBoxServiceBase.ReencryptionData> items)
    {
      throw new NotImplementedException();
    }

    public virtual void DeleteStrongBoxOrphans(int batchSize) => throw new NotImplementedException();
  }
}
