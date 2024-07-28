// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.AzureSearchVsIdeSupport
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal static class AzureSearchVsIdeSupport
  {
    private const string layer = "AzureSearchVsIdeSupport";
    internal const int MaxSupportedMajorVersionDefaultValue = 17;
    internal const int MaxSupportedMinorVersionIncrementDefaultValue = 8;
    internal static readonly HashSet<int> SupportedMajorVersionsDefaultValue = new HashSet<int>()
    {
      10,
      11,
      12,
      14,
      15,
      16,
      17
    };
    internal const string MaxSupportedMajorVersionRegistryPath = "/Configuration/Service/Gallery/VsIde/MaxSupportedMajorVersion";
    internal const string MaxSupportedMinorVersionIncrementRegistryPath = "/Configuration/Service/Gallery/VsIde/MaxSupportedMinorVersionIncrement";
    internal const string SupportedMajorVersionsRegistryPath = "/Configuration/Service/Gallery/VsIde/SupportedMajorVersions";
    internal static readonly IReadOnlyCollection<string> SupportedProductArchitecturesDefaultValues = (IReadOnlyCollection<string>) new HashSet<string>()
    {
      "x86",
      "amd64",
      "arm64"
    };
    internal const string SupportedProductArchitecturesRegistryPath = "/Configuration/Service/Gallery/VsIde/SupportedProductArchitectures";

    internal static IReadOnlyCollection<int> GetSupportedMajorVersions(
      IVssRequestContext requestContext)
    {
      string str = requestContext != null ? AzureSearchVsIdeSupport.GetValueFromVssRegistryService<string>(requestContext, "/Configuration/Service/Gallery/VsIde/SupportedMajorVersions", string.Empty) : throw new ArgumentNullException(nameof (requestContext));
      requestContext.Trace(12060112, TraceLevel.Info, "Gallery", nameof (AzureSearchVsIdeSupport), "Semi-colon separated VS IDE major versions:{0}", (object) str);
      if (string.IsNullOrEmpty(str))
        return (IReadOnlyCollection<int>) AzureSearchVsIdeSupport.SupportedMajorVersionsDefaultValue;
      try
      {
        return (IReadOnlyCollection<int>) ((IEnumerable<string>) str.Split(';')).Select<string, int>((Func<string, int>) (i => Convert.ToInt32(i))).ToList<int>();
      }
      catch (InvalidCastException ex)
      {
        requestContext.Trace(12060112, TraceLevel.Error, "Gallery", nameof (AzureSearchVsIdeSupport), "Semi-colon separated VS IDE major versions contains invalid version numbers:{0}", (object) str);
        return (IReadOnlyCollection<int>) AzureSearchVsIdeSupport.SupportedMajorVersionsDefaultValue;
      }
    }

    internal static int GetMaxSupportedMinorVersionIncrement(IVssRequestContext requestContext) => requestContext != null ? AzureSearchVsIdeSupport.GetValueFromVssRegistryService<int>(requestContext, "/Configuration/Service/Gallery/VsIde/MaxSupportedMinorVersionIncrement", 8) : throw new ArgumentNullException(nameof (requestContext));

    internal static int GetMaxSupportedMajorVersion(IVssRequestContext requestContext) => requestContext != null ? AzureSearchVsIdeSupport.GetValueFromVssRegistryService<int>(requestContext, "/Configuration/Service/Gallery/VsIde/MaxSupportedMajorVersion", 17) : throw new ArgumentNullException(nameof (requestContext));

    internal static IReadOnlyCollection<string> GetSupportedProductArchitectures(
      IVssRequestContext requestContext)
    {
      string str = requestContext != null ? AzureSearchVsIdeSupport.GetValueFromVssRegistryService<string>(requestContext, "/Configuration/Service/Gallery/VsIde/SupportedProductArchitectures", string.Empty) : throw new ArgumentNullException(nameof (requestContext));
      requestContext.Trace(12060112, TraceLevel.Info, "Gallery", nameof (AzureSearchVsIdeSupport), "Semi-colon separated VS IDE product architectures:{0}", (object) str);
      if (string.IsNullOrEmpty(str))
        return AzureSearchVsIdeSupport.SupportedProductArchitecturesDefaultValues;
      return (IReadOnlyCollection<string>) ((IEnumerable<string>) str.Split(';')).ToHashSet<string>();
    }

    private static T GetValueFromVssRegistryService<T>(
      IVssRequestContext requestContext,
      string registryPath,
      T defaultValue)
    {
      RegistryQuery query = new RegistryQuery(registryPath);
      return requestContext.GetService<IVssRegistryService>().GetValue<T>(requestContext, in query, defaultValue);
    }
  }
}
