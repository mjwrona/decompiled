// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.RegisteredTfsConnections
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.TeamFoundation.Client
{
  public static class RegisteredTfsConnections
  {
    private static string m_currentUserRegistryRoot;
    private const string cInstancesKey = "TeamFoundation\\Instances";
    private const string cCollectionSubKey = "Collections";
    private const string cDeleted = "Deleted";
    private const string cType = "Type";
    private const string cUri = "Uri";
    private const string cOffline = "Offline";
    private const string cAutoReconnect = "AutoReconnect";
    private const string cInstanceId = "InstanceId";
    private const string cIsHosted = "IsHosted";
    private const char cSeparator = '\\';

    public static void RegisterConfigurationServer(TfsConfigurationServer configurationServer)
    {
      ArgumentUtility.CheckForNull<TfsConfigurationServer>(configurationServer, nameof (configurationServer));
      Guid instanceId = Guid.Empty;
      bool? isHosted = new bool?();
      if (configurationServer.HasAuthenticated)
      {
        instanceId = configurationServer.InstanceId;
        isHosted = new bool?(configurationServer.IsHostedServer);
      }
      RegisteredTfsConnections.RegisterConfigurationServer(configurationServer.Uri, instanceId, isHosted);
    }

    public static void RegisterConfigurationServer(Uri uri, Guid instanceId, bool? isHosted)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      string tfsDisplayName = UIHost.ExtractTFSDisplayName(uri);
      RegisteredTfsConnections.RegisterConfigurationServerInternal(uri, tfsDisplayName, RegisteredTfsConnections.ServerType.ConfigurationServer, instanceId, isHosted);
    }

    public static void UnregisterConfigurationServer(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      RegisteredTfsConnections.UnregisterConfigurationServerInternal(name);
    }

    public static RegisteredConfigurationServer[] GetConfigurationServers() => RegisteredTfsConnections.GetConfigurationServersInternal();

    public static RegisteredConfigurationServer GetConfigurationServer(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      return RegisteredTfsConnections.GetConfigurationServerInternal(name);
    }

    public static RegisteredConfigurationServer GetConfigurationServer(Uri uri)
    {
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      return RegisteredTfsConnections.GetConfigurationServerInternal(uri);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void UpdateConfigurationServers(List<Tuple<Uri, Guid>> servers)
    {
      ArgumentUtility.CheckForNull<List<Tuple<Uri, Guid>>>(servers, nameof (servers));
      Dictionary<string, Tuple<Uri, Guid>> dictionary1 = servers.ToDictionary<Tuple<Uri, Guid>, string>((Func<Tuple<Uri, Guid>, string>) (x => UIHost.ExtractTFSDisplayName(x.Item1)));
      Dictionary<string, RegisteredTfsConnections.ServerInfo> dictionary2 = RegisteredTfsConnections.GetInstancesInfos(true).ToDictionary<RegisteredTfsConnections.ServerInfo, string>((Func<RegisteredTfsConnections.ServerInfo, string>) (x => x.Name));
      foreach (KeyValuePair<string, Tuple<Uri, Guid>> keyValuePair in dictionary1)
      {
        if (!dictionary2.ContainsKey(keyValuePair.Key))
          RegisteredTfsConnections.RegisterConfigurationServer(keyValuePair.Value.Item1, keyValuePair.Value.Item2, new bool?(true));
      }
    }

    public static void RegisterProjectCollection(TfsTeamProjectCollection projectCollection)
    {
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(projectCollection, nameof (projectCollection));
      string name;
      string applicationName;
      if (projectCollection.CatalogNode == null)
      {
        name = RegisteredTfsConnections.GetLegacyServerName(projectCollection);
        applicationName = name;
        RegisteredTfsConnections.RegisterConfigurationServerInternal(projectCollection.Uri, name, RegisteredTfsConnections.ServerType.ProjectCollection, Guid.Empty, new bool?(false));
      }
      else
      {
        RegisteredTfsConnections.RegisterConfigurationServer(projectCollection.ConfigurationServer);
        applicationName = UIHost.ExtractTFSDisplayName(projectCollection.ConfigurationServer.Uri);
        name = projectCollection.CatalogNode.Resource.DisplayName;
      }
      RegisteredTfsConnections.RegisterCollectionInternal(applicationName, projectCollection.Uri, name, projectCollection.HasAuthenticated ? projectCollection.InstanceId : Guid.Empty);
    }

    public static void UnregisterProjectCollection(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      string[] strArray = name.Split('\\');
      if (strArray.Length == 1)
      {
        RegisteredTfsConnections.UnregisterCollectionInternal(strArray[0], strArray[0]);
      }
      else
      {
        if (strArray.Length != 2 || string.IsNullOrEmpty(strArray[0]) || string.IsNullOrEmpty(strArray[1]))
          return;
        RegisteredTfsConnections.UnregisterCollectionInternal(strArray[0], strArray[1]);
      }
    }

    public static RegisteredProjectCollection[] GetProjectCollections() => RegisteredTfsConnections.GetCollectionsInternal(false).ToArray();

    public static RegisteredProjectCollection[] GetLegacyProjectCollections() => RegisteredTfsConnections.GetCollectionsInternal(true).ToArray();

    public static List<RegisteredProjectCollection> GetProjectCollections(string serverName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(serverName, nameof (serverName));
      RegisteredConfigurationServer configurationServer = RegisteredTfsConnections.GetConfigurationServer(serverName);
      return configurationServer != null ? RegisteredTfsConnections.GetCollectionsInternal(configurationServer.Name, RegisteredTfsConnections.ServerType.ConfigurationServer) : new List<RegisteredProjectCollection>();
    }

    public static RegisteredProjectCollection GetProjectCollection(string name)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      name = name.Replace('/', '\\');
      foreach (RegisteredProjectCollection projectCollection in RegisteredTfsConnections.GetProjectCollections())
      {
        if (VssStringComparer.ServerName.Equals(projectCollection.Name.Replace('/', '\\').Trim(), name.Trim()))
          return projectCollection;
      }
      return (RegisteredProjectCollection) null;
    }

    public static RegisteredProjectCollection GetProjectCollection(Uri uri)
    {
      Uri qualifiedUriForName = TfsTeamProjectCollection.GetFullyQualifiedUriForName(uri.AbsoluteUri);
      ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
      foreach (RegisteredProjectCollection projectCollection in RegisteredTfsConnections.GetProjectCollections())
      {
        if (UriUtility.Equals(projectCollection.Uri, uri) || UriUtility.Equals(projectCollection.Uri, qualifiedUriForName))
          return projectCollection;
      }
      return (RegisteredProjectCollection) null;
    }

    internal static RegistryKey GetServerSubKey(Uri uri, string subkeyName)
    {
      try
      {
        ArgumentUtility.CheckForNull<Uri>(uri, nameof (uri));
        ArgumentUtility.CheckStringForNullOrEmpty(subkeyName, nameof (subkeyName));
        string str = (string) null;
        RegisteredConfigurationServer configurationServerInternal = RegisteredTfsConnections.GetConfigurationServerInternal(uri);
        if (configurationServerInternal != null)
        {
          str = configurationServerInternal.RegistryKeyName;
          if (subkeyName.EndsWith("Collections", StringComparison.OrdinalIgnoreCase))
            return (RegistryKey) null;
        }
        else
        {
          RegisteredProjectCollection projectCollection = RegisteredTfsConnections.GetProjectCollection(uri);
          if (projectCollection != null)
            str = projectCollection.RegistryKeyName;
        }
        return str == null ? (RegistryKey) null : RegisteredTfsConnections.OpenCurrentUser(str + "\\" + subkeyName, false, false);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
        return (RegistryKey) null;
      }
    }

    internal static void SetOfflineInternal(string tpcRegKey, bool offline) => RegisteredTfsConnections.SetBoolAttributeInternal(tpcRegKey, "Offline", offline);

    internal static void SetAutoReconnectInternal(string tpcRegKey, bool autoReconnect) => RegisteredTfsConnections.SetBoolAttributeInternal(tpcRegKey, "AutoReconnect", autoReconnect);

    private static void SetBoolAttributeInternal(
      string tpcRegKey,
      string attributeName,
      bool value)
    {
      using (RegistryKey registryKey = RegisteredTfsConnections.OpenCurrentUser(tpcRegKey, true, false))
      {
        try
        {
          registryKey.SetValue(attributeName, (object) (value ? 1 : 0));
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException(ex);
        }
      }
    }

    private static string GetLegacyServerName(TfsTeamProjectCollection server) => UIHost.ExtractTFSDisplayName(server.Uri);

    private static void RegisterConfigurationServerInternal(
      Uri uri,
      string name,
      RegisteredTfsConnections.ServerType type,
      Guid instanceId,
      bool? isHosted)
    {
      using (RegistryKey registryKey = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances", true, true))
      {
        using (RegistryKey subKey = registryKey.CreateSubKey(name.Trim(), RegistryKeyPermissionCheck.ReadWriteSubTree))
        {
          subKey.SetValue("Uri", (object) uri.AbsoluteUri.ToLowerInvariant(), RegistryValueKind.String);
          subKey.SetValue("Type", (object) (int) type, RegistryValueKind.DWord);
          subKey.SetValue("InstanceId", (object) instanceId, RegistryValueKind.String);
          if (isHosted.HasValue)
            subKey.SetValue("IsHosted", (object) isHosted.ToString(), RegistryValueKind.String);
          subKey.DeleteValue("Deleted", false);
        }
      }
    }

    private static void RegisterCollectionInternal(
      string applicationName,
      Uri uri,
      string name,
      Guid instanceId)
    {
      using (RegistryKey registryKey = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances", true, true))
      {
        using (RegistryKey instanceKey = registryKey.OpenSubKey(applicationName.Trim(), true))
        {
          RegisteredTfsConnections.RemoveDuplicateCollections(instanceKey, uri, name);
          using (RegistryKey subKey = instanceKey.CreateSubKey("Collections\\" + name.Trim(), RegistryKeyPermissionCheck.ReadWriteSubTree))
          {
            subKey.SetValue("Uri", (object) uri.AbsoluteUri.ToLowerInvariant(), RegistryValueKind.String);
            subKey.SetValue("Type", (object) 1, RegistryValueKind.DWord);
            subKey.SetValue("InstanceId", (object) instanceId, RegistryValueKind.String);
          }
        }
      }
    }

    private static void RemoveDuplicateCollections(RegistryKey instanceKey, Uri uri, string name)
    {
      using (RegistryKey registryKey1 = instanceKey.OpenSubKey("Collections", true))
      {
        if (registryKey1 == null)
          return;
        string[] subKeyNames = registryKey1.GetSubKeyNames();
        for (int index = 0; subKeyNames != null && index < subKeyNames.Length; ++index)
        {
          if (!TFStringComparer.TFSName.Equals(subKeyNames[index].Trim(), name.Trim()))
          {
            bool flag = false;
            using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyNames[index], true))
            {
              try
              {
                if (UriUtility.Equals(new Uri(registryKey2.GetValue("Uri") as string), uri))
                  flag = true;
              }
              catch (Exception ex)
              {
                TeamFoundationTrace.TraceException(ex);
                flag = true;
              }
            }
            if (flag)
              registryKey1.DeleteSubKeyTree(subKeyNames[index]);
          }
        }
      }
    }

    private static void RemoveConfigurationServer(string name)
    {
      using (RegistryKey registryKey = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances", true, true))
        registryKey.DeleteSubKeyTree(name.Trim(), false);
    }

    private static void UnregisterConfigurationServerInternal(string name)
    {
      using (RegistryKey registryKey = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances\\" + name.Trim(), true, false))
        registryKey?.SetValue("Deleted", (object) bool.TrueString);
    }

    private static void UnregisterCollectionInternal(
      string configurationServerName,
      string collectionName)
    {
      using (RegistryKey registryKey1 = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances", true, true))
      {
        string[] subKeyNames1 = registryKey1.GetSubKeyNames();
        for (int index1 = 0; subKeyNames1 != null && index1 < subKeyNames1.Length; ++index1)
        {
          if (TFStringComparer.TFSName.Equals(subKeyNames1[index1].Trim(), configurationServerName.Trim()))
          {
            using (RegistryKey registryKey2 = registryKey1.OpenSubKey(subKeyNames1[index1] + "\\Collections", true))
            {
              if (registryKey2 == null)
                break;
              string[] subKeyNames2 = registryKey2.GetSubKeyNames();
              for (int index2 = 0; subKeyNames2 != null && index2 < subKeyNames2.Length; ++index2)
              {
                if (TFStringComparer.TFSName.Equals(subKeyNames2[index2].Trim(), collectionName.Trim()))
                {
                  registryKey2.DeleteSubKeyTree(subKeyNames2[index2]);
                  break;
                }
              }
              break;
            }
          }
        }
      }
    }

    private static RegisteredConfigurationServer[] GetConfigurationServersInternal()
    {
      List<RegisteredTfsConnections.ServerInfo> instancesInfos = RegisteredTfsConnections.GetInstancesInfos();
      List<RegisteredConfigurationServer> configurationServerList = new List<RegisteredConfigurationServer>();
      foreach (RegisteredTfsConnections.ServerInfo info in instancesInfos)
      {
        if (info.Type == RegisteredTfsConnections.ServerType.ConfigurationServer)
          configurationServerList.Add(new RegisteredConfigurationServer(info));
      }
      return configurationServerList.ToArray();
    }

    private static List<RegisteredProjectCollection> GetCollectionsInternal(
      bool returnOnlyLegacyServers)
    {
      List<RegisteredProjectCollection> collectionsInternal = new List<RegisteredProjectCollection>();
      foreach (RegisteredTfsConnections.ServerInfo instancesInfo in RegisteredTfsConnections.GetInstancesInfos())
      {
        if (!returnOnlyLegacyServers || instancesInfo.Type != RegisteredTfsConnections.ServerType.ConfigurationServer)
          collectionsInternal.AddRange((IEnumerable<RegisteredProjectCollection>) RegisteredTfsConnections.GetCollectionsInternal(instancesInfo.Name, instancesInfo.Type));
      }
      return collectionsInternal;
    }

    private static List<RegisteredProjectCollection> GetCollectionsInternal(
      string serverName,
      RegisteredTfsConnections.ServerType serverType)
    {
      List<RegisteredProjectCollection> collectionsInternal = new List<RegisteredProjectCollection>();
      using (RegistryKey key = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances\\" + serverName + "\\Collections", true, false))
      {
        List<RegisteredTfsConnections.ServerInfo> infos = RegisteredTfsConnections.GetInfos(key);
        foreach (RegisteredTfsConnections.ServerInfo info in infos)
        {
          if (serverType != RegisteredTfsConnections.ServerType.ProjectCollection)
            info.Name = !TFUtil.IsHostedServer(info.Uri) || TFUtil.IsLegacyHostedServer(info.Uri) ? serverName + "\\" + info.Name : serverName;
          info.DisplayName = infos.Count != 1 ? info.Name : serverName;
          RegisteredProjectCollection projectCollection = new RegisteredProjectCollection(info);
          collectionsInternal.Add(projectCollection);
        }
      }
      return collectionsInternal;
    }

    private static RegisteredConfigurationServer GetConfigurationServerInternal(string name)
    {
      foreach (RegisteredTfsConnections.ServerInfo instancesInfo in RegisteredTfsConnections.GetInstancesInfos())
      {
        if (instancesInfo.Type == RegisteredTfsConnections.ServerType.ConfigurationServer && VssStringComparer.ServerName.Equals(name.Trim(), instancesInfo.Name.Trim()))
          return new RegisteredConfigurationServer(instancesInfo);
      }
      return (RegisteredConfigurationServer) null;
    }

    private static RegisteredConfigurationServer GetConfigurationServerInternal(Uri uri)
    {
      foreach (RegisteredTfsConnections.ServerInfo instancesInfo in RegisteredTfsConnections.GetInstancesInfos())
      {
        if (instancesInfo.Type == RegisteredTfsConnections.ServerType.ConfigurationServer && UriUtility.Equals(uri, instancesInfo.Uri))
          return new RegisteredConfigurationServer(instancesInfo);
      }
      return (RegisteredConfigurationServer) null;
    }

    private static List<RegisteredTfsConnections.ServerInfo> GetInstancesInfos(bool includeDeleted = false)
    {
      using (RegistryKey key = RegisteredTfsConnections.OpenCurrentUser("TeamFoundation\\Instances", true, false))
      {
        List<RegisteredTfsConnections.ServerInfo> source = RegisteredTfsConnections.GetInfos(key);
        if (!includeDeleted)
          source = source.Where<RegisteredTfsConnections.ServerInfo>((Func<RegisteredTfsConnections.ServerInfo, bool>) (x => !x.Deleted)).ToList<RegisteredTfsConnections.ServerInfo>();
        return source;
      }
    }

    private static List<RegisteredTfsConnections.ServerInfo> GetInfos(RegistryKey key)
    {
      List<RegisteredTfsConnections.ServerInfo> infos = new List<RegisteredTfsConnections.ServerInfo>();
      if (key != null)
      {
        string[] subKeyNames = key.GetSubKeyNames();
        for (int index = 0; subKeyNames != null && index < subKeyNames.Length; ++index)
        {
          bool flag = false;
          using (RegistryKey registryKey = key.OpenSubKey(subKeyNames[index], true))
          {
            try
            {
              RegisteredTfsConnections.ServerInfo serverInfo1 = new RegisteredTfsConnections.ServerInfo()
              {
                Name = subKeyNames[index].Trim()
              };
              serverInfo1.DisplayName = serverInfo1.Name;
              serverInfo1.Uri = new Uri(registryKey.GetValue("Uri") as string);
              serverInfo1.InstanceId = new Guid(registryKey.GetValue("InstanceId") as string);
              string b1 = registryKey.GetValue("IsHosted", (object) null) as string;
              serverInfo1.IsHosted = !string.IsNullOrWhiteSpace(b1) ? new bool?(string.Equals(bool.TrueString, b1, StringComparison.OrdinalIgnoreCase)) : new bool?();
              string b2 = registryKey.GetValue("Deleted", (object) bool.FalseString) as string;
              serverInfo1.Deleted = string.Equals(bool.TrueString, b2, StringComparison.OrdinalIgnoreCase);
              object obj1 = registryKey.GetValue("Offline", (object) 0);
              serverInfo1.Offline = obj1 is int num1 && num1 != 0;
              object obj2 = registryKey.GetValue("AutoReconnect", (object) 1);
              serverInfo1.AutoReconnect = !(obj2 is 0);
              object obj3 = registryKey.GetValue("Type");
              serverInfo1.Type = obj3 == null || !(obj3 is int num2) ? RegisteredTfsConnections.ServerType.ProjectCollection : (RegisteredTfsConnections.ServerType) num2;
              serverInfo1.RegistryKeyName = registryKey.Name.Substring(RegisteredTfsConnections.CurrentUserRegistryRoot.Length).Trim('\\');
              foreach (RegisteredTfsConnections.ServerInfo serverInfo2 in infos)
              {
                if (TFStringComparer.TFSName.Equals(serverInfo2.Name, serverInfo1.Name))
                  throw new Exception(ClientResources.RegisteredInstancedDuplicateInstances());
              }
              infos.Add(serverInfo1);
            }
            catch (Exception ex)
            {
              TeamFoundationTrace.TraceException(ex);
              flag = true;
            }
          }
          if (flag)
            key.DeleteSubKeyTree(subKeyNames[index]);
        }
      }
      return infos;
    }

    private static RegistryKey OpenCurrentUser(string key, bool writable, bool shouldCreate)
    {
      using (RegistryKey userRegistryRoot = UIHost.UserRegistryRoot)
      {
        RegistryKey registryKey = userRegistryRoot.OpenSubKey(key, writable);
        if (registryKey == null & shouldCreate)
          registryKey = userRegistryRoot.CreateSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree);
        return registryKey;
      }
    }

    private static string CurrentUserRegistryRoot
    {
      get
      {
        if (RegisteredTfsConnections.m_currentUserRegistryRoot == null)
        {
          using (RegistryKey userRegistryRoot = UIHost.UserRegistryRoot)
            RegisteredTfsConnections.m_currentUserRegistryRoot = userRegistryRoot.Name;
        }
        return RegisteredTfsConnections.m_currentUserRegistryRoot;
      }
    }

    internal class ServerInfo
    {
      public string Name { get; set; }

      public string DisplayName { get; set; }

      public Uri Uri { get; set; }

      public RegisteredTfsConnections.ServerType Type { get; set; }

      public bool Offline { get; set; }

      public bool Deleted { get; set; }

      public bool AutoReconnect { get; set; }

      public string RegistryKeyName { get; set; }

      public Guid InstanceId { get; set; }

      public bool? IsHosted { get; set; }
    }

    internal enum ServerType
    {
      ConfigurationServer,
      ProjectCollection,
    }
  }
}
