// Decompiled with JetBrains decompiler
// Type: Nest.GetCertificatesResponseBuilder
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Nest
{
  internal class GetCertificatesResponseBuilder : CustomResponseBuilderBase
  {
    public static GetCertificatesResponseBuilder Instance { get; } = new GetCertificatesResponseBuilder();

    public override object DeserializeResponse(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream)
    {
      if (!response.Success)
        return (object) new GetCertificatesResponse();
      return (object) new GetCertificatesResponse()
      {
        Certificates = (IReadOnlyCollection<ClusterCertificateInformation>) builtInSerializer.Deserialize<ClusterCertificateInformation[]>(stream)
      };
    }

    public override async Task<object> DeserializeResponseAsync(
      IElasticsearchSerializer builtInSerializer,
      IApiCallDetails response,
      Stream stream,
      CancellationToken ctx = default (CancellationToken))
    {
      GetCertificatesResponse certificatesResponse;
      if (response.Success)
      {
        GetCertificatesResponse certificatesResponse1 = new GetCertificatesResponse();
        GetCertificatesResponse certificatesResponse2 = certificatesResponse1;
        certificatesResponse2.Certificates = (IReadOnlyCollection<ClusterCertificateInformation>) await builtInSerializer.DeserializeAsync<ClusterCertificateInformation[]>(stream, ctx).ConfigureAwait(false);
        certificatesResponse = certificatesResponse1;
        certificatesResponse2 = (GetCertificatesResponse) null;
        certificatesResponse1 = (GetCertificatesResponse) null;
      }
      else
        certificatesResponse = new GetCertificatesResponse();
      return (object) certificatesResponse;
    }
  }
}
