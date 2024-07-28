// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.LocalizationHelper
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Resources;

namespace Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider
{
  [ExcludeFromCodeCoverage]
  public static class LocalizationHelper
  {
    public static ResourceMetricDefinitions Localize(
      this ResourceMetricDefinitions definitions,
      ResourceManager resourceManager)
    {
      if (definitions != null && resourceManager != null)
      {
        foreach (ResourceMetricDefinition definition in (List<ResourceMetricDefinition>) definitions)
        {
          if (!string.IsNullOrEmpty(definition.Name))
            definition.DisplayName = resourceManager.GetString(definition.Name);
        }
      }
      return definitions;
    }

    public static ResourceMetricResponses Localize(
      this ResourceMetricResponses responses,
      ResourceManager resourceManager)
    {
      if (responses != null && resourceManager != null)
      {
        foreach (ResourceMetricResponse response in (Collection<ResourceMetricResponse>) responses)
        {
          if (response != null && response.Data != null && !string.IsNullOrEmpty(response.Data.Name))
            response.Data.DisplayName = resourceManager.GetString(response.Data.Name);
        }
      }
      return responses;
    }

    public static ResourceMetricBatchResponses Localize(
      this ResourceMetricBatchResponses batchResponses,
      ResourceManager resourceManager)
    {
      if (batchResponses != null && resourceManager != null)
      {
        foreach (ResourceMetricBatchResponse batchResponse in (Collection<ResourceMetricBatchResponse>) batchResponses)
        {
          if (batchResponse != null)
            batchResponse.Responses.Localize(resourceManager);
        }
      }
      return batchResponses;
    }
  }
}
