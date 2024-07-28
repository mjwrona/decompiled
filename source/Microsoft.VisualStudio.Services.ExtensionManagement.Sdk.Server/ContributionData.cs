// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionData
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public class ContributionData
  {
    private int? m_hashCode;
    public IEnumerable<Contribution> Contributions;
    public IEnumerable<ContributionType> ContributionTypes;
    public IEnumerable<ContributionConstraint> Constraints;

    public override int GetHashCode()
    {
      if (!this.m_hashCode.HasValue)
      {
        this.m_hashCode = new int?(0);
        if (this.Contributions != null)
        {
          foreach (Contribution contribution in this.Contributions)
          {
            int? hashCode1 = this.m_hashCode;
            int hashCode2 = contribution.GetHashCode();
            this.m_hashCode = hashCode1.HasValue ? new int?(hashCode1.GetValueOrDefault() ^ hashCode2) : new int?();
          }
        }
        if (this.ContributionTypes != null)
        {
          foreach (ContributionType contributionType in this.ContributionTypes)
          {
            int? hashCode3 = this.m_hashCode;
            int hashCode4 = contributionType.GetHashCode();
            this.m_hashCode = hashCode3.HasValue ? new int?(hashCode3.GetValueOrDefault() ^ hashCode4) : new int?();
          }
        }
        if (this.Constraints != null)
        {
          foreach (ContributionConstraint constraint in this.Constraints)
          {
            int? hashCode5 = this.m_hashCode;
            int hashCode6 = constraint.GetHashCode();
            this.m_hashCode = hashCode5.HasValue ? new int?(hashCode5.GetValueOrDefault() ^ hashCode6) : new int?();
          }
        }
      }
      return this.m_hashCode.Value;
    }
  }
}
