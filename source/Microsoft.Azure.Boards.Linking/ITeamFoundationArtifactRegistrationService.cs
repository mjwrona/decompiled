// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Linking.ITeamFoundationArtifactRegistrationService
// Assembly: Microsoft.Azure.Boards.Linking, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2FA874A3-91E6-4EEC-B5F5-3126D83824FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Linking.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Boards.Linking
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.Server.Core.RegistrationProvider, Microsoft.TeamFoundation.Server.Core")]
  public interface ITeamFoundationArtifactRegistrationService : IVssFrameworkService
  {
    FrameworkRegistrationEntry[] GetRegistrationEntries(
      IVssRequestContext requestContext,
      string toolId);
  }
}
