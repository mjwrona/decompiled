// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.Controllers.NuGetClientController
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.NuGet.Server.Controllers
{
  [ControllerApiVersion(1.0)]
  [ClientGroupByResource("NuGet")]
  [VersionedApiControllerCustomName(Area = "nuget", ResourceName = "client")]
  public class NuGetClientController : NuGetApiController
  {
    private static readonly object KnownFilesLock = new object();
    private static readonly IReadOnlyList<string> KnownFileNames = (IReadOnlyList<string>) new string[6]
    {
      "AuthHelperBundle.zip",
      "CredentialProviderBundle.zip",
      "NuGet.exe",
      "Vss.NuGet.zip",
      "Microsoft.VisualStudio.Services.NuGet.AuthHelper.zip",
      "Microsoft.VisualStudio.Services.NuGet.CredentialProvider.zip"
    };
    private static readonly IDictionary<string, string[]> KnownAliases = (IDictionary<string, string[]>) new Dictionary<string, string[]>((IEqualityComparer<string>) StringComparer.InvariantCultureIgnoreCase)
    {
      {
        "Microsoft.VisualStudio.Services.NuGet.AuthHelper.zip",
        new string[1]{ "Vss.NuGet.AuthHelper.zip" }
      }
    };
    private static IReadOnlyList<NuGetClientController.FileInfo> knownFileInfo;

    [HttpGet]
    [ClientIgnore]
    public IHttpActionResult Get(string fileName)
    {
      if (fileName == null)
        return (IHttpActionResult) this.NotFound();
      NuGetClientController.FileInfo fileInfoOrDefault = this.GetFileInfoOrDefault(this.TfsRequestContext, fileName);
      if (fileInfoOrDefault == null)
        return (IHttpActionResult) this.NotFound();
      if (this.Request.Headers.IfNoneMatch.Contains(fileInfoOrDefault.ETag))
        return this.NotModified(fileInfoOrDefault.ETag);
      this.TfsRequestContext.UpdateTimeToFirstPage();
      return this.File(fileInfoOrDefault);
    }

    private static IEnumerable<NuGetClientController.FileInfo> PrepareFileInfo(
      IVssRequestContext requestContext,
      IEnumerable<string> fileNames)
    {
      string packagingClientsPath;
      if (NuGetClientController.TryGetPackagingClientDirectory(requestContext, out packagingClientsPath))
      {
        SHA256 sha256 = SHA256.Create();
        foreach (string fileName1 in fileNames)
        {
          string fileName = fileName1;
          string fullName = Path.Combine(packagingClientsPath, fileName);
          if (System.IO.File.Exists(fullName))
          {
            using (FileStream stream = System.IO.File.OpenRead(fullName))
            {
              yield return new NuGetClientController.FileInfo(fileName, fullName, "\"" + Convert.ToBase64String(sha256.ComputeHash((Stream) stream)) + "\"");
              if (NuGetClientController.KnownAliases.ContainsKey(fileName))
              {
                string[] strArray = NuGetClientController.KnownAliases[fileName];
                for (int index = 0; index < strArray.Length; ++index)
                  yield return new NuGetClientController.FileInfo(strArray[index], fullName, "\"" + Convert.ToBase64String(sha256.ComputeHash((Stream) stream)) + "\"");
                strArray = (string[]) null;
              }
              else
                continue;
            }
            fullName = (string) null;
            fileName = (string) null;
          }
        }
      }
    }

    private static bool TryGetPackagingClientDirectory(
      IVssRequestContext requestContext,
      out string packagingClientsPath)
    {
      string physicalDirectory = requestContext.ServiceHost.PhysicalDirectory;
      packagingClientsPath = Path.Combine(physicalDirectory, "..\\..\\PackagingClients\\NuGet");
      if (Directory.Exists(packagingClientsPath))
        return true;
      packagingClientsPath = Path.Combine(physicalDirectory, "..\\..\\approot\\PackagingClients\\NuGet");
      return Directory.Exists(packagingClientsPath);
    }

    private NuGetClientController.FileInfo GetFileInfoOrDefault(
      IVssRequestContext requestContext,
      string fileName)
    {
      lock (NuGetClientController.KnownFilesLock)
      {
        if (NuGetClientController.knownFileInfo == null)
          NuGetClientController.knownFileInfo = (IReadOnlyList<NuGetClientController.FileInfo>) NuGetClientController.PrepareFileInfo(requestContext, (IEnumerable<string>) NuGetClientController.KnownFileNames).ToList<NuGetClientController.FileInfo>();
      }
      return NuGetClientController.knownFileInfo.FirstOrDefault<NuGetClientController.FileInfo>((Func<NuGetClientController.FileInfo, bool>) (x => x.Name.Equals(fileName, StringComparison.OrdinalIgnoreCase)));
    }

    private IHttpActionResult File(NuGetClientController.FileInfo fileInfo)
    {
      HttpResponseMessage response = new HttpResponseMessage();
      StreamContent streamContent = new StreamContent((Stream) System.IO.File.OpenRead(fileInfo.FullName));
      streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
      {
        FileName = fileInfo.Name
      };
      response.Content = (HttpContent) streamContent;
      response.Headers.ETag = fileInfo.ETag;
      return (IHttpActionResult) this.ResponseMessage(response);
    }

    private IHttpActionResult NotModified(EntityTagHeaderValue etag) => (IHttpActionResult) this.ResponseMessage(new HttpResponseMessage(HttpStatusCode.NotModified)
    {
      Headers = {
        ETag = etag
      }
    });

    private class FileInfo
    {
      public FileInfo(string name, string fullName, string etag)
      {
        this.Name = name;
        this.FullName = fullName;
        this.ETag = new EntityTagHeaderValue(etag);
      }

      public string Name { get; private set; }

      public string FullName { get; private set; }

      public EntityTagHeaderValue ETag { get; private set; }
    }
  }
}
