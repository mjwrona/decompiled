// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PlatformInfrastructureOrganizationService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Commerce;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class PlatformInfrastructureOrganizationService : IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "PlatformInfrastructureOrganizationService";

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Guid CreateInfrastructureOrganization(
      IVssRequestContext requestContext,
      string collectionHostName,
      string organizationHostName,
      string hostRegion,
      string tags)
    {
      requestContext.CheckDeploymentRequestContext();
      requestContext.TraceAlways(5109145, TraceLevel.Info, "Commerce", nameof (PlatformInfrastructureOrganizationService), "CreateInfrastructureOrganization: organizationHost=" + organizationHostName + ", collectionHost=" + collectionHostName + ", hostRegion=" + hostRegion + ", tags=" + tags);
      DateTime utcNow1 = DateTime.UtcNow;
      Guid infrastructureOrganization = InfrastructureHostHelper.CreateInfrastructureOrganization(requestContext, collectionHostName, organizationHostName, hostRegion, ServiceHostTags.FromString(tags));
      DateTime utcNow2 = DateTime.UtcNow;
      this.PublishCommerceIntelligenceEvent(requestContext, infrastructureOrganization, collectionHostName, organizationHostName, hostRegion, utcNow1, utcNow2);
      return infrastructureOrganization;
    }

    public IEnumerable<string> GetInfrastructureOrganizationProperties(
      IVssRequestContext requestContext,
      Guid propertyKind,
      List<string> properties)
    {
      requestContext.CheckOrganizationRequestContext();
      requestContext.TraceAlways(5109146, TraceLevel.Info, "Commerce", nameof (PlatformInfrastructureOrganizationService), string.Format("GetInfrastructureOrganizationProperties: propertyKind={0}, organizationHost={1}, properties={2}", (object) propertyKind, (object) requestContext.ServiceHost.InstanceId, (object) string.Join(",", properties.ToArray())));
      if (!requestContext.IsInfrastructureHost())
        throw new NotSupportedException("Not infrastructure host");
      CommercePropertyStore commercePropertyStore = new CommercePropertyStore();
      if (!commercePropertyStore.HasPropertyKind(requestContext, propertyKind))
        return (IEnumerable<string>) null;
      PropertiesCollection properties1 = commercePropertyStore.GetProperties(requestContext, propertyKind);
      List<string> organizationProperties = new List<string>();
      foreach (string property in properties)
      {
        string str;
        properties1.TryGetValue<string>(property, out str);
        organizationProperties.Add(str);
      }
      return (IEnumerable<string>) organizationProperties;
    }

    private void PublishCommerceIntelligenceEvent(
      IVssRequestContext requestContext,
      Guid infrastructureHostId,
      string collectionHostName,
      string organizationHostName,
      string hostRegion,
      DateTime startTime,
      DateTime endTime)
    {
      CustomerIntelligenceData eventData = new CustomerIntelligenceData();
      eventData.Add(CustomerIntelligenceProperty.StartTime, (object) startTime);
      eventData.Add(CustomerIntelligenceProperty.EndTime, (object) endTime);
      eventData.Add("InfrastructureHostOrganizationId", (object) infrastructureHostId);
      eventData.Add("InfrastructureHostOrganizationName", organizationHostName);
      eventData.Add("InfrastructureHostCollectionName", collectionHostName);
      eventData.Add("InfrastructureHostRegion", hostRegion);
      CustomerIntelligence.PublishEvent(requestContext, "CreateInfrastructureHost", eventData);
    }
  }
}
