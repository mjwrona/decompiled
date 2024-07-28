// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectoryEntityIdentifierHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class GitHubDirectoryEntityIdentifierHelper
  {
    internal static DirectoryEntityIdentifierV1 CreateUserEntityId(string originId) => new DirectoryEntityIdentifierV1("ghb", "user", originId);

    internal static bool TryParseUserId(DirectoryEntityIdentifier entityId, out string gitHubUserId)
    {
      gitHubUserId = "";
      if (entityId.Version == 1)
      {
        DirectoryEntityIdentifierV1 entityIdentifierV1 = (DirectoryEntityIdentifierV1) entityId;
        if ("ghb".Equals(entityIdentifierV1.Source) && "user".Equals(entityIdentifierV1.Type))
        {
          gitHubUserId = entityIdentifierV1.Id;
          return true;
        }
      }
      return false;
    }
  }
}
