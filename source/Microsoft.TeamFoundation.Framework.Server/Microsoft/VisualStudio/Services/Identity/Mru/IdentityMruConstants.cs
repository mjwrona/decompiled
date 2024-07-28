// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.Mru.IdentityMruConstants
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.VisualStudio.Services.Identity.Mru
{
  internal class IdentityMruConstants
  {
    internal static readonly Guid PropertyArtifactKind = new Guid("8A3DD362-93CC-44AF-BADF-0EC5F86C8FE9");

    internal static class Tracing
    {
      internal const string Area = "Microsoft.VisualStudio.Services.Identity.Mru";

      internal static class Layer
      {
        internal const string IdMruService = "IdentityIdMruService";
        internal const string MruService = "IdentityMruService";
        internal const string MruPropertyHelper = "IdentityMruPropertyHelper";
        internal const string MruServiceSettings = "IdentityMruSettings";
        internal const string MruController = "MruIdentitiesController";
      }
    }

    internal static class FeatureFlags
    {
      internal const string MruServiceFeatureFlag = "Microsoft.VisualStudio.Services.Identity.IdentityMruService";
    }

    internal static class Registry
    {
      internal static class Keys
      {
        internal const string Root = "/Configuration/Identity/IdentityMru/MruService";
        internal const string RootFilter = "/Configuration/Identity/IdentityMru/MruService/...";
        internal const string MruMaxSizeKey = "/Configuration/Identity/IdentityMru/MruService/MruMaxSizeKey";
      }

      internal static class Defaults
      {
        internal const int DefaultMruMaxSize = 50;
      }
    }
  }
}
