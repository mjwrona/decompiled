// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SearchFilter.AncestorAndIdentitySearchFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity.SearchFilter
{
  internal sealed class AncestorAndIdentitySearchFilter : IdentitySearchFilterBase
  {
    private const int MaxFilterByAncestorsCount = 25;
    private const int MaxFilterByIdentitiesCount = 25;

    private HashSet<IdentityDescriptor> FilterByAncestors { get; set; }

    private HashSet<IdentityDescriptor> FilterByIdentites { get; set; }

    internal AncestorAndIdentitySearchFilter(
      IVssRequestContext requestContext,
      IIdentityProvider identityProvider,
      IEnumerable<IdentityDescriptor> filterByAncestors,
      IEnumerable<IdentityDescriptor> filterByIdentites)
      : this(requestContext, identityProvider, AncestorAndIdentitySearchFilter.GetIdentitySearchParameters(filterByAncestors, filterByIdentites))
    {
    }

    internal AncestorAndIdentitySearchFilter(
      IVssRequestContext requestContext,
      IIdentityProvider identityProvider,
      IdentitySearchParameters searchParameters)
      : base(requestContext, identityProvider, searchParameters)
    {
    }

    protected override FilterApplicability EvaluateApplicability(IVssRequestContext requestContext)
    {
      this.FilterByAncestors = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      this.FilterByIdentites = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      IEnumerable<IdentityDescriptor> filterByAncestors = this.SearchParameters.FilterByAncestors;
      IEnumerable<IdentityDescriptor> filterByIdentities = this.SearchParameters.FilterByIdentities;
      if (filterByAncestors.IsNullOrEmpty<IdentityDescriptor>() && filterByIdentities.IsNullOrEmpty<IdentityDescriptor>())
        return FilterApplicability.NotApplicable;
      if (!filterByAncestors.IsNullOrEmpty<IdentityDescriptor>())
      {
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> source = this.IdentityProvider.ReadIdentities(requestContext, (IList<IdentityDescriptor>) filterByAncestors.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null)).ToList<IdentityDescriptor>()).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null && identity.IsActive && identity.IsContainer));
        this.FilterByAncestors.AddRange<IdentityDescriptor, HashSet<IdentityDescriptor>>(source.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)));
        this.FilterByIdentites.AddRange<IdentityDescriptor, HashSet<IdentityDescriptor>>(source.Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)));
      }
      if (!filterByIdentities.IsNullOrEmpty<IdentityDescriptor>())
        this.FilterByIdentites.AddRange<IdentityDescriptor, HashSet<IdentityDescriptor>>(this.IdentityProvider.ReadIdentities(requestContext, (IList<IdentityDescriptor>) filterByIdentities.Where<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (x => x != (IdentityDescriptor) null)).ToList<IdentityDescriptor>()).Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (identity => identity != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (identity => identity.Descriptor)));
      return this.FilterByAncestors.Any<IdentityDescriptor>() || this.FilterByIdentites.Any<IdentityDescriptor>() ? FilterApplicability.Applicable : FilterApplicability.Unresolved;
    }

    protected override void ValidateIdentitySearchParameters(
      IVssRequestContext requestContext,
      IdentitySearchParameters identitySearchParameters)
    {
      ArgumentUtility.CheckForNull<IdentitySearchParameters>(identitySearchParameters, nameof (identitySearchParameters));
      if (identitySearchParameters.FilterByAncestors != null)
        ArgumentUtility.CheckBoundsInclusive(identitySearchParameters.FilterByAncestors.Count<IdentityDescriptor>(), 0, 25, "FilterByAncestors");
      if (identitySearchParameters.FilterByIdentities == null)
        return;
      ArgumentUtility.CheckBoundsInclusive(identitySearchParameters.FilterByIdentities.Count<IdentityDescriptor>(), 0, 25, "FilterByIdentities");
    }

    internal override IEnumerable<T> FilterIdentities<T>(
      IVssRequestContext requestContext,
      IEnumerable<T> identities,
      Func<T, IdentityDescriptor> getIdentityDescriptor)
    {
      if (identities == null)
        return Enumerable.Empty<T>();
      ArgumentUtility.CheckForNull<Func<T, IdentityDescriptor>>(getIdentityDescriptor, nameof (getIdentityDescriptor));
      switch (this.FilterApplicability)
      {
        case FilterApplicability.NotApplicable:
          return identities;
        case FilterApplicability.Applicable:
          return identities.Where<T>((Func<T, bool>) (identity =>
          {
            if ((object) identity == null)
              return false;
            IdentityDescriptor identityDescriptor = getIdentityDescriptor(identity);
            if (identityDescriptor == (IdentityDescriptor) null)
              return false;
            return this.FilterByIdentites.Contains(identityDescriptor) || this.FilterByAncestors.Any<IdentityDescriptor>((Func<IdentityDescriptor, bool>) (groupDescriptor => this.IdentityProvider.IsMember(requestContext, groupDescriptor, identityDescriptor)));
          }));
        case FilterApplicability.Unresolved:
          return Enumerable.Empty<T>();
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    private static IdentitySearchParameters GetIdentitySearchParameters(
      IEnumerable<IdentityDescriptor> filterByAncestors,
      IEnumerable<IdentityDescriptor> filterByIdentities)
    {
      return new IdentitySearchParameters(string.Empty, IdentitySearchKind.None, IdentitySearchType.All, (IEnumerable<string>) null, 0, (string) null, filterByAncestors, filterByIdentities);
    }
  }
}
