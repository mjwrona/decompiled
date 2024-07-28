// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.MediaTypeUtils
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.OData
{
  internal static class MediaTypeUtils
  {
    private static readonly ODataPayloadKind[] allSupportedPayloadKinds = new ODataPayloadKind[16]
    {
      ODataPayloadKind.ResourceSet,
      ODataPayloadKind.Resource,
      ODataPayloadKind.Property,
      ODataPayloadKind.MetadataDocument,
      ODataPayloadKind.ServiceDocument,
      ODataPayloadKind.Value,
      ODataPayloadKind.BinaryValue,
      ODataPayloadKind.Collection,
      ODataPayloadKind.EntityReferenceLinks,
      ODataPayloadKind.EntityReferenceLink,
      ODataPayloadKind.Batch,
      ODataPayloadKind.Error,
      ODataPayloadKind.Parameter,
      ODataPayloadKind.IndividualProperty,
      ODataPayloadKind.Delta,
      ODataPayloadKind.Asynchronous
    };
    private static readonly UTF8Encoding encodingUtf8NoPreamble = new UTF8Encoding(false, true);
    private const int MatchInfoCacheMaxSize = 256;
    private static MediaTypeUtils.MatchInfoConcurrentCache MatchInfoCache = new MediaTypeUtils.MatchInfoConcurrentCache(256);

    internal static UTF8Encoding EncodingUtf8NoPreamble => MediaTypeUtils.encodingUtf8NoPreamble;

    internal static Encoding FallbackEncoding => (Encoding) MediaTypeUtils.EncodingUtf8NoPreamble;

    internal static Encoding MissingEncoding => Encoding.UTF8;

    internal static ODataFormat GetContentTypeFromSettings(
      ODataMessageWriterSettings settings,
      ODataPayloadKind payloadKind,
      ODataMediaTypeResolver mediaTypeResolver,
      out ODataMediaType mediaType,
      out Encoding encoding)
    {
      IList<ODataMediaTypeFormat> list = (IList<ODataMediaTypeFormat>) mediaTypeResolver.GetMediaTypeFormats(payloadKind).ToList<ODataMediaTypeFormat>();
      if (list == null || list.Count == 0)
        throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType((object) null, (object) settings.AcceptableMediaTypes));
      bool? useFormat = settings.UseFormat;
      bool flag = true;
      ODataFormat actualFormat;
      if (useFormat.GetValueOrDefault() == flag & useFormat.HasValue)
      {
        mediaType = MediaTypeUtils.GetDefaultMediaType(list, settings.Format, out actualFormat);
        encoding = mediaType.SelectEncoding();
      }
      else
      {
        IList<KeyValuePair<ODataMediaType, string>> keyValuePairList = HttpUtils.MediaTypesFromString(settings.AcceptableMediaTypes);
        MediaTypeUtils.ConvertApplicationJsonInAcceptableMediaTypes(keyValuePairList, (ODataVersion) ((int) settings.Version ?? 0));
        string str = (string) null;
        ODataMediaTypeFormat odataMediaTypeFormat;
        if (keyValuePairList == null || keyValuePairList.Count == 0)
        {
          odataMediaTypeFormat = list[0];
        }
        else
        {
          MediaTypeUtils.MediaTypeMatchInfo mediaTypeMatchInfo = MediaTypeUtils.MatchMediaTypes(keyValuePairList.Select<KeyValuePair<ODataMediaType, string>, ODataMediaType>((Func<KeyValuePair<ODataMediaType, string>, ODataMediaType>) (kvp => kvp.Key)), list.Select<ODataMediaTypeFormat, ODataMediaType>((Func<ODataMediaTypeFormat, ODataMediaType>) (smt => smt.MediaType)).ToArray<ODataMediaType>());
          if (mediaTypeMatchInfo == null)
            throw new ODataContentTypeException(Strings.MediaTypeUtils_DidNotFindMatchingMediaType((object) string.Join(", ", list.Select<ODataMediaTypeFormat, string>((Func<ODataMediaTypeFormat, string>) (mt => mt.MediaType.ToText())).ToArray<string>()), (object) settings.AcceptableMediaTypes));
          odataMediaTypeFormat = list[mediaTypeMatchInfo.TargetTypeIndex];
          str = keyValuePairList[mediaTypeMatchInfo.SourceTypeIndex].Value;
        }
        actualFormat = odataMediaTypeFormat.Format;
        mediaType = odataMediaTypeFormat.MediaType;
        if (keyValuePairList != null && mediaType.Parameters != null)
        {
          if (keyValuePairList.Any<KeyValuePair<ODataMediaType, string>>((Func<KeyValuePair<ODataMediaType, string>, bool>) (t => t.Key.Parameters != null && t.Key.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => string.Compare(p.Key, "metadata", StringComparison.OrdinalIgnoreCase) == 0)))))
            mediaType = new ODataMediaType(mediaType.Type, mediaType.SubType, mediaType.Parameters.Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (p => new KeyValuePair<string, string>(string.Compare(p.Key, "odata.metadata", StringComparison.OrdinalIgnoreCase) == 0 ? "metadata" : p.Key, p.Value))));
          if (keyValuePairList.Any<KeyValuePair<ODataMediaType, string>>((Func<KeyValuePair<ODataMediaType, string>, bool>) (t => t.Key.Parameters != null && t.Key.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => string.Compare(p.Key, "streaming", StringComparison.OrdinalIgnoreCase) == 0)))))
            mediaType = new ODataMediaType(mediaType.Type, mediaType.SubType, mediaType.Parameters.Select<KeyValuePair<string, string>, KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, KeyValuePair<string, string>>) (p => new KeyValuePair<string, string>(string.Compare(p.Key, "odata.streaming", StringComparison.OrdinalIgnoreCase) == 0 ? "streaming" : p.Key, p.Value))));
        }
        string acceptCharsetHeader = settings.AcceptableCharsets;
        if (str != null)
          acceptCharsetHeader = acceptCharsetHeader == null ? str : str + "," + acceptCharsetHeader;
        encoding = MediaTypeUtils.GetEncoding(acceptCharsetHeader, payloadKind, mediaType, true);
      }
      return actualFormat;
    }

    internal static IList<ODataPayloadKindDetectionResult> GetPayloadKindsForContentType(
      string contentTypeHeader,
      ODataMediaTypeResolver mediaTypeResolver,
      out ODataMediaType contentType,
      out Encoding encoding)
    {
      encoding = (Encoding) null;
      string charset;
      contentType = MediaTypeUtils.ParseContentType(contentTypeHeader, out charset);
      ODataMediaType[] targetTypes = new ODataMediaType[1]
      {
        contentType
      };
      List<ODataPayloadKindDetectionResult> kindsForContentType = new List<ODataPayloadKindDetectionResult>();
      for (int index = 0; index < MediaTypeUtils.allSupportedPayloadKinds.Length; ++index)
      {
        ODataPayloadKind supportedPayloadKind = MediaTypeUtils.allSupportedPayloadKinds[index];
        IList<ODataMediaTypeFormat> list = (IList<ODataMediaTypeFormat>) mediaTypeResolver.GetMediaTypeFormats(supportedPayloadKind).ToList<ODataMediaTypeFormat>();
        MediaTypeUtils.MediaTypeMatchInfo mediaTypeMatchInfo = MediaTypeUtils.MatchMediaTypes(list.Select<ODataMediaTypeFormat, ODataMediaType>((Func<ODataMediaTypeFormat, ODataMediaType>) (smt => smt.MediaType)), targetTypes);
        if (mediaTypeMatchInfo != null)
          kindsForContentType.Add(new ODataPayloadKindDetectionResult(supportedPayloadKind, list[mediaTypeMatchInfo.SourceTypeIndex].Format));
      }
      if (!string.IsNullOrEmpty(charset))
        encoding = HttpUtils.GetEncodingFromCharsetName(charset);
      return (IList<ODataPayloadKindDetectionResult>) kindsForContentType;
    }

    internal static bool MediaTypeAndSubtypeAreEqual(
      string firstTypeAndSubtype,
      string secondTypeAndSubtype)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(firstTypeAndSubtype, nameof (firstTypeAndSubtype));
      ExceptionUtils.CheckArgumentNotNull<string>(secondTypeAndSubtype, nameof (secondTypeAndSubtype));
      return HttpUtils.CompareMediaTypeNames(firstTypeAndSubtype, secondTypeAndSubtype);
    }

    internal static bool MediaTypeStartsWithTypeAndSubtype(string mediaType, string typeAndSubtype)
    {
      ExceptionUtils.CheckArgumentNotNull<string>(mediaType, nameof (mediaType));
      ExceptionUtils.CheckArgumentNotNull<string>(typeAndSubtype, nameof (typeAndSubtype));
      return mediaType.StartsWith(typeAndSubtype, StringComparison.OrdinalIgnoreCase);
    }

    internal static bool MediaTypeHasParameterWithValue(
      this ODataMediaType mediaType,
      string parameterName,
      string parameterValue)
    {
      return mediaType.Parameters != null && mediaType.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => HttpUtils.CompareMediaTypeParameterNames(p.Key, parameterName) && string.Compare(p.Value, parameterValue, StringComparison.OrdinalIgnoreCase) == 0));
    }

    internal static bool HasStreamingSetToTrue(this ODataMediaType mediaType) => mediaType.MediaTypeHasParameterWithValue("odata.streaming", "true");

    internal static bool HasIeee754CompatibleSetToTrue(this ODataMediaType mediaType) => mediaType.MediaTypeHasParameterWithValue("IEEE754Compatible", "true");

    internal static void CheckMediaTypeForWildCards(ODataMediaType mediaType)
    {
      if (HttpUtils.CompareMediaTypeNames("*", mediaType.Type) || HttpUtils.CompareMediaTypeNames("*", mediaType.SubType))
        throw new ODataContentTypeException(Strings.ODataMessageReader_WildcardInContentType((object) mediaType.FullTypeName));
    }

    internal static string AlterContentTypeForJsonPadding(string contentType)
    {
      if (contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase))
        return contentType.Remove(0, "application/json".Length).Insert(0, "text/javascript");
      return contentType.StartsWith("text/plain", StringComparison.OrdinalIgnoreCase) ? contentType.Remove(0, "text/plain".Length).Insert(0, "text/javascript") : throw new ODataException(Strings.ODataMessageWriter_JsonPaddingOnInvalidContentType((object) contentType));
    }

    internal static ODataFormat GetFormatFromContentType(
      string contentTypeName,
      ODataPayloadKind[] supportedPayloadKinds,
      ODataMediaTypeResolver mediaTypeResolver,
      out ODataMediaType mediaType,
      out Encoding encoding,
      out ODataPayloadKind selectedPayloadKind)
    {
      string charset;
      mediaType = MediaTypeUtils.ParseContentType(contentTypeName, out charset);
      for (int index = 0; index < supportedPayloadKinds.Length; ++index)
      {
        ODataPayloadKind supportedPayloadKind = supportedPayloadKinds[index];
        IList<ODataMediaTypeFormat> list = (IList<ODataMediaTypeFormat>) mediaTypeResolver.GetMediaTypeFormats(supportedPayloadKind).ToList<ODataMediaTypeFormat>();
        MediaTypeUtils.MatchInfoCacheKey key = new MediaTypeUtils.MatchInfoCacheKey(mediaTypeResolver, supportedPayloadKind, contentTypeName);
        MediaTypeUtils.MediaTypeMatchInfo mediaTypeMatchInfo;
        if (!MediaTypeUtils.MatchInfoCache.TryGetValue(key, out mediaTypeMatchInfo))
        {
          mediaTypeMatchInfo = MediaTypeUtils.MatchMediaTypes(list.Select<ODataMediaTypeFormat, ODataMediaType>((Func<ODataMediaTypeFormat, ODataMediaType>) (smt => smt.MediaType)), new ODataMediaType[1]
          {
            mediaType
          });
          MediaTypeUtils.MatchInfoCache.Add(key, mediaTypeMatchInfo);
        }
        if (mediaTypeMatchInfo != null)
        {
          selectedPayloadKind = supportedPayloadKind;
          encoding = MediaTypeUtils.GetEncoding(charset, selectedPayloadKind, mediaType, false);
          return list[mediaTypeMatchInfo.SourceTypeIndex].Format;
        }
      }
      throw new ODataContentTypeException(Strings.MediaTypeUtils_CannotDetermineFormatFromContentType((object) string.Join(", ", ((IEnumerable<ODataPayloadKind>) supportedPayloadKinds).SelectMany<ODataPayloadKind, string>((Func<ODataPayloadKind, IEnumerable<string>>) (pk => mediaTypeResolver.GetMediaTypeFormats(pk).Select<ODataMediaTypeFormat, string>((Func<ODataMediaTypeFormat, string>) (mt => mt.MediaType.ToText())))).ToArray<string>()), (object) contentTypeName));
    }

    private static ODataMediaType ParseContentType(string contentTypeHeader, out string charset)
    {
      IList<KeyValuePair<ODataMediaType, string>> keyValuePairList = HttpUtils.MediaTypesFromString(contentTypeHeader);
      ODataMediaType mediaType = keyValuePairList.Count == 1 ? keyValuePairList[0].Key : throw new ODataContentTypeException(Strings.MediaTypeUtils_NoOrMoreThanOneContentTypeSpecified((object) contentTypeHeader));
      MediaTypeUtils.CheckMediaTypeForWildCards(mediaType);
      charset = keyValuePairList[0].Value;
      return mediaType;
    }

    private static ODataMediaType GetDefaultMediaType(
      IList<ODataMediaTypeFormat> supportedMediaTypes,
      ODataFormat specifiedFormat,
      out ODataFormat actualFormat)
    {
      for (int index = 0; index < supportedMediaTypes.Count; ++index)
      {
        ODataMediaTypeFormat supportedMediaType = supportedMediaTypes[index];
        if (specifiedFormat == null || supportedMediaType.Format == specifiedFormat)
        {
          actualFormat = supportedMediaType.Format;
          return supportedMediaType.MediaType;
        }
      }
      throw new ODataException(Strings.ODataUtils_DidNotFindDefaultMediaType((object) specifiedFormat));
    }

    private static Encoding GetEncoding(
      string acceptCharsetHeader,
      ODataPayloadKind payloadKind,
      ODataMediaType mediaType,
      bool useDefaultEncoding)
    {
      return payloadKind == ODataPayloadKind.BinaryValue ? (Encoding) null : HttpUtils.EncodingFromAcceptableCharsets(acceptCharsetHeader, mediaType, (Encoding) MediaTypeUtils.encodingUtf8NoPreamble, useDefaultEncoding ? (Encoding) MediaTypeUtils.encodingUtf8NoPreamble : (Encoding) null);
    }

    private static MediaTypeUtils.MediaTypeMatchInfo MatchMediaTypes(
      IEnumerable<ODataMediaType> sourceTypes,
      ODataMediaType[] targetTypes)
    {
      MediaTypeUtils.MediaTypeMatchInfo mediaTypeMatchInfo = (MediaTypeUtils.MediaTypeMatchInfo) null;
      int sourceIndex = 0;
      if (sourceTypes != null)
      {
        foreach (ODataMediaType sourceType in sourceTypes)
        {
          int targetIndex = 0;
          foreach (ODataMediaType targetType in targetTypes)
          {
            MediaTypeUtils.MediaTypeMatchInfo other = new MediaTypeUtils.MediaTypeMatchInfo(sourceType, targetType, sourceIndex, targetIndex);
            if (!other.IsMatch)
            {
              ++targetIndex;
            }
            else
            {
              if (mediaTypeMatchInfo == null)
                mediaTypeMatchInfo = other;
              else if (mediaTypeMatchInfo.CompareTo(other) < 0)
                mediaTypeMatchInfo = other;
              ++targetIndex;
            }
          }
          ++sourceIndex;
        }
      }
      return mediaTypeMatchInfo ?? (MediaTypeUtils.MediaTypeMatchInfo) null;
    }

    private static void ConvertApplicationJsonInAcceptableMediaTypes(
      IList<KeyValuePair<ODataMediaType, string>> specifiedTypes,
      ODataVersion version)
    {
      if (specifiedTypes == null)
        return;
      for (int index1 = 0; index1 < specifiedTypes.Count; ++index1)
      {
        KeyValuePair<ODataMediaType, string> specifiedType = specifiedTypes[index1];
        ODataMediaType key1 = specifiedType.Key;
        if (HttpUtils.CompareMediaTypeNames(key1.SubType, "json") && HttpUtils.CompareMediaTypeNames(key1.Type, "application") && (key1.Parameters == null || !key1.Parameters.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (p => HttpUtils.IsMetadataParameter(p.Key)))))
        {
          IList<KeyValuePair<string, string>> list = key1.Parameters != null ? (IList<KeyValuePair<string, string>>) key1.Parameters.ToList<KeyValuePair<string, string>>() : (IList<KeyValuePair<string, string>>) null;
          List<KeyValuePair<string, string>> parameters = new List<KeyValuePair<string, string>>(list == null ? 1 : list.Count + 1);
          parameters.Add(new KeyValuePair<string, string>(version < ODataVersion.V401 ? "odata.metadata" : "metadata", "minimal"));
          if (list != null)
            parameters.AddRange((IEnumerable<KeyValuePair<string, string>>) list);
          IList<KeyValuePair<ODataMediaType, string>> keyValuePairList = specifiedTypes;
          int index2 = index1;
          ODataMediaType key2 = new ODataMediaType(key1.Type, key1.SubType, (IEnumerable<KeyValuePair<string, string>>) parameters);
          specifiedType = specifiedTypes[index1];
          string str = specifiedType.Value;
          KeyValuePair<ODataMediaType, string> keyValuePair = new KeyValuePair<ODataMediaType, string>(key2, str);
          keyValuePairList[index2] = keyValuePair;
        }
      }
    }

    private sealed class MediaTypeMatchInfo : IComparable<MediaTypeUtils.MediaTypeMatchInfo>
    {
      private const int DefaultQualityValue = 1000;
      private readonly int sourceIndex;
      private readonly int targetIndex;

      public MediaTypeMatchInfo(
        ODataMediaType sourceType,
        ODataMediaType targetType,
        int sourceIndex,
        int targetIndex)
      {
        this.sourceIndex = sourceIndex;
        this.targetIndex = targetIndex;
        this.MatchTypes(sourceType, targetType);
      }

      public int SourceTypeIndex => this.sourceIndex;

      public int TargetTypeIndex => this.targetIndex;

      public int MatchingTypeNamePartCount { get; private set; }

      public int MatchingParameterCount { get; private set; }

      public int QualityValue { get; private set; }

      public int SourceTypeParameterCountForMatching { get; private set; }

      public bool IsMatch => this.QualityValue != 0 && this.MatchingTypeNamePartCount >= 0 && (this.MatchingTypeNamePartCount <= 1 || this.MatchingParameterCount == -1 || this.MatchingParameterCount >= this.SourceTypeParameterCountForMatching);

      public int CompareTo(MediaTypeUtils.MediaTypeMatchInfo other)
      {
        ExceptionUtils.CheckArgumentNotNull<MediaTypeUtils.MediaTypeMatchInfo>(other, nameof (other));
        if (this.MatchingTypeNamePartCount > other.MatchingTypeNamePartCount)
          return 1;
        if (this.MatchingTypeNamePartCount == other.MatchingTypeNamePartCount)
        {
          if (this.MatchingParameterCount > other.MatchingParameterCount)
            return 1;
          if (this.MatchingParameterCount == other.MatchingParameterCount)
          {
            int num = this.QualityValue.CompareTo(other.QualityValue);
            if (num != 0)
              return num;
            return other.TargetTypeIndex >= this.TargetTypeIndex ? 1 : -1;
          }
        }
        return -1;
      }

      private static int ParseQualityValue(string qualityValueText)
      {
        int qualityValue = 1000;
        if (qualityValueText.Length > 0)
        {
          int textIndex = 0;
          HttpUtils.ReadQualityValue(qualityValueText, ref textIndex, out qualityValue);
        }
        return qualityValue;
      }

      private static bool TryFindMediaTypeParameter(
        IList<KeyValuePair<string, string>> parameters,
        string parameterName,
        out string parameterValue)
      {
        parameterValue = (string) null;
        if (parameters != null)
        {
          for (int index = 0; index < parameters.Count; ++index)
          {
            KeyValuePair<string, string> parameter = parameters[index];
            string key = parameter.Key;
            if (HttpUtils.CompareMediaTypeParameterNames(parameterName, key))
            {
              ref string local = ref parameterValue;
              parameter = parameters[index];
              string str = parameter.Value;
              local = str;
              return true;
            }
          }
        }
        return false;
      }

      private static bool IsQualityValueParameter(string parameterName) => HttpUtils.CompareMediaTypeParameterNames("q", parameterName);

      private void MatchTypes(ODataMediaType sourceType, ODataMediaType targetType)
      {
        this.MatchingTypeNamePartCount = -1;
        if (sourceType.Type == "*")
          this.MatchingTypeNamePartCount = 0;
        else if (HttpUtils.CompareMediaTypeNames(sourceType.Type, targetType.Type))
        {
          if (sourceType.SubType == "*")
            this.MatchingTypeNamePartCount = 1;
          else if (HttpUtils.CompareMediaTypeNames(sourceType.SubType, targetType.SubType))
            this.MatchingTypeNamePartCount = 2;
        }
        this.QualityValue = 1000;
        this.SourceTypeParameterCountForMatching = 0;
        this.MatchingParameterCount = 0;
        IList<KeyValuePair<string, string>> list1 = sourceType.Parameters != null ? (IList<KeyValuePair<string, string>>) sourceType.Parameters.ToList<KeyValuePair<string, string>>() : (IList<KeyValuePair<string, string>>) null;
        IList<KeyValuePair<string, string>> list2 = targetType.Parameters != null ? (IList<KeyValuePair<string, string>>) targetType.Parameters.ToList<KeyValuePair<string, string>>() : (IList<KeyValuePair<string, string>>) null;
        bool flag1 = list2 != null && list2.Count > 0;
        bool flag2 = list1 != null && list1.Count > 0;
        if (flag2)
        {
          for (int index = 0; index < list1.Count; ++index)
          {
            string key = list1[index].Key;
            if (MediaTypeUtils.MediaTypeMatchInfo.IsQualityValueParameter(key))
            {
              this.QualityValue = MediaTypeUtils.MediaTypeMatchInfo.ParseQualityValue(list1[index].Value.Trim());
              break;
            }
            this.SourceTypeParameterCountForMatching = index + 1;
            string parameterValue;
            if (flag1 && MediaTypeUtils.MediaTypeMatchInfo.TryFindMediaTypeParameter(list2, key, out parameterValue) && string.Compare(list1[index].Value.Trim(), parameterValue.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
              ++this.MatchingParameterCount;
          }
        }
        if (flag2 && this.SourceTypeParameterCountForMatching != 0 && this.MatchingParameterCount != this.SourceTypeParameterCountForMatching)
          return;
        this.MatchingParameterCount = -1;
      }
    }

    private sealed class MatchInfoCacheKey : IEquatable<MediaTypeUtils.MatchInfoCacheKey>
    {
      public MatchInfoCacheKey(
        ODataMediaTypeResolver resolver,
        ODataPayloadKind payloadKind,
        string contentTypeName)
      {
        this.MediaTypeResolver = resolver;
        this.PayloadKind = payloadKind;
        this.ContentTypeName = contentTypeName;
      }

      private ODataMediaTypeResolver MediaTypeResolver { get; set; }

      private ODataPayloadKind PayloadKind { get; set; }

      private string ContentTypeName { get; set; }

      public override bool Equals(object obj) => this.Equals(obj as MediaTypeUtils.MatchInfoCacheKey);

      public bool Equals(MediaTypeUtils.MatchInfoCacheKey other)
      {
        if (other == null)
          return false;
        if (this == other)
          return true;
        return this.MediaTypeResolver == other.MediaTypeResolver && this.PayloadKind == other.PayloadKind && this.ContentTypeName == other.ContentTypeName;
      }

      public override int GetHashCode()
      {
        int num = this.MediaTypeResolver.GetHashCode() ^ this.PayloadKind.GetHashCode();
        return this.ContentTypeName == null ? num : num ^ this.ContentTypeName.GetHashCode();
      }
    }

    private sealed class MatchInfoConcurrentCache
    {
      private readonly ConcurrentDictionary<MediaTypeUtils.MatchInfoCacheKey, MediaTypeUtils.MediaTypeMatchInfo> dict;

      public MatchInfoConcurrentCache(int maxSize) => this.dict = new ConcurrentDictionary<MediaTypeUtils.MatchInfoCacheKey, MediaTypeUtils.MediaTypeMatchInfo>(4, maxSize);

      public bool TryGetValue(
        MediaTypeUtils.MatchInfoCacheKey key,
        out MediaTypeUtils.MediaTypeMatchInfo value)
      {
        return this.dict.TryGetValue(key, out value);
      }

      public void Add(MediaTypeUtils.MatchInfoCacheKey key, MediaTypeUtils.MediaTypeMatchInfo value)
      {
        try
        {
          this.dict.TryAdd(key, value);
        }
        catch (OverflowException ex)
        {
          this.dict.Clear();
          this.dict.TryAdd(key, value);
        }
      }
    }
  }
}
