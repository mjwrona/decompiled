// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.TfvcIdentityHelper
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class TfvcIdentityHelper
  {
    internal static Microsoft.VisualStudio.Services.Identity.Identity FindIdentity(
      IVssRequestContext requestContext,
      Guid vsid,
      bool throwOnMissing = true)
    {
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity1 = IdentityHelper.FindIdentity(requestContext, vsid, throwOnMissing);
        if (identity1 == null)
        {
          requestContext.TraceAlways(700145, TraceLevel.Info, TraceArea.Identities, TraceLayer.BusinessLogic, "Found no identity for VSID:{0} {1}", (object) vsid, (object) Environment.StackTrace);
          Microsoft.VisualStudio.Services.Identity.Identity identity2 = new Microsoft.VisualStudio.Services.Identity.Identity();
          identity2.Id = vsid;
          identity2.CustomDisplayName = vsid.ToString();
          identity2.Descriptor = new IdentityDescriptor();
          identity1 = identity2;
        }
        return identity1;
      }
      catch (Microsoft.VisualStudio.Services.Identity.IdentityNotFoundException ex)
      {
        requestContext.TraceAlways(700146, TraceLevel.Info, TraceArea.Identities, TraceLayer.BusinessLogic, "Found no identity for VSID:{0} {1}", (object) vsid, (object) Environment.StackTrace);
        throw new IdentityNotFoundException("IdentityNotFoundException", vsid.ToString());
      }
    }

    internal static Microsoft.VisualStudio.Services.Identity.Identity FindIdentity(
      IVssRequestContext requestContext,
      string name)
    {
      try
      {
        return IdentityHelper.FindIdentity(requestContext, name, true, true);
      }
      catch (Microsoft.VisualStudio.Services.Identity.IdentityNotFoundException ex)
      {
        throw new IdentityNotFoundException("IdentityNotFoundException", name);
      }
    }
  }
}
