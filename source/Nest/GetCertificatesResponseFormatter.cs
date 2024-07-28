// Decompiled with JetBrains decompiler
// Type: Nest.GetCertificatesResponseFormatter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using Elasticsearch.Net.Utf8Json.Formatters;
using System;
using System.Collections.Generic;

namespace Nest
{
  internal class GetCertificatesResponseFormatter : 
    IJsonFormatter<GetCertificatesResponse>,
    IJsonFormatter
  {
    private static readonly ReadOnlyCollectionFormatter<ClusterCertificateInformation> Formatter = new ReadOnlyCollectionFormatter<ClusterCertificateInformation>();

    public void Serialize(
      ref JsonWriter writer,
      GetCertificatesResponse value,
      IJsonFormatterResolver formatterResolver)
    {
      throw new NotImplementedException();
    }

    public GetCertificatesResponse Deserialize(
      ref JsonReader reader,
      IJsonFormatterResolver formatterResolver)
    {
      GetCertificatesResponse certificatesResponse1 = new GetCertificatesResponse();
      if (reader.ReadIsNull())
        return certificatesResponse1;
      switch (reader.GetCurrentJsonToken())
      {
        case JsonToken.BeginObject:
          int count = 0;
          while (reader.ReadIsInObject(ref count))
          {
            ArraySegment<byte> bytes = reader.ReadPropertyNameSegmentRaw();
            int num;
            if (ResponseFormatterHelpers.ServerErrorFields.TryGetValue(bytes, out num))
            {
              switch (num)
              {
                case 0:
                  if (reader.GetCurrentJsonToken() == JsonToken.String)
                  {
                    GetCertificatesResponse certificatesResponse2 = certificatesResponse1;
                    Error error = new Error();
                    error.Reason = reader.ReadString();
                    certificatesResponse2.Error = error;
                    continue;
                  }
                  IJsonFormatter<Error> formatter = formatterResolver.GetFormatter<Error>();
                  certificatesResponse1.Error = formatter.Deserialize(ref reader, formatterResolver);
                  continue;
                case 1:
                  if (reader.GetCurrentJsonToken() == JsonToken.Number)
                  {
                    certificatesResponse1.StatusCode = new int?(reader.ReadInt32());
                    continue;
                  }
                  reader.ReadNextBlock();
                  continue;
                default:
                  continue;
              }
            }
            else
              reader.ReadNextBlock();
          }
          break;
        case JsonToken.BeginArray:
          certificatesResponse1.Certificates = (IReadOnlyCollection<ClusterCertificateInformation>) GetCertificatesResponseFormatter.Formatter.Deserialize(ref reader, formatterResolver);
          break;
      }
      return certificatesResponse1;
    }
  }
}
