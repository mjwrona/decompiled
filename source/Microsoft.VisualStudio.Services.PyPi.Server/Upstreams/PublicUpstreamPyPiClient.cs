// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Upstreams.PublicUpstreamPyPiClient
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.CommonPatterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PublicRepositories;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.PublicRepository;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Upstreams
{
  public class PublicUpstreamPyPiClient : IPublicUpstreamPyPiClient, IUpstreamPyPiClient
  {
    private readonly 
    #nullable disable
    Uri packageSourceUri;
    private readonly IHttpClient httpClient;
    private readonly IConverter<PyPiUpstreamJsonMetadataPackageFileMetadataRequest, IReadOnlyDictionary<string, string[]>> upstreamJsonMetadataToIngestionMetadataConverter;
    private readonly IConverter<string, PyPiPackageRegistrationState> registrationStateConverter;
    private readonly IFactory<PyPiPackageName, Task<PyPiPackageRegistrationState>> packageRegistrationStateFactory;

    public PublicUpstreamPyPiClient(
      Uri packageSourceUri,
      IHttpClient httpClient,
      IConverter<PyPiUpstreamJsonMetadataPackageFileMetadataRequest, IReadOnlyDictionary<string, string[]>> upstreamJsonMetadataToIngestionMetadataConverter,
      IConverter<string, PyPiPackageRegistrationState> registrationStateConverter)
    {
      this.packageSourceUri = packageSourceUri;
      this.httpClient = httpClient;
      this.upstreamJsonMetadataToIngestionMetadataConverter = upstreamJsonMetadataToIngestionMetadataConverter;
      this.registrationStateConverter = registrationStateConverter;
      this.packageRegistrationStateFactory = ByFuncInputFactory.For<PyPiPackageName, Task<PyPiPackageRegistrationState>>((Func<PyPiPackageName, Task<PyPiPackageRegistrationState>>) (packageName => this.FetchLimitedMetadataDictionary(packageName))).SingleElementCache<PyPiPackageName, Task<PyPiPackageRegistrationState>>((IEqualityComparer<PyPiPackageName>) PackageNameComparer.NormalizedName);
    }

    private async Task<PyPiPackageRegistrationState> FetchLimitedMetadataDictionary(
      PyPiPackageName packageName)
    {
      PublicUpstreamPyPiClient upstreamPyPiClient = this;
      // ISSUE: reference to a compiler-generated method
      PyPiPackageRegistrationState registrationState = await PublicUpstreamHttpClientHelper.GetWithErrorHandlingNullIf404Async<PyPiPackageRegistrationState>(upstreamPyPiClient.httpClient, upstreamPyPiClient.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageName) packageName), upstreamPyPiClient.ConstructPackageMetadataJsonEndpointUri(packageName), HttpCompletionOption.ResponseContentRead, new Func<HttpResponseMessage, Task<PyPiPackageRegistrationState>>(upstreamPyPiClient.\u003CFetchLimitedMetadataDictionary\u003Eb__6_0));
      if ((object) registrationState == null)
        registrationState = new PyPiPackageRegistrationState(ImmutableDictionary<PyPiPackageVersion, PyPiPackageVersionRegistrationState>.Empty, (PyPiChangelogCursor) null);
      return registrationState;
    }

    public async Task<PyPiPackageRegistrationState> GetRegistrationState(PyPiPackageName packageName) => await this.packageRegistrationStateFactory.Get(packageName);

    public async Task<IEnumerable<LimitedPyPiMetadata>> GetLimitedMetadataList(
      PyPiPackageName packageName,
      IEnumerable<PyPiPackageVersion> versions)
    {
      PyPiPackageRegistrationState metadataDictionary = await this.packageRegistrationStateFactory.Get(packageName);
      return Process();

      static PyPiPackageFile ConvertFile(PyPiPackageVersionFileRegistrationState x) => new PyPiPackageFile(x.Filename, (IStorageId) null, (IReadOnlyCollection<HashAndType>) x.Digests, x.Size, x.UploadTime, x.Packagetype.GetValueOrDefault());

      static string PickRequiresPythonOrDefault(
        IEnumerable<PyPiPackageVersionFileRegistrationState> files)
      {
        return files.Select<PyPiPackageVersionFileRegistrationState, string>((Func<PyPiPackageVersionFileRegistrationState, string>) (x => x.RequiresPython)).FirstOrDefault<string>((Func<string, bool>) (x => !string.IsNullOrWhiteSpace(x)));
      }

      IEnumerable<LimitedPyPiMetadata> Process()
      {
        foreach (PyPiPackageVersion version in versions)
        {
          PyPiPackageVersionRegistrationState registrationState;
          if (metadataDictionary.Versions.TryGetValue(version, out registrationState))
            yield return new LimitedPyPiMetadata(registrationState.CanonicalIdentity, PickRequiresPythonOrDefault((IEnumerable<PyPiPackageVersionFileRegistrationState>) registrationState.Files), ((IEnumerable<IUnstoredPyPiPackageFile>) registrationState.Files.Select<PyPiPackageVersionFileRegistrationState, PyPiPackageFile>(new Func<PyPiPackageVersionFileRegistrationState, PyPiPackageFile>(ConvertFile))).ToImmutableArray<IUnstoredPyPiPackageFile>());
        }
      }
    }

    public async Task<Stream> GetFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      PyPiUpstreamMetadata upstreamMetadata = await this.GetUpstreamMetadata(downstreamFeedRequest, packageIdentity, filePath);
      Uri uri = upstreamMetadata.RawFileMetadata != null ? new Uri(PyPiMetadataUtils.GetOptionalMetadataField("url", upstreamMetadata.RawFileMetadata)) : throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_UpstreamDoesNotHaveFileRequested((object) filePath, (object) packageIdentity.DisplayStringForMessages, (object) this.packageSourceUri.AbsoluteUri), this.packageSourceUri);
      return await PublicUpstreamHttpClientHelper.GetStreamWithErrorHandlingAsync(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity, (IPackageFileName) new SimplePackageFileName(filePath)), uri, HttpCompletionOption.ResponseHeadersRead);
    }

    public async Task<Stream> GetGpgSignatureForFile(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string filePath)
    {
      PyPiUpstreamMetadata upstreamMetadata = await this.GetUpstreamMetadata(downstreamFeedRequest, packageIdentity, filePath);
      UriBuilder uriBuilder = upstreamMetadata.RawFileMetadata != null ? new UriBuilder(new Uri(PyPiMetadataUtils.GetOptionalMetadataField("url", upstreamMetadata.RawFileMetadata))) : throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_UpstreamDoesNotHaveFileRequested((object) filePath, (object) packageIdentity.DisplayStringForMessages, (object) this.packageSourceUri.AbsoluteUri), this.packageSourceUri);
      uriBuilder.Path += ".asc";
      return await PublicUpstreamHttpClientHelper.GetStreamWithErrorHandlingNullIf404Async(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity, (IPackageFileName) new SimplePackageFileName(filePath)), uriBuilder.Uri, HttpCompletionOption.ResponseHeadersRead);
    }

    public async Task<IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>> GetPackageVersions(
      IFeedRequest _,
      PyPiPackageName packageName)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      return (IReadOnlyList<VersionWithSourceChain<PyPiPackageVersion>>) (await this.packageRegistrationStateFactory.Get(packageName)).Versions.Keys.Select<PyPiPackageVersion, VersionWithSourceChain<PyPiPackageVersion>>(PublicUpstreamPyPiClient.\u003C\u003EO.\u003C0\u003E__FromExternalSource ?? (PublicUpstreamPyPiClient.\u003C\u003EO.\u003C0\u003E__FromExternalSource = new Func<PyPiPackageVersion, VersionWithSourceChain<PyPiPackageVersion>>(VersionWithSourceChain.FromExternalSource<PyPiPackageVersion>))).ToList<VersionWithSourceChain<PyPiPackageVersion>>();
    }

    public async Task<PyPiUpstreamMetadata> GetUpstreamMetadata(
      IFeedRequest downstreamFeedRequest,
      PyPiPackageIdentity packageIdentity,
      string requestFilePath)
    {
      return await PublicUpstreamHttpClientHelper.GetWithErrorHandlingAsync<PyPiUpstreamMetadata>(this.httpClient, this.packageSourceUri.AbsoluteUri, PackageIdentifierForMessages.From((IPackageIdentity) packageIdentity), this.ConstructPackageMetadataJsonEndpointUri(packageIdentity), HttpCompletionOption.ResponseContentRead, (Func<HttpResponseMessage, Task<PyPiUpstreamMetadata>>) (async response => await this.GetPackageMetadata(response, requestFilePath)));
    }

    private async Task<PyPiUpstreamMetadata> GetPackageMetadata(
      HttpResponseMessage httpResponse,
      string requestFilePath)
    {
      PyPiUpstreamJsonMetadataPackageFileMetadataRequest fileMetadataRequest1 = new PyPiUpstreamJsonMetadataPackageFileMetadataRequest();
      PyPiUpstreamJsonMetadataPackageFileMetadataRequest fileMetadataRequest2 = fileMetadataRequest1;
      fileMetadataRequest2.UpstreamJsonMetadata = await httpResponse.Content.ReadAsStringAsync();
      fileMetadataRequest1.UpstreamPackageFileName = requestFilePath;
      PyPiUpstreamJsonMetadataPackageFileMetadataRequest input = fileMetadataRequest1;
      fileMetadataRequest2 = (PyPiUpstreamJsonMetadataPackageFileMetadataRequest) null;
      fileMetadataRequest1 = (PyPiUpstreamJsonMetadataPackageFileMetadataRequest) null;
      return new PyPiUpstreamMetadata(this.upstreamJsonMetadataToIngestionMetadataConverter.Convert(input), ImmutableArray<UpstreamSourceInfo>.Empty);
    }

    private Uri ConstructPackageMetadataJsonEndpointUri(PyPiPackageIdentity packageIdentity) => new Uri(this.PyPiApiBaseUri, "pypi/" + packageIdentity.Name.NormalizedName + "/" + packageIdentity.Version.NormalizedVersion + "/json");

    private Uri ConstructPackageMetadataJsonEndpointUri(PyPiPackageName packageName) => new Uri(this.PyPiApiBaseUri, "pypi/" + packageName.NormalizedName + "/json");

    private Uri PyPiApiBaseUri => new Uri(this.packageSourceUri, "pypi");

    public async Task<List<ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>>> ChangelogSinceSerial(
      PyPiChangelogCursor since)
    {
      List<ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>> list;
      try
      {
        using (HttpResponseMessage response = await this.httpClient.PostAsync(this.PyPiApiBaseUri, (HttpContent) new StringContent(string.Format("<?xml version='1.0'?>\r\n                            <methodCall>\r\n                                <methodName>changelog_since_serial</methodName>\r\n                                <params>\r\n                                    <param>\r\n                                        <value>\r\n                                            <int>{0}</int>\r\n                                        </value>\r\n                                    </param>\r\n                                </params>\r\n                            </methodCall>", (object) since.SinceSerial), Encoding.UTF8, "text/xml"), HttpCompletionOption.ResponseHeadersRead))
          list = new PublicUpstreamPyPiClient.ChangelogSinceSerialResponse(this.ParseXmlResponse(await response.Content.ReadAsStringAsync())).ChangedPackages.ToList<ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>>();
      }
      catch (Exception ex)
      {
        throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_UpstreamFailure((object) this.PyPiApiBaseUri.AbsoluteUri, (object) ex.Message), this.PyPiApiBaseUri);
      }
      return list;
    }

    public async Task<ulong> ChangelogLastSerial()
    {
      ulong num;
      try
      {
        using (HttpResponseMessage response = await this.httpClient.PostAsync(this.PyPiApiBaseUri, (HttpContent) new StringContent("<?xml version='1.0'?>\r\n                                    <methodCall>\r\n                                        <methodName>changelog_last_serial</methodName>\r\n                                    </methodCall>", Encoding.UTF8, "text/xml"), HttpCompletionOption.ResponseHeadersRead))
          num = ulong.Parse(this.ParseXmlResponse(await response.Content.ReadAsStringAsync()).XPathSelectElement("/methodResponse/params/param/value/int").Value);
      }
      catch (Exception ex)
      {
        throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_UpstreamFailure((object) this.PyPiApiBaseUri.AbsoluteUri, (object) ex.Message), this.PyPiApiBaseUri);
      }
      return num;
    }

    private XDocument ParseXmlResponse(string responseContent)
    {
      XDocument xmlDoc = XDocument.Parse(responseContent);
      (string FaultCode, string FaultString) fault;
      return !PublicUpstreamPyPiClient.IsFault(xmlDoc, out fault) ? xmlDoc : throw new PublicUpstreamFailureException(Microsoft.VisualStudio.Services.PyPi.Server.Resources.Error_UpstreamFailure((object) this.PyPiApiBaseUri.AbsoluteUri, (object) ("Fault code: " + fault.FaultCode + "; Fault string: " + fault.FaultString)), this.PyPiApiBaseUri);
    }

    private static bool IsFault(XDocument xmlDoc, out (string FaultCode, string FaultString) fault)
    {
      string expression = "/methodResponse/fault/value/struct/member[name='faultCode']/value/int | /methodResponse/fault/value/struct/member[name='faultString']/value/string";
      IEnumerable<XElement> source = xmlDoc.XPathSelectElements(expression);
      if (source.Count<XElement>() == 2)
      {
        fault = (source.ElementAt<XElement>(0).Value, source.ElementAt<XElement>(1).Value);
        return true;
      }
      fault = (string.Empty, string.Empty);
      return false;
    }

    private record ChangelogEntry(
      PyPiPackageIdentity PackageIdentity,
      PyPiChangelogCursor ChangelogCursor,
      DateTimeOffset DateTimeOffset)
    {
      public static PublicUpstreamPyPiClient.ChangelogEntry Create(params string[] values)
      {
        try
        {
          PyPiPackageName name = new PyPiPackageName(values[0]);
          PyPiPackageVersion piPackageVersion = PyPiPackageVersionParser.Parse(values[1]);
          DateTimeOffset DateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(long.Parse(values[2]));
          PyPiChangelogCursor ChangelogCursor = PyPiChangelogCursor.Parse(values[4]);
          PyPiPackageVersion version = piPackageVersion;
          return new PublicUpstreamPyPiClient.ChangelogEntry(new PyPiPackageIdentity(name, version), ChangelogCursor, DateTimeOffset);
        }
        catch (Exception ex)
        {
          return (PublicUpstreamPyPiClient.ChangelogEntry) null;
        }
      }

      public DateTime UtcDateTime => this.DateTimeOffset.UtcDateTime;

      [CompilerGenerated]
      protected virtual bool PrintMembers(
      #nullable enable
      StringBuilder builder)
      {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        builder.Append("PackageIdentity = ");
        builder.Append((object) this.PackageIdentity);
        builder.Append(", ChangelogCursor = ");
        builder.Append((object) this.ChangelogCursor);
        builder.Append(", DateTimeOffset = ");
        builder.Append(this.DateTimeOffset.ToString());
        builder.Append(", UtcDateTime = ");
        builder.Append(this.UtcDateTime.ToString());
        return true;
      }
    }

    private record ChangelogSinceSerialResponse(
    #nullable disable
    XDocument Document)
    {
      private const string ChangelogXPathExpression = "//data[not(.//data) and count(value[1]/string) = 1 and count(value[2]/string) = 1 and count(value[3]/int) = 1 and count(value[4]/string) = 1 and count(value[5]/int) = 1]";

      public IEnumerable<ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>> ChangedPackages => this.Document.XPathSelectElements("//data[not(.//data) and count(value[1]/string) = 1 and count(value[2]/string) = 1 and count(value[3]/int) = 1 and count(value[4]/string) = 1 and count(value[5]/int) = 1]").Select<XElement, PublicUpstreamPyPiClient.ChangelogEntry>((Func<XElement, PublicUpstreamPyPiClient.ChangelogEntry>) (data => PublicUpstreamPyPiClient.ChangelogEntry.Create(data.Elements((XName) "value").Select<XElement, string>((Func<XElement, string>) (x => x.Value)).ToArray<string>()))).Where<PublicUpstreamPyPiClient.ChangelogEntry>((Func<PublicUpstreamPyPiClient.ChangelogEntry, bool>) (ce => ce != null)).Select<PublicUpstreamPyPiClient.ChangelogEntry, ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>>((Func<PublicUpstreamPyPiClient.ChangelogEntry, ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>>) (ce => new ChangedPackage<PyPiChangelogCursor, PyPiPackageIdentity>(ce.ChangelogCursor, ce.UtcDateTime, ce.PackageIdentity)));

      [CompilerGenerated]
      protected virtual bool PrintMembers(
      #nullable enable
      StringBuilder builder)
      {
        RuntimeHelpers.EnsureSufficientExecutionStack();
        builder.Append("Document = ");
        builder.Append((object) this.Document);
        builder.Append(", ChangedPackages = ");
        builder.Append((object) this.ChangedPackages);
        return true;
      }
    }
  }
}
