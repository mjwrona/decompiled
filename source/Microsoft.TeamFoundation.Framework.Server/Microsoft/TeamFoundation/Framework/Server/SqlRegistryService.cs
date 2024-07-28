// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SqlRegistryService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SqlRegistryService : 
    VssBaseService,
    ISqlRegistryService,
    IVssRegistryService,
    IVssFrameworkService
  {
    private static readonly XmlSerializer s_registryItemDataSerializer = new XmlSerializer(typeof (List<RegistryItem>));
    private static readonly VssPerformanceCounter s_registryQueriesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_RegistryQueriesPerSec");
    private static readonly VssPerformanceCounter s_deploymentRegistryQueriesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_DeploymentRegistryQueriesPerSec");
    private static readonly XmlReaderSettings s_readerSettings = new XmlReaderSettings()
    {
      CheckCharacters = false,
      ConformanceLevel = ConformanceLevel.Fragment,
      DtdProcessing = DtdProcessing.Prohibit,
      XmlResolver = (XmlResolver) null
    };
    private const string c_area = "Registry";
    private const string c_layer = "SqlRegistryService";

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual IVssRegistryService GetParent(IVssRequestContext requestContext) => (IVssRegistryService) requestContext.GetParentService<ISqlRegistryService>();

    public virtual IEnumerable<RegistryItem> Read(
      IVssRequestContext requestContext,
      in RegistryQuery query)
    {
      return this.Read(requestContext, in query, out long _);
    }

    protected IEnumerable<RegistryItem> Read(
      IVssRequestContext requestContext,
      in RegistryQuery query,
      out long sequenceId)
    {
      if (!requestContext.ServiceHost.HasDatabaseAccess)
      {
        sequenceId = 0L;
        return (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;
      }
      SqlRegistryService.s_registryQueriesPerSec.Increment();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        SqlRegistryService.s_deploymentRegistryQueriesPerSec.Increment();
      PathTable<RegistryItem> pathTable = new PathTable<RegistryItem>('/', true);
      using (RegistryComponent component = requestContext.CreateComponent<RegistryComponent>())
      {
        foreach (RegistryItem referencedObject in component.QueryRegistry(query.Path, query.Depth, out sequenceId))
        {
          if (query.Matches(referencedObject.Path))
            pathTable.AddUnsorted(referencedObject.Path, referencedObject);
        }
        if (sequenceId == 0L)
        {
          if (component.Version >= 5)
            throw new RegistryUninitializedException(component.PartitionId);
        }
      }
      pathTable.Sort((Func<string, RegistryItem, RegistryItem, bool>) ((p, i1, i2) =>
      {
        TeamFoundationTracingService.TraceRaw(97056, TraceLevel.Error, "Registry", nameof (SqlRegistryService), "Found duplicate registry entry for path: '{0}' Value 1: '{1}' Value 2: '{2}' Value 2 is being preferred.", (object) p, (object) i1.Value, (object) i2.Value);
        return true;
      }));
      return pathTable.EnumSubTreeReferencedObjects((string) null, true, PathTableRecursion.Full);
    }

    public virtual IEnumerable<IEnumerable<RegistryItem>> Read(
      IVssRequestContext requestContext,
      IEnumerable<RegistryQuery> queries)
    {
      foreach (PathTable<string> pathTable in this.Read(requestContext, queries, out long _))
      {
        if (pathTable != null)
          yield return pathTable.EnumSubTree((string) null, true, PathTableRecursion.Full).Select<PathTableTokenAndValue<string>, RegistryItem>((Func<PathTableTokenAndValue<string>, RegistryItem>) (s => new RegistryItem(s.Token, s.Value)));
        else
          yield return (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;
      }
    }

    protected IEnumerable<PathTable<string>> Read(
      IVssRequestContext requestContext,
      IEnumerable<RegistryQuery> queries,
      out long sequenceId)
    {
      if (!requestContext.ServiceHost.HasDatabaseAccess)
      {
        sequenceId = 0L;
        return queries.Select<RegistryQuery, PathTable<string>>((Func<RegistryQuery, PathTable<string>>) (s => (PathTable<string>) null));
      }
      SqlRegistryService.s_registryQueriesPerSec.Increment();
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        SqlRegistryService.s_deploymentRegistryQueriesPerSec.Increment();
      List<RegistryQuery> list = queries.ToList<RegistryQuery>();
      PathTable<string>[] pathTableArray;
      using (RegistryComponent component = requestContext.CreateComponent<RegistryComponent>())
      {
        RegistryComponent.RegistryComponentQuery[] componentQueries = new RegistryComponent.RegistryComponentQuery[list.Count];
        for (int index = 0; index < componentQueries.Length; ++index)
          componentQueries[index] = new RegistryComponent.RegistryComponentQuery(list[index].Path, list[index].Depth);
        pathTableArray = new PathTable<string>[componentQueries.Length];
        foreach (RegistryComponent.RegistryItemWithIndex registryItemWithIndex in component.QueryRegistry(componentQueries, out sequenceId))
        {
          int queryIndex = registryItemWithIndex.QueryIndex;
          if (list[queryIndex].Matches(registryItemWithIndex.Item.Path))
          {
            if (pathTableArray[queryIndex] == null)
              pathTableArray[queryIndex] = new PathTable<string>('/', true);
            pathTableArray[queryIndex].AddUnsorted(registryItemWithIndex.Item.Path, registryItemWithIndex.Item.Value);
          }
        }
        if (sequenceId == 0L)
        {
          if (component.Version >= 5)
            throw new RegistryUninitializedException(component.PartitionId);
        }
      }
      foreach (PathTable<string> pathTable in pathTableArray)
        pathTable?.Sort((Func<string, string, string, bool>) ((p, v1, v2) =>
        {
          TeamFoundationTracingService.TraceRaw(97056, TraceLevel.Error, "Registry", nameof (SqlRegistryService), "Found duplicate registry entry for path: '{0}' Value 1: '{1}' Value 2: '{2}' Value 2 is being preferred.", (object) p, (object) v1, (object) v2);
          return true;
        }));
      return (IEnumerable<PathTable<string>>) pathTableArray;
    }

    internal static IEnumerable<RegistryItem> DeserializeSqlNotification(
      IVssRequestContext requestContext,
      string eventData)
    {
      return SqlRegistryService.DeserializeSqlNotification(requestContext, eventData, out long _);
    }

    internal static IEnumerable<RegistryItem> DeserializeSqlNotification(
      IVssRequestContext requestContext,
      string eventData,
      out long sequenceId)
    {
      if (string.IsNullOrEmpty(eventData))
      {
        sequenceId = 0L;
        return Enumerable.Empty<RegistryItem>();
      }
      sequenceId = SqlRegistryService.GetSequenceIdFromSqlNotification(eventData);
      List<RegistryItem> registryItemList;
      using (XmlReader xmlReader = XmlReader.Create((TextReader) new StringReader(eventData), SqlRegistryService.s_readerSettings))
        registryItemList = (List<RegistryItem>) SqlRegistryService.s_registryItemDataSerializer.Deserialize(xmlReader);
      for (int index = 0; index < registryItemList.Count; ++index)
        registryItemList[index] = new RegistryItem(RegistryComponent.DatabaseToRegistryPath(registryItemList[index].Path), registryItemList[index].Value);
      return (IEnumerable<RegistryItem>) registryItemList;
    }

    private static long GetSequenceIdFromSqlNotification(string eventData)
    {
      using (XmlReader xmlReader = XmlReader.Create((TextReader) new StringReader(eventData), SqlRegistryService.s_readerSettings))
      {
        if (xmlReader.Read())
        {
          while (xmlReader.MoveToNextAttribute())
          {
            long result;
            if (xmlReader.Name.Equals("SequenceId", StringComparison.Ordinal) && long.TryParse(xmlReader.Value, out result))
              return result;
          }
        }
      }
      return 0;
    }

    public List<RegistryAuditEntry> QueryAuditLog(
      IVssRequestContext requestContext,
      int changeIndex,
      bool returnOlder)
    {
      using (RegistryComponent component = requestContext.CreateComponent<RegistryComponent>())
        return component.QueryAuditLog("/", (long) changeIndex, returnOlder, 1024).ToList<RegistryAuditEntry>();
    }

    public virtual void Write(IVssRequestContext requestContext, IEnumerable<RegistryItem> items) => this.Write(requestContext, long.MaxValue, items, out IList<RegistryUpdateRecord> _);

    public virtual void RegisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback,
      bool fallThru,
      IEnumerable<RegistryQuery> filters,
      Guid serviceHostId = default (Guid))
    {
      throw new NotImplementedException();
    }

    public virtual void UnregisterNotification(
      IVssRequestContext requestContext,
      RegistrySettingsChangedCallback callback)
    {
      throw new NotImplementedException();
    }

    protected long Write(
      IVssRequestContext requestContext,
      long currentSequenceId,
      IEnumerable<RegistryItem> items,
      out IList<RegistryUpdateRecord> updateRecords)
    {
      ArgumentUtility.CheckForNull<IEnumerable<RegistryItem>>(items, nameof (items));
      if (!requestContext.ServiceHost.HasDatabaseAccess)
      {
        updateRecords = (IList<RegistryUpdateRecord>) null;
        return 0;
      }
      foreach (RegistryItem registryItem in items)
        RegistryHelpers.CheckPath(registryItem.Path, false);
      using (RegistryComponent component = requestContext.CreateComponent<RegistryComponent>())
        return component.UpdateRegistry(requestContext.GetUserId().ToString(), currentSequenceId, items, true, out updateRecords);
    }
  }
}
