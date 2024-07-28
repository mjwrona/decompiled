// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.ClientFeatureProviderService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  internal class ClientFeatureProviderService : IClientFeatureProviderService, IVssFrameworkService
  {
    private const string c_featuresSharedDataKey = "_features";
    private const string c_featureFlagsSharedDataKey = "_featureFlags";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void AddFeatureState(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string featureId)
    {
      bool flag = requestContext.GetService<IContributedFeatureService>().IsFeatureEnabled(requestContext, featureId);
      object obj;
      Dictionary<string, bool> dictionary;
      if (sharedData.TryGetValue("_features", out obj) && obj is WebSdkMetadataDictionary<string, bool>)
      {
        dictionary = (Dictionary<string, bool>) (obj as WebSdkMetadataDictionary<string, bool>);
      }
      else
      {
        dictionary = (Dictionary<string, bool>) new WebSdkMetadataDictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        sharedData.Add("_features", (object) dictionary);
      }
      dictionary[featureId] = flag;
    }

    public void AddFeatureFlagState(
      IVssRequestContext requestContext,
      DataProviderSharedData sharedData,
      string featureFlagName)
    {
      bool flag = requestContext.IsFeatureEnabled(featureFlagName);
      object obj;
      WebSdkMetadataDictionary<string, bool> metadataDictionary;
      if (sharedData.TryGetValue("_featureFlags", out obj) && obj is WebSdkMetadataDictionary<string, bool>)
      {
        metadataDictionary = obj as WebSdkMetadataDictionary<string, bool>;
      }
      else
      {
        metadataDictionary = new WebSdkMetadataDictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        sharedData.Add("_featureFlags", (object) metadataDictionary);
      }
      metadataDictionary[featureFlagName] = flag;
    }
  }
}
