// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.LogStoreConnectionEndpoint
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage.Shared.Protocol;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class LogStoreConnectionEndpoint : ILogStoreConnectionEndpoint
  {
    private CloudBlobClient _cloudBlobClient;
    private CloudStorageAccount _cloudStorageAccount;
    private IVssRequestContext _requestContext;
    private string _storageConnectionString;
    private const string StorageAccountConnectionStringPattern = "^DefaultEndpointsProtocol=(http[s]?);AccountName=(.+);AccountKey=(.+)$";
    private static Regex _storageAccountConnectionStringPatternRegex = new Regex("^DefaultEndpointsProtocol=(http[s]?);AccountName=(.+);AccountKey=(.+)$", RegexOptions.Compiled, TimeSpan.FromMilliseconds((double) TestResultsConstants.RegexTimeOutInMilliSeconds));

    public LogStoreConnectionEndpoint(
      IVssRequestContext requestContext,
      string storageConnectionString)
    {
      this._requestContext = requestContext;
      this._storageConnectionString = storageConnectionString;
      this._initializeBlobEndpoints();
    }

    public CloudBlobClient GetCloudBlobClient() => this._cloudBlobClient;

    public string GetAccountName() => this._getAccountName(this._storageConnectionString);

    public CloudStorageAccount GetCloudStorageAccount() => this._cloudStorageAccount;

    public async Task<ILogStoreContainerSegment> GetBlobContainersAsync(
      string prefix,
      int maxResults,
      BlobContinuationToken blobContinuationToken,
      BlobRequestOptions blobRequestOptions,
      ILogStoreOperationContext logStoreOperationContext)
    {
      return (ILogStoreContainerSegment) new LogStoreContainerSegment(await this._cloudBlobClient.ListContainersSegmentedAsync(prefix, ContainerListingDetails.None, new int?(maxResults), blobContinuationToken, blobRequestOptions, logStoreOperationContext.GetOperationContext()).ConfigureAwait(false), this._requestContext);
    }

    public bool EnableCors(string corsDomainName) => this.InitializeCors(corsDomainName);

    public IList<string> GetCorsAllowedHostList() => this.GetCorsAllowedOriginsList();

    private void _initializeBlobEndpoints()
    {
      try
      {
        if (CloudStorageAccount.TryParse(this._storageConnectionString, out this._cloudStorageAccount))
          this._cloudBlobClient = this._cloudStorageAccount.CreateCloudBlobClient();
        else
          this._requestContext.Trace(0, TraceLevel.Error, "TestManagement", "LogStorage", nameof (_initializeBlobEndpoints), (object) "Invalid connection string");
      }
      catch (StorageException ex)
      {
        if (ex.RequestInformation.HttpStatusCode == 400)
          this._requestContext.Trace(0, TraceLevel.Warning, "TestManagement", "LogStorage", nameof (_initializeBlobEndpoints), (object) "Unable to create the blob client. Exception Hit: {0}, StatusMessage: {1}", (object) ex.Message, (object) ex.RequestInformation.HttpStatusMessage);
        throw;
      }
    }

    private string _getAccountName(string connString)
    {
      try
      {
        Match match = new RegexWrapper(LogStoreConnectionEndpoint._storageAccountConnectionStringPatternRegex).Match(connString);
        if (match.Success)
          return match.Groups[2].ToString();
      }
      catch (RegexMatchTimeoutException ex)
      {
        this._requestContext.Trace(1015794, TraceLevel.Error, "TestManagement", "LogStorage", string.Format("sanitizing of string: '{0}' has failed with exception: {1}", (object) connString, (object) ex));
      }
      return (string) null;
    }

    private bool InitializeCors(string corsDomainName)
    {
      try
      {
        ServiceProperties serviceProperties = this._cloudBlobClient.GetServiceProperties();
        this.ConfigureCors(serviceProperties, corsDomainName);
        this._cloudBlobClient.SetServiceProperties(serviceProperties);
      }
      catch (Exception ex)
      {
        this._requestContext.Trace(0, TraceLevel.Warning, "TestManagement", "LogStorage", nameof (InitializeCors), (object) "Unable to set Cors. Exception Hit: {0}", (object) ex.Message);
        return false;
      }
      return true;
    }

    private void ConfigureCors(ServiceProperties serviceProperties, string corsDomainName)
    {
      serviceProperties.Cors = new CorsProperties();
      if (corsDomainName == null)
        serviceProperties.Cors.CorsRules.Clear();
      else
        serviceProperties.Cors.CorsRules.Add(new CorsRule()
        {
          AllowedHeaders = (IList<string>) new List<string>()
          {
            "*"
          },
          AllowedMethods = CorsHttpMethods.Get | CorsHttpMethods.Head | CorsHttpMethods.Options,
          AllowedOrigins = (IList<string>) new List<string>()
          {
            corsDomainName
          },
          ExposedHeaders = (IList<string>) new List<string>()
          {
            "*"
          },
          MaxAgeInSeconds = 200
        });
    }

    private IList<string> GetCorsAllowedOriginsList()
    {
      List<string> allowedOriginsList = new List<string>();
      try
      {
        ServiceProperties serviceProperties = this._cloudBlobClient.GetServiceProperties();
        if (serviceProperties?.Cors?.CorsRules != null)
        {
          foreach (CorsRule corsRule in (IEnumerable<CorsRule>) serviceProperties.Cors.CorsRules)
          {
            if (corsRule?.AllowedOrigins != null)
              allowedOriginsList.AddRange((IEnumerable<string>) corsRule.AllowedOrigins);
          }
        }
      }
      catch (Exception ex)
      {
        this._requestContext.Trace(0, TraceLevel.Warning, "TestManagement", "LogStorage", nameof (GetCorsAllowedOriginsList), (object) "Unable to get Cors rules. Exception Hit: {0}", (object) ex.Message);
      }
      return (IList<string>) allowedOriginsList;
    }
  }
}
