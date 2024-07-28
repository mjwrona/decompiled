// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.SecretsScannerWrapper
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public class SecretsScannerWrapper
  {
    private const string c_layer = "SecretsScannerWrapper";
    private const int c_maxResourceLength = 256;

    public SecretsScannerWrapper() => this.ContinueScan = true;

    public bool ContinueScan { get; private set; }

    public ScanResult Scan(
      IVssRequestContext requestContext,
      Stream inputStream,
      long contentLength,
      SourceContext resourceContext,
      bool isInternalExperience = true,
      Encoding inputStreamEncoding = null)
    {
      try
      {
        ArgumentUtility.CheckForNull<SourceContext>(resourceContext, nameof (resourceContext));
        return requestContext.GetService<IStreamScannerService>().ScanContentStream(requestContext, inputStream, resourceContext.ProjectId, resourceContext.RepositoryId, resourceContext.ResourceId, SecretsScannerWrapper.Right(resourceContext.ResourceName, 256), isInternalExperience);
      }
      catch (SecretsScanTimeoutException ex)
      {
        requestContext.TraceException(27009016, nameof (SecretsScannerWrapper), (Exception) ex);
        this.ContinueScan = true;
      }
      catch (CircuitBreakerException ex)
      {
        requestContext.TraceException(27009003, nameof (SecretsScannerWrapper), (Exception) ex);
        this.ContinueScan = false;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (SecretsScannerWrapper), ex);
        this.ContinueScan = false;
      }
      return new ScanResult()
      {
        Violations = (IList<Violation>) Array.Empty<Violation>()
      };
    }

    public ScanConfiguration GetCurrentConfiguration(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken)
    {
      try
      {
        return requestContext.GetService<IStreamScannerService>().GetScanConfiguration(requestContext, cancellationToken);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(nameof (SecretsScannerWrapper), ex);
        this.ContinueScan = false;
        return (ScanConfiguration) null;
      }
    }

    public Task<ScanConfiguration> GetCurrentConfigurationAsync(
      IVssRequestContext requestContext,
      CancellationToken cancellationToken)
    {
      return requestContext.GetService<IStreamScannerService>().GetScanConfigurationAsync(requestContext, cancellationToken);
    }

    public async Task<BatchScanResult> ScanBatchAsync(
      IVssRequestContext requestContext,
      Container container,
      BatchScanOptions batchScanOptions = null,
      CompressionLevel batchScanTargetCompressionLevel = CompressionLevel.NoCompression,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      return await requestContext.GetService<IStreamScannerService>().ScanContainer(requestContext, container, batchScanOptions, batchScanTargetCompressionLevel, cancellationToken);
    }

    internal static string Right(string value, int length)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      return value.Length > length ? value.Substring(value.Length - length) : value;
    }
  }
}
