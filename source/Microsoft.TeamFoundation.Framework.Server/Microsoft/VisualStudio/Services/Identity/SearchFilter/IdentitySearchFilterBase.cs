// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SearchFilter.IdentitySearchFilterBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity.SearchFilter
{
  internal abstract class IdentitySearchFilterBase
  {
    protected IIdentityProvider IdentityProvider { get; }

    protected IdentitySearchParameters SearchParameters { get; }

    protected internal FilterApplicability FilterApplicability { get; }

    internal IdentitySearchFilterBase(
      IVssRequestContext requestContext,
      IIdentityProvider identityProvider,
      IdentitySearchParameters searchParameters)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IIdentityProvider>(identityProvider, nameof (identityProvider));
      ArgumentUtility.CheckForNull<IdentitySearchParameters>(searchParameters, nameof (searchParameters));
      this.ValidateIdentitySearchParameters(requestContext, searchParameters);
      this.IdentityProvider = identityProvider;
      this.SearchParameters = searchParameters;
      this.FilterApplicability = this.EvaluateApplicability(requestContext);
    }

    protected abstract FilterApplicability EvaluateApplicability(IVssRequestContext requestContext);

    protected abstract void ValidateIdentitySearchParameters(
      IVssRequestContext requestContext,
      IdentitySearchParameters searchParameters);

    internal abstract IEnumerable<T> FilterIdentities<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> identities,
      Func<T, IdentityDescriptor> getIdentityDescriptor);

    internal static class FeatureFlags
    {
      internal const string DisableIdentitySearchFilters = "VisualStudio.Services.Identity.DisableIdentitySearchFilters";
      internal const string CheckGuestAccessForIdentityPicker = "Microsoft.AzureDevOps.Identity.CheckGuestAccessForIdentityPicker.M202";
    }
  }
}
