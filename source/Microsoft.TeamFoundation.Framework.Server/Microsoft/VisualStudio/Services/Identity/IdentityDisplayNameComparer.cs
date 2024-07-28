// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDisplayNameComparer
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal class IdentityDisplayNameComparer : IComparer<Microsoft.VisualStudio.Services.Identity.Identity>
  {
    private readonly bool m_lookForward;

    public IdentityDisplayNameComparer()
      : this(true)
    {
    }

    public IdentityDisplayNameComparer(bool lookForward) => this.m_lookForward = lookForward;

    public int Compare(Microsoft.VisualStudio.Services.Identity.Identity x, Microsoft.VisualStudio.Services.Identity.Identity y)
    {
      Microsoft.VisualStudio.Services.Identity.Identity identity1 = this.m_lookForward ? x : y;
      Microsoft.VisualStudio.Services.Identity.Identity identity2 = this.m_lookForward ? y : x;
      int num = string.Compare(identity1.DisplayName, identity2.DisplayName, StringComparison.CurrentCultureIgnoreCase);
      if (num == 0)
      {
        Guid id = identity1.Id;
        string strA = id.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
        id = identity2.Id;
        string strB = id.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
        num = string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase);
      }
      return num;
    }

    public int Compare(Microsoft.VisualStudio.Services.Identity.Identity x, string y) => this.m_lookForward ? string.Compare(x.DisplayName, y, StringComparison.CurrentCultureIgnoreCase) : string.Compare(y, x.DisplayName, StringComparison.CurrentCultureIgnoreCase);
  }
}
