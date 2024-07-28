// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.MemberOf2Controller
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Events;
using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Identity
{
  [ControllerApiVersion(6.0)]
  [VersionedApiControllerCustomName(Area = "IMS", ResourceName = "MembersOf", ResourceVersion = 2)]
  public class MemberOf2Controller : MemberOfController
  {
    private static readonly string c_OnDemandMembershipUpdateFeatureName = "VisualStudio.Services.Identity.OnDemandMembershipUpdate";
    private const string TraceLayer = "MemberOf2Controller";

    [HttpPost]
    [ClientLocationId("22865B02-9E4A-479E-9E18-E35B8803B8A0")]
    public IQueryable<IdentityDescriptor> RefreshMembersOf(
      string memberId,
      QueryMembership queryMembership = QueryMembership.Expanded)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(memberId, nameof (memberId));
      this.TfsRequestContext.CheckHostedDeployment();
      this.TfsRequestContext.CheckProjectCollectionRequestContext();
      if (!this.TfsRequestContext.IsFeatureEnabled(MemberOf2Controller.c_OnDemandMembershipUpdateFeatureName))
        throw new FeatureDisabledException(MemberOf2Controller.c_OnDemandMembershipUpdateFeatureName);
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = MembersController.ReadIdentity(this.TfsRequestContext, memberId, queryMembership);
      if (AadIdentityHelper.IsAadUser(identity1.Descriptor))
        this.PublishUserMembershipsUpdateEvent(this.TfsRequestContext, identity1.Descriptor);
      this.TfsRequestContext.RootContext.Items[RequestContextItemsKeys.IgnoreMembershipCache] = (object) true;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = MembersController.ReadIdentity(this.TfsRequestContext, memberId, queryMembership);
      this.TfsRequestContext.RootContext.Items.Remove(RequestContextItemsKeys.IgnoreMembershipCache);
      return identity2.MemberOf.AsQueryable<IdentityDescriptor>();
    }

    private void PublishUserMembershipsUpdateEvent(
      IVssRequestContext requestContext,
      IdentityDescriptor identityDescriptor)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string resource = vssRequestContext.GetService<IVssRegistryService>().GetValue(vssRequestContext, (RegistryQuery) "/Service/Aad/GraphApiResource", false, (string) null);
      UserMembershipsUpdateEvent membershipsUpdateEvent = new UserMembershipsUpdateEvent(identityDescriptor, false, true, resource);
      try
      {
        ITeamFoundationEventService service = vssRequestContext.GetService<ITeamFoundationEventService>();
        Stopwatch timer = Stopwatch.StartNew();
        IVssRequestContext requestContext1 = vssRequestContext;
        UserMembershipsUpdateEvent notificationEvent = membershipsUpdateEvent;
        service.SyncPublishNotification(requestContext1, (object) notificationEvent);
        timer.Stop();
        requestContext.TraceConditionally(4510379, TraceLevel.Info, this.TraceArea, nameof (MemberOf2Controller), (Func<string>) (() => string.Format("Published UserMembershipsUpdateEvent took {0} ms.", (object) timer.ElapsedMilliseconds)));
      }
      catch (Exception ex)
      {
        requestContext.TraceException(4510380, this.TraceArea, nameof (MemberOf2Controller), ex);
        throw;
      }
    }
  }
}
