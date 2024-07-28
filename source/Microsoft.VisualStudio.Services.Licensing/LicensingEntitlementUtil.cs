// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.LicensingEntitlementUtil
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class LicensingEntitlementUtil
  {
    public static string GetImageResourceUrl(IVssRequestContext requestContext, Guid identityId)
    {
      if (string.IsNullOrEmpty(identityId.ToString()))
        return string.Empty;
      IVssRequestContext requestContext1;
      Guid serviceAreaIdentifier;
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
      {
        requestContext1 = requestContext;
        serviceAreaIdentifier = Guid.Empty;
      }
      else
      {
        requestContext1 = requestContext.To(TeamFoundationHostType.Application);
        serviceAreaIdentifier = ServiceInstanceTypes.SPS;
      }
      ILocationService service = requestContext1.ToDeployment().GetService<ILocationService>();
      IVssRequestContext requestContext2 = requestContext1.ToDeployment().Elevate();
      return service.GetLocationData(requestContext2, serviceAreaIdentifier).GetResourceUri(requestContext2, "Profile", ProfileResourceIds.AvatarLocationid, (object) new
      {
        parentresource = "Profiles",
        id = identityId.ToString()
      }).ToString() + "?size=medium&format=png";
    }
  }
}
