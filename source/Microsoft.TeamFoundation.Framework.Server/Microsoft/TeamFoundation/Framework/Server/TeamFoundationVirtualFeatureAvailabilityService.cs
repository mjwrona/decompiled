// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationVirtualFeatureAvailabilityService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TeamFoundationVirtualFeatureAvailabilityService : 
    ITeamFoundationFeatureAvailabilityService,
    IVssFrameworkService
  {
    private const string c_area = "VirtualFeatureAvailabilityService";
    private const string c_layer = "BusinessLogic";

    public IEnumerable<FeatureAvailabilityInformation> GetFeatureInformation(
      IVssRequestContext requestContext)
    {
      return this.UseRootService<IEnumerable<FeatureAvailabilityInformation>>(requestContext, (string) null, (Func<IVssRequestContext, ITeamFoundationFeatureAvailabilityService, IEnumerable<FeatureAvailabilityInformation>>) ((rootContext, rootService) => rootService.GetFeatureInformation(rootContext)));
    }

    public FeatureAvailabilityInformation GetFeatureInformation(
      IVssRequestContext requestContext,
      string featureName,
      bool checkFeatureExists = true)
    {
      return this.UseRootService<FeatureAvailabilityInformation>(requestContext, featureName, (Func<IVssRequestContext, ITeamFoundationFeatureAvailabilityService, FeatureAvailabilityInformation>) ((rootContext, rootService) => rootService.GetFeatureInformation(rootContext, featureName, checkFeatureExists)));
    }

    public IEnumerable<FeatureAvailabilityInformation> GetFeatureInformation(
      IVssRequestContext requestContext,
      Guid? userId)
    {
      return this.UseRootService<IEnumerable<FeatureAvailabilityInformation>>(requestContext, (string) null, (Func<IVssRequestContext, ITeamFoundationFeatureAvailabilityService, IEnumerable<FeatureAvailabilityInformation>>) ((rootContext, rootService) => rootService.GetFeatureInformation(rootContext, userId)));
    }

    public FeatureAvailabilityInformation GetFeatureInformation(
      IVssRequestContext requestContext,
      string featureName,
      Guid? userId,
      bool checkFeatureExists = true)
    {
      return this.UseRootService<FeatureAvailabilityInformation>(requestContext, featureName, (Func<IVssRequestContext, ITeamFoundationFeatureAvailabilityService, FeatureAvailabilityInformation>) ((rootContext, rootService) => rootService.GetFeatureInformation(requestContext, featureName, userId, checkFeatureExists)));
    }

    public bool IsFeatureEnabled(IVssRequestContext requestContext, string featureName) => this.UseRootService<bool>(requestContext, featureName, (Func<IVssRequestContext, ITeamFoundationFeatureAvailabilityService, bool>) ((rootContext, rootService) => rootService.IsFeatureEnabled(rootContext, featureName)));

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void SetFeatureState(
      IVssRequestContext requestContext,
      string featureName,
      FeatureAvailabilityState state,
      Guid? userId = null,
      bool checkFeatureExists = true)
    {
      throw new VirtualServiceHostException();
    }

    public T UseRootService<T>(
      IVssRequestContext requestContext,
      string featureName,
      Func<IVssRequestContext, ITeamFoundationFeatureAvailabilityService, T> function)
    {
      IVssRequestContext context = requestContext.IsSystemContext ? requestContext.RootContext.Elevate() : requestContext.RootContext;
      return context.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? function(context, context.GetService<ITeamFoundationFeatureAvailabilityService>()) : function(requestContext, (ITeamFoundationFeatureAvailabilityService) requestContext.GetService<TeamFoundationFeatureAvailabilityService>());
    }
  }
}
