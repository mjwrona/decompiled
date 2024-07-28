// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.WebServerResources
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Microsoft.Azure.Pipelines.WebServer
{
  internal static class WebServerResources
  {
    private static ResourceManager s_resMgr = new ResourceManager(nameof (WebServerResources), typeof (WebServerResources).GetTypeInfo().Assembly);

    public static ResourceManager Manager => WebServerResources.s_resMgr;

    private static string Get(string resourceName) => WebServerResources.s_resMgr.GetString(resourceName, CultureInfo.CurrentUICulture);

    private static string Get(string resourceName, CultureInfo culture) => culture == null ? WebServerResources.Get(resourceName) : WebServerResources.s_resMgr.GetString(resourceName, culture);

    public static int GetInt(string resourceName) => (int) WebServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static int GetInt(string resourceName, CultureInfo culture) => culture == null ? WebServerResources.GetInt(resourceName) : (int) WebServerResources.s_resMgr.GetObject(resourceName, culture);

    public static bool GetBool(string resourceName) => (bool) WebServerResources.s_resMgr.GetObject(resourceName, CultureInfo.CurrentUICulture);

    public static bool GetBool(string resourceName, CultureInfo culture) => culture == null ? WebServerResources.GetBool(resourceName) : (bool) WebServerResources.s_resMgr.GetObject(resourceName, culture);

    private static string Format(string resourceName, params object[] args) => WebServerResources.Format(resourceName, CultureInfo.CurrentUICulture, args);

    private static string Format(string resourceName, CultureInfo culture, params object[] args)
    {
      if (culture == null)
        culture = CultureInfo.CurrentUICulture;
      string format = WebServerResources.Get(resourceName, culture);
      if (args == null)
        return format;
      for (int index = 0; index < args.Length; ++index)
      {
        if (args[index] is DateTime)
        {
          DateTime dateTime = (DateTime) args[index];
          Calendar calendar = DateTimeFormatInfo.CurrentInfo.Calendar;
          if (dateTime > calendar.MaxSupportedDateTime)
            args[index] = (object) calendar.MaxSupportedDateTime;
          else if (dateTime < calendar.MinSupportedDateTime)
            args[index] = (object) calendar.MinSupportedDateTime;
        }
      }
      return string.Format((IFormatProvider) CultureInfo.CurrentCulture, format, args);
    }

    public static string CreatePipeline_UnsupportedConfigurationType(object arg0) => WebServerResources.Format(nameof (CreatePipeline_UnsupportedConfigurationType), arg0);

    public static string CreatePipeline_UnsupportedConfigurationType(
      object arg0,
      CultureInfo culture)
    {
      return WebServerResources.Format(nameof (CreatePipeline_UnsupportedConfigurationType), culture, arg0);
    }

    public static string CreatePipeline_UnsupportedRepositoryType(object arg0) => WebServerResources.Format(nameof (CreatePipeline_UnsupportedRepositoryType), arg0);

    public static string CreatePipeline_UnsupportedRepositoryType(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (CreatePipeline_UnsupportedRepositoryType), culture, arg0);

    public static string InvalidRunPipelineRequest_DuplicateFileContent(object arg0) => WebServerResources.Format(nameof (InvalidRunPipelineRequest_DuplicateFileContent), arg0);

    public static string InvalidRunPipelineRequest_DuplicateFileContent(
      object arg0,
      CultureInfo culture)
    {
      return WebServerResources.Format(nameof (InvalidRunPipelineRequest_DuplicateFileContent), culture, arg0);
    }

    public static string InvalidRunPipelineRequest_MissingRootPart() => WebServerResources.Get(nameof (InvalidRunPipelineRequest_MissingRootPart));

    public static string InvalidRunPipelineRequest_MissingRootPart(CultureInfo culture) => WebServerResources.Get(nameof (InvalidRunPipelineRequest_MissingRootPart), culture);

    public static string InvalidRunPipelineRequest_NotMultipart() => WebServerResources.Get(nameof (InvalidRunPipelineRequest_NotMultipart));

    public static string InvalidRunPipelineRequest_NotMultipart(CultureInfo culture) => WebServerResources.Get(nameof (InvalidRunPipelineRequest_NotMultipart), culture);

    public static string PipelineNotFound(object arg0) => WebServerResources.Format(nameof (PipelineNotFound), arg0);

    public static string PipelineNotFound(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (PipelineNotFound), culture, arg0);

    public static string RequestUrlSignatureValidationRequired() => WebServerResources.Get(nameof (RequestUrlSignatureValidationRequired));

    public static string RequestUrlSignatureValidationRequired(CultureInfo culture) => WebServerResources.Get(nameof (RequestUrlSignatureValidationRequired), culture);

    public static string ArtifactNotFound(object arg0) => WebServerResources.Format(nameof (ArtifactNotFound), arg0);

    public static string ArtifactNotFound(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (ArtifactNotFound), culture, arg0);

    public static string CannotNegotiateConnection(object arg0) => WebServerResources.Format(nameof (CannotNegotiateConnection), arg0);

    public static string CannotNegotiateConnection(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (CannotNegotiateConnection), culture, arg0);

    public static string CannotStartConnection(object arg0) => WebServerResources.Format(nameof (CannotStartConnection), arg0);

    public static string CannotStartConnection(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (CannotStartConnection), culture, arg0);

    public static string CannotStartSocketConnection(object arg0) => WebServerResources.Format(nameof (CannotStartSocketConnection), arg0);

    public static string CannotStartSocketConnection(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (CannotStartSocketConnection), culture, arg0);

    public static string CannotSubscribe(object arg0) => WebServerResources.Format(nameof (CannotSubscribe), arg0);

    public static string CannotSubscribe(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (CannotSubscribe), culture, arg0);

    public static string ReachedMaxRetries(object arg0) => WebServerResources.Format(nameof (ReachedMaxRetries), arg0);

    public static string ReachedMaxRetries(object arg0, CultureInfo culture) => WebServerResources.Format(nameof (ReachedMaxRetries), culture, arg0);

    public static string YamlOverrideNonDryRun() => WebServerResources.Get(nameof (YamlOverrideNonDryRun));

    public static string YamlOverrideNonDryRun(CultureInfo culture) => WebServerResources.Get(nameof (YamlOverrideNonDryRun), culture);
  }
}
