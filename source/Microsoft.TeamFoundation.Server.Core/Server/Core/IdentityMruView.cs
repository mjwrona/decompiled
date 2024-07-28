// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMruView
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class IdentityMruView : IdentityPropertiesView
  {
    public static IEnumerable<Guid> ReadMru(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      string mruName,
      int maxSize)
    {
      IEnumerable<Guid> source = IdentityPropertiesView.CreateView<IdentityMruView>(requestContext, identity.TeamFoundationId, string.Format("{0}.{1}", (object) "IdentityMru", (object) mruName)).Read();
      if (source != null)
        source = source.Take<Guid>(maxSize);
      return source;
    }

    public static void UpdateMru(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      string mruName,
      IEnumerable<Guid> updatedEntries,
      int maxSize)
    {
      IdentityPropertiesView.CreateView<IdentityMruView>(requestContext, identity.TeamFoundationId, string.Format("{0}.{1}", (object) "IdentityMru", (object) mruName)).Update(requestContext, updatedEntries.Take<Guid>(maxSize));
    }

    public static void DeleteMru(
      IVssRequestContext requestContext,
      TeamFoundationIdentity identity,
      string mruName)
    {
      IdentityMruView view = IdentityPropertiesView.CreateView<IdentityMruView>(requestContext, identity.TeamFoundationId, string.Format("{0}.{1}", (object) "IdentityMru", (object) mruName));
      view.RemoveViewProperty(IdentityPropertyScope.Local, "");
      view.Update(requestContext);
    }

    private IEnumerable<Guid> Read()
    {
      IEnumerable<Guid> guids = (IEnumerable<Guid>) null;
      object propertyValue;
      if (this.TryGetViewProperty(IdentityPropertyScope.Local, "", out propertyValue) && propertyValue is string str)
        guids = JsonConvert.DeserializeObject<IEnumerable<Guid>>(str);
      return guids;
    }

    private void Update(IVssRequestContext requestContext, IEnumerable<Guid> updatedMruItems)
    {
      this.SetViewProperty(IdentityPropertyScope.Local, "", (object) JsonConvert.SerializeObject((object) updatedMruItems));
      this.Update(requestContext);
    }
  }
}
