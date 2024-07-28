// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Linking.IArtifactLinkTypesService
// Assembly: Microsoft.Azure.Boards.Linking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2FA874A3-91E6-4EEC-B5F5-3126D83824FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Linking.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.Linking
{
  [DefaultServiceImplementation(typeof (ArtifactLinkTypesService))]
  public interface IArtifactLinkTypesService : IVssFrameworkService
  {
    Dictionary<string, List<RegistrationArtifactType>> GetArtifactLinkTypes(
      IVssRequestContext requestContext);

    IReadOnlyCollection<RegistrationArtifactType> GetArtifactLinkTypes(
      IVssRequestContext requestContext,
      string toolId);
  }
}
