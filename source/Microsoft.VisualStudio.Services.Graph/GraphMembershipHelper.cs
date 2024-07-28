// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.GraphMembershipHelper
// Assembly: Microsoft.VisualStudio.Services.Graph, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00390AA0-D8BB-45EB-AEF5-70DC8BFC765D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Graph.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph
{
  public static class GraphMembershipHelper
  {
    private const string c_area = "Graph";
    private const string c_layer = "GraphMembershipHelper";

    public static void CheckForNullIdentities<T>(
      IVssRequestContext context,
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identities,
      IList<T> readIdentitiesKeys,
      bool shouldThrow = true)
    {
      ArgumentUtility.CheckForNull<IList<Microsoft.VisualStudio.Services.Identity.Identity>>(identities, nameof (identities));
      ArgumentUtility.CheckForNull<IList<T>>(readIdentitiesKeys, nameof (readIdentitiesKeys));
      if (identities.Count != readIdentitiesKeys.Count)
        throw new ArgumentException(string.Format("Number of identities = {0} is not equal to number of read keys = {1}", (object) identities.Count, (object) readIdentitiesKeys.Count));
      int num = identities.Count<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x == null));
      if (num == 0)
        return;
      IEnumerable<T> objs = identities.Zip<Microsoft.VisualStudio.Services.Identity.Identity, T, KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>((IEnumerable<T>) readIdentitiesKeys, (Func<Microsoft.VisualStudio.Services.Identity.Identity, T, KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>) ((identity, key) => new KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>(key, identity))).Where<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, bool>) (x => x.Value == null)).Select<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T>((Func<KeyValuePair<T, Microsoft.VisualStudio.Services.Identity.Identity>, T>) (x => x.Key));
      context.TraceAlways(15280158, TraceLevel.Error, "Graph", nameof (GraphMembershipHelper), "Identity not found for keys : " + objs.Serialize<IEnumerable<T>>());
      if (shouldThrow)
        throw new IdentityNotFoundException(string.Format("Unable to find identities for {0} keys", (object) num));
    }
  }
}
