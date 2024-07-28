// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectRevision
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectRevision
  {
    private long m_modified;
    private long m_deleted;

    internal ProjectRevision(long modifiedRevision, long deletedRevision)
    {
      this.m_modified = modifiedRevision;
      this.m_deleted = deletedRevision;
    }

    internal long Modified => this.m_modified;

    internal long Deleted => this.m_deleted;

    public override string ToString() => this.m_modified.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture) + ":" + this.m_deleted.ToString("X", (IFormatProvider) CultureInfo.InvariantCulture);

    public static ProjectRevision FromString(string projectRevision)
    {
      if (string.IsNullOrEmpty(projectRevision))
        return new ProjectRevision(0L, 0L);
      string[] strArray = projectRevision.Split(':');
      long result1;
      long result2;
      if (strArray.Length != 2 || !long.TryParse(strArray[0], NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result1) || !long.TryParse(strArray[1], NumberStyles.HexNumber, (IFormatProvider) CultureInfo.InvariantCulture, out result2))
        throw new ArgumentException("Provided ProjectRevision string is invalid");
      return new ProjectRevision(result1, result2);
    }
  }
}
