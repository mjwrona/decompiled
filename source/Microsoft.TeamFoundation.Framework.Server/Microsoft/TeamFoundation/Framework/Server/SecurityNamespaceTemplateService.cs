// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceTemplateService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SecurityNamespaceTemplateService : VssBaseService, IVssFrameworkService
  {
    internal const string NamespaceTemplatesFromBinaryFeatureFlag = "VisualStudio.Services.Security.NamespaceTemplatesFromBinary";
    internal const string DisableSecurityTemplateEntryResiliency = "VisualStudio.Services.Security.DisableSecurityTemplateEntryResiliency";
    private ILockName m_lock;
    private INotificationRegistration m_securityTemplatesRegistration;
    private SecurityNamespaceTemplateService.SecurityNamespaceData m_data;
    private static readonly IReadOnlyDictionary<Guid, NamespaceDescription> emptyDict = (IReadOnlyDictionary<Guid, NamespaceDescription>) new Dictionary<Guid, NamespaceDescription>();
    private const string c_area = "Security";
    private const string c_layer = "SecurityNamespaceTemplateService";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56620, "Security", nameof (SecurityNamespaceTemplateService), nameof (ServiceStart));
      try
      {
        requestContext.CheckDeploymentRequestContext();
        this.m_lock = this.CreateLockName(requestContext, "snts");
        this.m_securityTemplatesRegistration = requestContext.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(requestContext, "Default", SqlNotificationEventClasses.SecurityNamespaceTemplatesChanged, new SqlNotificationCallback(this.OnSecurityNamespaceTemplatesChanged), false, false);
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnFeatureFlagChange), "/FeatureAvailability/Entries/VisualStudio.Services.Security.NamespaceTemplatesFromBinary/AvailabilityState");
        this.LoadSecurityNamespaceTemplateData(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56622, "Security", nameof (SecurityNamespaceTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56621, "Security", nameof (SecurityNamespaceTemplateService), nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_securityTemplatesRegistration.Unregister(requestContext);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnFeatureFlagChange));
    }

    private void OnFeatureFlagChange(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSecurityNamespaceTemplateData(requestContext, true);
    }

    public long SequenceId => this.m_data.CombinedSequenceId;

    public IReadOnlyDictionary<Guid, NamespaceDescription> GetNamespaceTemplatesByHostType(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      out long sequenceId)
    {
      requestContext.TraceEnter(56623, "Security", nameof (SecurityNamespaceTemplateService), nameof (GetNamespaceTemplatesByHostType));
      try
      {
        hostType = TeamFoundationHostTypeHelper.NormalizeHostType(hostType);
        ArgumentUtility.CheckForMultipleBits((int) hostType, nameof (hostType));
        SecurityNamespaceTemplateService.SecurityNamespaceData data = this.m_data;
        sequenceId = data.CombinedSequenceId;
        IDictionary<Guid, NamespaceDescription> dictionary;
        return !data.Map.TryGetValue(hostType, out dictionary) ? SecurityNamespaceTemplateService.emptyDict : (IReadOnlyDictionary<Guid, NamespaceDescription>) dictionary;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56625, "Security", nameof (SecurityNamespaceTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56624, "Security", nameof (SecurityNamespaceTemplateService), nameof (GetNamespaceTemplatesByHostType));
      }
    }

    public NamespaceDescription GetSecurityNamespaceTemplate(
      IVssRequestContext requestContext,
      TeamFoundationHostType hostType,
      Guid namespaceId,
      out long sequenceId)
    {
      NamespaceDescription namespaceDescription;
      return this.GetNamespaceTemplatesByHostType(requestContext, hostType, out sequenceId).TryGetValue(namespaceId, out namespaceDescription) ? namespaceDescription : (NamespaceDescription) null;
    }

    private void OnSecurityNamespaceTemplatesChanged(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceEnter(56626, "Security", nameof (SecurityNamespaceTemplateService), nameof (OnSecurityNamespaceTemplatesChanged));
      try
      {
        this.LoadSecurityNamespaceTemplateData(requestContext);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56628, "Security", nameof (SecurityNamespaceTemplateService), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56627, "Security", nameof (SecurityNamespaceTemplateService), nameof (OnSecurityNamespaceTemplatesChanged));
      }
    }

    internal void LoadSecurityNamespaceTemplateData(
      IVssRequestContext requestContext,
      bool isFeatureFlagChange = false)
    {
      long dbSequenceId;
      IDictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>> namespaceTemplates = this.GetDbSecurityNamespaceTemplates(requestContext, out dbSequenceId);
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Security.NamespaceTemplatesFromBinary"))
        this.MergeBinarySecurityNamespaceTemplatesToMap(requestContext, namespaceTemplates);
      else
        requestContext.Trace(56997, TraceLevel.Info, "Security", nameof (SecurityNamespaceTemplateService), "Not loading binary templates");
      using (requestContext.Lock(this.m_lock))
      {
        if (((this.m_data == null ? 1 : (dbSequenceId > this.m_data.DbSequenceId ? 1 : 0)) | (isFeatureFlagChange ? 1 : 0)) == 0)
          return;
        SecurityNamespaceTemplateService.SecurityNamespaceData data = this.m_data;
        long binarySequenceId = data != null ? data.BinarySequenceId : 0L;
        if (isFeatureFlagChange)
          ++binarySequenceId;
        this.m_data = new SecurityNamespaceTemplateService.SecurityNamespaceData(dbSequenceId, binarySequenceId, namespaceTemplates);
      }
    }

    internal virtual IDictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>> GetDbSecurityNamespaceTemplates(
      IVssRequestContext requestContext,
      out long dbSequenceId)
    {
      Dictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>> namespaceTemplates = new Dictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>>();
      IReadOnlyList<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate> namespaceTemplateList;
      using (SecurityNamespaceTemplateComponent component = requestContext.CreateComponent<SecurityNamespaceTemplateComponent>())
        namespaceTemplateList = component.QuerySecurityNamespaceTemplates(out dbSequenceId);
      foreach (SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate namespaceTemplate in (IEnumerable<SecurityNamespaceTemplateComponent.SecurityNamespaceTemplate>) namespaceTemplateList)
      {
        TeamFoundationHostType hostType = (TeamFoundationHostType) namespaceTemplate.HostType;
        IDictionary<Guid, NamespaceDescription> dictionary;
        if (!namespaceTemplates.TryGetValue(hostType, out dictionary))
          namespaceTemplates[hostType] = dictionary = (IDictionary<Guid, NamespaceDescription>) new Dictionary<Guid, NamespaceDescription>();
        NamespaceDescription namespaceDescription = SecurityNamespaceTemplateService.DeserializeDescriptionXml(TeamFoundationSerializationUtility.SerializeToXml(namespaceTemplate.Description));
        namespaceDescription.NamespaceId = namespaceTemplate.NamespaceId;
        namespaceDescription.IsProjected = true;
        dictionary.Add(namespaceTemplate.NamespaceId, namespaceDescription);
      }
      return (IDictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>>) namespaceTemplates;
    }

    internal virtual IEnumerable<SecurityNamespaceTemplate> GetBinarySecurityNamespaceTemplates(
      IVssRequestContext requestContext)
    {
      HashSet<int> intSet = new HashSet<int>();
      List<SecurityNamespaceTemplate> namespaceTemplates1 = new List<SecurityNamespaceTemplate>();
      using (IDisposableReadOnlyList<SecurityNamespaceTemplateProvider> extensions = requestContext.GetExtensions<SecurityNamespaceTemplateProvider>())
      {
        requestContext.TraceAlways(56997, TraceLevel.Info, "Security", nameof (SecurityNamespaceTemplateService), string.Format("Found {0} binary template providers.", (object) extensions.Count));
        foreach (SecurityNamespaceTemplateProvider templateProvider in (IEnumerable<SecurityNamespaceTemplateProvider>) extensions)
        {
          try
          {
            IEnumerable<SecurityNamespaceTemplate> namespaceTemplates2 = templateProvider.CreateSecurityNamespaceTemplates(requestContext);
            List<Guid> values = new List<Guid>();
            foreach (SecurityNamespaceTemplate namespaceTemplate1 in namespaceTemplates2)
            {
              values.Add(namespaceTemplate1.NamespaceId);
              namespaceTemplate1.ValidateSecurityNamespaceTemplate(requestContext);
              foreach (SecurityNamespaceTemplate namespaceTemplate2 in (IEnumerable<SecurityNamespaceTemplate>) namespaceTemplate1.DecomposeHostType())
              {
                if (intSet.Contains(namespaceTemplate2.Signature))
                  throw new DuplicateSecurityNamespaceTemplateException(namespaceTemplate2.HostType, namespaceTemplate2.NamespaceId);
                intSet.Add(namespaceTemplate2.Signature);
                namespaceTemplates1.Add(namespaceTemplate2);
              }
            }
            string str = string.Join<Guid>(",", (IEnumerable<Guid>) values);
            requestContext.TraceAlways(56997, TraceLevel.Info, "Security", nameof (SecurityNamespaceTemplateService), "Found following templates: " + str);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(56998, "Security", nameof (SecurityNamespaceTemplateService), ex);
            throw;
          }
        }
      }
      return (IEnumerable<SecurityNamespaceTemplate>) namespaceTemplates1;
    }

    internal void MergeBinarySecurityNamespaceTemplatesToMap(
      IVssRequestContext requestContext,
      IDictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>> map)
    {
      foreach (SecurityNamespaceTemplate namespaceTemplate in this.GetBinarySecurityNamespaceTemplates(requestContext))
      {
        IDictionary<Guid, NamespaceDescription> dictionary;
        if (!map.TryGetValue(namespaceTemplate.HostType, out dictionary))
          map[namespaceTemplate.HostType] = dictionary = (IDictionary<Guid, NamespaceDescription>) new Dictionary<Guid, NamespaceDescription>();
        dictionary[namespaceTemplate.NamespaceId] = namespaceTemplate.Description;
      }
    }

    internal static NamespaceDescription DeserializeDescriptionXml(XmlNode node)
    {
      switch (node.Name)
      {
        case "SecurityNamespaceDescription":
          return (NamespaceDescription) TeamFoundationSerializationUtility.Deserialize<SecurityNamespaceDescription>(node);
        case "RemoteSecurityNamespaceDescription":
          return (NamespaceDescription) TeamFoundationSerializationUtility.Deserialize<RemoteSecurityNamespaceDescription>(node);
        default:
          throw new ArgumentException(string.Format("Invalid security namespace type {0}. Valid types are \"SecurityNamespaceDescription\" and \"RemoteSecurityNamespaceDescription\".", (object) node.Name));
      }
    }

    private class SecurityNamespaceData
    {
      public readonly long DbSequenceId;
      public readonly long BinarySequenceId;
      public readonly IDictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>> Map;

      public SecurityNamespaceData(
        long dbSequenceId,
        long binarySequenceId,
        IDictionary<TeamFoundationHostType, IDictionary<Guid, NamespaceDescription>> map)
      {
        this.DbSequenceId = dbSequenceId;
        this.BinarySequenceId = binarySequenceId;
        this.Map = map;
      }

      public long CombinedSequenceId => this.BinarySequenceId + this.DbSequenceId;
    }
  }
}
