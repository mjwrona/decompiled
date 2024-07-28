// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PermissionLevel.PermissionLevelDescriptorExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;

namespace Microsoft.TeamFoundation.Framework.Server.PermissionLevel
{
  internal static class PermissionLevelDescriptorExtensions
  {
    private const string c_PermissionLevelIdentifierSeparator = ";";

    public static IdentityDescriptor ToIdentityDescriptor(
      this PermissionLevelDescriptor permissionLevelDescriptor)
    {
      ArgumentUtility.CheckForNull<PermissionLevelDescriptor>(permissionLevelDescriptor, nameof (permissionLevelDescriptor));
      return new IdentityDescriptor("Microsoft.VisualStudio.Services.PermissionLevel.PermissionLevelIdentity", PermissionLevelDescriptorExtensions.ConstructIdentityDescriptorIdentifier(permissionLevelDescriptor));
    }

    private static string ConstructIdentityDescriptorIdentifier(
      PermissionLevelDescriptor permissionLevelDescriptor)
    {
      return permissionLevelDescriptor.DefinitionId.ToString() + ";" + permissionLevelDescriptor.ResourceId;
    }
  }
}
