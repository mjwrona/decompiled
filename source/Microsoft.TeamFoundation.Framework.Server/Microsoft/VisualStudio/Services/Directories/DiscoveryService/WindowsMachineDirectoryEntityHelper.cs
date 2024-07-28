// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryEntityHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal class WindowsMachineDirectoryEntityHelper
  {
    internal static DirectoryEntityIdentifierV1 CreateUserId(string id, string source = "wmd") => new DirectoryEntityIdentifierV1(source, "user", id);

    internal static DirectoryEntityIdentifierV1 CreateGroupId(string id, string source = "wmd") => new DirectoryEntityIdentifierV1(source, "group", id);

    internal static bool IsWindowsMachineDirectoryGroup(
      DirectoryEntityIdentifier directoryEntityIdentifier)
    {
      return directoryEntityIdentifier.Version == 1 && directoryEntityIdentifier is DirectoryEntityIdentifierV1 entityIdentifierV1 && VssStringComparer.DirectoryName.Equals(entityIdentifierV1.Source, "wmd") && VssStringComparer.DirectoryEntityIdentifierConstants.Equals(entityIdentifierV1.Type, "group");
    }
  }
}
