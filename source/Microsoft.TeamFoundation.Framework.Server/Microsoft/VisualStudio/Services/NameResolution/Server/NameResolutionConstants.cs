// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  public static class NameResolutionConstants
  {
    internal static readonly string RegistryRoot = "/Service/NameResolution/Settings/";
    internal static readonly string HitTTLKey = NameResolutionConstants.RegistryRoot + "HitTTL";
    internal static readonly string MissTTLKey = NameResolutionConstants.RegistryRoot + "MissTTL";
    internal static readonly string InactivityIntervalKey = NameResolutionConstants.RegistryRoot + "InactivityInterval";
    internal static readonly string NameResolutionStoreSilentFailure = NameResolutionConstants.RegistryRoot + nameof (NameResolutionStoreSilentFailure);
    internal static readonly string MpsCategory = "Mps";
    internal const int HitTTLDefault = 3600;
    internal const int MissTTLDefault = 300;
    internal const int InactivityIntervalDefault = 15;
    internal const int CleanupIntervalDefault = 15;
    public const int NamespaceMaxSize = 256;
    public const int NameMaxSize = 256;
    public const string TreatCachedNullAsCacheMiss = "TreatCachedNullAsCacheMiss";
    public static readonly Guid ReservedValue = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
  }
}
