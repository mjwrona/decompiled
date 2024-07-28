// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.EnterpriseBillingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Commerce.Service
{
  public class EnterpriseBillingService : IEnterpriseBillingService, IVssFrameworkService
  {
    private const string Area = "Commerce";
    private const string Layer = "EnterpriseBillingService";

    public bool IsEnterpriseBillingEnabled(IVssRequestContext collectionContext)
    {
      try
      {
        collectionContext.CheckProjectCollectionRequestContext();
        Guid instanceId = collectionContext.ServiceHost.InstanceId;
        collectionContext.TraceAlways(5109310, TraceLevel.Info, "Commerce", nameof (EnterpriseBillingService), "Enter IsEnterprisBillingEnabled for organization {0}", (object) instanceId);
        IVssRequestContext vssRequestContext = collectionContext.GetAuthenticatedIdentity() != null ? collectionContext.To(TeamFoundationHostType.Deployment).Elevate() : throw new InvalidAccessException(string.Format("User {0} not a valid collection user", (object) collectionContext.GetAuthenticatedIdentity()));
        AzureResourceAccount accountByCollectionId = vssRequestContext.GetService<PlatformSubscriptionService>().GetAzureResourceAccountByCollectionId(vssRequestContext, instanceId, true);
        if (accountByCollectionId == null || accountByCollectionId.AzureSubscriptionId == Guid.Empty)
          return false;
        bool flag = AssignmentBillingHelper.IsAssignmentBillingEnabledForSubscription(collectionContext, accountByCollectionId.AzureSubscriptionId);
        collectionContext.TraceAlways(5109311, TraceLevel.Info, "Commerce", nameof (EnterpriseBillingService), "isEnterpriseBillingEnabled is {0} for organization {1}", flag ? (object) "enabled" : (object) "disabled", (object) instanceId);
        return flag;
      }
      catch (Exception ex)
      {
        collectionContext.TraceException(5109312, TraceLevel.Info, "Commerce", nameof (EnterpriseBillingService), ex);
        throw;
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckProjectCollectionRequestContext();
  }
}
