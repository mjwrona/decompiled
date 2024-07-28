// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph.Extensions.MsGraphRequestExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Graph.MicrosoftGraph.Extensions
{
  internal static class MsGraphRequestExtensions
  {
    private static readonly HashSet<Type> SupportedRequestTypes = new HashSet<Type>()
    {
      typeof (GetServicePrincipalsByIdsRequest),
      typeof (MsGraphGetApplicationByIdRequest),
      typeof (MsGraphCreateApplicationRequest),
      typeof (MsGraphUpdateApplicationRequest),
      typeof (MsGraphDeleteApplicationRequest),
      typeof (MsGraphCreateServicePrincipalRequest),
      typeof (MsGraphDeleteServicePrincipalRequest),
      typeof (MsGraphAddApplicationPasswordRequest),
      typeof (MsGraphRemoveApplicationPasswordRequest),
      typeof (MsGraphAddApplicationFederatedCredentialsRequest),
      typeof (MsGraphRemoveApplicationFederatedCredentialsRequest)
    };

    public static bool IsHttpResponseLoggingForAppsEnabled<T>(
      this MicrosoftGraphClientRequest<T> request,
      IVssRequestContext context)
      where T : MicrosoftGraphClientResponse
    {
      return MsGraphRequestExtensions.SupportedRequestTypes.Contains(request.GetType()) && context.IsFeatureEnabled("VisualStudio.Services.Aad.MicrosoftGraph.LogRawHttpExceptions");
    }

    public static bool IsHttpResponseDisplayingForAppsEnabled<T>(
      this MicrosoftGraphClientRequest<T> request,
      IVssRequestContext context)
      where T : MicrosoftGraphClientResponse
    {
      return MsGraphRequestExtensions.SupportedRequestTypes.Contains(request.GetType()) && context.IsFeatureEnabled("VisualStudio.Services.Aad.MicrosoftGraph.DisplayRawHttpExceptions");
    }
  }
}
