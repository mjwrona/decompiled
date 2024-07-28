// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Server.ManifestStorageService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 57F50803-C5C4-41A9-A26F-AD293D563111
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ExtensionManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Server
{
  internal class ManifestStorageService : IManifestStorageService, IVssFrameworkService
  {
    private const string s_area = "ManifestStorageService";
    private const string s_layer = "Service";
    private object m_lock = new object();
    public static readonly char[] RegistrySeparators = new char[1]
    {
      '/'
    };
    private const string ManifestRegistryPath = "/Service/Extensions/Manifests";
    private const string ManifestExtensionRegistryPath = "/Service/Extensions/Manifests/{0}/{1}/**";
    private const string SingleManifestExtensionRegistryPath = "/Service/Extensions/Manifests/{0}/{1}/{2}/{3}/**";
    private Dictionary<string, List<ManifestStorageData>> m_manifestStorageData;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
      this.m_manifestStorageData = (Dictionary<string, List<ManifestStorageData>>) null;
      requestContext.GetService<CachedRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/Extensions/Manifests/...");
      this.LoadExtensionData(requestContext);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => requestContext.GetService<CachedRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));

    private Dictionary<string, List<ManifestStorageData>> GetExtensionData(
      IVssRequestContext requestContext)
    {
      Dictionary<string, List<ManifestStorageData>> extensionData;
      if ((extensionData = this.m_manifestStorageData) == null)
        extensionData = this.LoadExtensionData(requestContext);
      return extensionData;
    }

    private Dictionary<string, List<ManifestStorageData>> LoadExtensionData(
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.Trace(10013510, TraceLevel.Info, nameof (ManifestStorageService), "Service", "Loading extension manifest content.");
      RegistryEntryCollection registryEntryCollection = vssRequestContext.GetService<CachedRegistryService>().ReadEntries(vssRequestContext, (RegistryQuery) string.Format("{0}/**", (object) "/Service/Extensions/Manifests"));
      Dictionary<string, List<ManifestStorageData>> dictionary = new Dictionary<string, List<ManifestStorageData>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        string[] strArray = registryEntry.Path.Split(ManifestStorageService.RegistrySeparators, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 8)
        {
          ManifestStorageData manifestStorageData = new ManifestStorageData();
          manifestStorageData.PublisherName = strArray[3];
          manifestStorageData.ExtensionName = strArray[4];
          manifestStorageData.Version = strArray[5];
          manifestStorageData.Language = strArray[6];
          int result = 0;
          if (int.TryParse(registryEntry.Value, out result))
          {
            manifestStorageData.FileId = result;
            string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(manifestStorageData.PublisherName, manifestStorageData.ExtensionName);
            List<ManifestStorageData> manifestStorageDataList;
            if (!dictionary.TryGetValue(fullyQualifiedName, out manifestStorageDataList))
            {
              manifestStorageDataList = new List<ManifestStorageData>();
              dictionary.Add(fullyQualifiedName, manifestStorageDataList);
            }
            manifestStorageDataList.Add(manifestStorageData);
          }
        }
        else
          vssRequestContext.Trace(10013515, TraceLevel.Error, nameof (ManifestStorageService), "Service", "Invalid registry entry: {0}", (object) registryEntry.Path);
      }
      this.m_manifestStorageData = dictionary;
      return dictionary;
    }

    public void DeleteStorageItems(
      IVssRequestContext requestContext,
      IEnumerable<ManifestStorageData> storageData)
    {
      if (storageData == null)
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      List<int> intList = new List<int>();
      List<string> registryPathPatterns = new List<string>();
      foreach (ManifestStorageData manifestStorageData in storageData)
      {
        intList.Add(manifestStorageData.FileId);
        registryPathPatterns.Add(string.Format("/Service/Extensions/Manifests/{0}/{1}/{2}/{3}/**", (object) manifestStorageData.PublisherName, (object) manifestStorageData.ExtensionName, (object) manifestStorageData.Version, (object) manifestStorageData.Language));
      }
      if (intList.Count<int>() <= 0)
        return;
      vssRequestContext.GetService<IVssRegistryService>().DeleteEntries(vssRequestContext, (IEnumerable<string>) registryPathPatterns);
      vssRequestContext.GetService<ITeamFoundationFileService>().DeleteFiles(vssRequestContext, (IEnumerable<int>) intList);
    }

    public void DeleteManifests(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      Dictionary<string, List<ManifestStorageData>> extensionData = this.GetExtensionData(vssRequestContext);
      List<ManifestStorageData> manifestStorageDataList;
      if (extensionData == null || !extensionData.TryGetValue(GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName), out manifestStorageDataList))
        return;
      List<int> fileIds = new List<int>();
      foreach (ManifestStorageData manifestStorageData in manifestStorageDataList)
        fileIds.Add(manifestStorageData.FileId);
      vssRequestContext.GetService<IVssRegistryService>().DeleteEntries(vssRequestContext, string.Format("/Service/Extensions/Manifests/{0}/{1}/**", (object) publisherName, (object) extensionName));
      vssRequestContext.GetService<ITeamFoundationFileService>().DeleteFiles(vssRequestContext, (IEnumerable<int>) fileIds);
    }

    public bool TryGetManifest(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string language,
      IDictionary<string, object> extraProperties,
      out ExtensionManifest extensionManifest)
    {
      extensionManifest = (ExtensionManifest) null;
      bool manifest = false;
      try
      {
        requestContext.Trace(10013520, TraceLevel.Info, nameof (ManifestStorageService), "Service", "Attempting to load manifest: {0}.{1} {2} {3}", (object) publisherName, (object) extensionName, (object) version, (object) language);
        Dictionary<string, List<ManifestStorageData>> extensionData = this.GetExtensionData(requestContext);
        List<ManifestStorageData> manifestStorageDataList;
        if (extensionData != null && extensionData.TryGetValue(GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName), out manifestStorageDataList))
        {
          ManifestStorageData manifestStorageData1 = (ManifestStorageData) null;
          foreach (ManifestStorageData manifestStorageData2 in manifestStorageDataList)
          {
            if (manifestStorageData2.Version.Equals(version, StringComparison.OrdinalIgnoreCase) && manifestStorageData2.Language.Equals(language))
            {
              requestContext.Trace(10013525, TraceLevel.Info, nameof (ManifestStorageService), "Service", "Found manifest for {0}.{1} {2} {3}", (object) publisherName, (object) extensionName, (object) version, (object) language);
              manifestStorageData1 = manifestStorageData2;
              break;
            }
          }
          if (manifestStorageData1 != null)
          {
            using (Stream manifestStream = requestContext.To(TeamFoundationHostType.Deployment).Elevate().GetService<ITeamFoundationFileService>().RetrieveFile(requestContext, (long) manifestStorageData1.FileId, false, out byte[] _, out long _, out CompressionType _))
            {
              extensionManifest = ExtensionUtil.LoadManifest(manifestStorageData1.PublisherName, manifestStorageData1.ExtensionName, version, manifestStream, extraProperties, true);
              manifest = true;
            }
          }
          else
            requestContext.Trace(10013530, TraceLevel.Info, nameof (ManifestStorageService), "Service", "No manifest found for {0}.{1} {2} {3}", (object) publisherName, (object) extensionName, (object) version, (object) language);
        }
        else
          requestContext.Trace(10013531, TraceLevel.Info, nameof (ManifestStorageService), "Service", "No manifest found for {0}.{1} {2} {3}", (object) publisherName, (object) extensionName, (object) version, (object) language);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013535, nameof (ManifestStorageService), "Service", ex);
      }
      return manifest;
    }

    public void StoreManifest(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      string language,
      ExtensionFile manifestFile)
    {
      requestContext.Trace(10013540, TraceLevel.Info, nameof (ManifestStorageService), "Service", "StoreManifest:: Storing manifest: {0}.{1} {2} {3}", (object) publisherName, (object) extensionName, (object) version, (object) language);
      ManifestStorageService.ManifestStorageRefreshData taskArgs = new ManifestStorageService.ManifestStorageRefreshData();
      taskArgs.PublisherName = publisherName;
      taskArgs.ExtensionName = extensionName;
      taskArgs.Version = version;
      taskArgs.Language = language;
      taskArgs.ManifestFile = manifestFile;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      vssRequestContext.GetService<TeamFoundationTaskService>().AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.UpdateExtensionContent), (object) taskArgs, 0));
    }

    public Dictionary<string, List<ManifestStorageData>> GetStorageData(
      IVssRequestContext requestContext)
    {
      return this.LoadExtensionData(requestContext);
    }

    private void UpdateExtensionContent(IVssRequestContext requestContext, object taskArgs)
    {
      if (!(taskArgs is ManifestStorageService.ManifestStorageRefreshData storageRefreshData))
        return;
      requestContext.Trace(10013545, TraceLevel.Info, nameof (ManifestStorageService), "Service", "UpdateExtensionContent:: Storing manifest: {0}.{1} {2} {3}", (object) storageRefreshData.PublisherName, (object) storageRefreshData.ExtensionName, (object) storageRefreshData.Version, (object) storageRefreshData.Language);
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      ITeamFoundationFileService service1 = vssRequestContext.GetService<ITeamFoundationFileService>();
      IPublishedExtensionCache service2 = vssRequestContext.GetService<IPublishedExtensionCache>();
      int num = 0;
      IVssRequestContext requestContext1 = vssRequestContext;
      ExtensionFile manifestFile = storageRefreshData.ManifestFile;
      using (Stream content = service2.GetExtensionAsset(requestContext1, manifestFile).SyncResult<Stream>())
        num = service1.UploadFile(vssRequestContext, content, OwnerId.Generic, Guid.Empty);
      if (num > 0)
      {
        requestContext.Trace(10013550, TraceLevel.Info, nameof (ManifestStorageService), "Service", "UpdateExtensionContent:: Manifest stored to file storage: {0}.{1} {2} {3}", (object) storageRefreshData.PublisherName, (object) storageRefreshData.ExtensionName, (object) storageRefreshData.Version, (object) storageRefreshData.Language);
        string str = string.Format("/Service/Extensions/Manifests/{0}/{1}/{2}/{3}/FileId", (object) storageRefreshData.PublisherName, (object) storageRefreshData.ExtensionName, (object) storageRefreshData.Version, (object) storageRefreshData.Language);
        RegistryQuery query = new RegistryQuery(str);
        IVssRegistryService service3 = vssRequestContext.GetService<IVssRegistryService>();
        int fileId = -1;
        lock (this.m_lock)
        {
          fileId = service3.GetValue<int>(vssRequestContext, in query, -1);
          service3.SetValue<int>(vssRequestContext, str, num);
        }
        if (fileId == -1)
          return;
        service1.DeleteFile(vssRequestContext, (long) fileId);
      }
      else
        requestContext.Trace(10013555, TraceLevel.Info, nameof (ManifestStorageService), "Service", "UpdateExtensionContent:: Manifest not stored to file storage: {0}.{1} {2} {3}", (object) storageRefreshData.PublisherName, (object) storageRefreshData.ExtensionName, (object) storageRefreshData.Version, (object) storageRefreshData.Language);
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      requestContext.Trace(10013560, TraceLevel.Info, nameof (ManifestStorageService), "Service", "OnRegistryChanged:: Clearing manifest cache");
      this.m_manifestStorageData = (Dictionary<string, List<ManifestStorageData>>) null;
      this.LoadExtensionData(requestContext);
    }

    private class ManifestStorageRefreshData : BaseManifestStorageData
    {
      public ExtensionFile ManifestFile { get; set; }
    }
  }
}
