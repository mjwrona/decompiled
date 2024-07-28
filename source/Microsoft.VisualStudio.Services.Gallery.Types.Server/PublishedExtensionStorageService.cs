// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Types.Server.PublishedExtensionStorageService
// Assembly: Microsoft.VisualStudio.Services.Gallery.Types.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF687265-4AE2-4CD2-A134-409D61826008
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Types.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.Types.Server
{
  public class PublishedExtensionStorageService : 
    IPublishedExtensionStorageService,
    IVssFrameworkService
  {
    private readonly string s_persistentPublishedExtensionRegistryPath = "/Service/Extensions/PublishedExtension/{0}/{1}/{2}/FileId";
    private readonly string s_persistentPublishedExtensionRegistryPathLastUpdated = "/Service/Extensions/PublishedExtension/{0}/{1}/{2}/LastUpdatedTicks";
    private readonly string s_persistentPublishedExtensionRegistryPathAllVersions = "/Service/Extensions/PublishedExtension/{0}/{1}/**";
    private static readonly string s_area = "Gallery";
    private static readonly string s_layer = nameof (PublishedExtensionStorageService);
    private object m_lock = new object();

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public bool TryGetPublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      out PublishedExtension publishedExtension)
    {
      bool publishedExtensionObject = false;
      publishedExtension = (PublishedExtension) null;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (!this.IsPersistPublishedExtensionFeatureFlagEnabled(vssRequestContext))
        return publishedExtensionObject;
      vssRequestContext.Trace(100136301, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "TryGetPublishedExtensionObject: extension:" + publisherName + "." + extensionName + "." + version);
      if (!string.IsNullOrEmpty(publisherName) && !string.IsNullOrEmpty(extensionName))
      {
        if (!string.IsNullOrEmpty(version))
        {
          try
          {
            RegistryQuery query = new RegistryQuery(string.Format(this.s_persistentPublishedExtensionRegistryPath, (object) publisherName, (object) extensionName, (object) version));
            string json = (string) null;
            int num = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, in query, -1);
            if (num != -1)
            {
              ITeamFoundationFileService service = vssRequestContext.GetService<ITeamFoundationFileService>();
              CompressionType compressionType = CompressionType.None;
              IVssRequestContext requestContext1 = vssRequestContext;
              long fileId = (long) num;
              ref CompressionType local = ref compressionType;
              using (Stream stream = service.RetrieveFile(requestContext1, fileId, out local))
              {
                using (StreamReader streamReader = new StreamReader(stream))
                  json = streamReader.ReadToEnd();
              }
              vssRequestContext.Trace(100136302, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "TryGetPublishedExtensionObject: Found fileId in registry: extension:" + publisherName + "." + extensionName + "." + version + ", " + string.Format("fileId:{0}, jsonPayloadLength:{1}", (object) num, (object) json?.Length));
              if (!string.IsNullOrEmpty(json))
              {
                publishedExtension = JsonUtilities.Deserialize<PublishedExtension>(json);
                publishedExtensionObject = true;
              }
            }
          }
          catch (Exception ex)
          {
            vssRequestContext.TraceException(100136303, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, ex);
          }
          return publishedExtensionObject;
        }
      }
      return publishedExtensionObject;
    }

    public void StorePublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version,
      PublishedExtension publishedExtension)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      if (!this.IsPersistPublishedExtensionFeatureFlagEnabled(vssRequestContext))
        return;
      if (this.IsBypassGalleryCallsFFEnabled(vssRequestContext))
      {
        vssRequestContext.Trace(100136304, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "StorePublishedExtensionObject: BypassGalleryCalls FF is set, returning without writing to file service");
      }
      else
      {
        vssRequestContext.Trace(100136305, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, string.Format("StorePublishedExtensionObject: extension:{0}.{1}.{2}, flags:{3}", (object) publisherName, (object) extensionName, (object) version, (object) publishedExtension?.Flags));
        if (string.IsNullOrEmpty(publisherName) || string.IsNullOrEmpty(extensionName) || string.IsNullOrEmpty(version) || publishedExtension == null)
          return;
        string str1 = string.Format(this.s_persistentPublishedExtensionRegistryPathLastUpdated, (object) publisherName, (object) extensionName, (object) version);
        RegistryQuery query1 = new RegistryQuery(str1);
        IVssRegistryService service1 = vssRequestContext.GetService<IVssRegistryService>();
        long num1 = service1.GetValue<long>(vssRequestContext, in query1, 0L);
        vssRequestContext.Trace(100136307, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "StorePublishedExtensionObject: Attempting to write object to file service. " + string.Format("lastUpdatedTicks:{0}, pubExt.LastUpdated.Ticks:{1}", (object) num1, (object) publishedExtension.LastUpdated.Ticks));
        if (num1 != 0L && publishedExtension.LastUpdated.Ticks <= num1)
          return;
        string s;
        try
        {
          s = publishedExtension.Serialize<PublishedExtension>();
        }
        catch (Exception ex)
        {
          vssRequestContext.TraceException(100136308, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, ex);
          s = (string) null;
        }
        if (string.IsNullOrEmpty(s))
          return;
        int fileId = -1;
        vssRequestContext.Trace(100136309, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "StorePublishedExtensionObject: Writing object to file service. Extension:" + publisherName + "." + extensionName + "." + version);
        ITeamFoundationFileService service2 = vssRequestContext.GetService<ITeamFoundationFileService>();
        int num2 = service2.UploadFile(vssRequestContext, Encoding.UTF8.GetBytes(s));
        if (num2 > 0)
        {
          lock (this.m_lock)
          {
            string str2 = string.Format(this.s_persistentPublishedExtensionRegistryPath, (object) publisherName, (object) extensionName, (object) version);
            RegistryQuery query2 = new RegistryQuery(str2);
            fileId = service1.GetValue<int>(vssRequestContext, in query2, -1);
            service1.SetValue<int>(vssRequestContext, str2, num2);
            service1.SetValue<long>(vssRequestContext, str1, DateTime.UtcNow.Ticks);
          }
          if (fileId == -1)
            return;
          service2.DeleteFile(vssRequestContext, (long) fileId);
        }
        else
          vssRequestContext.TraceAlways(100136310, TraceLevel.Error, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "Error uploading file to file service. extension:" + publisherName + "." + extensionName + "." + version);
      }
    }

    public void StorePublishedExtensionObject(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      string version)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      if (!this.IsPersistPublishedExtensionFeatureFlagEnabled(vssRequestContext))
        return;
      vssRequestContext.Trace(100136311, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "StorePublishedExtensionObject2: extension:" + publisherName + "." + extensionName + "." + version);
      if (string.IsNullOrEmpty(publisherName) || string.IsNullOrEmpty(extensionName) || string.IsNullOrEmpty(version) || this.IsBypassGalleryCallsFFEnabled(vssRequestContext))
        return;
      ExtensionQueryFlags flags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeCategoryAndTags | ExtensionQueryFlags.IncludeVersionProperties | ExtensionQueryFlags.ExcludeNonValidated | ExtensionQueryFlags.IncludeInstallationTargets | ExtensionQueryFlags.IncludeAssetUri;
      if (!vssRequestContext.IsFeatureEnabled(GallerySdkFeatureFlags.UseCdnAssetUri))
        flags |= ExtensionQueryFlags.UseFallbackAssetUri;
      string version1 = version;
      if (!string.IsNullOrEmpty(version) && version.Equals("latest", StringComparison.OrdinalIgnoreCase))
      {
        flags |= ExtensionQueryFlags.IncludeLatestVersionOnly;
        version1 = (string) null;
      }
      PublishedExtension extension = vssRequestContext.GetService<IGalleryService>().GetExtension(vssRequestContext, publisherName, extensionName, version1, flags);
      if (extension == null)
        return;
      this.StorePublishedExtensionObject(vssRequestContext, publisherName, extensionName, extension.Versions[0].Version, extension);
    }

    public void DeletePublishedExtensionObjects(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      if (!this.IsPersistPublishedExtensionFeatureFlagEnabled(vssRequestContext))
        return;
      vssRequestContext.Trace(100136312, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, "DeletePublishedExtensionObjects: extension:" + publisherName + "." + extensionName);
      if (string.IsNullOrEmpty(publisherName) || string.IsNullOrEmpty(extensionName))
        return;
      IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
      RegistryEntryCollection registryEntryCollection = service.ReadEntries(vssRequestContext, (RegistryQuery) string.Format(this.s_persistentPublishedExtensionRegistryPathAllVersions, (object) publisherName, (object) extensionName));
      vssRequestContext.Trace(100136313, TraceLevel.Info, PublishedExtensionStorageService.s_area, PublishedExtensionStorageService.s_layer, string.Format("DeletePublishedExtensionObjects: Found {0} entries to delete for extension {1}.{2}", (object) registryEntryCollection?.Count, (object) publisherName, (object) extensionName));
      List<int> fileIds = new List<int>();
      foreach (RegistryEntry registryEntry in registryEntryCollection)
      {
        if (registryEntry.Path.EndsWith("FileId", StringComparison.OrdinalIgnoreCase))
        {
          int result = -1;
          if (int.TryParse(registryEntry.Value, out result))
            fileIds.Add(result);
        }
      }
      vssRequestContext.GetService<ITeamFoundationFileService>().DeleteFiles(vssRequestContext, (IEnumerable<int>) fileIds);
      service.DeleteEntries(vssRequestContext, string.Format(this.s_persistentPublishedExtensionRegistryPathAllVersions, (object) publisherName, (object) extensionName));
    }

    protected internal virtual bool IsBypassGalleryCallsFFEnabled(IVssRequestContext requestContext) => requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.BypassGalleryCalls);

    protected internal virtual bool IsPersistPublishedExtensionFeatureFlagEnabled(
      IVssRequestContext requestContext)
    {
      return requestContext.IsFeatureEnabled(GallerySdkFeatureFlags.PersistPublishedExtension);
    }
  }
}
