// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureFlagsController
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.FeatureAvailability;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [VersionedApiControllerCustomName(Area = "FeatureAvailability", ResourceName = "FeatureFlags")]
  public class FeatureFlagsController : TfsApiController
  {
    public const string RouteName = "FeatureFlags";

    protected override void InitializeExceptionMap(ApiExceptionMapping exceptionMap)
    {
      base.InitializeExceptionMap(exceptionMap);
      exceptionMap.AddStatusCode<MissingFeatureException>(HttpStatusCode.NotFound);
      exceptionMap.AddStatusCode<InvalidFeatureFlagStateValueException>(HttpStatusCode.BadRequest);
      exceptionMap.AddStatusCode<IdentityNotFoundException>(HttpStatusCode.BadRequest);
    }

    public override string TraceArea => "FeatureFlags";

    public override string ActivityLogArea => "Framework";

    [TraceFilter(1011031, 1011040)]
    [HttpGet]
    public IQueryable<FeatureFlag> GetAllFeatureFlags()
    {
      this.AllowErrorDetails();
      return this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(this.TfsRequestContext).Select<FeatureAvailabilityInformation, FeatureFlag>(new Func<FeatureAvailabilityInformation, FeatureFlag>(this.ConvertToFeatureFlag)).AsQueryable<FeatureFlag>();
    }

    [TraceFilter(1011000, 1011010)]
    [HttpGet]
    public IQueryable<FeatureFlag> GetAllFeatureFlags(string userEmail)
    {
      this.AllowErrorDetails();
      Guid identity = this.GetIdentity(userEmail);
      return this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(this.TfsRequestContext, new Guid?(identity)).Select<FeatureAvailabilityInformation, FeatureFlag>((Func<FeatureAvailabilityInformation, FeatureFlag>) (x => this.ConvertToFeatureFlag(x, userEmail))).AsQueryable<FeatureFlag>();
    }

    [TraceFilter(1011011, 1011020)]
    [HttpGet]
    public FeatureFlag GetFeatureFlagByName(string name) => this.GetFeatureFlagByName(name, true);

    [TraceFilter(1011011, 1011020)]
    [HttpGet]
    public FeatureFlag GetFeatureFlagByName(string name, bool checkFeatureExists)
    {
      this.AllowErrorDetails();
      return this.ConvertToFeatureFlag(this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(this.TfsRequestContext, name, checkFeatureExists));
    }

    [TraceFilter(1011041, 1011050)]
    [HttpGet]
    public FeatureFlag GetFeatureFlagByNameAndUserId(Guid userId, string name) => this.GetFeatureFlagByNameAndUserId(userId, name, true);

    [TraceFilter(1011041, 1011050)]
    [HttpGet]
    public FeatureFlag GetFeatureFlagByNameAndUserId(
      Guid userId,
      string name,
      bool checkFeatureExists)
    {
      this.AllowErrorDetails();
      return this.ConvertToFeatureFlag(this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(this.TfsRequestContext, name, new Guid?(userId), checkFeatureExists), new Guid?(userId));
    }

    [TraceFilter(1011041, 1011050)]
    [HttpGet]
    public FeatureFlag GetFeatureFlagByNameAndUserEmail(string userEmail, string name) => this.GetFeatureFlagByNameAndUserEmail(userEmail, name, true);

    [TraceFilter(1011041, 1011050)]
    [HttpGet]
    public FeatureFlag GetFeatureFlagByNameAndUserEmail(
      string userEmail,
      string name,
      bool checkFeatureExists)
    {
      this.AllowErrorDetails();
      Guid identity = this.GetIdentity(userEmail);
      return this.ConvertToFeatureFlag(this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().GetFeatureInformation(this.TfsRequestContext, name, new Guid?(identity), checkFeatureExists), userEmail);
    }

    [TraceFilter(1011021, 1011030)]
    [HttpPatch]
    public FeatureFlag UpdateFeatureFlag(
      string name,
      FeatureFlagPatch state,
      bool checkFeatureExists = true,
      bool setAtApplicationLevelAlso = false)
    {
      this.AllowErrorDetails();
      FeatureAvailabilityState state1 = FeatureFlagsController.CheckAndConvertState(state);
      ITeamFoundationFeatureAvailabilityService service = this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      service.SetFeatureState(this.TfsRequestContext, name, state1, checkFeatureExists: checkFeatureExists);
      if (setAtApplicationLevelAlso && this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        if (!vssRequestContext.IsVirtualServiceHost())
          vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(vssRequestContext, name, state1, checkFeatureExists: checkFeatureExists);
        else
          this.TfsRequestContext.Trace(1011025, TraceLevel.Warning, this.TraceArea, nameof (UpdateFeatureFlag), "The current service has virtual application host, parameter setAtApplicationLevelAlso should not be set to true.");
      }
      return this.ConvertToFeatureFlag(service.GetFeatureInformation(this.TfsRequestContext, name));
    }

    [TraceFilter(1011021, 1011030)]
    [HttpPatch]
    public FeatureFlag UpdateFeatureFlag(
      string userEmail,
      string name,
      FeatureFlagPatch state,
      bool checkFeatureExists = true,
      bool setAtApplicationLevelAlso = false)
    {
      this.AllowErrorDetails();
      ITeamFoundationFeatureAvailabilityService service = this.TfsRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      FeatureAvailabilityState state1 = FeatureFlagsController.CheckAndConvertState(state);
      Guid identity = this.GetIdentity(userEmail);
      service.SetFeatureState(this.TfsRequestContext, name, state1, new Guid?(identity), checkFeatureExists);
      if (setAtApplicationLevelAlso && this.TfsRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
        if (!vssRequestContext.IsVirtualServiceHost())
          vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().SetFeatureState(vssRequestContext, name, state1, new Guid?(identity), checkFeatureExists);
        else
          this.TfsRequestContext.Trace(1011035, TraceLevel.Warning, this.TraceArea, nameof (UpdateFeatureFlag), "The current service has virtual application host, parameter setAtApplicationLevelAlso should not be set to true.");
      }
      return this.ConvertToFeatureFlag(service.GetFeatureInformation(this.TfsRequestContext, name, new Guid?(identity)), userEmail);
    }

    private static FeatureAvailabilityState CheckAndConvertState(FeatureFlagPatch state)
    {
      FeatureAvailabilityState result;
      if (state == null || !System.Enum.TryParse<FeatureAvailabilityState>(state.State, true, out result))
        throw new InvalidFeatureFlagStateValueException(state == null ? "null" : state.State);
      return result;
    }

    private void AllowErrorDetails() => this.Request.Properties[HttpPropertyKeys.IncludeErrorDetailKey] = (object) new Lazy<bool>((Func<bool>) (() => true));

    private FeatureFlag ConvertToFeatureFlag(FeatureAvailabilityInformation info) => this.ConvertToFeatureFlagImpl(info, (object) new
    {
      name = info.Name
    });

    private FeatureFlag ConvertToFeatureFlag(FeatureAvailabilityInformation info, Guid? userId) => this.ConvertToFeatureFlagImpl(info, (object) new
    {
      name = info.Name,
      userId = userId
    });

    private FeatureFlag ConvertToFeatureFlag(FeatureAvailabilityInformation info, string userEmail) => this.ConvertToFeatureFlagImpl(info, (object) new
    {
      name = info.Name,
      userEmail = userEmail
    });

    private FeatureFlag ConvertToFeatureFlagImpl(
      FeatureAvailabilityInformation info,
      object userIdentifier)
    {
      string name = info.Name;
      string description = info.Description;
      string uri = this.Url.Route(FeatureAvailabilityResourceIds.FeatureFlagsLocationId, userIdentifier);
      FeatureAvailabilityState availabilityState = info.EffectiveState;
      string effectiveState = availabilityState.ToString();
      availabilityState = info.ExplicitState;
      string explicitState = availabilityState.ToString();
      return new FeatureFlag(name, description, uri, effectiveState, explicitState);
    }

    private Guid GetIdentity(string email)
    {
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = this.TfsRequestContext.GetService<IdentityService>().ReadIdentities(this.TfsRequestContext, IdentitySearchFilter.AccountName, email, QueryMembership.None, (IEnumerable<string>) null);
      return identityList != null && identityList.Count != 0 ? identityList[0].Id : throw new IdentityNotFoundException();
    }
  }
}
