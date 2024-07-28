// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.IdentityProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  internal abstract class IdentityProvider
  {
    internal abstract IDictionary<string, QueryTokenResult> GetIdentities(
      IdentityProviderAdapterGetRequest ipdGetRequest);

    internal abstract IList<Guid> GetIdentities(IVssRequestContext requestContext);

    internal abstract bool RemoveIdentities(IVssRequestContext requestContext, IList<string> vsids);

    internal abstract bool AddIdentities(IVssRequestContext requestContext, IList<string> vsids);

    internal abstract byte[] GetIdentityImage(
      IVssRequestContext requestContext,
      string objectId,
      Dictionary<string, object> options = null);

    internal static void AddOrMergeQueryTokenResult(
      IDictionary<string, QueryTokenResult> resultBuilder,
      QueryTokenResult newQtr,
      bool replace = false)
    {
      try
      {
        if (resultBuilder.ContainsKey(newQtr.QueryToken) && !replace)
          resultBuilder[newQtr.QueryToken].Identities = (IList<Identity>) IdentityProvider.Merge((IEnumerable<Identity>) resultBuilder[newQtr.QueryToken].Identities, (IEnumerable<Identity>) newQtr.Identities);
        else
          resultBuilder[newQtr.QueryToken] = newQtr;
      }
      catch (Exception ex)
      {
        throw new IdentityPickerValidateException("Error constructing response for a query token", ex);
      }
    }

    private static List<Identity> Merge(IEnumerable<Identity> list1, IEnumerable<Identity> list2)
    {
      SortedSet<Identity> collection = new SortedSet<Identity>();
      if (list1 != null)
      {
        foreach (Identity identity in list1)
          collection.Add(identity);
      }
      if (list2 != null)
      {
        foreach (Identity identity in list2)
          collection.Add(identity);
      }
      return new List<Identity>((IEnumerable<Identity>) collection);
    }
  }
}
