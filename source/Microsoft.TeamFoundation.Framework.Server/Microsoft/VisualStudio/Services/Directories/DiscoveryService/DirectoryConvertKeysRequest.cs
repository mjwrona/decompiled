// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.DirectoryConvertKeysRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  public class DirectoryConvertKeysRequest : DirectoryRequest
  {
    private static readonly DirectoryConvertKeyResult s_defaultResult = new DirectoryConvertKeyResult()
    {
      Exception = (Exception) new DirectoryConvertKeyFailedException()
    };

    public IEnumerable<string> Keys { get; set; }

    public string ConvertFrom { get; set; }

    public string ConvertTo { get; set; }

    internal override DirectoryResponse Execute(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories)
    {
      IEnumerable<string> keys = this.GetKeys();
      Dictionary<string, DirectoryConvertKeyResult> results = keys.ToDictionary<string, string, DirectoryConvertKeyResult>((Func<string, string>) (key => key), (Func<string, DirectoryConvertKeyResult>) (key => DirectoryConvertKeysRequest.s_defaultResult));
      switch (this.ConvertFrom)
      {
        case "DirectoryEntityIdentifier":
          switch (this.ConvertTo)
          {
            case "DirectoryEntityIdentifier":
              using (Dictionary<string, DirectoryConvertKeyResult>.Enumerator enumerator = results.GetEnumerator())
              {
                while (enumerator.MoveNext())
                {
                  KeyValuePair<string, DirectoryConvertKeyResult> current = enumerator.Current;
                  results[current.Key] = new DirectoryConvertKeyResult()
                  {
                    Key = current.Key
                  };
                }
                break;
              }
            case "VisualStudioIdentifier":
              this.ConvertKeysWithVsd(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
              break;
            case "AzureActiveDirectoryObjectIdentifier":
              this.ConvertKeysWithAad(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
              break;
            case "ActiveDirectoryObjectIdentifier":
              this.ConvertKeysWithAd(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
              break;
            case "WindowsMachineDirectoryObjectIdentifier":
              this.ConvertKeysWithWmd(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
              break;
            case "GitHubIdentifier":
              this.ConvertKeysWithGitHub(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
              break;
            default:
              results = this.CreateNotSupportedResults(keys);
              break;
          }
          break;
        case "VisualStudioIdentifier":
          this.ConvertKeysWithVsd(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
          break;
        case "AzureActiveDirectoryObjectIdentifier":
          this.ConvertKeysWithAad(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
          break;
        case "ActiveDirectoryObjectIdentifier":
          this.ConvertKeysWithAd(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
          break;
        case "WindowsMachineDirectoryObjectIdentifier":
          this.ConvertKeysWithWmd(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
          break;
        case "GitHubIdentifier":
          this.ConvertKeysWithGitHub(context, directories, (IDictionary<string, DirectoryConvertKeyResult>) results);
          break;
        default:
          results = this.CreateNotSupportedResults(keys);
          break;
      }
      return (DirectoryResponse) new DirectoryConvertKeysResponse()
      {
        Results = (IDictionary<string, DirectoryConvertKeyResult>) results
      };
    }

    private IEnumerable<string> GetKeys() => this.Keys == null ? (IEnumerable<string>) Array.Empty<string>() : (IEnumerable<string>) new HashSet<string>(this.Keys, (IEqualityComparer<string>) VssStringComparer.DirectoryKeyStringComparer);

    private void ConvertKeysWithAd(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories,
      IDictionary<string, DirectoryConvertKeyResult> results)
    {
      if (this.TryConvertKeysUsingDirectory(context, directories, results, "ad"))
        return;
      DirectoryDiscoveryServiceException serviceException = context.ExecutionEnvironment.IsOnPremisesDeployment ? new DirectoryDiscoveryServiceException(FrameworkResources.MissingRegisteredDirectory((object) "ad")) : (DirectoryDiscoveryServiceException) new DirectoryDiscoveryServiceClientException(FrameworkResources.DirectoryOperationNotSupported((object) "ad", (object) context.ExecutionEnvironment.Flags));
      foreach (string key in results.Keys.ToList<string>())
        results[key] = new DirectoryConvertKeyResult()
        {
          Exception = (Exception) serviceException
        };
    }

    private void ConvertKeysWithWmd(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories,
      IDictionary<string, DirectoryConvertKeyResult> results)
    {
      if (this.TryConvertKeysUsingDirectory(context, directories, results, "wmd"))
        return;
      DirectoryDiscoveryServiceException serviceException = context.ExecutionEnvironment.IsOnPremisesDeployment ? new DirectoryDiscoveryServiceException(FrameworkResources.MissingRegisteredDirectory((object) "wmd")) : (DirectoryDiscoveryServiceException) new DirectoryDiscoveryServiceClientException(FrameworkResources.DirectoryOperationNotSupported((object) "wmd", (object) context.ExecutionEnvironment.Flags));
      foreach (string key in results.Keys.ToList<string>())
        results[key] = new DirectoryConvertKeyResult()
        {
          Exception = (Exception) serviceException
        };
    }

    private void ConvertKeysWithVsd(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories,
      IDictionary<string, DirectoryConvertKeyResult> results)
    {
      this.TryConvertKeysUsingDirectory(context, directories, results, "vsd");
    }

    private void ConvertKeysWithAad(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories,
      IDictionary<string, DirectoryConvertKeyResult> results)
    {
      if (this.TryConvertKeysUsingDirectory(context, directories, results, "aad"))
        return;
      DirectoryDiscoveryServiceException serviceException = context.ExecutionEnvironment.IsOnPremisesDeployment ? (DirectoryDiscoveryServiceException) new DirectoryDiscoveryServiceClientException(FrameworkResources.DirectoryOperationNotSupported((object) "aad", (object) context.ExecutionEnvironment.Flags)) : new DirectoryDiscoveryServiceException(FrameworkResources.MissingRegisteredDirectory((object) "aad"));
      foreach (string key in results.Keys.ToList<string>())
        results[key] = new DirectoryConvertKeyResult()
        {
          Exception = (Exception) serviceException
        };
    }

    private void ConvertKeysWithGitHub(
      IVssRequestContext context,
      IEnumerable<IDirectory> directories,
      IDictionary<string, DirectoryConvertKeyResult> results)
    {
      if (this.TryConvertKeysUsingDirectory(context, directories, results, "ghb"))
        return;
      DirectoryDiscoveryServiceException serviceException = context.ExecutionEnvironment.IsOnPremisesDeployment ? (DirectoryDiscoveryServiceException) new DirectoryDiscoveryServiceClientException(FrameworkResources.DirectoryOperationNotSupported((object) "ghb", (object) context.ExecutionEnvironment.Flags)) : new DirectoryDiscoveryServiceException(FrameworkResources.MissingRegisteredDirectory((object) "ghb"));
      foreach (string key in results.Keys.ToList<string>())
        results[key] = new DirectoryConvertKeyResult()
        {
          Exception = (Exception) serviceException
        };
    }

    private bool TryConvertKeysUsingDirectory(
      IVssRequestContext context,
      IEnumerable<IDirectory> availableDirectories,
      IDictionary<string, DirectoryConvertKeyResult> results,
      string conversionDirectoryName)
    {
      IDirectory directory1 = availableDirectories.SingleOrDefault<IDirectory>((Func<IDirectory, bool>) (dir => VssStringComparer.DirectoryName.Equals(conversionDirectoryName, dir.Name)));
      if (directory1 == null)
        return false;
      IDirectory directory2 = directory1;
      IVssRequestContext context1 = context;
      DirectoryInternalConvertKeysRequest convertKeysRequest = new DirectoryInternalConvertKeysRequest();
      convertKeysRequest.Directories = (IEnumerable<string>) new string[1]
      {
        conversionDirectoryName
      };
      convertKeysRequest.Keys = (IEnumerable<string>) results.Keys;
      convertKeysRequest.ConvertFrom = this.ConvertFrom;
      convertKeysRequest.ConvertTo = this.ConvertTo;
      DirectoryInternalConvertKeysRequest request = convertKeysRequest;
      DirectoryInternalConvertKeysResponse convertKeysResponse = directory2.ConvertKeys(context1, request);
      foreach (string key in results.Keys.ToList<string>())
      {
        DirectoryInternalConvertKeyResult convertKeyResult;
        if (convertKeysResponse.Results != null && convertKeysResponse.Results.TryGetValue(key, out convertKeyResult) && convertKeyResult != null && convertKeyResult.Key != null)
          results[key] = new DirectoryConvertKeyResult()
          {
            Key = convertKeyResult.Key
          };
        else
          results[key] = new DirectoryConvertKeyResult()
          {
            Exception = (Exception) new DirectoryKeyNotFoundException("Key not found: " + key)
          };
      }
      return true;
    }

    private Dictionary<string, DirectoryConvertKeyResult> CreateNotSupportedResults(
      IEnumerable<string> keys)
    {
      DirectoryDiscoveryServiceClientException ex = new DirectoryDiscoveryServiceClientException(string.Format("Conversion from {0} to {1} not supported.", (object) this.ConvertFrom, (object) this.ConvertTo));
      return keys.ToDictionary<string, string, DirectoryConvertKeyResult>((Func<string, string>) (key => key), (Func<string, DirectoryConvertKeyResult>) (key => new DirectoryConvertKeyResult()
      {
        Exception = (Exception) ex
      }));
    }
  }
}
