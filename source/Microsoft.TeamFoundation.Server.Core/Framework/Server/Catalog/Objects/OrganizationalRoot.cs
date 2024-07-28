// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Catalog.Objects.OrganizationalRoot
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("For PowerTools BackCompat only.  Use Microsoft.TeamFoundation.Server.Core.Catalog.Objects.OrganizationalRoot")]
  public class OrganizationalRoot : Microsoft.TeamFoundation.Server.Core.Catalog.Objects.OrganizationalRoot
  {
    private Microsoft.TeamFoundation.Server.Core.Catalog.Objects.OrganizationalRoot m_root;

    public OrganizationalRoot(Microsoft.TeamFoundation.Server.Core.Catalog.Objects.OrganizationalRoot root) => this.m_root = root;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public ICollection<SharePointWebApplication> SharePointWebApplications
    {
      get
      {
        List<SharePointWebApplication> pointWebApplications = new List<SharePointWebApplication>();
        foreach (Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication pointWebApplication in (IEnumerable<Microsoft.TeamFoundation.Server.Core.Catalog.Objects.SharePointWebApplication>) this.m_root.SharePointWebApplications)
          pointWebApplications.Add(new SharePointWebApplication(pointWebApplication));
        return (ICollection<SharePointWebApplication>) pointWebApplications;
      }
    }
  }
}
