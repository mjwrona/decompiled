// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.CollectionCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class CollectionCacheService : IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private readonly PropertyContainerCache<Collection> m_cache;
    private INotificationRegistration m_organizationAccountRegistration;
    private const string c_area = "Organization";
    private const string c_layer = "CollectionCacheService";

    public CollectionCacheService() => this.m_cache = new PropertyContainerCache<Collection>();

    public void ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      context.CheckDeploymentRequestContext();
      this.m_serviceHostId = context.ServiceHost.InstanceId;
      this.m_organizationAccountRegistration = context.GetService<ITeamFoundationSqlNotificationService>().CreateRegistration(context, "Default", SqlNotificationEventClasses.OrganizationAccountChanged, new SqlNotificationHandler(this.OnAccountChanged), false, false);
    }

    public void ServiceEnd(IVssRequestContext context) => this.m_organizationAccountRegistration.Unregister(context);

    private void OnAccountChanged(IVssRequestContext context, NotificationEventArgs args)
    {
      try
      {
        AccountChangedData accountChangedData = args.Deserialize<AccountChangedData>();
        this.m_cache.Remove(context, accountChangedData.AccountId);
      }
      catch (Exception ex)
      {
        context.TraceException(5004218, "Organization", nameof (CollectionCacheService), ex);
      }
    }

    public Collection Get(
      IVssRequestContext context,
      Guid accountId,
      IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      return this.m_cache.Get(context, accountId, propertyNames);
    }

    public void Update(
      IVssRequestContext context,
      Collection collection,
      IEnumerable<string> propertyNames)
    {
      this.ValidateRequestContext(context);
      this.m_cache.Update(context, collection, propertyNames);
    }

    public void Remove(IVssRequestContext context, Guid accountId)
    {
      this.ValidateRequestContext(context);
      this.m_cache.Remove(context, accountId);
    }

    private void ValidateRequestContext(IVssRequestContext context) => context.CheckServiceHostId(this.m_serviceHostId, (IVssFrameworkService) this);
  }
}
