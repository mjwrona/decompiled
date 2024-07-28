// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecurityNamespaceTemplateProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [InheritedExport]
  public abstract class SecurityNamespaceTemplateProvider
  {
    protected abstract IEnumerable<IMutableSecurityNamespaceTemplate> CreateMutableSecurityNamespaceTemplates(
      IVssRequestContext requestContext);

    public IEnumerable<SecurityNamespaceTemplate> CreateSecurityNamespaceTemplates(
      IVssRequestContext requestContext)
    {
      return this.CreateMutableSecurityNamespaceTemplates(requestContext).Select<IMutableSecurityNamespaceTemplate, SecurityNamespaceTemplate>((Func<IMutableSecurityNamespaceTemplate, SecurityNamespaceTemplate>) (mutableTemplate => mutableTemplate.GetSecurityNamespaceTemplate()));
    }
  }
}
