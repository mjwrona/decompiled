// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Client.RegistrationService
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Client.Internal;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Principal;
using System.Threading;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Client
{
  internal class RegistrationService : IFrameworkRegistration, IRegistration
  {
    private static readonly int s_maxCountForRetry = 10;
    private static readonly TimeSpan s_maxTimeForRetry = TimeSpan.FromSeconds(5.0);
    private static readonly TimeSpan s_timeToSleepBetweenRetries = TimeSpan.FromMilliseconds(500.0);
    public static readonly string s_vstfs = "vstfs";
    public static readonly string s_instanceIdName = nameof (InstanceId);
    public static readonly string RegistrationRefreshDurationInSeconds = nameof (RegistrationRefreshDurationInSeconds);
    public static readonly string FileCacheName = "RegProxyFileCache.xml";
    public static readonly double DefaultRefreshDurationInSeconds = 7200.0;
    public static bool s_enableDiskCaching;
    public static readonly string RegistrationFileCachePath = nameof (RegistrationFileCachePath);
    private RegistrationProxy m_RegProxy;
    private string m_Url;
    private TfsTeamProjectCollection m_tfsObject;
    private ArrayList m_RegEntryList;
    private double m_RefreshDurationInSeconds;
    private DateTime m_LastRefreshTime;
    private string m_FileCachePath;
    private string m_instanceCacheDirectory;
    private static Dictionary<string, string> s_serverMap = (Dictionary<string, string>) null;
    private static readonly string s_serverMapFileName = "ServerMap.xml";
    private static string s_serverMapPath = (string) null;
    private static readonly string s_arrayOfEntryElement = "ArrayOfEntry";
    private static readonly string s_entryElement = "Entry";
    private static readonly string s_keyElement = "Key";
    private static readonly string s_valueElement = "Value";

    static RegistrationService()
    {
      try
      {
        RegistrationService.s_enableDiskCaching = "false" != (string) RegistrationUtilities.GetValueFromRegistry(RegistrationUtilities.BisRegistryPath, "RegistrationFileCacheEnabled");
        RegistrationService.s_serverMapPath = Path.Combine(TfsConnection.ClientCacheDirectory, RegistrationService.s_serverMapFileName);
      }
      catch (Exception ex)
      {
        RegistrationService.s_enableDiskCaching = false;
      }
    }

    private string ConstructRegistrationUrl() => UriUtility.Combine(this.m_tfsObject.Uri, "/Services/v1.0/Registration.asmx", true).AbsoluteUri;

    internal RegistrationService(TfsTeamProjectCollection tfsObject)
    {
      ArgumentUtility.CheckForNull<TfsTeamProjectCollection>(tfsObject, nameof (tfsObject));
      this.m_tfsObject = tfsObject;
      this.m_Url = this.ConstructRegistrationUrl();
      try
      {
        this.m_FileCachePath = this.GetFileCachePath();
        if (RegistrationService.s_enableDiskCaching)
          new FileInfo(this.m_FileCachePath).Directory.Create();
      }
      catch (TeamFoundationServerUnauthorizedException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        RegistrationService.s_enableDiskCaching = false;
      }
      this.m_RefreshDurationInSeconds = this.GetRefreshDurationInSeconds();
      this.m_LastRefreshTime = DateTime.MinValue;
      this.m_RegEntryList = (ArrayList) null;
    }

    public static void ExpandRelativeUrls(FrameworkRegistrationEntry[] entries, string tfsServerUrl)
    {
      tfsServerUrl = ProxyUtilities.GetServerUrl(tfsServerUrl);
      RegistrationProxy.ExpandRelativeUrls(entries, tfsServerUrl);
    }

    private void RefreshCachesIfNeeded(bool direct)
    {
      if (RegistrationService.s_enableDiskCaching)
      {
        this.RefreshDiskAndMemoryCacheIfNeeded(direct);
      }
      else
      {
        if (!direct && !this.IsTimeToRefreshCache())
          return;
        this.RefreshMemoryCache();
      }
    }

    RegistrationEntry[] IRegistration.GetRegistrationEntries(string toolId) => FrameworkRegistrationEntry.Convert(this.GetRegistrationEntries(toolId));

    RegistrationEntry[] IRegistration.GetRegistrationEntriesFromServer(string toolId) => FrameworkRegistrationEntry.Convert(this.GetRegistrationEntriesFromServer(toolId));

    Guid IRegistration.InstanceId => this.InstanceId;

    string IRegistration.InstanceClientCacheDirectory => this.InstanceClientCacheDirectory;

    public FrameworkRegistrationEntry[] GetRegistrationEntries(string toolId)
    {
      this.RefreshCachesIfNeeded(false);
      return this.GetRegistrationEntriesFromMemoryCache(toolId ?? string.Empty);
    }

    public FrameworkRegistrationEntry[] GetRegistrationEntriesFromServer(string toolId)
    {
      if (this.m_RegProxy == null)
        this.m_RegProxy = new RegistrationProxy(this.m_tfsObject, this.m_Url);
      return this.m_RegProxy.GetRegistrationEntries(toolId ?? string.Empty);
    }

    private FrameworkRegistrationEntry[] GetRegistrationEntriesFromMemoryCache(string toolId)
    {
      if (toolId.Length == 0)
        return (FrameworkRegistrationEntry[]) this.m_RegEntryList.ToArray(typeof (FrameworkRegistrationEntry));
      if (!RegistrationUtilities.IsToolType(toolId))
        throw new ArgumentException(ClientResources.ToolIdMalformed((object) toolId));
      ArrayList arrayList = new ArrayList();
      foreach (FrameworkRegistrationEntry regEntry in this.m_RegEntryList)
      {
        if (VssStringComparer.XmlNodeName.Equals(regEntry.Type, toolId))
          arrayList.Add((object) regEntry);
      }
      return (FrameworkRegistrationEntry[]) arrayList.ToArray(typeof (FrameworkRegistrationEntry));
    }

    private bool IsTimeToRefreshCache() => DateTime.Compare(DateTime.UtcNow, this.m_LastRefreshTime.AddSeconds(this.m_RefreshDurationInSeconds)) >= 0;

    private void RefreshDiskAndMemoryCacheIfNeeded(bool direct)
    {
      lock (this)
      {
        XmlNode node = (XmlNode) null;
        FileInfo fileInfo = new FileInfo(this.m_FileCachePath);
        bool flag1 = true;
        bool flag2 = !this.m_LastRefreshTime.Equals(DateTime.MinValue);
        if (((!flag2 ? 1 : (this.m_RegEntryList == null ? 1 : 0)) | (direct ? 1 : 0)) != 0)
        {
          try
          {
            if (!fileInfo.Exists)
            {
              flag1 = false;
            }
            else
            {
              XmlReaderSettings settings = new XmlReaderSettings()
              {
                DtdProcessing = DtdProcessing.Prohibit,
                XmlResolver = (XmlResolver) null
              };
              using (TextReader input = TextReader.Synchronized((TextReader) fileInfo.OpenText()))
              {
                using (XmlReader reader = XmlReader.Create(input, settings))
                {
                  XmlDocument xmlDocument = new XmlDocument();
                  xmlDocument.Load(reader);
                  node = xmlDocument.SelectSingleNode("RegistrationEntries");
                  if (node == null)
                    flag1 = false;
                }
              }
            }
          }
          catch (FileNotFoundException ex)
          {
            flag1 = false;
          }
          catch (Exception ex)
          {
            TeamFoundationTrace.TraceException(TraceKeywordSets.General, ex);
            flag1 = false;
          }
        }
        bool flag3;
        if (!flag2 & flag1)
        {
          try
          {
            this.m_LastRefreshTime = new DateTime(Convert.ToInt64(RegistrationXmlSerializer.GetTerminalNodeValue(node, "LastRefreshTime"), (IFormatProvider) CultureInfo.InvariantCulture), DateTimeKind.Utc);
          }
          catch
          {
            flag3 = false;
          }
        }
        if (!this.IsTimeToRefreshCache() && this.m_RegEntryList == null)
        {
          if (!direct)
          {
            try
            {
              XmlNodeList xmlNodeList = node.SelectNodes("RegistrationEntry");
              ArrayList arrayList = new ArrayList();
              foreach (XmlNode root in xmlNodeList)
              {
                FrameworkRegistrationEntry registrationEntry = (FrameworkRegistrationEntry) FrameworkRegistrationXmlSerializer.Deserialize(root, typeof (FrameworkRegistrationEntry));
                arrayList.Add((object) registrationEntry);
              }
              this.m_RegEntryList = arrayList;
            }
            catch
            {
              this.m_LastRefreshTime = DateTime.MinValue;
              flag3 = false;
            }
          }
        }
        if (!(this.IsTimeToRefreshCache() | direct))
          return;
        this.RefreshMemoryCache();
        this.m_LastRefreshTime = DateTime.UtcNow;
        try
        {
          if (fileInfo.Exists)
            fileInfo.Delete();
          using (TextWriter w = TextWriter.Synchronized((TextWriter) fileInfo.CreateText()))
          {
            using (XmlTextWriter xmlTextWriter = new XmlTextWriter(w))
            {
              xmlTextWriter.Formatting = Formatting.Indented;
              xmlTextWriter.WriteStartElement("RegistrationEntries");
              foreach (FrameworkRegistrationEntry regEntry in this.m_RegEntryList)
                FrameworkRegistrationXmlSerializer.Serialize(regEntry, (XmlWriter) xmlTextWriter);
              xmlTextWriter.WriteStartElement("LastRefreshTime");
              xmlTextWriter.WriteString(Convert.ToString(this.m_LastRefreshTime.Ticks, (IFormatProvider) CultureInfo.InvariantCulture));
              xmlTextWriter.WriteEndElement();
              xmlTextWriter.WriteStartElement("BisDomainUrl");
              xmlTextWriter.WriteString(this.m_Url);
              xmlTextWriter.WriteEndElement();
              xmlTextWriter.WriteEndElement();
            }
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.Error("Exception while saving servermap.xml", ex);
        }
      }
    }

    private void RefreshMemoryCache()
    {
      if (this.m_RegProxy == null)
        this.m_RegProxy = new RegistrationProxy(this.m_tfsObject, this.m_Url);
      this.m_RegEntryList = new ArrayList((ICollection) this.m_RegProxy.GetRegistrationEntries(string.Empty));
      this.m_LastRefreshTime = DateTime.UtcNow;
    }

    internal void Refresh(bool force) => this.RefreshCachesIfNeeded(force);

    private double GetRefreshDurationInSeconds()
    {
      try
      {
        object valueFromRegistry = RegistrationUtilities.GetValueFromRegistry(RegistrationUtilities.BisRegistryPath, RegistrationService.RegistrationRefreshDurationInSeconds);
        if (valueFromRegistry != null)
        {
          if (valueFromRegistry.GetType() == typeof (string))
            return Convert.ToDouble((string) valueFromRegistry, (IFormatProvider) CultureInfo.InvariantCulture);
          return valueFromRegistry.GetType() == typeof (int) ? Convert.ToDouble((int) valueFromRegistry) : RegistrationService.DefaultRefreshDurationInSeconds;
        }
      }
      catch (SecurityException ex)
      {
        WindowsIdentity current = WindowsIdentity.GetCurrent();
        if (current != null)
        {
          if (current.ImpersonationLevel == TokenImpersonationLevel.Impersonation)
            goto label_9;
        }
        throw;
      }
label_9:
      return RegistrationService.DefaultRefreshDurationInSeconds;
    }

    private string GetFileCachePath() => Path.Combine(this.InstanceClientCacheDirectory, RegistrationService.FileCacheName);

    public string InstanceClientCacheDirectory
    {
      get
      {
        if (this.m_instanceCacheDirectory == null)
          this.m_instanceCacheDirectory = TfsClientCacheUtility.GetCacheDirectory(this.m_tfsObject.Uri, this.InstanceId);
        return this.m_instanceCacheDirectory;
      }
    }

    public Guid InstanceId => this.GetInstanceId();

    private Guid GetInstanceId()
    {
      if (RegistrationService.s_serverMap == null)
      {
        if (RegistrationService.s_enableDiskCaching)
          RegistrationService.LoadServerMap();
        if (RegistrationService.s_serverMap == null)
          RegistrationService.s_serverMap = new Dictionary<string, string>();
      }
      if (RegistrationService.s_serverMap.ContainsKey(this.m_Url))
        return new Guid(RegistrationService.s_serverMap[this.m_Url]);
      string g = (string) null;
      foreach (FrameworkRegistrationEntry registrationEntry in new RegistrationProxy(this.m_tfsObject, this.m_Url).GetRegistrationEntries(RegistrationService.s_vstfs))
      {
        foreach (RegistrationExtendedAttribute2 extendedAttribute in registrationEntry.RegistrationExtendedAttributes)
        {
          if (TFStringComparer.InstanceId.Equals(extendedAttribute.Name, RegistrationService.s_instanceIdName))
          {
            g = extendedAttribute.Value.Trim();
            break;
          }
        }
      }
      if (g == null)
        g = Guid.Empty.ToString();
      RegistrationService.s_serverMap[this.m_Url] = g;
      if (RegistrationService.s_enableDiskCaching)
      {
        try
        {
          RegistrationService.SaveServerMap();
        }
        catch (IOException ex)
        {
          TeamFoundationTrace.Error("Exception while saving servermap.xml", (Exception) ex);
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.Error("Exception while saving servermap.xml", ex);
          throw;
        }
      }
      return new Guid(g);
    }

    private static void LoadServerMap()
    {
      if (RegistrationService.s_serverMap == null)
        RegistrationService.s_serverMap = new Dictionary<string, string>();
      else
        RegistrationService.s_serverMap.Clear();
      FileStream file = (FileStream) null;
      try
      {
        XmlDocument xmlDocument = TFCommonUtil.OpenXmlFile(out file, RegistrationService.s_serverMapPath, FileShare.Read, false);
        if (xmlDocument == null)
          return;
        XmlNode xmlNode = xmlDocument.SelectSingleNode(RegistrationService.s_arrayOfEntryElement);
        if (xmlNode == null)
          return;
        foreach (XmlNode childNode1 in xmlNode.ChildNodes)
        {
          if (StringComparer.OrdinalIgnoreCase.Equals(childNode1.Name, RegistrationService.s_entryElement))
          {
            string key = (string) null;
            string str = (string) null;
            foreach (XmlNode childNode2 in childNode1.ChildNodes)
            {
              if (StringComparer.OrdinalIgnoreCase.Equals(childNode2.Name, RegistrationService.s_keyElement))
              {
                XmlNode firstChild = childNode2.FirstChild;
                if (firstChild != null && firstChild is XmlText)
                  key = firstChild.Value;
              }
              else if (StringComparer.OrdinalIgnoreCase.Equals(childNode2.Name, RegistrationService.s_valueElement))
              {
                XmlNode firstChild = childNode2.FirstChild;
                if (firstChild != null && firstChild is XmlText)
                  str = firstChild.Value;
              }
            }
            if (key != null && str != null)
              RegistrationService.s_serverMap[key] = str;
          }
        }
      }
      catch
      {
      }
      finally
      {
        file?.Close();
      }
    }

    private static void SaveServerMap()
    {
      Directory.CreateDirectory(TfsConnection.ClientCacheDirectory);
      using (FileStream outStream = RegistrationService.OpenFileStream(RegistrationService.s_serverMapPath, FileMode.Create, FileAccess.Write))
      {
        XmlDocument xmlDocument = new XmlDocument();
        XmlNode node1 = xmlDocument.CreateNode(XmlNodeType.Element, RegistrationService.s_arrayOfEntryElement, (string) null);
        xmlDocument.AppendChild(node1);
        foreach (KeyValuePair<string, string> server in RegistrationService.s_serverMap)
        {
          XmlNode node2 = xmlDocument.CreateNode(XmlNodeType.Element, RegistrationService.s_entryElement, (string) null);
          node1.AppendChild(node2);
          XmlNode node3 = xmlDocument.CreateNode(XmlNodeType.Element, RegistrationService.s_keyElement, (string) null);
          node3.InnerText = server.Key;
          node2.AppendChild(node3);
          XmlNode node4 = xmlDocument.CreateNode(XmlNodeType.Element, RegistrationService.s_valueElement, (string) null);
          node4.InnerText = server.Value;
          node2.AppendChild(node4);
        }
        xmlDocument.Save((Stream) outStream);
      }
    }

    private static FileStream OpenFileStream(string filePath, FileMode mode, FileAccess access)
    {
      DateTime dateTime = DateTime.UtcNow + RegistrationService.s_maxTimeForRetry;
      int num = 0;
      while (true)
      {
        try
        {
          return new FileStream(filePath, mode, access, FileShare.Read);
        }
        catch (IOException ex)
        {
          if (DateTime.UtcNow < dateTime && ++num < RegistrationService.s_maxCountForRetry)
            Thread.Sleep(RegistrationService.s_timeToSleepBetweenRetries);
          else
            throw;
        }
      }
    }
  }
}
