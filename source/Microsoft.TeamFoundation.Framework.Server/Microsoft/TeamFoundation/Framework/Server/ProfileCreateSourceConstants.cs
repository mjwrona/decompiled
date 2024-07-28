// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ProfileCreateSourceConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [GenerateSpecificConstants(null)]
  public static class ProfileCreateSourceConstants
  {
    [GenerateConstant(null)]
    public static readonly string Key = "createprofilesource";
    [GenerateConstant(null)]
    public static readonly string Web = "web";
    [GenerateConstant(null)]
    public const string WebWithAccount = "webWithAccount";
    public static readonly string Client = "client";
    public static readonly string Unknown = "unknown";
    public static readonly string Ibiza = "ibiza";
    public static readonly string IdentityMeEndpoint = "me_endpoint";
    public static readonly string VisualStudioVersion = nameof (VisualStudioVersion);
    public static readonly string VisualStudioSku = nameof (VisualStudioSku);
    public static readonly string Marketplace = nameof (Marketplace);
    public static readonly string IDE = nameof (IDE);
    public static readonly string DirectoryService = nameof (DirectoryService);
    public static readonly string IsCompact = "iscompact";
  }
}
