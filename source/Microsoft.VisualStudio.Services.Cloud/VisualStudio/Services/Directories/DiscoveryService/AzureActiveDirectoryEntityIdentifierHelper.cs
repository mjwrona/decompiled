// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.AzureActiveDirectoryEntityIdentifierHelper
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class AzureActiveDirectoryEntityIdentifierHelper
  {
    internal static DirectoryEntityIdentifierV1 CreateUserId(Guid oid) => AzureActiveDirectoryEntityIdentifierHelper.CreateEntityIdentifier(oid, "aad", "user");

    internal static DirectoryEntityIdentifierV1 CreateGroupId(Guid oid) => AzureActiveDirectoryEntityIdentifierHelper.CreateEntityIdentifier(oid, "aad", "group");

    internal static DirectoryEntityIdentifierV1 CreateServicePrincipalId(Guid oid) => AzureActiveDirectoryEntityIdentifierHelper.CreateEntityIdentifier(oid, "aad", "servicePrincipal");

    internal static bool TryGetUserOid(DirectoryEntityIdentifier entityId, out Guid oid) => AzureActiveDirectoryEntityIdentifierHelper.TryParseOid(entityId, "user", out oid);

    internal static bool TryGetServicePrincipalOid(DirectoryEntityIdentifier entityId, out Guid oid) => AzureActiveDirectoryEntityIdentifierHelper.TryParseOid(entityId, "servicePrincipal", out oid);

    internal static bool TryGetGroupOid(DirectoryEntityIdentifier entityId, out Guid oid) => AzureActiveDirectoryEntityIdentifierHelper.TryParseOid(entityId, "group", out oid);

    private static bool TryParseOid(DirectoryEntityIdentifier entityId, string Type, out Guid oid)
    {
      oid = Guid.Empty;
      if (entityId.Version == 1)
      {
        DirectoryEntityIdentifierV1 entityIdentifierV1 = (DirectoryEntityIdentifierV1) entityId;
        if ("aad".Equals(entityIdentifierV1.Source) && (string.IsNullOrEmpty(Type) || Type.Equals(entityIdentifierV1.Type)) && Guid.TryParseExact(entityIdentifierV1.Id, "N", out oid))
          return true;
      }
      return false;
    }

    private static DirectoryEntityIdentifierV1 CreateEntityIdentifier(
      Guid oid,
      string source,
      string type)
    {
      return new DirectoryEntityIdentifierV1(source, type, oid.ToString("N"));
    }
  }
}
