// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  public class NameResolutionComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[6]
    {
      (IComponentCreator) new ComponentCreator<NameResolutionComponent>(1),
      (IComponentCreator) new ComponentCreator<NameResolutionComponent2>(2),
      (IComponentCreator) new ComponentCreator<NameResolutionComponent3>(3),
      (IComponentCreator) new ComponentCreator<NameResolutionComponent4>(4),
      (IComponentCreator) new ComponentCreator<NameResolutionComponent5>(5),
      (IComponentCreator) new ComponentCreator<NameResolutionComponent6>(6)
    }, "NameResolution");
    private static readonly SqlMetaData[] typ_NameResolutionTable2 = new SqlMetaData[7]
    {
      new SqlMetaData("Namespace", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Value", SqlDbType.UniqueIdentifier),
      new SqlMetaData("IsPrimary", SqlDbType.Bit),
      new SqlMetaData("IsEnabled", SqlDbType.Bit),
      new SqlMetaData("TTL", SqlDbType.Int),
      new SqlMetaData("Revision", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_NameResolutionQueryTable = new SqlMetaData[3]
    {
      new SqlMetaData("Namespace", SqlDbType.NVarChar, 256L),
      new SqlMetaData("Name", SqlDbType.NVarChar, 256L),
      new SqlMetaData("OrderId", SqlDbType.Int)
    };
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static NameResolutionComponent()
    {
      NameResolutionComponent.s_sqlExceptionFactories.Add(1110003, new SqlExceptionFactory(typeof (MultiplePrimaryNameResolutionEntriesException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new MultiplePrimaryNameResolutionEntriesException(sqEr.ExtractString("Value"), 0))));
      NameResolutionComponent.s_sqlExceptionFactories.Add(1110004, new SqlExceptionFactory(typeof (NameResolutionEntryAlreadyExistsException), (Func<IVssRequestContext, int, SqlException, SqlError, Exception>) ((rc, en, sqEx, sqEr) => (Exception) new NameResolutionEntryAlreadyExistsException(sqEr.ExtractString("Name"), sqEr.ExtractString("Value"), sqEr.ExtractString("ConflictingValue")))));
    }

    protected SqlParameter BindNameResolutionTable(
      string parameterName,
      IEnumerable<NameResolutionEntry> rows)
    {
      rows = rows ?? Enumerable.Empty<NameResolutionEntry>();
      return this.BindTable(parameterName, "typ_NameResolutionTable2", NameResolutionComponent.BindNameResolutionRow(rows));
    }

    internal static IEnumerable<SqlDataRecord> BindNameResolutionRow(
      IEnumerable<NameResolutionEntry> rows)
    {
      if (rows != null)
      {
        foreach (NameResolutionEntry row in rows)
        {
          SqlDataRecord record = new SqlDataRecord(NameResolutionComponent.typ_NameResolutionTable2);
          record.SetString(0, row.Namespace);
          record.SetString(1, row.Name);
          record.SetGuid(2, row.Value);
          record.SetBoolean(3, row.IsPrimary);
          record.SetBoolean(4, row.IsEnabled);
          record.SetNullableInt32(5, row.TTL);
          record.SetInt32(6, row.Revision);
          yield return record;
        }
      }
    }

    protected SqlParameter BindNameResolutionQueryTable(
      string parameterName,
      IEnumerable<NameResolutionQuery> rows,
      bool omitNullEntries = false)
    {
      rows = rows ?? Enumerable.Empty<NameResolutionQuery>();
      Func<NameResolutionQuery, int, IEnumerable<SqlDataRecord>> selector = (Func<NameResolutionQuery, int, IEnumerable<SqlDataRecord>>) ((row, index) =>
      {
        if (omitNullEntries && row == null)
          return Enumerable.Empty<SqlDataRecord>();
        SqlDataRecord sqlDataRecord = new SqlDataRecord(NameResolutionComponent.typ_NameResolutionQueryTable);
        sqlDataRecord.SetString(0, row.Namespace);
        sqlDataRecord.SetString(1, row.Name);
        sqlDataRecord.SetInt32(2, index);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          sqlDataRecord
        };
      });
      return this.BindTable(parameterName, "typ_NameResolutionQueryTable", rows.SelectMany<NameResolutionQuery, SqlDataRecord>(selector));
    }

    public virtual void SetResolutionEntries(
      IEnumerable<NameResolutionEntry> entries,
      bool overwriteExisting)
    {
      this.PrepareStoredProcedure("prc_SetNameResolutionEntries", ReplicationType.Synchronous);
      this.BindNameResolutionTable("@entries", (IEnumerable<NameResolutionEntry>) entries.Select<NameResolutionEntry, NameResolutionEntry>(new System.Func<NameResolutionEntry, NameResolutionEntry>(this.TranslateEntryToDatabase)).ToArray<NameResolutionEntry>());
      this.BindBoolean("@overwriteExisting", overwriteExisting);
      this.ExecuteNonQuery();
    }

    public virtual List<NameResolutionEntry> RemoveResolutionEntries(Guid value, string @namespace = null)
    {
      this.PrepareStoredProcedure("prc_RemoveNameResolutionEntries", ReplicationType.Synchronous);
      this.BindGuid("@value", value);
      this.BindString("@namespace", this.TranslateNamespaceToDatabase(@namespace), 256, true, SqlDbType.NVarChar);
      this.ExecuteNonQuery();
      return new List<NameResolutionEntry>();
    }

    public virtual List<NameResolutionEntry> RemoveResolutionEntries(
      IEnumerable<NameResolutionEntry> entries)
    {
      this.PrepareStoredProcedure("prc_RemoveNameResolutionEntries", ReplicationType.Synchronous);
      List<NameResolutionEntry> source = new List<NameResolutionEntry>(entries);
      this.BindNameResolutionTable("@entries", (IEnumerable<NameResolutionEntry>) source.Select<NameResolutionEntry, NameResolutionEntry>(new System.Func<NameResolutionEntry, NameResolutionEntry>(this.TranslateEntryToDatabase)).ToArray<NameResolutionEntry>());
      this.ExecuteNonQuery();
      return source;
    }

    public virtual List<NameResolutionEntry> GetNameResolutionEntriesByValue(
      Guid value,
      bool includeExpired = false)
    {
      this.PrepareStoredProcedure("prc_GetNameResolutionEntriesByValue");
      this.BindGuid("@value", value);
      this.BindBoolean("@includeExpired", includeExpired);
      return this.ExecuteQueryReturningNameResolutionEntries();
    }

    public virtual IList<NameResolutionEntry> GetNameResolutionEntries(
      IList<NameResolutionQuery> queries,
      bool includeExpired = false)
    {
      this.PrepareStoredProcedure("prc_GetNameResolutionEntries");
      this.BindNameResolutionQueryTable("@queries", (IEnumerable<NameResolutionQuery>) queries.Select<NameResolutionQuery, NameResolutionQuery>(new System.Func<NameResolutionQuery, NameResolutionQuery>(this.TranslateQueryToDatabase)).ToArray<NameResolutionQuery>(), true);
      this.BindBoolean("@includeExpired", includeExpired);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<NameResolutionComponent.NameResolutionEntryData>((ObjectBinder<NameResolutionComponent.NameResolutionEntryData>) new NameResolutionComponent.NameResolutionEntriesColumns());
      NameResolutionEntry[] resolutionEntries = new NameResolutionEntry[queries.Count];
      foreach (NameResolutionComponent.NameResolutionEntryData resolutionEntryData in resultCollection.GetCurrent<NameResolutionComponent.NameResolutionEntryData>())
      {
        if (resolutionEntryData.Entry != null)
          this.TranslateEntryFromDatabase(resolutionEntryData.Entry);
        resolutionEntries[resolutionEntryData.OrderId] = resolutionEntryData.Entry;
      }
      return (IList<NameResolutionEntry>) resolutionEntries;
    }

    public virtual IList<NameResolutionEntry> GetAllNameResolutionEntries() => throw new ServiceVersionNotSupportedException("NameResolution", 4, 4);

    protected List<NameResolutionEntry> ExecuteQueryReturningNameResolutionEntries()
    {
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<NameResolutionEntry>((ObjectBinder<NameResolutionEntry>) new NameResolutionEntryColumns());
      List<NameResolutionEntry> items = resultCollection.GetCurrent<NameResolutionEntry>().Items;
      foreach (NameResolutionEntry entry in items)
        this.TranslateEntryFromDatabase(entry);
      return items;
    }

    protected string TranslateNamespaceToDatabase(string @namespace)
    {
      string name = (string) null;
      this.TranslateNamespaceAndNameToDatabase(ref @namespace, ref name);
      return @namespace;
    }

    protected NameResolutionEntry TranslateEntryToDatabase(NameResolutionEntry entry)
    {
      string @namespace = entry.Namespace;
      string name = entry.Name;
      if (!this.TranslateNamespaceAndNameToDatabase(ref @namespace, ref name))
        return entry;
      entry = entry.Clone();
      entry.Namespace = @namespace;
      entry.Name = name;
      return entry;
    }

    protected NameResolutionQuery TranslateQueryToDatabase(NameResolutionQuery query)
    {
      if (query == null)
        return (NameResolutionQuery) null;
      string @namespace = query.Namespace;
      string name = query.Name;
      return this.TranslateNamespaceAndNameToDatabase(ref @namespace, ref name) ? new NameResolutionQuery(@namespace, name) : query;
    }

    protected virtual bool TranslateNamespaceAndNameToDatabase(
      ref string @namespace,
      ref string name)
    {
      if (!(@namespace == "Collection"))
        return false;
      @namespace = "CollectionUrl";
      if (this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        name = VirtualPathUtility.AppendTrailingSlash(name);
      return true;
    }

    protected virtual void TranslateEntryFromDatabase(NameResolutionEntry entry)
    {
      if (entry == null || !(entry.Namespace == "CollectionUrl"))
        return;
      entry.Namespace = "Collection";
      if (!this.RequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      entry.Name = VirtualPathUtility.RemoveTrailingSlash(entry.Name);
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) NameResolutionComponent.s_sqlExceptionFactories;

    private class NameResolutionEntryData
    {
      public NameResolutionEntry Entry;
      public int OrderId;
    }

    private class NameResolutionEntriesColumns : 
      ObjectBinder<NameResolutionComponent.NameResolutionEntryData>
    {
      private readonly NameResolutionEntryColumns EntryColumns = new NameResolutionEntryColumns();
      private SqlColumnBinder OrderIdColumn = new SqlColumnBinder("OrderId");

      protected override NameResolutionComponent.NameResolutionEntryData Bind() => new NameResolutionComponent.NameResolutionEntryData()
      {
        Entry = this.EntryColumns.Bind(this.Reader),
        OrderId = this.OrderIdColumn.GetInt32((IDataReader) this.Reader)
      };
    }
  }
}
