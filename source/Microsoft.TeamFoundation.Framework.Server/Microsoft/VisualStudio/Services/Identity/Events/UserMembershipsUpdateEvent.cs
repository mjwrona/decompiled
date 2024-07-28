// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Events.UserMembershipsUpdateEvent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Identity.Events
{
  public class UserMembershipsUpdateEvent
  {
    private const string c_area = "Aad";
    private const string c_layer = "UserMembershipsUpdateEvent";
    private const string c_onDemandMembershipUpdateFeatureName = "VisualStudio.Services.Identity.OnDemandMembershipUpdate";
    private static readonly string VstsAadResouces = "/Configuration/AAD/VstsAadResources";

    public UserMembershipsUpdateEvent(
      IdentityDescriptor identityDescriptor,
      bool tokenCacheUpdated,
      bool onDemandMembershipUpdateRequested,
      string resource)
    {
      this.IdentityDescriptor = identityDescriptor;
      this.TokenCacheUpdated = tokenCacheUpdated;
      this.OnDemandMembershipUpdateRequested = onDemandMembershipUpdateRequested;
      this.Resource = resource;
    }

    public IdentityDescriptor IdentityDescriptor { get; private set; }

    public bool TokenCacheUpdated { get; private set; }

    public bool OnDemandMembershipUpdateRequested { get; private set; }

    public string Resource { get; private set; }

    public bool IsValidEvent(IVssRequestContext requestContext)
    {
      if (this.IdentityDescriptor == (IdentityDescriptor) null)
      {
        requestContext.Trace(10013035, TraceLevel.Error, "Aad", nameof (UserMembershipsUpdateEvent), "Request identity is null.");
        return false;
      }
      if (this.Resource == null)
        return false;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.OnDemandMembershipUpdate"))
      {
        if (!this.OnDemandMembershipUpdateRequested && !this.TokenCacheUpdated)
          return false;
      }
      else if (!this.TokenCacheUpdated)
        return false;
      if (this.CheckResource(requestContext, this.Resource))
        return true;
      requestContext.Trace(10013040, TraceLevel.Error, "Aad", nameof (UserMembershipsUpdateEvent), "Target resource " + this.Resource + " is not in the resource set.");
      return false;
    }

    private bool CheckResource(IVssRequestContext requestContext, string resource)
    {
      string enumerable = requestContext.GetService<IVssRegistryService>().GetValue(requestContext, (RegistryQuery) UserMembershipsUpdateEvent.VstsAadResouces, string.Empty);
      requestContext.Trace(10013041, TraceLevel.Info, "Aad", nameof (UserMembershipsUpdateEvent), "The resource string is " + enumerable + ".");
      if (enumerable.IsNullOrEmpty<char>())
        return false;
      try
      {
        return new HashSet<string>((IEnumerable<string>) Array.ConvertAll<string, string>(enumerable.Split(','), (Converter<string, string>) (r => r.Trim())), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Contains(resource);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013038, TraceLevel.Error, "Aad", nameof (UserMembershipsUpdateEvent), ex);
        return false;
      }
    }
  }
}
