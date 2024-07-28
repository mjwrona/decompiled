// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache.CachedExtensionData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Extension.Cache
{
  public class CachedExtensionData
  {
    private readonly IDictionary<string, PublishedExtension> m_ExtensionIdentifiersToExtensionMap;
    private readonly KeyNameToValueListMap<Guid> m_CategoryToExtensionIdsMap;
    private readonly KeyNameToValueListMap<Guid> m_TagToExtensionIdsMap;
    private readonly List<Guid> m_AllExtensionIds;
    private readonly KeyNameToValueListMap<Guid> m_TargetPlatformToExtensionIdsMap;
    private readonly double m_AverageRating;
    private readonly int m_MinVotesRequired;

    public int MinVotesRequired { get; set; }

    public double AverageRating { get; set; }

    public CachedExtensionData()
    {
      this.m_ExtensionIdentifiersToExtensionMap = (IDictionary<string, PublishedExtension>) new Dictionary<string, PublishedExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_AllExtensionIds = new List<Guid>();
      this.m_CategoryToExtensionIdsMap = new KeyNameToValueListMap<Guid>();
      this.m_TagToExtensionIdsMap = new KeyNameToValueListMap<Guid>();
      this.m_TargetPlatformToExtensionIdsMap = new KeyNameToValueListMap<Guid>();
    }

    public CachedExtensionData(int minVotesRequired, double averageRating)
      : this()
    {
      this.m_MinVotesRequired = minVotesRequired;
      this.m_AverageRating = averageRating;
    }

    public void AddExtension(PublishedExtension extension)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
      PublishedExtension publishedExtension = (PublishedExtension) null;
      if (this.m_ExtensionIdentifiersToExtensionMap.TryGetValue(fullyQualifiedName, out publishedExtension))
        return;
      this.m_AllExtensionIds.Add(extension.ExtensionId);
      this.m_ExtensionIdentifiersToExtensionMap.Add(extension.ExtensionId.ToString(), extension);
      this.m_ExtensionIdentifiersToExtensionMap.Add(fullyQualifiedName, extension);
      if (!extension.IsPublic())
        return;
      this.m_CategoryToExtensionIdsMap.TryAddValueToKeys(extension.Categories, extension.ExtensionId);
      this.m_TagToExtensionIdsMap.TryAddValueToKeys(extension.Tags, extension.ExtensionId);
      if (!extension.IsVsCodeExtension())
        return;
      this.m_TargetPlatformToExtensionIdsMap.TryAddValueToKeys(GalleryServerUtil.GetSupportedTargetPlatformsForExtension(extension), extension.ExtensionId);
    }

    public void AddExtensions(List<PublishedExtension> extensions)
    {
      foreach (PublishedExtension extension in extensions)
        this.AddExtension(extension);
      TeamFoundationTracingService.TraceRaw(12062054, TraceLevel.Info, "gallery", "AddExtensionToCache", string.Format("AddExtensionsToCache | Extension count: {0} ", (object) extensions.Count));
    }

    public PublishedExtension GetExtension(string extensionIdentifier)
    {
      PublishedExtension extension = (PublishedExtension) null;
      this.m_ExtensionIdentifiersToExtensionMap.TryGetValue(extensionIdentifier, out extension);
      return extension;
    }

    public List<PublishedExtension> GetExtensionsInCategory(string category) => this.GetExtensionsForKey(category, this.m_CategoryToExtensionIdsMap);

    public List<PublishedExtension> GetExtensionsWithTag(string tag) => this.GetExtensionsForKey(tag, this.m_TagToExtensionIdsMap);

    public List<PublishedExtension> GetExtensionsInTargetPlatform(string targetPlatform) => this.GetExtensionsForKey(targetPlatform, this.m_TargetPlatformToExtensionIdsMap);

    public List<PublishedExtension> GetAllExtensions() => this.GetExtensionsFromIds(this.m_AllExtensionIds);

    public List<PublishedExtension> GetExtensionsFromIds(List<Guid> extensionIds)
    {
      List<PublishedExtension> extensionsFromIds = new List<PublishedExtension>();
      foreach (Guid extensionId in extensionIds)
        extensionsFromIds.Add(this.m_ExtensionIdentifiersToExtensionMap[extensionId.ToString()]);
      return extensionsFromIds;
    }

    public int GetExtensionCount() => this.m_AllExtensionIds.Count;

    public virtual void UpdateExtension(PublishedExtension extension)
    {
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
      PublishedExtension extension1;
      if (this.m_ExtensionIdentifiersToExtensionMap.TryGetValue(extension.ExtensionId.ToString(), out extension1))
      {
        this.m_ExtensionIdentifiersToExtensionMap[fullyQualifiedName] = extension;
        this.m_ExtensionIdentifiersToExtensionMap[extension1.ExtensionId.ToString()] = extension;
        CachedExtensionData.UpdateKeysToExtensionIdMapping(extension.Categories ?? new List<string>(), extension1.Categories ?? new List<string>(), extension.ExtensionId, this.m_CategoryToExtensionIdsMap);
        CachedExtensionData.UpdateKeysToExtensionIdMapping(extension.Tags ?? new List<string>(), extension1.Tags ?? new List<string>(), extension.ExtensionId, this.m_TagToExtensionIdsMap);
        if (extension.IsVsCodeExtension())
        {
          List<string> platformsForExtension = GalleryServerUtil.GetSupportedTargetPlatformsForExtension(extension1);
          CachedExtensionData.UpdateKeysToExtensionIdMapping(GalleryServerUtil.GetSupportedTargetPlatformsForExtension(extension) ?? new List<string>(), platformsForExtension, extension.ExtensionId, this.m_TargetPlatformToExtensionIdsMap);
        }
        TeamFoundationTracingService.TraceRaw(12062056, TraceLevel.Info, "gallery", nameof (UpdateExtension), string.Format("UpdateExtensionInCache | Extension name: {0} | CategoriesCount: {1} | TagsCount: {2}", (object) fullyQualifiedName, (object) extension.Categories.Count, (object) extension.Tags.Count));
      }
      else
      {
        this.AddExtension(extension);
        TeamFoundationTracingService.TraceRaw(12062054, TraceLevel.Info, "gallery", "AddExtensionToCache", string.Format("AddExtensionToCache | Extension name: {0} | CategoriesCount: {1} | TagsCount: {2}", (object) fullyQualifiedName, (object) extension.Categories?.Count, (object) extension.Tags?.Count));
      }
    }

    public virtual void RemoveExtension(string extensionIdentifier)
    {
      PublishedExtension extension;
      if (!this.m_ExtensionIdentifiersToExtensionMap.TryGetValue(extensionIdentifier, out extension))
        return;
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(extension.Publisher.PublisherName, extension.ExtensionName);
      this.m_ExtensionIdentifiersToExtensionMap.Remove(fullyQualifiedName);
      this.m_ExtensionIdentifiersToExtensionMap.Remove(extension.ExtensionId.ToString());
      this.m_AllExtensionIds.Remove(extension.ExtensionId);
      bool flag1 = false;
      bool flag2 = false;
      if (extension.IsPublic())
      {
        flag1 = this.m_CategoryToExtensionIdsMap.TryRemoveValueFromKeys(extension.Categories, extension.ExtensionId);
        flag2 = this.m_TagToExtensionIdsMap.TryRemoveValueFromKeys(extension.Tags, extension.ExtensionId);
        if (extension.IsVsCodeExtension())
          this.m_TargetPlatformToExtensionIdsMap.TryRemoveValueFromKeys(GalleryServerUtil.GetSupportedTargetPlatformsForExtension(extension), extension.ExtensionId);
      }
      TeamFoundationTracingService.TraceRaw(12062055, TraceLevel.Info, "gallery", nameof (RemoveExtension), string.Format("RemoveExtensionFromCache| Extension name: {0} | RemovedExtensionFromAllCategories: {1} | RemovedExtensionFromAllTags: {2}", (object) fullyQualifiedName, (object) flag1, (object) flag2));
    }

    private List<PublishedExtension> GetExtensionsForKey(
      string keyName,
      KeyNameToValueListMap<Guid> keyNameToValueListMap)
    {
      List<Guid> valueListForKey = (List<Guid>) null;
      return keyNameToValueListMap.TryGetValueListForKey(keyName, out valueListForKey) ? this.GetExtensionsFromIds(valueListForKey) : (List<PublishedExtension>) null;
    }

    private static void UpdateKeysToExtensionIdMapping(
      List<string> newKeyNames,
      List<string> existingKeyNames,
      Guid extensionId,
      KeyNameToValueListMap<Guid> keyNameToValueListMap)
    {
      HashSet<string> stringSet1 = new HashSet<string>((IEnumerable<string>) existingKeyNames, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> stringSet2 = new HashSet<string>((IEnumerable<string>) newKeyNames, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      stringSet1.ExceptWith((IEnumerable<string>) newKeyNames);
      stringSet2.ExceptWith((IEnumerable<string>) existingKeyNames);
      foreach (string keyName in stringSet1)
        keyNameToValueListMap.TryRemoveValueFromKey(keyName, extensionId);
      foreach (string keyName in stringSet2)
        keyNameToValueListMap.TryAddValueToKey(keyName, extensionId);
      TeamFoundationTracingService.TraceRaw(12062056, TraceLevel.Info, "gallery", "UpdateExtensionKeys", string.Format("UpdateExtensionKeys | Extension Id: {0} | RemovedExtensionFromKeys: {1} | AddedExtensionToKeys: {2}", (object) extensionId, (object) stringSet1?.Count, (object) stringSet2?.Count));
    }
  }
}
