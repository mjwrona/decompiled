// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PipelineHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Providers;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class PipelineHelper
  {
    private const string c_layer = "PipelineHelper";

    public static string CreateArtifactName(string resourceName, string suffix = "")
    {
      ArgumentUtility.CheckStringForNullOrEmpty(resourceName, nameof (resourceName));
      resourceName = resourceName.Replace('/', '_');
      return string.IsNullOrEmpty(suffix) ? resourceName : PipelinesResources.ArtifactNameWithSuffix((object) resourceName, (object) suffix);
    }

    public static string GetHeaderValue(
      this IDictionary<string, string> dictionary,
      string key,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(dictionary, nameof (dictionary));
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      string str;
      if (!dictionary.TryGetValue(key, out str) & throwIfNotFound)
        throw new ArgumentException(key + " is a required Header value for the request.");
      return str ?? string.Empty;
    }

    public static string RemovePrefix(
      this string source,
      string prefix,
      StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
      return !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(prefix) && source.StartsWith(prefix, comparison) ? source.Substring(prefix.Length) : source;
    }

    public static string RemoveSuffix(
      this string source,
      string suffix,
      StringComparison comparison = StringComparison.OrdinalIgnoreCase)
    {
      return !string.IsNullOrEmpty(source) && !string.IsNullOrEmpty(suffix) && source.EndsWith(suffix, comparison) ? source.Substring(0, source.Length - suffix.Length) : source;
    }

    public static void ValidatePayloadSignature(
      IVssRequestContext requestContext,
      string providerId,
      string jsonPayload,
      IDictionary<string, string> headers)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceInfo(TracePoints.Provider.ValidateIncomingEvent, nameof (PipelineHelper), "Entering ValidatePayloadSignature");
      ArgumentUtility.CheckStringForNullOrEmpty(jsonPayload, nameof (jsonPayload));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(headers, nameof (headers));
      if (!requestContext.GetService<IPipelineSourceProviderService>().GetProvider(providerId).EventsHandler.IsValidEvent(requestContext, jsonPayload, headers))
        throw new ArgumentException(PipelinesResources.ExceptionPayloadSignatureMismatch());
    }

    public static bool IsValidSignature(
      IVssRequestContext requestContext,
      string primarySecretName,
      string secondarySecretName,
      string stringToValidate,
      string signature)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      byte[] bytes = Encoding.UTF8.GetBytes(stringToValidate);
      return PublishingUtils.DoesIncomingPayloadMatchHash(vssRequestContext.Elevate(), bytes, signature, "ConfigurationSecrets", primarySecretName) || PublishingUtils.DoesIncomingPayloadMatchHash(vssRequestContext.Elevate(), bytes, signature, "ConfigurationSecrets", secondarySecretName);
    }

    public static void VerifyRepositoryParameters(
      IVssRequestContext requestContext,
      string repositoryType,
      string repositoryId,
      string branch,
      Guid? serviceConnectionId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(repositoryType, nameof (repositoryType));
      ArgumentUtility.CheckStringForNullOrEmpty(repositoryId, nameof (repositoryId));
      ArgumentUtility.CheckStringForNullOrEmpty(branch, nameof (branch));
      if (!PipelineHelper.IsExternalSourceProvider(requestContext, repositoryType))
        return;
      ArgumentUtility.CheckForNull<Guid>(serviceConnectionId, nameof (serviceConnectionId));
    }

    private static bool IsExternalSourceProvider(
      IVssRequestContext requestContext,
      string repositoryType)
    {
      return requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryType).RequiresServiceConnection(requestContext);
    }
  }
}
