// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Principal;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class WindowsMachineDirectoryHelper
  {
    private const string Localhost = "localhost";
    private const char DomainAccountNameSeparator = '\\';
    private readonly string localWindowsMachineDirectoryEntryPath;
    private static WindowsMachineDirectoryHelper instance;
    private readonly HashSet<string> schemaClassNamesToSearch = new HashSet<string>((IEqualityComparer<string>) VssStringComparer.WMDSchemaClassName)
    {
      "user",
      "group"
    };

    public static WindowsMachineDirectoryHelper Instance
    {
      get => WindowsMachineDirectoryHelper.instance ?? (WindowsMachineDirectoryHelper.instance = new WindowsMachineDirectoryHelper());
      internal set => WindowsMachineDirectoryHelper.instance = value;
    }

    internal WindowsMachineDirectoryHelper() => this.localWindowsMachineDirectoryEntryPath = WindowsMachineDirectoryHelper.GetLocalWindowsMachineDirectoryEntryPath();

    internal static bool IsLocalMachine(string domainName) => VssStringComparer.MachineName.Equals(domainName, Environment.MachineName);

    private static bool IsLocalHost(string domainName) => VssStringComparer.MachineName.Equals(domainName, "localhost");

    internal virtual IList<IDirectoryEntity> Search(
      IVssRequestContext context,
      string pattern,
      int sizeLimit,
      HashSet<string> schemaClassNamesToSearch,
      HashSet<string> machineDirectoryPropertiesToSearch,
      IEnumerable<string> propertiesToReturn)
    {
      SortedSet<IDirectoryEntity> source = new SortedSet<IDirectoryEntity>(DirectoryEntityComparer.DefaultComparer);
      if (schemaClassNamesToSearch.IsNullOrEmpty<string>())
        return (IList<IDirectoryEntity>) source.ToList<IDirectoryEntity>();
      HashSet<string> propertiesToReturn1 = WindowsMachineDirectoryHelper.GetAllPropertiesToReturn(propertiesToReturn);
      using (DirectoryEntry machineDirectoryEntry = this.GetLocalMachineDirectoryEntry())
      {
        foreach (DirectoryEntry child in machineDirectoryEntry.Children)
        {
          if (source.Count < sizeLimit)
          {
            if (WindowsMachineDirectoryHelper.IsMatchingDirectoryEntry(child, schemaClassNamesToSearch, machineDirectoryPropertiesToSearch, pattern))
            {
              IDirectoryEntity directoryEntity = WindowsMachineDirectoryHelper.CreateDirectoryEntity(child, (IEnumerable<string>) propertiesToReturn1);
              if (directoryEntity != null)
                source.Add(directoryEntity);
            }
          }
          else
            break;
        }
        return (IList<IDirectoryEntity>) source.ToList<IDirectoryEntity>();
      }
    }

    internal virtual IDictionary<SecurityIdentifier, IDirectoryEntity> GetDirectoryEntities(
      IVssRequestContext context,
      IEnumerable<SecurityIdentifier> sids,
      IEnumerable<string> propertiesToReturn)
    {
      Dictionary<SecurityIdentifier, IDirectoryEntity> directoryEntities = new Dictionary<SecurityIdentifier, IDirectoryEntity>();
      if (sids.IsNullOrEmpty<SecurityIdentifier>())
        return (IDictionary<SecurityIdentifier, IDirectoryEntity>) directoryEntities;
      HashSet<string> propertiesToReturn1 = WindowsMachineDirectoryHelper.GetAllPropertiesToReturn(propertiesToReturn);
      foreach (KeyValuePair<SecurityIdentifier, DirectoryEntry> directoryEntry in (IEnumerable<KeyValuePair<SecurityIdentifier, DirectoryEntry>>) this.GetDirectoryEntries(context, sids))
      {
        SecurityIdentifier key = directoryEntry.Key;
        IDirectoryEntity directoryEntity = WindowsMachineDirectoryHelper.CreateDirectoryEntity(directoryEntry.Value, (IEnumerable<string>) propertiesToReturn1);
        if (directoryEntity != null)
          directoryEntities[key] = directoryEntity;
      }
      return (IDictionary<SecurityIdentifier, IDirectoryEntity>) directoryEntities;
    }

    internal virtual IDictionary<SecurityIdentifier, DirectoryEntry> GetDirectoryEntries(
      IVssRequestContext context,
      IEnumerable<SecurityIdentifier> ids,
      string schemaClassName = null)
    {
      Dictionary<SecurityIdentifier, DirectoryEntry> directoryEntries = new Dictionary<SecurityIdentifier, DirectoryEntry>();
      if (ids.IsNullOrEmpty<SecurityIdentifier>())
        return (IDictionary<SecurityIdentifier, DirectoryEntry>) directoryEntries;
      HashSet<SecurityIdentifier> securityIdentifierSet = new HashSet<SecurityIdentifier>(ids);
      using (DirectoryEntry machineDirectoryEntry = this.GetLocalMachineDirectoryEntry())
      {
        foreach (DirectoryEntry child in machineDirectoryEntry.Children)
        {
          if (string.IsNullOrWhiteSpace(schemaClassName))
          {
            if (!this.schemaClassNamesToSearch.Contains(child.SchemaClassName))
              continue;
          }
          else if (!VssStringComparer.WMDSchemaClassName.Equals(schemaClassName, child.SchemaClassName))
            continue;
          SecurityIdentifier objectSid = (SecurityIdentifier) null;
          if (!WindowsMachineDirectoryHelper.TryGetObjectSid(child, out objectSid) || !securityIdentifierSet.Contains(objectSid))
            child.Dispose();
          else
            directoryEntries[objectSid] = child;
        }
        return (IDictionary<SecurityIdentifier, DirectoryEntry>) directoryEntries;
      }
    }

    internal static IDirectoryEntity CreateDirectoryEntity(
      DirectoryEntry directoryEntry,
      IEnumerable<string> propertiesToReturn)
    {
      if (string.IsNullOrWhiteSpace(directoryEntry.SchemaClassName))
        return (IDirectoryEntity) null;
      IDirectoryEntity directoryEntity = (IDirectoryEntity) null;
      if (VssStringComparer.DirectoryEntityPropertyComparer.Equals(directoryEntry.SchemaClassName, "user"))
        directoryEntity = (IDirectoryEntity) WindowsMachineDirectoryHelper.CreateDirectoryUser(directoryEntry, propertiesToReturn);
      else if (VssStringComparer.DirectoryEntityPropertyComparer.Equals(directoryEntry.SchemaClassName, "group"))
        directoryEntity = (IDirectoryEntity) WindowsMachineDirectoryHelper.CreateDirectoryGroup(directoryEntry, propertiesToReturn);
      return directoryEntity;
    }

    internal static HashSet<string> GetAllPropertiesToReturn(IEnumerable<string> propertiesToReturn)
    {
      HashSet<string> propertiesToReturn1 = new HashSet<string>((IEnumerable<string>) WindowsMachineDirectoryProperty.DirectoryEntityRequiredProperties, (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer);
      if (propertiesToReturn == null)
        return propertiesToReturn1;
      propertiesToReturn1.UnionWith(propertiesToReturn);
      return propertiesToReturn1;
    }

    internal static bool TryGetObjectSid(
      DirectoryEntityIdentifier entityId,
      out SecurityIdentifier objectSid)
    {
      objectSid = (SecurityIdentifier) null;
      if (entityId.Version != 1 || !(entityId is DirectoryEntityIdentifierV1 entityIdentifierV1) || !"wmd".Equals(entityIdentifierV1.Source) || !"user".Equals(entityIdentifierV1.Type) && !"group".Equals(entityIdentifierV1.Type))
        return false;
      try
      {
        objectSid = new SecurityIdentifier(entityIdentifierV1.Id);
      }
      catch (ArgumentException ex)
      {
        return false;
      }
      return true;
    }

    internal static bool TryGetPropertyValue(
      PropertyCollection properties,
      string propertyName,
      out object value)
    {
      value = (object) null;
      if (properties.Count < 1 || !properties.Contains(propertyName))
        return false;
      PropertyValueCollection property = properties[propertyName];
      if (property == null || property.Count <= 0)
        return false;
      value = property.Value;
      return true;
    }

    internal static Tuple<string, string> RetrieveDomainNameAndPrefix(string query)
    {
      if (string.IsNullOrWhiteSpace(query))
        return new Tuple<string, string>(string.Empty, string.Empty);
      if (query.Length == 1)
        return new Tuple<string, string>(string.Empty, query);
      int length = query.IndexOf('\\', 0);
      if (length == -1)
        return new Tuple<string, string>(string.Empty, query);
      string str1 = query.Substring(0, length);
      if (length == query.Length - 1)
        return new Tuple<string, string>(str1, string.Empty);
      string str2 = query.Substring(length + 1, query.Length - (length + 1));
      return new Tuple<string, string>(str1, str2);
    }

    private static bool IsMatchingDirectoryEntry(
      DirectoryEntry directoryEntry,
      HashSet<string> schemaClassNamesToSearch,
      HashSet<string> machineDirectoryPropertiesToSearch,
      string prefix)
    {
      object obj;
      return schemaClassNamesToSearch.Contains(directoryEntry.SchemaClassName) && (string.IsNullOrWhiteSpace(prefix) || machineDirectoryPropertiesToSearch.Contains("Name") && VssStringComparer.DirectoryEntryNameComparer.StartsWith(directoryEntry.Name, prefix) || machineDirectoryPropertiesToSearch.Contains("FullName") && WindowsMachineDirectoryHelper.TryGetPropertyValue(directoryEntry.Properties, "FullName", out obj) && obj is string main && VssStringComparer.DirectoryEntryNameComparer.StartsWith(main, prefix));
    }

    private static IDirectoryUser CreateDirectoryUser(
      DirectoryEntry directoryEntry,
      IEnumerable<string> propertiesToReturn)
    {
      SecurityIdentifier sid;
      IDictionary<string, object> properties;
      string originDirectory;
      string source;
      if (!WindowsMachineDirectoryHelper.TryGetDirectoryEntryInformation(directoryEntry, propertiesToReturn, WindowsMachineDirectoryProperty.UserPropertiesToReturnMap, out sid, out properties, out originDirectory, out source))
        return (IDirectoryUser) null;
      DirectoryUser directoryUser = new DirectoryUser();
      directoryUser.EntityId = WindowsMachineDirectoryEntityHelper.CreateUserId(sid.ToString(), source).Encode();
      directoryUser.OriginDirectory = originDirectory;
      directoryUser.OriginId = sid.ToString();
      directoryUser.Properties = properties;
      return (IDirectoryUser) directoryUser;
    }

    private static bool TryGetObjectSid(
      DirectoryEntry directoryEntry,
      out SecurityIdentifier objectSid)
    {
      objectSid = (SecurityIdentifier) null;
      object obj = (object) null;
      if (directoryEntry.Properties == null || !WindowsMachineDirectoryHelper.TryGetPropertyValue(directoryEntry.Properties, "ObjectSid", out obj) || !(obj is byte[] binaryForm))
        return false;
      objectSid = new SecurityIdentifier(binaryForm, 0);
      return true;
    }

    private static IDirectoryGroup CreateDirectoryGroup(
      DirectoryEntry directoryEntry,
      IEnumerable<string> propertiesToReturn)
    {
      SecurityIdentifier sid;
      IDictionary<string, object> properties;
      string originDirectory;
      string source;
      if (!WindowsMachineDirectoryHelper.TryGetDirectoryEntryInformation(directoryEntry, propertiesToReturn, WindowsMachineDirectoryProperty.GroupPropertiesToReturnMap, out sid, out properties, out originDirectory, out source))
        return (IDirectoryGroup) null;
      DirectoryGroup directoryGroup = new DirectoryGroup();
      directoryGroup.EntityId = WindowsMachineDirectoryEntityHelper.CreateGroupId(sid.ToString(), source).Encode();
      directoryGroup.OriginDirectory = originDirectory;
      directoryGroup.OriginId = sid.ToString();
      directoryGroup.Properties = properties;
      return (IDirectoryGroup) directoryGroup;
    }

    private DirectoryEntry GetLocalMachineDirectoryEntry()
    {
      DirectoryEntry machineDirectoryEntry = new DirectoryEntry();
      machineDirectoryEntry.AuthenticationType |= AuthenticationTypes.ReadonlyServer;
      machineDirectoryEntry.Path = this.localWindowsMachineDirectoryEntryPath;
      return machineDirectoryEntry;
    }

    private static bool TryGetDirectoryEntryInformation(
      DirectoryEntry directoryEntry,
      IEnumerable<string> propertiesToReturn,
      IReadOnlyDictionary<string, string> propertiesToReturnMap,
      out SecurityIdentifier sid,
      out IDictionary<string, object> properties,
      out string originDirectory,
      out string source)
    {
      sid = (SecurityIdentifier) null;
      properties = (IDictionary<string, object>) null;
      originDirectory = (string) null;
      source = (string) null;
      if (!WindowsMachineDirectoryHelper.TryGetObjectSid(directoryEntry, out sid))
        return false;
      string domainName;
      bool flag = WindowsMachineDirectoryHelper.IsWindowsMachineDirectoryEntry(sid, out domainName);
      properties = WindowsMachineDirectoryHelper.GetDirectoryEntityPropertiesToReturn(directoryEntry, propertiesToReturn, propertiesToReturnMap);
      WindowsMachineDirectoryHelper.AddScopeNameIfRequired(properties, propertiesToReturn, domainName);
      originDirectory = flag ? "wmd" : "ad";
      source = flag ? "wmd" : "ad";
      return true;
    }

    private static IDictionary<string, object> GetDirectoryEntityPropertiesToReturn(
      DirectoryEntry directoryEntry,
      IEnumerable<string> propertiesToReturn,
      IReadOnlyDictionary<string, string> propertiesToReturnMap)
    {
      Dictionary<string, object> propertiesToReturn1 = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer);
      foreach (string key in propertiesToReturn)
      {
        string propertyName = (string) null;
        object obj;
        if (propertiesToReturnMap.TryGetValue(key, out propertyName) && WindowsMachineDirectoryHelper.TryGetPropertyValue(directoryEntry.Properties, propertyName, out obj))
          propertiesToReturn1[key] = obj;
      }
      return (IDictionary<string, object>) propertiesToReturn1;
    }

    private static void AddScopeNameIfRequired(
      IDictionary<string, object> properties,
      IEnumerable<string> propertiesToReturn,
      string domainName)
    {
      if (!propertiesToReturn.Contains<string>("ScopeName", (IEqualityComparer<string>) VssStringComparer.DirectoryEntityPropertyComparer))
        return;
      properties["ScopeName"] = (object) domainName;
    }

    private static bool IsWindowsMachineDirectoryEntry(
      SecurityIdentifier sid,
      out string domainName)
    {
      domainName = (string) null;
      SecurityIdentifierInfo securityIdentifierInfo = new SecurityIdentifierInfo(sid);
      SidIdentityHelper.ResolveSid(securityIdentifierInfo, out domainName, out string _, out Microsoft.TeamFoundation.Common.Internal.NativeMethods.AccountType _, out bool _, out bool _);
      return WindowsMachineDirectoryHelper.IsWindowsMachineSecurityIdentifier(securityIdentifierInfo, domainName);
    }

    internal static bool IsWindowsMachineSecurityIdentifier(
      SecurityIdentifierInfo securityIdentifierInfo,
      string domainName)
    {
      return WindowsMachineDirectoryHelper.IsLocalMachine(domainName) || WindowsMachineDirectoryHelper.IsLocalHost(domainName) || SidIdentityHelper.IsBuiltInAccount(securityIdentifierInfo.GetBinaryForm());
    }

    internal static bool IsWindowsMachineSecurityIdentifier(
      string securityIdentifier,
      string domainName)
    {
      try
      {
        return WindowsMachineDirectoryHelper.IsWindowsMachineSecurityIdentifier(new SecurityIdentifierInfo(new SecurityIdentifier(securityIdentifier)), domainName);
      }
      catch (ArgumentException ex)
      {
        return false;
      }
    }

    internal static string GetLocalWindowsMachineDirectoryEntryPath()
    {
      string directoryEntryPath = "WinNT://" + Environment.MachineName + ",computer";
      WindowsMachineDirectoryHelper.ValidateDirectoryEntryPath(directoryEntryPath);
      return directoryEntryPath;
    }

    private static void ValidateDirectoryEntryPath(string directoryEntryPath)
    {
      try
      {
        if (!DirectoryEntry.Exists(directoryEntryPath))
          throw new DirectoryDiscoveryServiceException(FrameworkResources.UnableToBindToDirectoryEntry((object) directoryEntryPath));
      }
      catch (Exception ex)
      {
        if (!(ex is DirectoryDiscoveryServiceException))
          throw new DirectoryDiscoveryServiceException(FrameworkResources.UnableToBindToDirectoryEntry((object) directoryEntryPath), ex);
        throw;
      }
    }
  }
}
