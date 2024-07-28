// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryHelpers
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RegistryHelpers
  {
    public const string RootPath = "/";
    public const char Separator = '/';
    public const int MaxPathLength = 259;
    private const string c_separatorString = "/";
    private static readonly char[] c_separators = new char[1]
    {
      '/'
    };
    private const string c_area = "Registry";
    private const string c_layer = "RegistryHelpers";
    private const int c_defaultCommandTimeout = 3660;

    public static bool IsSubItem(string item, string parent)
    {
      ArgumentUtility.CheckForNull<string>(item, nameof (item));
      ArgumentUtility.CheckForNull<string>(parent, nameof (parent));
      if (!item.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
        return false;
      return item.Length == parent.Length || parent[parent.Length - 1] == '/' || item[parent.Length] == '/';
    }

    public static string CombinePath(string parent, string relative)
    {
      ArgumentUtility.CheckForNull<string>(parent, nameof (parent));
      ArgumentUtility.CheckForNull<string>(relative, nameof (relative));
      if (parent.Length > 0 && parent[parent.Length - 1] == '/')
        parent = parent.Substring(0, parent.Length - 1);
      string str = relative.Length <= 0 || relative[0] == '/' ? parent + relative : parent + "/" + relative;
      return str.Length <= 259 ? str : throw new InvalidPathException(TFCommonResources.InvalidServerPathTooLong((object) str));
    }

    public static string MakeRelative(string path, string parent)
    {
      ArgumentUtility.CheckForNull<string>(path, nameof (path));
      ArgumentUtility.CheckForNull<string>(parent, nameof (parent));
      string str = string.Empty;
      int relativeStartIndex = RegistryHelpers.GetRelativeStartIndex(path, parent);
      if (relativeStartIndex >= 0)
        str = path.Substring(relativeStartIndex);
      if (str[str.Length - 1] == '/')
        str = str.TrimEnd(RegistryHelpers.c_separators);
      return str;
    }

    private static int GetRelativeStartIndex(string path, string parent)
    {
      int relativeStartIndex = 0;
      if (path.StartsWith(parent, StringComparison.OrdinalIgnoreCase))
      {
        if (path.Length == parent.Length)
          relativeStartIndex = -1;
        else if (parent.Length > 0 && parent[parent.Length - 1] == '/')
          relativeStartIndex = parent.Length - 1;
        else if (path.Length > parent.Length && path[parent.Length] == '/')
          relativeStartIndex = parent.Length;
      }
      return relativeStartIndex;
    }

    public static void CheckPath(string path, bool allowPatterns) => RegistryQuery.CheckOrParseHelper(path, allowPatterns, false, out string _, out string _, out int _);

    public static string GetDeploymentValueRaw(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      string path,
      string defaultValue = null,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      return RegistryHelpers.GetDeploymentValueRaw<string>(configurationDatabaseConnectionInfo, path, defaultValue, logger, commandTimeout);
    }

    public static T GetDeploymentValueRaw<T>(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      string path,
      T defaultValue = null,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      RegistryHelpers.CheckConfigurationDatabase(configurationDatabaseConnectionInfo, nameof (GetDeploymentValueRaw));
      return RegistryHelpers.GetValueRaw<T>(configurationDatabaseConnectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, path, defaultValue, logger, commandTimeout);
    }

    public static IEnumerable<RegistryEntry> GetDeploymentValuesRaw(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      string path,
      ITFLogger logger = null,
      int commandTimeout = 3660,
      int depth = 2147483647)
    {
      RegistryHelpers.CheckConfigurationDatabase(configurationDatabaseConnectionInfo, nameof (GetDeploymentValuesRaw));
      return RegistryHelpers.GetValuesRaw(configurationDatabaseConnectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, path, logger, commandTimeout, depth);
    }

    public static string GetValueRaw(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      string path,
      string defaultValue = null,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      return RegistryHelpers.GetValueRaw<string>(connectionInfo, partitionId, path, defaultValue, logger, commandTimeout);
    }

    public static T GetValueRaw<T>(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      string path,
      T defaultValue = null,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      T valueRaw = defaultValue;
      RegistryEntry registryEntry = RegistryHelpers.GetValuesRaw(connectionInfo, partitionId, path, logger, commandTimeout, 0).FirstOrDefault<RegistryEntry>();
      if (registryEntry != null)
        valueRaw = RegistryUtility.FromString<T>(registryEntry.Value, defaultValue);
      return valueRaw;
    }

    public static IEnumerable<RegistryEntry> GetValuesRaw(
      ISqlConnectionInfo connectionInfo,
      Guid hostId,
      string path,
      ITFLogger logger = null,
      int commandTimeout = 3660,
      int depth = 2147483647)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      return RegistryHelpers.GetValuesRaw(connectionInfo, DatabasePartitionComponent.GetPartitionId(connectionInfo, hostId), path, logger, commandTimeout, depth);
    }

    public static IEnumerable<RegistryEntry> GetValuesRaw(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      string path,
      ITFLogger logger = null,
      int commandTimeout = 3660,
      int depth = 2147483647)
    {
      RegistryQuery query = new RegistryQuery(path, false);
      if (depth > 0)
        query = new RegistryQuery(query.Path, query.Pattern, depth);
      foreach (RegistryItem registryItem in RegistryHelpers.ReadRaw(connectionInfo, partitionId, query, logger, commandTimeout))
        yield return new RegistryEntry(registryItem.Path, registryItem.Value);
    }

    public static IEnumerable<RegistryItem> DeploymentReadRaw(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      RegistryQuery query,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      RegistryHelpers.CheckConfigurationDatabase(configurationDatabaseConnectionInfo, nameof (DeploymentReadRaw));
      return RegistryHelpers.ReadRaw(configurationDatabaseConnectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, query, logger, commandTimeout);
    }

    public static IEnumerable<RegistryItem> ReadRaw(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      RegistryQuery query,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      if (connectionInfo == null)
        return (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;
      PathTable<RegistryItem> pathTable = new PathTable<RegistryItem>('/', true);
      using (RegistryComponent componentRaw = connectionInfo.CreateComponentRaw<RegistryComponent>(commandTimeout, handleNoResourceManagementSchema: true, logger: logger))
      {
        componentRaw.PartitionId = partitionId;
        foreach (RegistryItem referencedObject in componentRaw.QueryRegistry(query.Path, query.Depth, out long _))
        {
          if (query.Matches(referencedObject.Path))
            pathTable.AddUnsorted(referencedObject.Path, referencedObject);
        }
      }
      pathTable.Sort((Func<string, RegistryItem, RegistryItem, bool>) ((p, i1, i2) =>
      {
        TeamFoundationTracingService.TraceRaw(97056, TraceLevel.Error, "Registry", nameof (RegistryHelpers), "Found duplicate registry entry for path: '{0}' Value 1: '{1}' Value 2: '{2}' Value 2 is being preferred.", (object) p, (object) i1.Value, (object) i2.Value);
        return true;
      }));
      return pathTable.EnumSubTreeReferencedObjects((string) null, true, PathTableRecursion.Full);
    }

    public static IEnumerable<IEnumerable<RegistryItem>> DeploymentReadRaw(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      IEnumerable<RegistryQuery> queries,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      RegistryHelpers.CheckConfigurationDatabase(configurationDatabaseConnectionInfo, nameof (DeploymentReadRaw));
      return RegistryHelpers.ReadRaw(configurationDatabaseConnectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, queries, logger, commandTimeout);
    }

    public static IEnumerable<IEnumerable<RegistryItem>> ReadRaw(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      IEnumerable<RegistryQuery> queries,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      if (connectionInfo == null)
      {
        foreach (RegistryQuery query in queries)
          yield return (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;
      }
      else
      {
        List<RegistryQuery> list = queries.ToList<RegistryQuery>();
        PathTable<string>[] pathTableArray1;
        using (RegistryComponent componentRaw = connectionInfo.CreateComponentRaw<RegistryComponent>(commandTimeout, handleNoResourceManagementSchema: true, logger: logger))
        {
          componentRaw.PartitionId = partitionId;
          RegistryComponent.RegistryComponentQuery[] componentQueries = new RegistryComponent.RegistryComponentQuery[list.Count];
          for (int index = 0; index < componentQueries.Length; ++index)
            componentQueries[index] = new RegistryComponent.RegistryComponentQuery(list[index].Path, list[index].Depth);
          pathTableArray1 = new PathTable<string>[componentQueries.Length];
          foreach (RegistryComponent.RegistryItemWithIndex registryItemWithIndex in componentRaw.QueryRegistry(componentQueries, out long _))
          {
            int queryIndex = registryItemWithIndex.QueryIndex;
            if (list[queryIndex].Matches(registryItemWithIndex.Item.Path))
            {
              if (pathTableArray1[queryIndex] == null)
                pathTableArray1[queryIndex] = new PathTable<string>('/', true);
              pathTableArray1[queryIndex].AddUnsorted(registryItemWithIndex.Item.Path, registryItemWithIndex.Item.Value);
            }
          }
        }
        foreach (PathTable<string> pathTable in pathTableArray1)
          pathTable?.Sort((Func<string, string, string, bool>) ((p, v1, v2) =>
          {
            TeamFoundationTracingService.TraceRaw(97056, TraceLevel.Error, "Registry", nameof (RegistryHelpers), "Found duplicate registry entry for path: '{0}' Value 1: '{1}' Value 2: '{2}' Value 2 is being preferred.", (object) p, (object) v1, (object) v2);
            return true;
          }));
        PathTable<string>[] pathTableArray = pathTableArray1;
        for (int index = 0; index < pathTableArray.Length; ++index)
        {
          PathTable<string> pathTable = pathTableArray[index];
          if (pathTable != null)
            yield return pathTable.EnumSubTree((string) null, true, PathTableRecursion.Full).Select<PathTableTokenAndValue<string>, RegistryItem>((Func<PathTableTokenAndValue<string>, RegistryItem>) (s => new RegistryItem(s.Token, s.Value)));
          else
            yield return (IEnumerable<RegistryItem>) RegistryItem.EmptyArray;
        }
        pathTableArray = (PathTable<string>[]) null;
      }
    }

    public static void SetDeploymentValueRaw<T>(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      string path,
      T value,
      string logIdentityName = "00000000-0000-0000-0000-000000000000",
      bool logRegistryChanges = true,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      string registryValue = (object) value != null ? RegistryUtility.ToString<T>(value) : (string) null;
      RegistryHelpers.SetDeploymentValuesRaw(configurationDatabaseConnectionInfo, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
      {
        new RegistryEntry(path, registryValue)
      }, logIdentityName, (logRegistryChanges ? 1 : 0) != 0, logger, commandTimeout);
    }

    public static void SetDeploymentValuesRaw(
      ISqlConnectionInfo configurationDatabaseConnectionInfo,
      IEnumerable<RegistryEntry> entries,
      string logIdentityName = "00000000-0000-0000-0000-000000000000",
      bool logRegistryChanges = true,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      RegistryHelpers.CheckConfigurationDatabase(configurationDatabaseConnectionInfo, nameof (SetDeploymentValuesRaw));
      RegistryHelpers.SetValuesRaw(configurationDatabaseConnectionInfo, DatabasePartitionConstants.DeploymentHostPartitionId, entries, logIdentityName, logRegistryChanges, logger, commandTimeout);
    }

    public static void SetValueRaw<T>(
      ISqlConnectionInfo connectionInfo,
      Guid hostId,
      string path,
      T value,
      string logIdentityName = "00000000-0000-0000-0000-000000000000",
      bool logRegistryChanges = true,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      string registryValue = (object) value != null ? RegistryUtility.ToString<T>(value) : (string) null;
      RegistryHelpers.SetValuesRaw(connectionInfo, hostId, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
      {
        new RegistryEntry(path, registryValue)
      }, logIdentityName, (logRegistryChanges ? 1 : 0) != 0, logger, commandTimeout);
    }

    public static void SetValueRaw<T>(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      string path,
      T value,
      string logIdentityName = "00000000-0000-0000-0000-000000000000",
      bool logRegistryChanges = true,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      string registryValue = (object) value != null ? RegistryUtility.ToString<T>(value) : (string) null;
      RegistryHelpers.SetValuesRaw(connectionInfo, partitionId, (IEnumerable<RegistryEntry>) new RegistryEntry[1]
      {
        new RegistryEntry(path, registryValue)
      }, logIdentityName, (logRegistryChanges ? 1 : 0) != 0, logger, commandTimeout);
    }

    public static void SetValuesRaw(
      ISqlConnectionInfo connectionInfo,
      Guid hostId,
      IEnumerable<RegistryEntry> entries,
      string logIdentityName = "00000000-0000-0000-0000-000000000000",
      bool logRegistryChanges = true,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      ArgumentUtility.CheckForEmptyGuid(hostId, nameof (hostId));
      RegistryHelpers.SetValuesRaw(connectionInfo, DatabasePartitionComponent.GetPartitionId(connectionInfo, hostId), entries, logIdentityName, logRegistryChanges, logger, commandTimeout);
    }

    public static void SetValuesRaw(
      ISqlConnectionInfo connectionInfo,
      int partitionId,
      IEnumerable<RegistryEntry> entries,
      string logIdentityName = "00000000-0000-0000-0000-000000000000",
      bool logRegistryChanges = true,
      ITFLogger logger = null,
      int commandTimeout = 3660)
    {
      foreach (RegistryEntry entry in entries)
        RegistryHelpers.CheckPath(entry.Path, false);
      using (RegistryComponent componentRaw = connectionInfo.CreateComponentRaw<RegistryComponent>(commandTimeout, handleNoResourceManagementSchema: true, logger: logger))
      {
        componentRaw.PartitionId = partitionId;
        componentRaw.UpdateRegistry(logIdentityName, entries.Select<RegistryEntry, RegistryItem>((Func<RegistryEntry, RegistryItem>) (s => new RegistryItem(s.Path, s.Value))));
      }
    }

    private static void CheckConfigurationDatabase(
      ISqlConnectionInfo connectionInfo,
      [CallerMemberName] string callingMethodName = "")
    {
      if (connectionInfo != null && connectionInfo.InitialCatalog.IndexOf("Configuration", StringComparison.OrdinalIgnoreCase) < 0)
      {
        TeamFoundationTracingService.TraceRaw(0, TraceLevel.Error, "Registry", nameof (RegistryHelpers), "RegistryHelpers.{0} called for a non-configuration database: {1}", (object) callingMethodName, (object) connectionInfo.InitialCatalog);
        throw new InvalidOperationException(string.Format("RegistryHelpers.{0} only supported against configuration databases.", (object) callingMethodName));
      }
    }
  }
}
