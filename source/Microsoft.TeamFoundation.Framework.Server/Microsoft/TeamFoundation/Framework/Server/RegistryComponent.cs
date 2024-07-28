// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class RegistryComponent : TeamFoundationSqlResourceComponent
  {
    public static readonly ComponentFactory ComponentFactory = new ComponentFactory(new IComponentCreator[7]
    {
      (IComponentCreator) new ComponentCreator<RegistryComponent2>(2),
      (IComponentCreator) new ComponentCreator<RegistryComponent3>(3),
      (IComponentCreator) new ComponentCreator<RegistryComponent4>(4),
      (IComponentCreator) new ComponentCreator<RegistryComponent5>(5),
      (IComponentCreator) new ComponentCreator<RegistryComponent5>(6),
      (IComponentCreator) new ComponentCreator<RegistryComponent7>(7),
      (IComponentCreator) new ComponentCreator<RegistryComponent8>(8)
    }, "Registry");
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static RegistryComponent() => RegistryComponent.s_sqlExceptionFactories.Add(800053, new SqlExceptionFactory(typeof (RegistryUninitializedException)));

    public virtual IEnumerable<RegistryItem> QueryRegistry(
      string registryPath,
      int depth,
      out long sequenceId)
    {
      sequenceId = 0L;
      this.PrepareStoredProcedure("prc_QueryRegistry");
      this.BindString("@registryPath", RegistryComponent.RegistryToDatabasePath(registryPath), 260, false, SqlDbType.NVarChar);
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), "prc_QueryRegistry", this.RequestContext))
      {
        resultCollection.AddBinder<RegistryItem>((ObjectBinder<RegistryItem>) new RegistryComponent.RegistryItemColumns());
        List<RegistryItem> registryItemList = new List<RegistryItem>();
        foreach (RegistryItem registryItem in resultCollection.GetCurrent<RegistryItem>())
        {
          if (RegistryHelpers.IsSubItem(registryItem.Path, registryPath))
            registryItemList.Add(registryItem);
        }
        return (IEnumerable<RegistryItem>) registryItemList;
      }
    }

    public virtual IEnumerable<RegistryComponent.RegistryItemWithIndex> QueryRegistry(
      RegistryComponent.RegistryComponentQuery[] componentQueries,
      out long sequenceId)
    {
      if (componentQueries.Length == 0)
      {
        this.QueryRegistry("/", 0, out sequenceId);
        return (IEnumerable<RegistryComponent.RegistryItemWithIndex>) RegistryComponent.RegistryItemWithIndex.EmptyArray;
      }
      RegistryComponent.RegistryComponentQuery componentQuery = componentQueries[0];
      return this.QueryRegistryHelper(this.QueryRegistry(componentQuery.Path, componentQuery.Depth, out sequenceId), componentQueries);
    }

    private IEnumerable<RegistryComponent.RegistryItemWithIndex> QueryRegistryHelper(
      IEnumerable<RegistryItem> firstResults,
      RegistryComponent.RegistryComponentQuery[] componentQueries)
    {
      foreach (RegistryItem firstResult in firstResults)
        yield return new RegistryComponent.RegistryItemWithIndex(0, firstResult);
      for (int i = 1; i < componentQueries.Length; ++i)
      {
        foreach (RegistryItem registryItem in this.QueryRegistry(componentQueries[i].Path, componentQueries[i].Depth, out long _))
          yield return new RegistryComponent.RegistryItemWithIndex(i, registryItem);
      }
    }

    public long UpdateRegistry(string identityName, IEnumerable<RegistryItem> entriesToUpdate) => this.UpdateRegistry(identityName, long.MaxValue, entriesToUpdate, true, out IList<RegistryUpdateRecord> _);

    public virtual long UpdateRegistry(
      string identityName,
      long currentSequenceId,
      IEnumerable<RegistryItem> entriesToUpdate,
      bool overwriteExisting,
      out IList<RegistryUpdateRecord> updateRecords)
    {
      throw new NotImplementedException();
    }

    public virtual IEnumerable<RegistryAuditEntry> QueryAuditLog(
      string registryPath,
      long changeIndex,
      bool returnOlder,
      int pageSize)
    {
      RegistryComponent registryComponent = this;
      registryComponent.PrepareStoredProcedure("prc_QueryRegistryAuditLog");
      registryComponent.BindString("@registryPath", RegistryComponent.RegistryToDatabasePath(registryPath), 260, false, SqlDbType.NVarChar);
      registryComponent.BindLong("@changeIndex", changeIndex);
      registryComponent.BindBoolean("@returnOlder", returnOlder);
      registryComponent.BindInt("@pageSize", pageSize);
      using (ResultCollection result = new ResultCollection((IDataReader) registryComponent.ExecuteReader(), "prc_QueryRegistryAuditLog", registryComponent.RequestContext))
      {
        result.AddBinder<RegistryAuditEntry>((ObjectBinder<RegistryAuditEntry>) new RegistryComponent.RegistryAuditEntryColumns());
        foreach (RegistryAuditEntry registryAuditEntry in result.GetCurrent<RegistryAuditEntry>())
        {
          if (RegistryHelpers.IsSubItem(registryAuditEntry.Entry.Path, registryPath))
            yield return registryAuditEntry;
        }
      }
    }

    public virtual void InitializeRegistrySequenceId()
    {
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) RegistryComponent.s_sqlExceptionFactories;

    public static string RegistryToDatabasePath(string registryPath)
    {
      registryPath = registryPath.Replace('/', '\\');
      return registryPath[registryPath.Length - 1] != '\\' ? "#" + registryPath + (object) '\\' : "#" + registryPath;
    }

    public static string DatabaseToRegistryPath(string databasePath)
    {
      int index = databasePath.Length - 1;
      while (index >= 2 && databasePath[index] == '\\')
        --index;
      return databasePath.Substring(1, index - 1 + 1).Replace('\\', '/');
    }

    protected class RegistryItemColumns : ObjectBinder<RegistryItem>
    {
      private SqlColumnBinder m_registryPathColumn = new SqlColumnBinder("RegistryPath");
      private SqlColumnBinder m_registryValueColumn = new SqlColumnBinder("RegValue");

      protected override RegistryItem Bind() => new RegistryItem(RegistryComponent.DatabaseToRegistryPath(this.m_registryPathColumn.GetString((IDataReader) this.Reader, false)), this.m_registryValueColumn.GetString((IDataReader) this.Reader, false));
    }

    protected class RegistryAuditEntryColumns : ObjectBinder<RegistryAuditEntry>
    {
      private SqlColumnBinder m_changeIndexColumn = new SqlColumnBinder("ChangeIndex");
      private SqlColumnBinder m_changeTypeColumn = new SqlColumnBinder("ChangeType");
      private SqlColumnBinder m_changeTimeColumn = new SqlColumnBinder("ChangeTime");
      private SqlColumnBinder m_identityNameColumn = new SqlColumnBinder("IdentityName");
      private SqlColumnBinder m_registryPathColumn = new SqlColumnBinder("RegistryPath");
      private SqlColumnBinder m_registryValueColumn = new SqlColumnBinder("RegValue");

      protected override RegistryAuditEntry Bind()
      {
        RegistryAuditEntry registryAuditEntry = new RegistryAuditEntry();
        RegistryEntry registryEntry = new RegistryEntry();
        registryAuditEntry.ChangeIndex = (int) this.m_changeIndexColumn.GetInt64((IDataReader) this.Reader);
        registryAuditEntry.ChangeTime = this.m_changeTimeColumn.GetDateTime((IDataReader) this.Reader);
        registryAuditEntry.ChangeType = (RegistryChangeType) this.m_changeTypeColumn.GetByte((IDataReader) this.Reader);
        registryAuditEntry.IdentityName = this.m_identityNameColumn.GetString((IDataReader) this.Reader, false);
        registryEntry.Path = RegistryComponent.DatabaseToRegistryPath(this.m_registryPathColumn.GetString((IDataReader) this.Reader, false));
        registryEntry.Value = this.m_registryValueColumn.GetString((IDataReader) this.Reader, true);
        registryAuditEntry.Entry = registryEntry;
        return registryAuditEntry;
      }
    }

    public readonly struct RegistryComponentQuery
    {
      public readonly string Path;
      public readonly int Depth;

      public RegistryComponentQuery(string path, int depth)
      {
        this.Path = path;
        this.Depth = depth;
      }
    }

    public readonly struct RegistryItemWithIndex
    {
      public readonly int QueryIndex;
      public readonly RegistryItem Item;
      public static readonly RegistryComponent.RegistryItemWithIndex[] EmptyArray = Array.Empty<RegistryComponent.RegistryItemWithIndex>();

      public RegistryItemWithIndex(int queryIndex, RegistryItem item)
      {
        this.QueryIndex = queryIndex;
        this.Item = item;
      }
    }
  }
}
