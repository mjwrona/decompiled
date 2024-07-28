// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.VsoKustoClusterInfoProvider
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public static class VsoKustoClusterInfoProvider
  {
    private static string VSOKustoEndpointTemplate = "https://{0}.kusto.windows.net/{1}";
    private static string VSOEUKustoEndpointTemplate = "https://{0}.northeurope.kusto.windows.net/{1}";
    private const string VSO = "vso";
    private const string VSOPpe = "vsoppe";
    private const string VSODev = "vsodev";
    private const string VSOEU = "vsoeu";

    public static VsoKustoClusterInfoProvider.KustoIngestionConfig GetKustoIngestionConfig(
      IServicingContext servicingContext,
      DeploymentRealm realm,
      string region,
      string ingestionEndpointName)
    {
      if (string.IsNullOrEmpty(ingestionEndpointName))
        throw new ArgumentNullException(nameof (ingestionEndpointName));
      if (DeploymentFeatureAvailabilityReader.Instance.IsFeatureEnabled("CHOOSE_DATABOUNDARY_BY_REGION"))
      {
        DataBoundary regionDataBoundary = AzureRegions.GetRegionDataBoundary(region);
        VsoKustoClusterInfoProvider.KustoIngestionConfig kustoTelemetrySource = VsoKustoClusterInfoProvider.GetKustoTelemetrySource(regionDataBoundary, realm, ingestionEndpointName);
        servicingContext.LogInfo(string.Format("Feature flag {0} set to true. Connection uri is taken. DataBoundary({1}), Cluster({2}), DB({3})", (object) "CHOOSE_DATABOUNDARY_BY_REGION", (object) regionDataBoundary, (object) kustoTelemetrySource.ClusterName, (object) kustoTelemetrySource.DatabaseName));
        return kustoTelemetrySource;
      }
      VsoKustoClusterInfoProvider.KustoIngestionConfig kustoTelemetrySource1 = VsoKustoClusterInfoProvider.GetDefaultKustoTelemetrySource(realm, ingestionEndpointName);
      servicingContext.LogInfo("Feature flag CHOOSE_DATABOUNDARY_BY_REGION set to false. Default kusto connection is used: Cluster " + kustoTelemetrySource1.ClusterName + ", DB " + kustoTelemetrySource1.DatabaseName);
      return kustoTelemetrySource1;
    }

    public static string GetKustoConnectionUri(DeploymentRealm realm, string region)
    {
      DataBoundary regionDataBoundary = AzureRegions.GetRegionDataBoundary(region);
      return DeploymentFeatureAvailabilityReader.Instance.IsFeatureEnabled("CHOOSE_DATABOUNDARY_BY_REGION") && 1 == realm && 2 == regionDataBoundary ? string.Format(VsoKustoClusterInfoProvider.VSOEUKustoEndpointTemplate, (object) "vsoeu", (object) "vso") : VsoKustoClusterInfoProvider.CreateGlobalKustoConnectionUri(realm);
    }

    private static string CreateGlobalKustoConnectionUri(DeploymentRealm realm)
    {
      switch (realm - 1)
      {
        case 0:
        case 4:
          return string.Format(VsoKustoClusterInfoProvider.VSOKustoEndpointTemplate, (object) "vso", (object) "vso");
        case 1:
        case 5:
          return string.Format(VsoKustoClusterInfoProvider.VSOKustoEndpointTemplate, (object) "vso", (object) "vsoppe");
        case 2:
        case 3:
          return string.Format(VsoKustoClusterInfoProvider.VSOKustoEndpointTemplate, (object) "vsodev", (object) "vsodev");
        default:
          throw new ArgumentOutOfRangeException(nameof (realm));
      }
    }

    internal static VsoKustoClusterInfoProvider.KustoIngestionConfig GetKustoTelemetrySource(
      DataBoundary dataBoundary,
      DeploymentRealm realm,
      string ingestionEndpointName)
    {
      if (realm == 1)
        return VsoKustoClusterInfoProvider.GetKustoTelemetrySourceByDataBoundary(dataBoundary, ingestionEndpointName);
      return realm - 2 <= 4 ? VsoKustoClusterInfoProvider.GetDefaultKustoTelemetrySource(realm, ingestionEndpointName) : throw new ArgumentOutOfRangeException(nameof (realm));
    }

    internal static VsoKustoClusterInfoProvider.KustoIngestionConfig GetDefaultKustoTelemetrySource(
      DeploymentRealm realm,
      string ingestionEndpointName)
    {
      switch (realm - 1)
      {
        case 0:
        case 4:
          return new VsoKustoClusterInfoProvider.KustoIngestionConfig(ingestionEndpointName, "vso");
        case 1:
        case 5:
          return new VsoKustoClusterInfoProvider.KustoIngestionConfig(ingestionEndpointName, "vsoppe");
        case 2:
        case 3:
          return new VsoKustoClusterInfoProvider.KustoIngestionConfig(ingestionEndpointName, "vsodev");
        default:
          throw new ArgumentOutOfRangeException(nameof (realm));
      }
    }

    internal static VsoKustoClusterInfoProvider.KustoIngestionConfig GetKustoTelemetrySourceByDataBoundary(
      DataBoundary dataBoundary,
      string ingestionEndpointName)
    {
      return new VsoKustoClusterInfoProvider.KustoIngestionConfig(dataBoundary == 2 ? "vsoeu" : ingestionEndpointName, "vso");
    }

    public class KustoIngestionConfig
    {
      public string ClusterName { get; }

      public string DatabaseName { get; }

      public KustoIngestionConfig(string clusterName, string databaseName)
      {
        this.ClusterName = clusterName;
        this.DatabaseName = databaseName;
      }
    }
  }
}
