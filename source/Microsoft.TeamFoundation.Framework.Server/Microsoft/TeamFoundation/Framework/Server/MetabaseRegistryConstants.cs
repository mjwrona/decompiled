// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.MetabaseRegistryConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal static class MetabaseRegistryConstants
  {
    public static readonly string Root = "/Configuration/Metabase";
    public static readonly string Deployment = MetabaseRegistryConstants.Root + "/Deployment";
    public static readonly string Shared = MetabaseRegistryConstants.Root + "/Shared";
    public static readonly string ApplicationRelativePathValue = "ApplicationRelativePath";
    public static readonly string ApplicationRelativePathContainsValue = "ApplicationRelativePathContains";
    public static readonly string UserAgentMatchValue = "UserAgentMatch";
    public static readonly string UserAgentStartsWithFilter = "UserAgentStartsWith";
    public static readonly string RequiredAuthenticationValue = "RequiredAuthentication";
    public static readonly string AllowedHandlersValue = "AllowedHandlers";
    public static readonly string AllowNonSslValue = "AllowNonSsl";
    public static readonly string AllowCORS = nameof (AllowCORS);
    public static readonly string ExactPathMatchOnlyValue = "ExactPathMatchOnly";
    public static readonly string HostedOnlyValue = "HostedOnly";
    public static readonly string AuthenticationMechanismsValue = "AuthenticationMechanisms";
  }
}
