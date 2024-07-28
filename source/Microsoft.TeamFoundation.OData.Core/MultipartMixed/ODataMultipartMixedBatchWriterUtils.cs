// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MultipartMixed.ODataMultipartMixedBatchWriterUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.OData.MultipartMixed
{
  internal static class ODataMultipartMixedBatchWriterUtils
  {
    internal static string CreateBatchBoundary(bool isResponse) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, isResponse ? "batchresponse_{0}" : "batch_{0}", new object[1]
    {
      (object) Guid.NewGuid()
    });

    internal static string CreateChangeSetBoundary(bool isResponse, string changesetId) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, isResponse ? "changesetresponse_{0}" : "changeset_{0}", new object[1]
    {
      (object) changesetId
    });

    internal static string CreateMultipartMixedContentType(string boundary) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}; {1}={2}", new object[3]
    {
      (object) "multipart/mixed",
      (object) nameof (boundary),
      (object) boundary
    });

    internal static string GetBatchBoundaryFromMediaType(ODataMediaType mediaType)
    {
      KeyValuePair<string, string> keyValuePair1 = new KeyValuePair<string, string>();
      IEnumerable<KeyValuePair<string, string>> parameters = mediaType.Parameters;
      if (parameters != null)
      {
        bool flag = false;
        foreach (KeyValuePair<string, string> keyValuePair2 in parameters.Where<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => HttpUtils.CompareMediaTypeParameterNames("boundary", p.Key))))
        {
          if (flag)
            throw new ODataException(Strings.MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads((object) mediaType.ToText(), (object) "boundary"));
          keyValuePair1 = keyValuePair2;
          flag = true;
        }
      }
      string boundary = keyValuePair1.Key != null ? keyValuePair1.Value : throw new ODataException(Strings.MediaTypeUtils_BoundaryMustBeSpecifiedForBatchPayloads((object) mediaType.ToText(), (object) "boundary"));
      ValidationUtils.ValidateBoundaryString(boundary);
      return boundary;
    }

    internal static void WriteStartBoundary(TextWriter writer, string boundary, bool firstBoundary)
    {
      if (!firstBoundary)
        writer.WriteLine();
      writer.WriteLine("--{0}", (object) boundary);
    }

    internal static void WriteEndBoundary(
      TextWriter writer,
      string boundary,
      bool missingStartBoundary)
    {
      if (!missingStartBoundary)
        writer.WriteLine();
      writer.Write("--{0}--", (object) boundary);
    }

    internal static void WriteRequestPreamble(
      TextWriter writer,
      string httpMethod,
      Uri uri,
      Uri baseUri,
      bool inChangeSetBound,
      string contentId,
      BatchPayloadUriOption payloadUriOption)
    {
      ODataMultipartMixedBatchWriterUtils.WriteHeaders(writer, inChangeSetBound, contentId);
      writer.WriteLine();
      ODataMultipartMixedBatchWriterUtils.WriteRequestUri(writer, httpMethod, uri, baseUri, payloadUriOption);
    }

    internal static void WriteResponsePreamble(
      TextWriter writer,
      bool inChangeSetBound,
      string contentId)
    {
      ODataMultipartMixedBatchWriterUtils.WriteHeaders(writer, inChangeSetBound, contentId);
      writer.WriteLine();
    }

    internal static void WriteChangeSetPreamble(TextWriter writer, string changeSetBoundary)
    {
      string mixedContentType = ODataMultipartMixedBatchWriterUtils.CreateMultipartMixedContentType(changeSetBoundary);
      writer.WriteLine("{0}: {1}", (object) "Content-Type", (object) mixedContentType);
      writer.WriteLine();
    }

    private static void WriteHeaders(TextWriter writer, bool inChangeSetBound, string contentId)
    {
      writer.WriteLine("{0}: {1}", (object) "Content-Type", (object) "application/http");
      writer.WriteLine("{0}: {1}", (object) "Content-Transfer-Encoding", (object) "binary");
      if (!inChangeSetBound || contentId == null)
        return;
      writer.WriteLine("{0}: {1}", (object) "Content-ID", (object) contentId);
    }

    private static void WriteRequestUri(
      TextWriter writer,
      string httpMethod,
      Uri uri,
      Uri baseUri,
      BatchPayloadUriOption payloadUriOption)
    {
      if (uri.IsAbsoluteUri)
      {
        string absoluteUri = uri.AbsoluteUri;
        switch (payloadUriOption)
        {
          case BatchPayloadUriOption.AbsoluteUri:
            writer.WriteLine("{0} {1} {2}", (object) httpMethod, (object) UriUtils.UriToString(uri), (object) "HTTP/1.1");
            break;
          case BatchPayloadUriOption.AbsoluteUriUsingHostHeader:
            string str1 = absoluteUri.Substring(absoluteUri.IndexOf('/', absoluteUri.IndexOf("//", StringComparison.Ordinal) + 2));
            writer.WriteLine("{0} {1} {2}", (object) httpMethod, (object) str1, (object) "HTTP/1.1");
            writer.WriteLine("Host: {0}:{1}", (object) uri.Host, (object) uri.Port);
            break;
          case BatchPayloadUriOption.RelativeUri:
            string str2 = UriUtils.UriToString(baseUri);
            string str3 = absoluteUri.Substring(str2.Length);
            writer.WriteLine("{0} {1} {2}", (object) httpMethod, (object) str3, (object) "HTTP/1.1");
            break;
        }
      }
      else
        writer.WriteLine("{0} {1} {2}", (object) httpMethod, (object) UriUtils.UriToString(uri), (object) "HTTP/1.1");
    }
  }
}
