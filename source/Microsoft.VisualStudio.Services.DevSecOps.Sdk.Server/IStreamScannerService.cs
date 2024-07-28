// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.IStreamScannerService
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  [DefaultServiceImplementation(typeof (FrameworkStreamScannerService))]
  public interface IStreamScannerService : IVssFrameworkService
  {
    ScanResult ScanPipelineDefinitionStream(
      IVssRequestContext requestContext,
      Stream inputStream,
      Guid projectId,
      PipelineType pipelineType,
      string pipelineId,
      int? revision = null,
      string resourceName = "DefaultResource.json",
      CancellationToken cancellationToken = default (CancellationToken));

    ScanResult ScanContentStream(
      IVssRequestContext requestContext,
      Stream inputStream,
      Guid projectId,
      string repositoryId,
      string commitId,
      string resourceName,
      bool isInternalExperience = true,
      CancellationToken cancellationToken = default (CancellationToken));

    ScanResult ScanStream(
      IVssRequestContext requestContext,
      Stream inputStream,
      string instanceId,
      string resourceName,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<BatchScanResult> ScanContainer(
      IVssRequestContext requestContext,
      Container container,
      BatchScanOptions batchScanOptions = null,
      CompressionLevel batchScanTargetCompressionLevel = CompressionLevel.NoCompression,
      CancellationToken cancellationToken = default (CancellationToken));

    ScanConfiguration GetScanConfiguration(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken);

    Task<ScanConfiguration> GetScanConfigurationAsync(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken);
  }
}
