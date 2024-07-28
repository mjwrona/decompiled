// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.Office.OfficeInstall
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client.ProjectSettings;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.Client.Office
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class OfficeInstall
  {
    public const string Version12 = "12.0";
    private const string MinRequiredOfficeVersion = "12.0";
    private const string CurVerSuffix = "\\CurVer";
    private const string ExcelVersionIndependentProgId = "Excel.Application";
    private const string OutlookVersionIndependentProgId = "Outlook.Application";
    private const string OfficeIntegrationProductName = "officeIntegration";
    private const string InstallKeyName = "Install";
    private const string OfficeIntegrationServicingPath = "SOFTWARE\\Microsoft\\DevDiv\\tfs\\Servicing";
    private const string PackagedComProgIdRoot = "PackagedCom\\ProgIdIndex";
    private static ConcurrentDictionary<string, bool> s_isInstalledCache = new ConcurrentDictionary<string, bool>();

    public static bool CanCreateExcelReport(TfsTeamProjectCollection teamProjectCollection) => teamProjectCollection.CatalogNode != null && OfficeInstall.IsExcelInstalled() && ReportingSettings.IsReportingCubeConfigured(teamProjectCollection, out string _, out string _, out string _);

    public static bool IsExcelInstalled() => OfficeInstall.IsOfficeAppInstalled("Excel.Application") && OfficeInstall.IsOfficeIntegrationAddinInstalled();

    public static bool IsProjectInstalled() => false;

    public static bool IsOutlookInstalled() => OfficeInstall.IsOfficeAppInstalled("Outlook.Application") && OfficeInstall.IsOfficeIntegrationAddinInstalled();

    public static bool IsPowerPointInstalled() => false;

    private static bool IsOfficeAppInstalled(
      string versionIndependentProgId,
      string minRequiredVersion = "12.0")
    {
      bool flag1 = false;
      if (OfficeInstall.s_isInstalledCache.TryGetValue(versionIndependentProgId, out flag1))
        return flag1;
      try
      {
        bool writable = false;
        bool flag2 = false;
        string str1 = versionIndependentProgId + ".";
        string name = versionIndependentProgId + "\\CurVer";
        using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(name, writable))
        {
          if (registryKey != null)
          {
            flag2 = true;
            string str2 = registryKey.GetValue(string.Empty) as string;
            if (!string.IsNullOrEmpty(str2))
            {
              if (str2.StartsWith(str1, StringComparison.OrdinalIgnoreCase))
                flag1 = OfficeInstall.CompareVersions(str2.Substring(str1.Length), minRequiredVersion) >= 0;
            }
          }
        }
        if (!flag2)
        {
          flag1 = Type.GetTypeFromProgID(versionIndependentProgId) != (Type) null;
          if (!flag1)
            flag1 = Type.GetTypeFromProgID(versionIndependentProgId + ".16") != (Type) null;
        }
        OfficeInstall.s_isInstalledCache[versionIndependentProgId] = flag1;
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      return flag1;
    }

    public static int CompareVersions(string v1, string v2)
    {
      if (v1 == null && v2 == null)
        return 0;
      if (v1 == null)
        return -1;
      if (v2 == null)
        return 1;
      if (v1.Equals(v2))
        return 0;
      char[] chArray = new char[1]{ '.' };
      string[] strArray1 = v1.Split(chArray);
      string[] strArray2 = v2.Split(chArray);
      int num1 = Math.Max(strArray1.GetUpperBound(0), strArray2.GetUpperBound(0)) + 1;
      for (int index = 0; index < num1; ++index)
      {
        string str1 = (string) null;
        string str2 = (string) null;
        if (index <= strArray1.GetUpperBound(0))
          str1 = strArray1[index];
        if (index <= strArray2.GetUpperBound(0))
          str2 = strArray2[index];
        if (string.IsNullOrEmpty(str1))
          str1 = "0";
        if (string.IsNullOrEmpty(str2))
          str2 = "0";
        if (!str1.Equals(str2))
        {
          try
          {
            int num2 = int.Parse(str1, (IFormatProvider) CultureInfo.InvariantCulture);
            int num3 = int.Parse(str2, (IFormatProvider) CultureInfo.InvariantCulture);
            if (num2 < num3)
              return -1;
            if (num2 > num3)
              return 1;
          }
          catch (FormatException ex)
          {
            int num4 = TFStringComparer.OfficeVersions.Compare(str1, str2);
            if (num4 != 0)
              return num4;
          }
        }
      }
      return 0;
    }

    private static bool IsOfficeIntegrationAddinInstalled() => OfficeInstall.IsOfficeIntegrationAddinInstalled("officeIntegration");

    private static bool IsOfficeIntegrationAddinInstalled(string productName)
    {
      bool flag = false;
      try
      {
        using (RegistryKey registryKey1 = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\DevDiv\\tfs\\Servicing"))
        {
          if (registryKey1 != null)
          {
            string[] subKeyNames = registryKey1.GetSubKeyNames();
            if (subKeyNames != null)
            {
              if (subKeyNames.Length != 0)
              {
                foreach (string str in subKeyNames)
                {
                  if (string.Equals(str, "17.0", StringComparison.OrdinalIgnoreCase))
                  {
                    using (RegistryKey registryKey2 = registryKey1.OpenSubKey(str))
                    {
                      if (registryKey2 != null)
                      {
                        using (RegistryKey registryKey3 = registryKey2.OpenSubKey(productName))
                        {
                          if (registryKey3 != null)
                          {
                            object obj = registryKey3.GetValue("Install");
                            if (obj != null)
                            {
                              if (Convert.ToInt32(obj) == 1)
                              {
                                flag = true;
                                break;
                              }
                            }
                          }
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException(ex);
      }
      return flag;
    }
  }
}
