// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataBatchPayloadUriConverter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;

namespace Microsoft.OData
{
  internal sealed class ODataBatchPayloadUriConverter : IODataPayloadUriConverter
  {
    private readonly IODataPayloadUriConverter batchMessagePayloadUriConverter;
    private HashSet<string> contentIdCache;

    internal ODataBatchPayloadUriConverter(
      IODataPayloadUriConverter batchMessagePayloadUriConverter)
    {
      this.batchMessagePayloadUriConverter = batchMessagePayloadUriConverter;
    }

    internal IODataPayloadUriConverter BatchMessagePayloadUriConverter => this.batchMessagePayloadUriConverter;

    internal IEnumerable<string> ContentIdCache => (IEnumerable<string>) this.contentIdCache;

    Uri IODataPayloadUriConverter.ConvertPayloadUri(Uri baseUri, Uri payloadUri)
    {
      ExceptionUtils.CheckArgumentNotNull<Uri>(payloadUri, nameof (payloadUri));
      if (this.contentIdCache != null && !payloadUri.IsAbsoluteUri)
      {
        string str = UriUtils.UriToString(payloadUri);
        if (str.Length > 0 && str[0] == '$')
        {
          int num = str.IndexOf('/', 1);
          if (this.contentIdCache.Contains(num <= 0 ? str.Substring(1) : str.Substring(1, num - 1)))
            return payloadUri;
        }
      }
      return this.batchMessagePayloadUriConverter != null ? this.batchMessagePayloadUriConverter.ConvertPayloadUri(baseUri, payloadUri) : (Uri) null;
    }

    internal void AddContentId(string contentId)
    {
      if (this.contentIdCache == null)
        this.contentIdCache = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
      this.contentIdCache.Add(contentId);
    }

    internal bool ContainsContentId(string contentId) => this.contentIdCache != null && this.contentIdCache.Contains(contentId);

    internal void Reset()
    {
      if (this.contentIdCache == null)
        return;
      this.contentIdCache.Clear();
    }
  }
}
