// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.PoolProvider.Contracts.PoolProviderResourceIds
// Assembly: Microsoft.Azure.Pipelines.PoolProvider.Contracts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2F8171A-7EDF-4EAC-B6BB-DAF285412F1E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.PoolProvider.Contracts.dll

using System;

namespace Microsoft.Azure.Pipelines.PoolProvider.Contracts
{
  public static class PoolProviderResourceIds
  {
    public const string Area = "poolprovider";
    public static readonly Guid AreaId = new Guid("{1BDE6430-04D5-48F4-9594-858F77E37202}");
    public const string AcquireAgentResource = "acquire";
    public static readonly Guid AcquireAgentLocationId = new Guid("{A271FA70-6A6A-408E-B95E-5816D32D24AA}");
    public const string AgentDefinitionResource = "definitions";
    public static readonly Guid AgentDefinitionLocationId = new Guid("{11969335-DA1A-4CD1-A6A9-7937D27EA434}");
    public const string AgentRequestStatusResource = "requeststatus";
    public static readonly Guid AgentRequestStatusLocationId = new Guid("{AF5F6072-4A83-4C52-9772-F1C300B504D8}");
    public const string ConfigurationInformationResource = "configuration";
    public static readonly Guid ConfigurationInformationLocationId = new Guid("{9D01A40B-87EC-4A3F-ACFE-96176EC83747}");
    public const string ReleaseAgentResource = "release";
    public static readonly Guid ReleaseAgentLocationId = new Guid("{77FF4A31-D3F6-497A-9586-355CA77ED8C7}");
    public const string GetAccountParallelismResource = "parallelism";
    public static readonly Guid GetAccountParallelismLocationId = new Guid("{326C3F50-2F9F-4616-8E85-A21D95679E7C}");
    public const string DeploymentArea = "deploymentpoolprovider";
    public static readonly Guid DeploymentAreaId = new Guid("{B7C9AD81-684C-4887-B602-4A1B41B46270}");
    public const string DeploymentAcquireAgentResource = "acquire";
    public static readonly Guid DeploymentAcquireAgentLocationId = new Guid("{ABB65937-7962-4AA9-84B1-C76EF978008D}");
    public const string DeploymentAgentRequestStatusResource = "requeststatus";
    public static readonly Guid DeploymentAgentRequestStatusLocationId = new Guid("{F974A5D2-8D86-4118-8B3D-75736B4851F2}");
    public const string DeploymentConfigurationInformationResource = "configuration";
    public static readonly Guid DeploymentConfigurationInformationLocationId = new Guid("{C113FD16-D00B-4638-99AE-DE4772DB848D}");
    public const string DeploymentReleaseAgentResource = "release";
    public static readonly Guid DeploymentReleaseAgentLocationId = new Guid("{E1DED42C-7FA4-4321-8F79-CC21BE1BB5F8}");
  }
}
