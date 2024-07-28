// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ModelVersionExtension
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public static class ModelVersionExtension
  {
    internal static Dictionary<string, int> s_version_map = new Dictionary<string, int>()
    {
      {
        "v1.0",
        1
      },
      {
        "v1.0-preview",
        1
      },
      {
        "v2.0",
        2
      },
      {
        "v2.0-preview",
        2
      },
      {
        "v3.0",
        3
      },
      {
        "v3.0-preview",
        3
      },
      {
        "v4.0-preview",
        4
      }
    };
    private static readonly int s_defaultVersion = 0;
    internal static int s_minVersion = 1;
    internal static int s_maxVersion = 4;
    private static Lazy<Dictionary<int, RegistryQuery>> s_versionRegQueryTable = new Lazy<Dictionary<int, RegistryQuery>>((Func<Dictionary<int, RegistryQuery>>) (() => Enumerable.Range(ModelVersionExtension.s_minVersion - 1, ModelVersionExtension.s_maxVersion - ModelVersionExtension.s_minVersion + 2).Append<int>(ModelVersionExtension.s_defaultVersion).Distinct<int>().ToDictionary<int, int, RegistryQuery>((Func<int, int>) (i => i), (Func<int, RegistryQuery>) (i => ModelVersionExtension.GetVersionStateRegQuery(i)))));
    private const string c_area = "Analytics";
    private const string c_layer = "ODataModelVersion";
    private const string c_majorVersionRegexGroupName = "majorVersion";
    private const string c_previewRegexGroupName = "preview";
    private static readonly Regex _odataModelAPIVersionRegex = new Regex("^(v(?<majorVersion>\\d+).\\d+(?<preview>-preview)?)$", RegexOptions.Compiled);
    private const string c_versionStateRegPathRoot = "/Analytics/ODataModelVersion/VersionState";
    private const string c_odataModelVersionKey = "AnalyticsODataModelVersion";
    private const string c_requestIsVersionedKey = "AnalyticsRequestIsVersioned";

    public static int GetDefaultVersion() => ModelVersionExtension.s_defaultVersion;

    public static int GetMinODataModelVersion() => ModelVersionExtension.s_minVersion;

    public static int GetMaxODataModelVersion() => ModelVersionExtension.s_maxVersion;

    public static ModelVersionExtension.VersionState GetVersionState(
      this IVssRequestContext requestContext,
      int version)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      RegistryQuery query;
      if (ModelVersionExtension.s_versionRegQueryTable.Value.TryGetValue(version, out query))
        return vssRequestContext.GetService<IVssRegistryService>().GetValue<ModelVersionExtension.VersionState>(vssRequestContext, in query, ModelVersionExtension.VersionState.Preview);
      throw new UnsupportedODataModelVersionException(AnalyticsResources.ODATA_NOT_SUPPORTED_VERSION((object) version), requestContext);
    }

    internal static RegistryQuery GetVersionStateRegQuery(int version) => new RegistryQuery(string.Format("{0}/{1}.0", (object) "/Analytics/ODataModelVersion/VersionState", (object) version));

    public static T GetVersionedModifier<T>(this MemberInfo target, int currentVersion) where T : VersionedModifierAttribute => target.GetCustomAttributes<T>(true).SingleOrDefault<T>((Func<T, bool>) (a => a.ShouldApply(currentVersion)));

    public static int ExtractODataModelVersion(IVssRequestContext requestContext, string input)
    {
      int odataModelVersion = ModelVersionExtension.GetDefaultVersion();
      if (!string.IsNullOrWhiteSpace(input))
      {
        input = input.ToLower();
        int num;
        if (ModelVersionExtension.s_version_map.TryGetValue(input, out num))
          odataModelVersion = num;
        else if (ModelVersionExtension._odataModelAPIVersionRegex.Match(input).Success)
          throw new UnsupportedODataModelVersionException(AnalyticsResources.ODATA_NOT_SUPPORTED_VERSION((object) input), requestContext);
      }
      return odataModelVersion;
    }

    public static bool IsRequestVersioned(this IVssRequestContext requestContext) => requestContext.Items.ContainsKey("AnalyticsRequestIsVersioned") && (bool) requestContext.Items["AnalyticsRequestIsVersioned"];

    public static void SetRequestVersioned(this IVssRequestContext requestContext, bool versioned) => requestContext.Items["AnalyticsRequestIsVersioned"] = (object) versioned;

    public static int GetODataModelVersion(this IVssRequestContext requestContext) => requestContext.Items.ContainsKey("AnalyticsODataModelVersion") ? int.Parse(requestContext.Items["AnalyticsODataModelVersion"].ToString()) : throw new UnsupportedODataModelVersionException(AnalyticsResources.ODATA_VERSION_NOT_FOUND(), requestContext);

    public static void TraceODataModelVersion(this IVssRequestContext requestContext) => requestContext.Trace(12013029, TraceLevel.Info, "Analytics", "ODataModelVersion", string.Format("Using odata model version {0}", (object) requestContext.GetODataModelVersion()));

    public static void SetODataModelVersion(this IVssRequestContext requestContext, int version) => requestContext.Items["AnalyticsODataModelVersion"] = (object) version;

    [Flags]
    public enum VersionState
    {
      Preview = 1,
      Released = 2,
      Any = Released | Preview, // 0x00000003
    }
  }
}
