// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ArtifactTypeCombiner
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ArtifactTypeCombiner : ICombiner<RegistrationArtifactType>
  {
    public RegistrationArtifactType Combine(
      RegistrationArtifactType t1,
      RegistrationArtifactType t2)
    {
      return new RegistrationArtifactType()
      {
        Name = t1.Name,
        OutboundLinkTypes = FrameworkRegistrationEntry.MergeArrays<OutboundLinkType>(t1.OutboundLinkTypes, t2.OutboundLinkTypes, (IComparer<OutboundLinkType>) new OutboundLinkTypeComparer(), (ICombiner<OutboundLinkType>) new SimpleCombiner<OutboundLinkType>())
      };
    }
  }
}
