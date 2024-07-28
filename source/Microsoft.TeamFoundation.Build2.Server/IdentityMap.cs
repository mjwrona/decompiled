// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IdentityMap
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class IdentityMap
  {
    private readonly IdentityService m_identityService;
    private readonly HashSet<string> m_imsCacheMisses = new HashSet<string>();
    private readonly Dictionary<string, IdentityRef> m_identityRefs = new Dictionary<string, IdentityRef>();

    public IdentityMap(IdentityService identityService) => this.m_identityService = identityService;

    public void AddIdentityRef(string identifier, IdentityRef identityRef)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(identifier, nameof (identifier));
      ArgumentUtility.CheckForNull<IdentityRef>(identityRef, nameof (identityRef));
      this.m_identityRefs[identifier] = identityRef;
    }

    public IdentityRef GetIdentityRef(IVssRequestContext requestContext, Guid identifier) => identifier == Guid.Empty ? (IdentityRef) null : this.GetIdentityRef(requestContext, identifier.ToString("D"));

    public IdentityRef GetIdentityRef(IVssRequestContext requestContext, string identifier)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "IdentityMap.GetIdentityRef"))
      {
        IdentityRef identityRef = (IdentityRef) null;
        if (!this.m_imsCacheMisses.Contains(identifier) && !this.m_identityRefs.TryGetValue(identifier, out identityRef))
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = this.m_identityService.GetIdentities(requestContext, identifier).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
          if (identity != null)
          {
            identityRef = identity.ToIdentityRef(requestContext);
            this.m_identityRefs[identifier] = identityRef;
          }
          else
            this.m_imsCacheMisses.Add(identifier);
        }
        return identityRef;
      }
    }
  }
}
