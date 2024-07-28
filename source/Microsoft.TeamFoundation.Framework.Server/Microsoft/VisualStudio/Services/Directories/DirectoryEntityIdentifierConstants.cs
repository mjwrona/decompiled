// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryEntityIdentifierConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.Directories
{
  internal static class DirectoryEntityIdentifierConstants
  {
    internal static class Source
    {
      internal const string AAD = "aad";
      internal const string AD = "ad";
      internal const string WMD = "wmd";
      internal const string IMS = "ims";
      internal const string GHB = "ghb";
    }

    internal static class Type
    {
      internal const string Group = "group";
      internal const string User = "user";
      internal const string ServicePrincipal = "servicePrincipal";
    }
  }
}
