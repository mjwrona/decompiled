// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.LatestItemVersionUri
// Assembly: Microsoft.TeamFoundation.VersionControl.Common.Integration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2A9D748-4CEE-4498-9785-584B91A44F85
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.Integration.dll

using Microsoft.TeamFoundation.Client;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  public class LatestItemVersionUri : VersionControlIntegrationUri
  {
    public LatestItemVersionUri(string teamFoundationServerUrl, int itemId)
      : base(teamFoundationServerUrl, ArtifactType.LatestItemVersion, itemId.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    {
    }

    public LatestItemVersionUri(TfsTeamProjectCollection teamProjectCollection, int itemId)
      : base(teamProjectCollection, ArtifactType.LatestItemVersion, itemId.ToString((IFormatProvider) CultureInfo.InvariantCulture))
    {
    }
  }
}
