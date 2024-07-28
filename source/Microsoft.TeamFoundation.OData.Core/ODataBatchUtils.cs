// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Microsoft.OData
{
  internal static class ODataBatchUtils
  {
    internal static Uri CreateOperationRequestUri(
      Uri uri,
      Uri baseUri,
      IODataPayloadUriConverter payloadUriConverter)
    {
      if (payloadUriConverter != null)
      {
        Uri operationRequestUri = payloadUriConverter.ConvertPayloadUri(baseUri, uri);
        if (operationRequestUri != (Uri) null)
          return operationRequestUri;
      }
      Uri operationRequestUri1;
      if (uri.IsAbsoluteUri)
        operationRequestUri1 = uri;
      else
        operationRequestUri1 = !(baseUri == (Uri) null) ? UriUtils.UriToAbsoluteUri(baseUri, uri) : throw new ODataException(UriUtils.UriToString(uri).StartsWith("$", StringComparison.Ordinal) ? Strings.ODataBatchUtils_RelativeUriStartingWithDollarUsedWithoutBaseUriSpecified((object) UriUtils.UriToString(uri)) : Strings.ODataBatchUtils_RelativeUriUsedWithoutBaseUriSpecified((object) UriUtils.UriToString(uri)));
      return operationRequestUri1;
    }

    internal static ODataReadStream CreateBatchOperationReadStream(
      ODataBatchReaderStream batchReaderStream,
      ODataBatchOperationHeaders headers,
      IODataStreamListener operationListener)
    {
      string str;
      if (!headers.TryGetValue("Content-Length", out str))
        return ODataReadStream.Create(batchReaderStream, operationListener);
      int length = int.Parse(str, (IFormatProvider) CultureInfo.InvariantCulture);
      if (length < 0)
        throw new ODataException(Strings.ODataBatchReaderStream_InvalidContentLengthSpecified((object) str));
      return ODataReadStream.Create(batchReaderStream, operationListener, length);
    }

    internal static ODataWriteStream CreateBatchOperationWriteStream(
      Stream outputStream,
      IODataStreamListener operationListener)
    {
      return new ODataWriteStream(outputStream, operationListener);
    }

    internal static void EnsureArraySize(
      ref byte[] buffer,
      int numberOfBytesInBuffer,
      int requiredByteCount)
    {
      int num1 = buffer.Length - numberOfBytesInBuffer;
      if (requiredByteCount <= num1)
        return;
      int num2 = requiredByteCount - num1;
      byte[] src = buffer;
      buffer = new byte[buffer.Length + num2];
      Buffer.BlockCopy((Array) src, 0, (Array) buffer, 0, numberOfBytesInBuffer);
    }

    internal static void ValidateReferenceUri(
      Uri uri,
      IEnumerable<string> dependsOnRequestIds,
      Uri baseUri)
    {
      if (UriUtils.UriToString(uri).IndexOf('$') == -1)
        return;
      string str1;
      if (uri.IsAbsoluteUri)
      {
        if (baseUri == (Uri) null)
          return;
        string str2 = UriUtils.UriToString(baseUri);
        if (!uri.AbsoluteUri.StartsWith(str2, StringComparison.Ordinal))
          return;
        str1 = uri.AbsoluteUri.Substring(str2.Length);
      }
      else
        str1 = UriUtils.UriToString(uri);
      while (str1.StartsWith("/", StringComparison.Ordinal))
        str1 = str1.Substring(1);
      if (str1.Length <= 0 || str1[0] != '$')
        return;
      int num = str1.IndexOf('/', 1);
      string p0 = num > 0 ? str1.Substring(1, num - 1) : str1.Substring(1);
      if (dependsOnRequestIds == null || !dependsOnRequestIds.Contains<string>(p0))
        throw new ODataException(Strings.ODataBatchReader_ReferenceIdNotIncludedInDependsOn((object) p0, (object) UriUtils.UriToString(uri), dependsOnRequestIds != null ? (object) string.Join(",", dependsOnRequestIds.ToArray<string>()) : (object) "null"));
    }
  }
}
