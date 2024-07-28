// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.RegistrationUtilities
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;

namespace Microsoft.TeamFoundation.Server
{
  public static class RegistrationUtilities
  {
    public static readonly string BisRegistryPath = "TeamFoundation";
    public const string BisName = "vstfs";
    public const string RosettaName = "Reports";
    public const string SharePointName = "Wss";
    public const string RegRegistrationServiceUrl = "RegistrationService";

    public static string GetServiceUrlForTool(
      TfsConnection server,
      string toolName,
      string interfaceName)
    {
      string str = (string) null;
      ILocationService service = server.GetService<ILocationService>();
      if (service != null && service.LocationForCurrentConnection("LocationService", FrameworkServiceIdentifiers.Location) != null)
      {
        foreach (ServiceDefinition serviceDefinition in service.FindServiceDefinitionsByToolType(toolName))
        {
          if (VssStringComparer.ServiceType.Equals(interfaceName, serviceDefinition.ServiceType))
          {
            str = service.LocationForCurrentConnection(serviceDefinition);
            break;
          }
        }
      }
      if (string.IsNullOrEmpty(str))
      {
        ServiceInterface[] interfacesForTool = RegistrationUtilities.GetServiceInterfacesForTool((IRegistration) server.GetService(typeof (IRegistration)), toolName);
        ServiceInterface serviceInterface = (ServiceInterface) null;
        for (int index = 0; index < interfacesForTool.Length; ++index)
        {
          if (VssStringComparer.ServiceInterface.Equals(interfacesForTool[index].Name, interfaceName))
          {
            serviceInterface = interfacesForTool[index];
            break;
          }
        }
        str = serviceInterface != null ? serviceInterface.Url : throw new TeamFoundationServerException(ClientResources.NoServiceInterfaceByName((object) interfaceName, (object) toolName));
      }
      return !string.IsNullOrEmpty(str) ? str : throw new TeamFoundationServerException(ClientResources.NullServiceUrl((object) interfaceName, (object) toolName));
    }

    public static string GetServiceUrlForTool(TfsConnection server, string toolName)
    {
      ServiceInterface[] interfacesForTool = RegistrationUtilities.GetServiceInterfacesForTool((IRegistration) server.GetService(typeof (IRegistration)), toolName);
      string str = interfacesForTool.Length == 1 ? interfacesForTool[0].Url : throw new TeamFoundationServerException(ClientResources.MoreThanOneServiceInstance((object) toolName));
      return !string.IsNullOrEmpty(str) ? str : throw new TeamFoundationServerException(ClientResources.NullOrEmptyServiceInterface((object) toolName, (object) interfacesForTool[0].Name));
    }

    private static ServiceInterface[] GetServiceInterfacesForTool(
      IRegistration regProxy,
      string toolName)
    {
      RegistrationEntry[] registrationEntries = regProxy.GetRegistrationEntries(toolName);
      ServiceInterface[] serviceInterfaceArray = registrationEntries != null && registrationEntries.Length >= 1 ? registrationEntries[0].ServiceInterfaces : throw new TeamFoundationServerException(ClientResources.NoRegistrationEntries((object) toolName));
      return serviceInterfaceArray != null && serviceInterfaceArray.Length != 0 ? serviceInterfaceArray : throw new TeamFoundationServerException(ClientResources.NoServiceInterfaces((object) toolName));
    }

    public static int Compare(string str1, string str2) => RegistrationUtilities.Compare(str1, str2, true);

    public static int Compare(string str1, string str2, bool caseInsensitiveFlag)
    {
      string x = (string) null;
      string y = (string) null;
      if (str1 != null)
        x = str1.Trim();
      if (str2 != null)
        y = str2.Trim();
      return caseInsensitiveFlag ? VssStringComparer.RegistrationUtilitiesCaseInsensitive.Compare(x, y) : VssStringComparer.RegistrationUtilities.Compare(x, y);
    }

    public static bool IsToolTypeWellFormed(string tool) => tool != null && tool.IndexOf("/", StringComparison.Ordinal) < 0 && tool.IndexOf("\\", StringComparison.Ordinal) < 0 && tool.IndexOf(".", StringComparison.Ordinal) < 0 && tool.Length != 0;

    public static bool IsToolType(string toolId) => RegistrationUtilities.IsToolTypeWellFormed(toolId);

    public static string GetServiceInterfaceUrl(
      RegistrationEntry registrationEntry,
      string serviceInterfaceName)
    {
      ServiceInterface[] serviceInterfaces = registrationEntry.ServiceInterfaces;
      string serviceInterfaceUrl = (string) null;
      for (int index = 0; index < serviceInterfaces.Length; ++index)
      {
        if (VssStringComparer.ServiceInterface.Equals(serviceInterfaces[index].Name, serviceInterfaceName))
          serviceInterfaceUrl = serviceInterfaces[index].Url;
      }
      return serviceInterfaceUrl;
    }

    internal static object GetValueFromRegistry(string registryKeyPath, string name) => ELeadRegistry.GetValueFromRegistry("Software\\Microsoft\\VisualStudio\\17.0\\" + registryKeyPath, name);

    internal static void GetNullElementIndexes(
      object[] arr,
      out int[] nullElementIndices,
      out object[] filteredArr)
    {
      ArrayList arrayList1 = new ArrayList();
      ArrayList arrayList2 = new ArrayList();
      for (int index = 0; index < arr.Length; ++index)
      {
        if (arr[index] == null)
          arrayList1.Add((object) index);
        else
          arrayList2.Add(arr[index]);
      }
      nullElementIndices = (int[]) arrayList1.ToArray(typeof (int));
      filteredArr = (object[]) arrayList2.ToArray(typeof (object));
    }

    internal static object[] MergeNullElements(object[] arr, int[] nullElementIndices)
    {
      object[] objArray = new object[arr.Length + nullElementIndices.Length];
      int num = 0;
      int index1 = 0;
      for (int index2 = 0; index2 < objArray.Length; ++index2)
      {
        if (index1 < nullElementIndices.Length && index2 == nullElementIndices[index1])
          ++index1;
        else
          objArray[index2] = arr[num++];
      }
      return objArray;
    }

    public static string GetConnectionString(RegistrationEntry entry, string databaseName)
    {
      if (entry == null || entry.Databases == null || string.IsNullOrEmpty(databaseName))
        return string.Empty;
      foreach (Database database in entry.Databases)
      {
        if (database.Name != null && VssStringComparer.DatabaseName.Equals(database.Name.Trim(), databaseName.Trim()))
        {
          string connectionString = database.ConnectionString;
          if (!string.IsNullOrEmpty(connectionString))
            connectionString = connectionString.Replace(RegistrationUtilities.ReplacementProperty("DatabaseName"), database.DatabaseName).Replace(RegistrationUtilities.ReplacementProperty("SQLServerName"), database.SQLServerName);
          return connectionString;
        }
      }
      return string.Empty;
    }

    private static string ReplacementProperty(string property) => "@" + property + "@";
  }
}
