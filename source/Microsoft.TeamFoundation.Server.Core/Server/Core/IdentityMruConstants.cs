// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMruConstants
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class IdentityMruConstants
  {
    public const string IdentityMruPropertyName = "IdentityMru";
    public const string IdentityMruMaxSizePath = "/IdentityMruMaxSize";
    public const int DefaultIdentityMruMaxSizePath = 50;
    public const string IdentityMruWebApiResource = "identityMru";
    public const string IdentityMruWebApiRouteName = "IdentityMru";
    public static readonly Guid IdentityMruWebApiLocationId = Guid.Parse("5EAD0B70-2572-4697-97E9-F341069A783A");
    public const string Area = "IdentityMru";
  }
}
