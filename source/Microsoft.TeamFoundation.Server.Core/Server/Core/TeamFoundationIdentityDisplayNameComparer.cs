// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TeamFoundationIdentityDisplayNameComparer
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class TeamFoundationIdentityDisplayNameComparer : 
    IComparer<TeamFoundationIdentity>,
    IEqualityComparer<TeamFoundationIdentity>
  {
    private bool m_lookForward;

    public TeamFoundationIdentityDisplayNameComparer()
      : this(true)
    {
    }

    public TeamFoundationIdentityDisplayNameComparer(bool lookForward) => this.m_lookForward = lookForward;

    public int Compare(TeamFoundationIdentity x, TeamFoundationIdentity y)
    {
      TeamFoundationIdentity foundationIdentity1 = this.m_lookForward ? x : y;
      TeamFoundationIdentity foundationIdentity2 = this.m_lookForward ? y : x;
      int num = string.Compare(foundationIdentity1.DisplayName, foundationIdentity2.DisplayName, StringComparison.CurrentCultureIgnoreCase);
      if (num == 0)
      {
        Guid teamFoundationId = foundationIdentity1.TeamFoundationId;
        string strA = teamFoundationId.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
        teamFoundationId = foundationIdentity2.TeamFoundationId;
        string strB = teamFoundationId.ToString((string) null, (IFormatProvider) CultureInfo.InvariantCulture);
        num = string.Compare(strA, strB, StringComparison.OrdinalIgnoreCase);
      }
      return num;
    }

    public int Compare(TeamFoundationIdentity x, string y) => this.m_lookForward ? string.Compare(x.DisplayName, y, StringComparison.CurrentCultureIgnoreCase) : string.Compare(y, x.DisplayName, StringComparison.CurrentCultureIgnoreCase);

    public bool Equals(TeamFoundationIdentity x, TeamFoundationIdentity y) => this.Compare(x, y) == 0;

    public int GetHashCode(TeamFoundationIdentity obj) => obj.TeamFoundationId.GetHashCode();
  }
}
