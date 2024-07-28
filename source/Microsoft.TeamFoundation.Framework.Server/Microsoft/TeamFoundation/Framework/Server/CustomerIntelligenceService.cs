// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CustomerIntelligenceService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CustomerIntelligenceService : IVssCustomerIntelligenceService, IVssFrameworkService
  {
    private bool m_isTracingSupported;
    private IDisposableReadOnlyList<ICustomerIntelligenceDataspaceProviderExtension> m_customerIntelligenceDataspaceProviderExtensions;
    private const string s_liveIdIdentifierSuffix = "@live.com";
    private const string s_liveIdIdentityType = "WLID";
    private static readonly string s_area = "Telemetry";
    private static readonly string s_layer = nameof (CustomerIntelligenceService);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_isTracingSupported = systemRequestContext.ExecutionEnvironment.IsHostedDeployment;
      this.m_customerIntelligenceDataspaceProviderExtensions = systemRequestContext.GetExtensions<ICustomerIntelligenceDataspaceProviderExtension>();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_customerIntelligenceDataspaceProviderExtensions == null)
        return;
      this.m_customerIntelligenceDataspaceProviderExtensions.Dispose();
      this.m_customerIntelligenceDataspaceProviderExtensions = (IDisposableReadOnlyList<ICustomerIntelligenceDataspaceProviderExtension>) null;
    }

    public static CustomerIntelligenceIdentityIdentifier ExtractIdentityIdentifier(
      IVssRequestContext requestContext,
      IdentityDescriptor descriptor = null)
    {
      if (descriptor == (IdentityDescriptor) null && requestContext.UserContext != (IdentityDescriptor) null)
        descriptor = requestContext.UserContext;
      CustomerIntelligenceIdentityIdentifier identityIdentifier = new CustomerIntelligenceIdentityIdentifier();
      if (descriptor != (IdentityDescriptor) null)
      {
        if (descriptor.Identifier != null && descriptor.Identifier.ToUpper().EndsWith("@live.com".ToUpper()) && descriptor.IdentityType != null && descriptor.IdentityType == "Microsoft.IdentityModel.Claims.ClaimsIdentity")
        {
          identityIdentifier.Id = descriptor.Identifier.ToUpper().Replace("@live.com".ToUpper(), string.Empty);
          identityIdentifier.IdType = "WLID";
        }
        else
        {
          identityIdentifier.Id = descriptor.Identifier;
          identityIdentifier.IdType = descriptor.IdentityType;
        }
      }
      return identityIdentifier;
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      double value)
    {
      this.Publish(requestContext, requestContext.ServiceHost.InstanceId, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), DateTime.UtcNow, area, feature, property, value);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      string property,
      double value)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(property, value);
      this.PublishCIEvents(requestContext, hostId, user, identityId, identityConsistentVSID, timeStamp, area, feature, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      string value)
    {
      this.Publish(requestContext, requestContext.ServiceHost.InstanceId, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), DateTime.UtcNow, area, feature, property, value);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      string property,
      string value)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(property, value);
      this.PublishCIEvents(requestContext, hostId, user, identityId, identityConsistentVSID, timeStamp, area, feature, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      string property,
      bool value)
    {
      this.Publish(requestContext, requestContext.ServiceHost.InstanceId, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), DateTime.UtcNow, area, feature, property, value);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      string property,
      bool value)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(property, value);
      this.PublishCIEvents(requestContext, hostId, user, identityId, identityConsistentVSID, timeStamp, area, feature, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      string area,
      string feature,
      CustomerIntelligenceData properties)
    {
      this.Publish(requestContext, requestContext.ServiceHost.InstanceId, area, feature, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string area,
      string feature,
      CustomerIntelligenceData properties)
    {
      this.Publish(requestContext, hostId, string.Empty, requestContext.GetUserId(), requestContext.GetUserCuid(), DateTime.UtcNow, area, feature, properties);
    }

    public virtual void Publish(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      CustomerIntelligenceData properties)
    {
      this.PublishCIEvents(requestContext, hostId, user, identityId, identityConsistentVSID, timeStamp, area, feature, properties);
    }

    private void PublishCIEvents(
      IVssRequestContext requestContext,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      DateTime timeStamp,
      string area,
      string feature,
      CustomerIntelligenceData properties)
    {
      requestContext.TraceEnter(15095041, CustomerIntelligenceService.s_area, CustomerIntelligenceService.s_layer, nameof (PublishCIEvents));
      if (!this.IsTracingEnabled(requestContext))
        return;
      user = string.Empty;
      string userAgent = requestContext.UserAgent;
      TeamFoundationHostType hostType = this.GetHostType(requestContext);
      Guid parentHostId = requestContext.ServiceHost.ParentServiceHost != null ? requestContext.ServiceHost.ParentServiceHost.InstanceId : Guid.Empty;
      Guid uniqueIdentifier = requestContext.UniqueIdentifier;
      string anonymousIdentifier = requestContext.GetAnonymousIdentifier();
      SupportsPublicAccess supportsPublicAccess1 = SupportsPublicAccess.None;
      SupportsPublicAccess supportsPublicAccess2;
      if (requestContext.RootContext.Items.TryGetValue<SupportsPublicAccess>(RequestContextItemsKeys.SupportsPublicAccess, out supportsPublicAccess2))
        supportsPublicAccess1 = supportsPublicAccess2;
      if (this.m_customerIntelligenceDataspaceProviderExtensions != null)
      {
        foreach (ICustomerIntelligenceDataspaceProviderExtension providerExtension in (IEnumerable<ICustomerIntelligenceDataspaceProviderExtension>) this.m_customerIntelligenceDataspaceProviderExtensions)
        {
          CustomerIntelligenceDataspaceInfo spaceInformation = providerExtension.GetDataSpaceInformation(requestContext);
          if (spaceInformation != null)
          {
            properties.AddDataspaceInformation(spaceInformation.Type, spaceInformation.Id, spaceInformation.Visibility);
            break;
          }
        }
      }
      string message1 = (string) null;
      string message2 = (string) null;
      string message3 = (string) null;
      IDictionary<string, object> data = properties.GetData();
      object obj1;
      if (data.TryGetValue("dataspaceType", out obj1))
      {
        message1 = obj1?.ToString();
        CiEventParamUtility.CheckParameterForValidCharacters(message1, "dataspaceType", true);
      }
      object obj2;
      if (data.TryGetValue("dataspaceId", out obj2))
        message2 = obj2?.ToString();
      object obj3;
      if (data.TryGetValue("dataspaceVisibility", out obj3))
        message3 = obj3?.ToString();
      CustomerIntelligenceService.NormalizeString(ref area);
      CustomerIntelligenceService.NormalizeString(ref feature);
      CustomerIntelligenceService.NormalizeString(ref userAgent);
      CustomerIntelligenceService.NormalizeString(ref anonymousIdentifier);
      CustomerIntelligenceService.NormalizeString(ref message1);
      CustomerIntelligenceService.NormalizeString(ref message2);
      CustomerIntelligenceService.NormalizeString(ref message3);
      try
      {
        if (area.Equals(CustomerIntelligenceArea.Survey))
        {
          Guid tenantId = requestContext.GetTenantId();
          requestContext.TracingService().TraceSurveyEvents(uniqueIdentifier, anonymousIdentifier, tenantId, hostId, parentHostId, (byte) hostType, identityId, identityConsistentVSID, area, feature, userAgent, this.ToJson(properties), message1, message2, message3, (byte) supportsPublicAccess1);
        }
        else
        {
          string json = this.ToJson(uniqueIdentifier, anonymousIdentifier, hostId, user, identityId, identityConsistentVSID, area, feature, timeStamp, userAgent, properties);
          requestContext.TracingService().TraceCustomerIntelligence(json, uniqueIdentifier, anonymousIdentifier, hostId, parentHostId, (byte) hostType, identityId, identityConsistentVSID, area, feature, userAgent, message1, message2, message3, (byte) supportsPublicAccess1);
        }
      }
      catch (EUIILeakException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(15095045, TraceLevel.Warning, CustomerIntelligenceService.s_area, CustomerIntelligenceService.s_layer, ex);
      }
      finally
      {
        requestContext.TraceLeave(15095050, CustomerIntelligenceService.s_area, CustomerIntelligenceService.s_layer, nameof (PublishCIEvents));
      }
    }

    private TeamFoundationHostType GetHostType(IVssRequestContext requestContext)
    {
      TeamFoundationHostType hostType = requestContext.ServiceHost.HostType;
      if ((hostType & TeamFoundationHostType.Deployment) == TeamFoundationHostType.Deployment)
        hostType = TeamFoundationHostType.Deployment;
      return hostType;
    }

    private Guid GetAccountHostId(IVssRequestContext requestContext) => requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? requestContext.ServiceHost.ParentServiceHost.InstanceId : requestContext.ServiceHost.InstanceId;

    public bool IsTracingEnabled(IVssRequestContext requestContext) => this.m_isTracingSupported && requestContext.IsFeatureEnabled("VisualStudio.FrameworkService.CustomerIntelligence");

    private string ToJson(
      Guid uniqueIdentifier,
      string anonymousIdentifier,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      string area,
      string feature,
      DateTime timeStamp,
      string userAgent,
      string property,
      object value)
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add(property, value);
      return this.ToJson(uniqueIdentifier, anonymousIdentifier, hostId, user, identityId, identityConsistentVSID, area, feature, timeStamp, userAgent, properties);
    }

    private string ToJson(
      Guid uniqueIdentifier,
      string anonymousIdentifier,
      Guid hostId,
      string user,
      Guid identityId,
      Guid identityConsistentVSID,
      string area,
      string feature,
      DateTime timeStamp,
      string userAgent,
      CustomerIntelligenceData properties)
    {
      return JsonConvert.SerializeObject((object) new Dictionary<string, object>()
      {
        [nameof (uniqueIdentifier)] = (object) uniqueIdentifier,
        [nameof (anonymousIdentifier)] = (object) anonymousIdentifier,
        [nameof (hostId)] = (object) hostId,
        ["identity"] = (object) user,
        ["vsid"] = (object) identityId,
        ["cuid"] = (object) identityConsistentVSID,
        [nameof (area)] = (object) area,
        [nameof (feature)] = (object) feature,
        [nameof (timeStamp)] = (object) timeStamp,
        [nameof (userAgent)] = (object) userAgent,
        [nameof (properties)] = (object) properties.GetData()
      });
    }

    private string ToJson(CustomerIntelligenceData properties) => JsonConvert.SerializeObject((object) properties.GetData());

    private static void NormalizeString(ref string message)
    {
      if (message != null)
        return;
      message = string.Empty;
    }
  }
}
