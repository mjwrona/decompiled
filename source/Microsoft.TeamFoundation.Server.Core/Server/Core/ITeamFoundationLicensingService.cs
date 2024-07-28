// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ITeamFoundationLicensingService
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  [DefaultServiceImplementation(typeof (TeamFoundationOnPremLicensingService))]
  public interface ITeamFoundationLicensingService : IVssFrameworkService
  {
    ILicenseFeature GetLicenseFeature(IVssRequestContext requestContext, Guid licenseFeatureId);

    Guid[] GetLicenseFeaturesInPreview(IVssRequestContext requestContext);

    bool IsFeatureSupported(
      IVssRequestContext requestContext,
      Guid featureId,
      IdentityDescriptor userContext);

    bool IsFeatureSupported(IVssRequestContext requestContext, Guid featureId);

    bool IsFeatureInAdvertisingMode(IVssRequestContext requestContext, Guid featureId);

    ILicenseType[] GetLicensesForUser(
      IVssRequestContext requestContext,
      IdentityDescriptor userDescriptor);

    ILicenseType GetLicenseType(
      IVssRequestContext requestContext,
      Guid licenseTypeId,
      out ILicenseFeature[] licenseFeatures);

    ILicenseType[] GetAllLicenses(IVssRequestContext requestContext);
  }
}
