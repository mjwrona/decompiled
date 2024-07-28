// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.OrganizationCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class OrganizationCacheService : IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private INotificationRegistration m_organizationRegistration;
    private readonly PropertyContainerCache<Microsoft.VisualStudio.Services.Organization.Organization> m_cache;
    private const string c_area = "Organization";
    private const string c_layer = "OrganizationCacheService";

    public OrganizationCacheService() => this.m_cache = new PropertyContainerCache<Microsoft.VisualStudio.Services.Organization.Organization>();

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.m_organizationRegistration = context.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(context, "Default", SqlNotificationEventClasses.OrganizationChanged, new SqlNotificationHandler(this.OnOrganizationChanged), false, false);
    }

    public void ServiceEnd(IVssRequestContext context) => this.m_organizationRegistration.Unregister(context);

    private void OnOrganizationChanged(IVssRequestContext context, NotificationEventArgs args)
    {
      try
      {
        OrganizationChangedData organizationChangedData = args.Deserialize<OrganizationChangedData>();
        this.m_cache.Remove(context, organizationChangedData.OrganizationId);
      }
      catch (Exception ex)
      {
        context.TraceException(5004318, "Organization", nameof (OrganizationCacheService), ex);
      }
    }

    public Microsoft.VisualStudio.Services.Organization.Organization Get(
      IVssRequestContext context,
      Guid organizationId,
      IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      return this.m_cache.Get(context, organizationId, propertyNames);
    }

    public void Update(
      IVssRequestContext context,
      Microsoft.VisualStudio.Services.Organization.Organization organization,
      IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      this.m_cache.Update(context, organization, propertyNames);
    }

    public void Remove(IVssRequestContext context, Guid organizationId)
    {
      this.ValidateRequestContext(context);
      this.m_cache.Remove(context, organizationId);
    }

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
  }
}
