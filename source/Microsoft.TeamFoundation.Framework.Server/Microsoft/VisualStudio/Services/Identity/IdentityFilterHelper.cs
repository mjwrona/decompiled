// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityFilterHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityFilterHelper
  {
    private const int c_bufferSize = 50;

    public static FilteredIdentitiesList FilterIdentities(
      IVssRequestContext requestContext,
      IdentityDomain hostDomain,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> unfilteredIdentities,
      IEnumerable<IdentityFilter> filters,
      int suggestedPageSize,
      string lastSearchResult,
      bool lookForward)
    {
      if (suggestedPageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof (suggestedPageSize));
      int num1 = 0;
      int num2 = 0;
      bool flag1 = false;
      List<Microsoft.VisualStudio.Services.Identity.Identity> identityList = new List<Microsoft.VisualStudio.Services.Identity.Identity>(suggestedPageSize + 50);
      IdentityDisplayNameEqualityComparer equalityComparer = new IdentityDisplayNameEqualityComparer();
      IdentityDisplayNameComparer displayNameComparer = new IdentityDisplayNameComparer(lookForward);
      foreach (Microsoft.VisualStudio.Services.Identity.Identity unfilteredIdentity in unfilteredIdentities)
      {
        if (unfilteredIdentity != null)
        {
          bool flag2 = true;
          if (string.IsNullOrEmpty(unfilteredIdentity.GetProperty<string>("RestrictedVisible", (string) null)) || requestContext.IsSystemContext)
          {
            if (filters != null)
            {
              foreach (IdentityFilter filter in filters)
              {
                if (!(flag2 = filter.IsMatch(requestContext, unfilteredIdentity)))
                  break;
              }
            }
            if (flag2)
            {
              ++num1;
              if (displayNameComparer.Compare(unfilteredIdentity, lastSearchResult) > 0)
              {
                int index = identityList.BinarySearch(unfilteredIdentity, (IComparer<Microsoft.VisualStudio.Services.Identity.Identity>) displayNameComparer);
                if (index < 0)
                  index = ~index;
                if (identityList.Count < suggestedPageSize)
                  identityList.Insert(index, unfilteredIdentity);
                else if (equalityComparer.Equals(unfilteredIdentity, identityList[identityList.Count - 1]))
                  identityList.Insert(index, unfilteredIdentity);
                else if (index < suggestedPageSize)
                {
                  identityList.Insert(index, unfilteredIdentity);
                  if (!equalityComparer.Equals(identityList[suggestedPageSize - 1], identityList[suggestedPageSize]))
                  {
                    if (!lookForward)
                      num2 += identityList.Count - suggestedPageSize;
                    identityList.RemoveRange(suggestedPageSize, identityList.Count - suggestedPageSize);
                    flag1 = true;
                  }
                }
                else
                {
                  if (!lookForward)
                    ++num2;
                  flag1 = true;
                }
              }
              else if (lookForward)
                ++num2;
            }
          }
        }
      }
      if (!lookForward)
        identityList.Reverse();
      return new FilteredIdentitiesList()
      {
        HasMoreItems = flag1,
        TotalItems = num1,
        StartingIndex = num2,
        Items = identityList.ToArray()
      };
    }
  }
}
