// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.Service.OrganizationBillingService
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.VisualStudio.Services.Commerce.Service
{
  public class OrganizationBillingService : IOrganizationBillingService, IVssFrameworkService
  {
    public bool IsDailyBillingEnabled(IVssRequestContext requestContext, Guid organizationId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      SubjectDescriptor subjectDescriptor = vssRequestContext.GetAuthenticatedDescriptor().ToSubjectDescriptor(requestContext);
      if (!CommerceDeploymentHelper.IsProjectCollectionValidUser(vssRequestContext, subjectDescriptor, organizationId))
        throw new InvalidAccessException(string.Format("User {0} does not have permissions to access organization information.", (object) subjectDescriptor));
      return requestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableDailyBillingJob") && requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisableBillingForDeniedMeters") && requestContext.IsFeatureEnabled("Microsoft.Azure.DevOps.Commerce.DisableBillingDuringMeterResetsForDeniedMeters") && !vssRequestContext.GetService<PlatformSubscriptionService>().IsAssignmentBillingEnabled(requestContext, organizationId) && DailyBillingHelper.IsDailyBillingEnabledOrganization(requestContext, organizationId);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
