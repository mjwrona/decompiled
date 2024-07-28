// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.LocationServiceConstants
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class LocationServiceConstants
  {
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string DataDirEnvVar = "LOCATION_CACHE_DIRECTORY";
    public static readonly Guid SelfReferenceLocationServiceIdentifier = new Guid("bf9cf1d0-24ac-4d35-aeca-6cd18c69c1fe");
    public static readonly Guid ApplicationLocationServiceIdentifier = new Guid("8d299418-9467-402b-a171-9165e2f703e2");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly Guid InvalidServiceDefinitionIdentifier = new Guid("39b10086-1c48-4f34-a73b-73043d5170df");
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string CollectionLocationServiceRelativePath = "/Services/v3.0/LocationService.asmx";
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static readonly string ApplicationLocationServiceRelativePath = "/TeamFoundation/Administration/v3.0/LocationService.asmx";
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string PublicAccessMappingMoniker = "PublicAccessMapping";
    [Obsolete]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public const string ServerAccessMappingMoniker = "ServerAccessMapping";
  }
}
