// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Deployment.WebApi.DeploymentResourceIds
// Assembly: Microsoft.Azure.Pipelines.Deployment.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8505F8FB-8448-4469-A2DD-E74F64B77053
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Pipelines.Deployment.WebApi.dll

using System;

namespace Microsoft.Azure.Pipelines.Deployment.WebApi
{
  public static class DeploymentResourceIds
  {
    public const string AreaString = "{8580c551-69db-4092-9050-c9ccd4521d2e}";
    public static readonly Guid AreaId = new Guid("{8580c551-69db-4092-9050-c9ccd4521d2e}");
    public const string AreaName = "Deployment";
    public const string ImageDetailsLocationIdString = "647BB185-908A-4660-B59B-DFF3D1ACE8DE";
    public const string AttestationDetailsLocationIdString = "45EED45C-A02D-4F52-99AE-4F1282049F6B";
    public const string DeploymentDetailsLocationIdString = "BB302EF9-066F-4FFB-AEE2-D61B91783B2A";
    public const string VulnerabilityDetailsLocationIdString = "AB55F461-1075-4C26-B84D-35CD2D5833BD";
    public const string ArtifactProvenancesV1LocationIdString = "D943A6F4-A813-4498-823A-4DA53BF9D0CD";
    public const string PipelineResourceTriggersLocationIdString = "E6D45067-5060-4116-89FD-57A54F256199";
    public static readonly Guid ImageDetailsLocationId = new Guid("647BB185-908A-4660-B59B-DFF3D1ACE8DE");
    public static readonly Guid AttestationDetailsLocationId = new Guid("45EED45C-A02D-4F52-99AE-4F1282049F6B");
    public static readonly Guid DeploymentDetailsLocationId = new Guid("BB302EF9-066F-4FFB-AEE2-D61B91783B2A");
    public static readonly Guid VulnerabilityDetailsLocationId = new Guid("AB55F461-1075-4C26-B84D-35CD2D5833BD");
    public static readonly Guid ArtifactProvenancesV1LocationId = new Guid("D943A6F4-A813-4498-823A-4DA53BF9D0CD");
    public static readonly Guid PipelineResourceTriggersLocationId = new Guid("E6D45067-5060-4116-89FD-57A54F256199");
    public const string ImageDetailsResource = "imagedetails";
    public const string AttestationDetailsResource = "attestationdetails";
    public const string DeploymentDetailsResource = "deploymentdetails";
    public const string VulnerabilityDetailsResource = "vulnerabilitydetails";
    public const string ArtifactProvenancesResource = "artifactprovenances";
    public const string PipelineResourceTriggersResource = "resourcetriggers";
  }
}
