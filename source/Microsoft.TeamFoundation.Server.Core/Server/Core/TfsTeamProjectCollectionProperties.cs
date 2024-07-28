// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.TfsTeamProjectCollectionProperties
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class TfsTeamProjectCollectionProperties : TeamProjectCollectionProperties
  {
    public TfsTeamProjectCollectionProperties()
    {
    }

    public TfsTeamProjectCollectionProperties(TeamProjectCollectionProperties baseProperties)
      : base(baseProperties)
    {
      this.EnableInheritedProcessCustomization = ProcessCustomizationType.Unknown;
    }

    internal TfsTeamProjectCollectionProperties(
      HostProperties serviceHostProperties,
      ISqlConnectionInfo frameworkConnectionInfo)
      : base(serviceHostProperties, frameworkConnectionInfo)
    {
    }

    internal TfsTeamProjectCollectionProperties(
      TeamFoundationServiceHostProperties serviceHostProperties,
      ISqlConnectionInfo frameworkConnectionInfo,
      bool isDefault)
      : base(serviceHostProperties, frameworkConnectionInfo, isDefault)
    {
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public ProcessCustomizationType EnableInheritedProcessCustomization { get; set; }
  }
}
