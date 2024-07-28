// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy.AzureBlobGeoRedundancyServiceSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Cloud.AzureBlobGeoRedundancy
{
  public class AzureBlobGeoRedundancyServiceSettings
  {
    public readonly int NumberOfRetries;
    public readonly TimeSpan RetryDelay;
    public readonly int NumberOfQueues;
    public readonly bool SynchronousWritesEnabled;
    public static readonly string RegistryPath = "/Service/AzureBlobGeoRedundancy/Settings/Service";
    public static readonly string RegistryFilter = AzureBlobGeoRedundancyServiceSettings.RegistryPath + "/*";
    public const string NumberOfRetriesRegistryKey = "NumberOfRetries";
    public const string RetryDelayRegistryKey = "RetryDelay";
    public const string NumberOfQueuesRegistryKey = "NumberOfQueues";
    public const string SynchronousWritesRegistryKey = "SynchronousWrites";
    private const int c_defaultNumberOfRetries = 3;
    private static readonly TimeSpan c_defaultRetryDelay = TimeSpan.FromMilliseconds(200.0);
    private const int c_defaultNumberOfQueues = 1;
    private const string c_area = "AzureBlobGeoRedundancy";
    private const string c_layer = "AzureBlobGeoRedundancyServiceSettings";

    public AzureBlobGeoRedundancyServiceSettings()
    {
      this.NumberOfRetries = 3;
      this.RetryDelay = AzureBlobGeoRedundancyServiceSettings.c_defaultRetryDelay;
      this.NumberOfQueues = 1;
    }

    public AzureBlobGeoRedundancyServiceSettings(IVssRequestContext requestContext)
    {
      RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, (RegistryQuery) AzureBlobGeoRedundancyServiceSettings.RegistryFilter);
      int num1 = registryEntryCollection.GetValueFromPath<int>(nameof (NumberOfRetries), 3);
      TimeSpan timeSpan = registryEntryCollection.GetValueFromPath<TimeSpan>(nameof (RetryDelay), AzureBlobGeoRedundancyServiceSettings.c_defaultRetryDelay);
      int num2 = registryEntryCollection.GetValueFromPath<int>(nameof (NumberOfQueues), 1);
      if (num1 < 0)
      {
        requestContext.Trace(15300003, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyServiceSettings), "Invalid number of retries. Must be >= 0. Reverting to default of {0}. See registry value '{1}/{2}'", (object) 3, (object) AzureBlobGeoRedundancyServiceSettings.RegistryPath, (object) nameof (NumberOfRetries));
        num1 = 3;
      }
      if (timeSpan < TimeSpan.FromTicks(0L))
      {
        requestContext.Trace(15300003, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyServiceSettings), "Invalid retry delay. Must be >= 0. Reverting to default of {0}. See registry value '{1}/{2}'", (object) AzureBlobGeoRedundancyServiceSettings.c_defaultRetryDelay, (object) AzureBlobGeoRedundancyServiceSettings.RegistryPath, (object) nameof (RetryDelay));
        timeSpan = AzureBlobGeoRedundancyServiceSettings.c_defaultRetryDelay;
      }
      if (num2 < 1)
      {
        requestContext.Trace(15300003, TraceLevel.Error, "AzureBlobGeoRedundancy", nameof (AzureBlobGeoRedundancyServiceSettings), "Invalid number of queues. Must be >= 1. Reverting to default of {0}. See registry value '{1}/{2}'", (object) 1, (object) AzureBlobGeoRedundancyServiceSettings.RegistryPath, (object) nameof (NumberOfQueues));
        num2 = 1;
      }
      this.NumberOfRetries = num1;
      this.RetryDelay = timeSpan;
      this.NumberOfQueues = num2;
      this.SynchronousWritesEnabled = registryEntryCollection.GetValueFromPath<bool>("SynchronousWrites", false);
    }
  }
}
