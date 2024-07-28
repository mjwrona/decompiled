// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.AccessCheckExceptionProperties
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class AccessCheckExceptionProperties
  {
    public static readonly string DisplayName = nameof (DisplayName);
    public static readonly string Token = nameof (Token);
    public static readonly string RequestedPermissions = nameof (RequestedPermissions);
    public static readonly string NamespaceId = nameof (NamespaceId);
    public static readonly string DescriptorIdentifier = "Descriptor.Identifier";
    public static readonly string DescriptorIdentityType = "Descriptor.IdentityType";
  }
}
