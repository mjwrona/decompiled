// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Catalog.Objects.CatalogObjectContext
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.ComponentModel;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server.Catalog.Objects
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Obsolete("For PowerTools BackCompat only.  Use Microsoft.TeamFoundation.Server.Core.Catalog.Objects.CatalogObjectContext")]
  public class CatalogObjectContext : Microsoft.TeamFoundation.Server.Core.Catalog.Objects.CatalogObjectContext
  {
    public CatalogObjectContext(IVssRequestContext requestContext)
      : base(requestContext, false)
    {
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    public OrganizationalRoot OrganizationalRoot => new OrganizationalRoot(base.OrganizationalRoot);
  }
}
