// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage.PyPiBlobGetSimplePackageHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.Facades;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.DataContracts;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.SimpleGetPackage
{
  public class PyPiBlobGetSimplePackageHandler : 
    IAsyncHandler<
    #nullable disable
    PackageNameRequest<PyPiPackageName>, HttpResponseMessage>,
    IHaveInputType<PackageNameRequest<PyPiPackageName>>,
    IHaveOutputType<HttpResponseMessage>
  {
    private readonly IReadMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataService;
    private readonly IAsyncHandler<BatchPackageFileRequest<PyPiPackageIdentity>, IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>> uriCalculatingHandler;
    private readonly ITracerService tracerService;

    public PyPiBlobGetSimplePackageHandler(
      IReadMetadataService<PyPiPackageIdentity, IPyPiMetadataEntry> metadataService,
      IAsyncHandler<BatchPackageFileRequest<PyPiPackageIdentity>, IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri>> uriCalculatingHandler,
      ITracerService tracerService)
    {
      this.metadataService = metadataService;
      this.uriCalculatingHandler = uriCalculatingHandler;
      this.tracerService = tracerService;
    }

    public async Task<HttpResponseMessage> Handle(PackageNameRequest<PyPiPackageName> request)
    {
      PyPiBlobGetSimplePackageHandler sendInTheThisObject = this;
      HttpResponseMessage httpResponseMessage1;
      using (sendInTheThisObject.tracerService.Enter((object) sendInTheThisObject, nameof (Handle)))
      {
        PackageNameQuery<IPyPiMetadataEntry> packageNameQuery = new PackageNameQuery<IPyPiMetadataEntry>((IPackageNameRequest) request);
        packageNameQuery.Options = new QueryOptions<IPyPiMetadataEntry>().WithFilter((Func<IPyPiMetadataEntry, bool>) (x => !x.IsDeleted())).OnlyProjecting((Expression<Func<IPyPiMetadataEntry, object>>) (v => (object) v.DeletedDate)).OnlyProjecting((Expression<Func<IPyPiMetadataEntry, object>>) (v => v.PackageFiles)).OnlyProjecting((Expression<Func<IPyPiMetadataEntry, object>>) (v => v.RequiresPython)).OnlyProjecting((Expression<Func<IPyPiMetadataEntry, object>>) (v => (object) v.PermanentDeletedDate));
        PackageNameQuery<IPyPiMetadataEntry> packageNameQueryRequest = packageNameQuery;
        List<IPyPiMetadataEntry> versionStatesAsync = await sendInTheThisObject.metadataService.GetPackageVersionStatesAsync(packageNameQueryRequest);
        if (versionStatesAsync.Count == 0)
          throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageNotFound((object) request.PackageName.DisplayName, (object) request.Feed.FullyQualifiedName));
        Dictionary<PyPiPackageIdentity, IPyPiMetadataEntry> identityToMetadata = versionStatesAsync.ToDictionary<IPyPiMetadataEntry, PyPiPackageIdentity, IPyPiMetadataEntry>((Func<IPyPiMetadataEntry, PyPiPackageIdentity>) (v => v.PackageIdentity), (Func<IPyPiMetadataEntry, IPyPiMetadataEntry>) (v => v));
        string str1 = HttpUtility.HtmlEncode(request.PackageName.DisplayName);
        StringBuilder sb = new StringBuilder();
        sb.Append("<!DOCTYPE html>\n<html><head><title>Links for " + str1 + "</title></head><body><h1>Links for " + str1 + "</h1>");
        List<PyPiPackageFileRequest> fileRequests = versionStatesAsync.SelectMany<IPyPiMetadataEntry, PyPiPackageFileRequest>((Func<IPyPiMetadataEntry, IEnumerable<PyPiPackageFileRequest>>) (v => v.PackageFiles.Select<PyPiPackageFile, PyPiPackageFileRequest>((Func<PyPiPackageFile, PyPiPackageFileRequest>) (f => new PyPiPackageFileRequest((IFeedRequest) request, v.PackageIdentity, f))))).ToList<PyPiPackageFileRequest>();
        IDictionary<IPackageFileRequest<PyPiPackageIdentity>, Uri> dictionary = await sendInTheThisObject.uriCalculatingHandler.Handle(new BatchPackageFileRequest<PyPiPackageIdentity>((IFeedRequest) request, (IReadOnlyCollection<IPackageFileRequest<PyPiPackageIdentity>>) fileRequests));
        foreach (PyPiPackageFileRequest key in fileRequests)
        {
          string str2 = HttpUtility.HtmlAttributeEncode(dictionary[(IPackageFileRequest<PyPiPackageIdentity>) key].AbsoluteUri);
          HashAndType hashAndType = key.PackageFile.Hashes.FirstOrDefault<HashAndType>((Func<HashAndType, bool>) (f => f.HashType == HashType.SHA256));
          string str3 = hashAndType != null ? HttpUtility.HtmlAttributeEncode("#sha256=" + hashAndType.Value) : string.Empty;
          VersionConstraintList requiresPython = identityToMetadata[key.PackageId].RequiresPython;
          string str4 = requiresPython != null ? " data-requires-python=\"" + HttpUtility.HtmlAttributeEncode(requiresPython.ToString()) + "\"" : "";
          string str5 = HttpUtility.HtmlEncode(key.FilePath);
          sb.Append("<a href=\"" + str2 + str3 + "\"" + str4 + ">" + str5 + "</a><br/>");
        }
        sb.Append("</body></html>");
        HttpResponseMessage httpResponseMessage2 = new HttpResponseMessage(HttpStatusCode.OK)
        {
          Content = (HttpContent) new StringContent(sb.ToString())
        };
        httpResponseMessage2.Content.Headers.ContentType = new MediaTypeHeaderValue("text/html");
        httpResponseMessage1 = httpResponseMessage2;
      }
      return httpResponseMessage1;
    }
  }
}
