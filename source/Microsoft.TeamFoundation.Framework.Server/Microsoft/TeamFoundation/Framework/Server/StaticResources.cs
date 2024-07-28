// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.StaticResources
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Internal;
using System;
using System.Configuration;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class StaticResources
  {
    public static string GetCdnRootUrl(IVssRequestContext requestContext) => StaticResourcesUrlBuilder.Current.GetCdnRootUrl(requestContext);

    internal static string GetStaticUrl(
      IVssRequestContext requestContext = null,
      string path = null,
      StaticResourcePathKind pathKind = StaticResourcePathKind.Relative)
    {
      return StaticResourcesUrlBuilder.Current.GetStaticUrl(requestContext, path, pathKind);
    }

    private static string GetLocation(
      string relativePath,
      string cdnRelativePath = null,
      IVssRequestContext requestContext = null)
    {
      string cdnRootUrl = StaticResources.GetCdnRootUrl(requestContext);
      return string.IsNullOrEmpty(cdnRootUrl) ? StaticResources.GetLocalLocation(relativePath, requestContext) : VirtualPathUtility.AppendTrailingSlash(cdnRootUrl) + (cdnRelativePath ?? relativePath);
    }

    private static string GetLocalLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetStaticUrl(requestContext, relativePath, StaticResourcePathKind.Remote);

    private static string GetPhysicalLocation(
      string relativePath,
      IVssRequestContext requestContext = null)
    {
      return StaticResources.GetStaticUrl(requestContext, relativePath, StaticResourcePathKind.Physical);
    }

    public static class ThirdParty
    {
      private static readonly string RelativeRoot = "3rdParty/";

      public static string GetLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocation(StaticResources.ThirdParty.RelativeRoot + relativePath, requestContext: requestContext);

      public static string GetLocalLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocalLocation(StaticResources.ThirdParty.RelativeRoot + relativePath, requestContext);

      public static string GetPhysicalLocation(
        string relativePath,
        IVssRequestContext requestContext = null)
      {
        return StaticResources.GetPhysicalLocation(StaticResources.ThirdParty.RelativeRoot + relativePath, requestContext);
      }

      public static class Content
      {
        private static readonly string RelativeRoot = StaticResources.ThirdParty.RelativeRoot + "_content/";

        public static string GetLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocation(StaticResources.ThirdParty.Content.RelativeRoot + relativePath, requestContext: requestContext);

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null)
        {
          return StaticResources.GetLocalLocation(StaticResources.ThirdParty.Content.RelativeRoot + relativePath, requestContext);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null)
        {
          return StaticResources.GetPhysicalLocation(StaticResources.ThirdParty.Content.RelativeRoot + relativePath, requestContext);
        }
      }

      public static class Scripts
      {
        private static readonly string RelativeRoot = StaticResources.ThirdParty.RelativeRoot + "_scripts/";

        public static string GetLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocation(StaticResources.ThirdParty.Scripts.RelativeRoot + relativePath, requestContext: requestContext);

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null)
        {
          return StaticResources.GetLocalLocation(StaticResources.ThirdParty.Scripts.RelativeRoot + relativePath, requestContext);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null)
        {
          return StaticResources.GetPhysicalLocation(StaticResources.ThirdParty.Scripts.RelativeRoot + relativePath, requestContext);
        }
      }

      public static class Fonts
      {
        private static readonly string RelativeRoot = StaticResources.ThirdParty.RelativeRoot + "_fonts/";

        public static string GetLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocation(StaticResources.ThirdParty.Fonts.RelativeRoot + relativePath, requestContext: requestContext);

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null)
        {
          return StaticResources.GetLocalLocation(StaticResources.ThirdParty.Fonts.RelativeRoot + relativePath, requestContext);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null)
        {
          return StaticResources.GetPhysicalLocation(StaticResources.ThirdParty.Fonts.RelativeRoot + relativePath, requestContext);
        }
      }
    }

    public static class Content
    {
      private static readonly string RelativeRoot = "content/";

      public static string GetLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocation(StaticResources.Content.RelativeRoot + relativePath, requestContext: requestContext);

      public static string GetLocalLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocalLocation(StaticResources.Content.RelativeRoot + relativePath, requestContext);

      public static string GetPhysicalLocation(
        string relativePath,
        IVssRequestContext requestContext = null)
      {
        return StaticResources.GetPhysicalLocation(StaticResources.Content.RelativeRoot + relativePath, requestContext);
      }
    }

    public static class Extensions
    {
      private static readonly string RelativeRoot = "_ext/";
      private static readonly string CdnRelativeRoot = "ext/";

      public static string GetLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocation(StaticResources.Extensions.RelativeRoot + relativePath, StaticResources.Extensions.CdnRelativeRoot + relativePath, requestContext);

      public static string GetLocalLocation(string relativePath, IVssRequestContext requestContext = null) => StaticResources.GetLocalLocation(StaticResources.Extensions.RelativeRoot + relativePath, requestContext);

      public static string GetPhysicalLocation(
        string relativePath,
        IVssRequestContext requestContext = null)
      {
        return StaticResources.GetPhysicalLocation(StaticResources.Extensions.RelativeRoot + relativePath, requestContext);
      }
    }

    public static class Versioned
    {
      private static string s_version;

      public static string Version
      {
        get
        {
          if (StaticResources.Versioned.s_version == null)
          {
            string appSetting = ConfigurationManager.AppSettings["staticContentVersion"];
            StaticResources.Versioned.s_version = !string.IsNullOrEmpty(appSetting) ? appSetting : throw new InvalidOperationException("staticContentVersion setting is not defined in the web.config");
          }
          return StaticResources.Versioned.s_version;
        }
        internal set => StaticResources.Versioned.s_version = value;
      }

      public static string GetLocation(
        string relativePath,
        IVssRequestContext requestContext = null,
        string version = null)
      {
        return StaticResources.GetLocation("tfs/" + (version ?? StaticResources.Versioned.Version) + "/" + relativePath, "v/" + (version ?? StaticResources.Versioned.Version) + "/" + relativePath, requestContext);
      }

      public static string GetLocalLocation(
        string relativePath,
        IVssRequestContext requestContext = null,
        string version = null)
      {
        return StaticResources.GetLocalLocation("tfs/" + (version ?? StaticResources.Versioned.Version) + "/" + relativePath, requestContext);
      }

      public static string GetPhysicalLocation(
        string relativePath,
        IVssRequestContext requestContext = null,
        string version = null)
      {
        return StaticResources.GetPhysicalLocation("tfs/" + (version ?? StaticResources.Versioned.Version) + "/" + relativePath, requestContext);
      }

      public static class Content
      {
        private static readonly string RelativeRoot = "_content/";

        public static string GetLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocation(StaticResources.Versioned.Content.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.Content.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.Content.RelativeRoot + relativePath, requestContext, version);
        }
      }

      public static class CssBundles
      {
        private static readonly string RelativeRoot = "_cssbundles/";

        public static string GetLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocation(StaticResources.Versioned.CssBundles.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.CssBundles.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.CssBundles.RelativeRoot + relativePath, requestContext, version);
        }
      }

      public static class Extensions
      {
        private static readonly string RelativeRoot = "_ext/";

        public static string GetLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocation(StaticResources.Versioned.Extensions.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.Extensions.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.Extensions.RelativeRoot + relativePath, requestContext, version);
        }
      }

      public static class Scripts
      {
        private static readonly string RelativeRoot = "_scripts/";

        public static string GetLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocation(StaticResources.Versioned.Scripts.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.Scripts.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.Scripts.RelativeRoot + relativePath, requestContext, version);
        }

        public static class TFS
        {
          private static readonly string RelativeRoot = StaticResources.Versioned.Scripts.RelativeRoot + "TFS/";

          public static string GetLocation(
            string relativePath,
            IVssRequestContext requestContext = null,
            string version = null)
          {
            return StaticResources.Versioned.GetLocation(StaticResources.Versioned.Scripts.TFS.RelativeRoot + relativePath, requestContext, version);
          }

          public static string GetLocalLocation(
            string relativePath,
            IVssRequestContext requestContext = null,
            string version = null)
          {
            return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.Scripts.TFS.RelativeRoot + relativePath, requestContext, version);
          }

          public static string GetPhysicalLocation(
            string relativePath,
            IVssRequestContext requestContext = null,
            string version = null)
          {
            return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.Scripts.TFS.RelativeRoot + relativePath, requestContext, version);
          }

          public static class Debug
          {
            private static readonly string RelativeRoot = StaticResources.Versioned.Scripts.TFS.RelativeRoot + "debug/";

            public static string GetLocation(
              string relativePath,
              IVssRequestContext requestContext = null,
              string version = null)
            {
              return StaticResources.Versioned.GetLocation(StaticResources.Versioned.Scripts.TFS.Debug.RelativeRoot + relativePath, requestContext, version);
            }

            public static string GetLocalLocation(
              string relativePath,
              IVssRequestContext requestContext = null,
              string version = null)
            {
              return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.Scripts.TFS.Debug.RelativeRoot + relativePath, requestContext, version);
            }

            public static string GetPhysicalLocation(
              string relativePath,
              IVssRequestContext requestContext = null,
              string version = null)
            {
              return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.Scripts.TFS.Debug.RelativeRoot + relativePath, requestContext, version);
            }
          }
        }
      }

      public static class Themes
      {
        private static readonly string RelativeRoot = "App_Themes/";

        public static string GetLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocation(StaticResources.Versioned.Themes.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetLocalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetLocalLocation(StaticResources.Versioned.Themes.RelativeRoot + relativePath, requestContext, version);
        }

        public static string GetPhysicalLocation(
          string relativePath,
          IVssRequestContext requestContext = null,
          string version = null)
        {
          return StaticResources.Versioned.GetPhysicalLocation(StaticResources.Versioned.Themes.RelativeRoot + relativePath, requestContext, version);
        }
      }
    }
  }
}
