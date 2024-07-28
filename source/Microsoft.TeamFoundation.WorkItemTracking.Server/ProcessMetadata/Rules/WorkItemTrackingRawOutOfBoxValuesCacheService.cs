// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules.WorkItemTrackingRawOutOfBoxValuesCacheService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules
{
  public class WorkItemTrackingRawOutOfBoxValuesCacheService : 
    IWorkItemTrackingRawOutOfBoxValuesCacheService,
    IVssFrameworkService
  {
    protected ConcurrentDictionary<string, IReadOnlyCollection<WorkItemFieldRule>> m_outOfBoxRules;
    protected ConcurrentDictionary<string, IReadOnlyCollection<HelpTextDescriptor>> m_outOfBoxHelpTexts;
    private int m_cacheVersion;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnexpectedHostTypeException(systemRequestContext.ServiceHost.HostType);
      this.m_outOfBoxRules = new ConcurrentDictionary<string, IReadOnlyCollection<WorkItemFieldRule>>();
      this.m_outOfBoxHelpTexts = new ConcurrentDictionary<string, IReadOnlyCollection<HelpTextDescriptor>>();
      this.m_cacheVersion = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ProcessMetadataFileIdCache>().GetFileIdsCacheVersion();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual IReadOnlyCollection<T> GetOutOfBoxRawValues<T>(
      IVssRequestContext requestContext,
      ProcessDescriptor systemDescriptor,
      string workItemTypeReferenceName,
      ProcessMetadataResourceType resourceType,
      string xmlRoot)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ProcessDescriptor>(systemDescriptor, nameof (systemDescriptor));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(workItemTypeReferenceName, nameof (workItemTypeReferenceName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(xmlRoot, nameof (xmlRoot));
      if (resourceType == ProcessMetadataResourceType.RuleMetadata && typeof (T) != typeof (WorkItemFieldRule) || resourceType == ProcessMetadataResourceType.HelpTextMetadata && typeof (T) != typeof (HelpTextDescriptor))
        throw new ArgumentException("resourceType argument doesn't match with the type(T) you supplied.", resourceType.ToString());
      this.ClearCacheOnStaleCacheVersion(requestContext);
      bool cacheMiss = false;
      IReadOnlyCollection<T> valuesToReturn = (IReadOnlyCollection<T>) null;
      requestContext.TraceBlock<bool>(911331, 911340, "WorkItemRulesService", nameof (WorkItemTrackingRawOutOfBoxValuesCacheService), nameof (GetOutOfBoxRawValues), (Func<bool>) (() =>
      {
        if (!systemDescriptor.IsSystem)
          throw new InvalidOperationException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.CacheMustBeCalledWithSystemDescriptor((object) systemDescriptor.TypeId));
        IReadOnlyCollection<WorkItemFieldRule> workItemFieldRules = (IReadOnlyCollection<WorkItemFieldRule>) null;
        IReadOnlyCollection<HelpTextDescriptor> helpTextDescriptors = (IReadOnlyCollection<HelpTextDescriptor>) null;
        string cacheKey = this.GetCacheKey(systemDescriptor.Name, workItemTypeReferenceName, resourceType.ToString());
        if (resourceType == ProcessMetadataResourceType.RuleMetadata && this.m_outOfBoxRules != null && this.m_outOfBoxRules.TryGetValue(cacheKey, out workItemFieldRules))
          valuesToReturn = (IReadOnlyCollection<T>) workItemFieldRules;
        else if (resourceType == ProcessMetadataResourceType.HelpTextMetadata && this.m_outOfBoxHelpTexts != null && this.m_outOfBoxHelpTexts.TryGetValue(cacheKey, out helpTextDescriptors))
          valuesToReturn = (IReadOnlyCollection<T>) helpTextDescriptors;
        if (valuesToReturn == null)
        {
          cacheMiss = true;
          IEnumerable<T> valuesFromResourceXml = this.GetValuesFromResourceXml<T>(requestContext, systemDescriptor, workItemTypeReferenceName, resourceType, xmlRoot);
          switch (resourceType)
          {
            case ProcessMetadataResourceType.RuleMetadata:
              valuesFromResourceXml.ForEach<T>((Action<T>) (rule => ((object) rule as WorkItemFieldRule).SetAccessModeToReadOnly()));
              this.m_outOfBoxRules.TryAdd(cacheKey, valuesFromResourceXml as IReadOnlyCollection<WorkItemFieldRule>);
              break;
            case ProcessMetadataResourceType.HelpTextMetadata:
              this.m_outOfBoxHelpTexts.TryAdd(cacheKey, valuesFromResourceXml as IReadOnlyCollection<HelpTextDescriptor>);
              break;
          }
          valuesToReturn = valuesFromResourceXml as IReadOnlyCollection<T>;
        }
        return valuesToReturn != null;
      }));
      if (cacheMiss)
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Process", systemDescriptor.Name);
        properties.Add(nameof (workItemTypeReferenceName), workItemTypeReferenceName);
        properties.Add("ElapsedTime", requestContext.LastTracedBlockElapsedMilliseconds());
        properties.Add("NumberOfItems", (double) valuesToReturn.Count);
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, nameof (GetOutOfBoxRawValues), typeof (T).Name, properties);
      }
      return valuesToReturn;
    }

    private string GetCacheKey(string process, string workitemtype, string resource) => string.Format("{0}:{1}:{2}", (object) process, (object) workitemtype, (object) resource);

    private void ClearCache()
    {
      this.m_outOfBoxRules = new ConcurrentDictionary<string, IReadOnlyCollection<WorkItemFieldRule>>();
      this.m_outOfBoxHelpTexts = new ConcurrentDictionary<string, IReadOnlyCollection<HelpTextDescriptor>>();
    }

    private bool ClearCacheOnStaleCacheVersion(IVssRequestContext requestContext)
    {
      int fileIdsCacheVersion = requestContext.To(TeamFoundationHostType.Deployment).GetService<ProcessMetadataFileIdCache>().GetFileIdsCacheVersion();
      if (this.m_cacheVersion >= fileIdsCacheVersion)
        return false;
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("Old Cache version", (double) this.m_cacheVersion);
      properties.Add("Current Cache version", (double) fileIdsCacheVersion);
      this.ClearCache();
      this.m_cacheVersion = fileIdsCacheVersion;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, this.GetType().Name, "Invalidating Cache", properties);
      return true;
    }

    protected virtual IEnumerable<T> GetValuesFromResourceXml<T>(
      IVssRequestContext requestContext,
      ProcessDescriptor systemDescriptor,
      string workItemTypeReferenceName,
      ProcessMetadataResourceType resourceType,
      string xmlRoot)
    {
      return (IEnumerable<T>) TeamFoundationSerializationUtility.Deserialize<T[]>(MetadataResourceManager.GetResourceXMLForProcessWorkItemType(requestContext, systemDescriptor, workItemTypeReferenceName, resourceType), new XmlRootAttribute(xmlRoot));
    }
  }
}
