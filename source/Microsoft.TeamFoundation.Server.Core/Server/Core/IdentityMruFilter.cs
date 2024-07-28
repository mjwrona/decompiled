// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMruFilter
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class IdentityMruFilter : IdentityFilter
  {
    private string m_factorValue;

    public IdentityMruFilter(string factorValue) => this.m_factorValue = factorValue;

    public override bool IsMatch(IVssRequestContext requestContext, Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      string[] source = identity.DisplayName.Split((char[]) null);
      string property1 = identity.GetProperty<string>("Account", string.Empty);
      string property2 = identity.GetProperty<string>("Mail", string.Empty);
      Func<string, bool> predicate = (Func<string, bool>) (part => part.StartsWith(this.m_factorValue, StringComparison.OrdinalIgnoreCase));
      return ((IEnumerable<string>) source).Any<string>(predicate) || identity.DisplayName.StartsWith(this.m_factorValue, StringComparison.OrdinalIgnoreCase) || property1.StartsWith(this.m_factorValue, StringComparison.OrdinalIgnoreCase) || property2.StartsWith(this.m_factorValue, StringComparison.OrdinalIgnoreCase);
    }
  }
}
