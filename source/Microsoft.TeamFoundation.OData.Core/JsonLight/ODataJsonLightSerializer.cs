// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightSerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.OData.JsonLight
{
  internal class ODataJsonLightSerializer : ODataSerializer
  {
    protected readonly ODataContextUriBuilder ContextUriBuilder;
    private readonly ODataJsonLightOutputContext jsonLightOutputContext;
    private readonly SimpleLazy<JsonLightInstanceAnnotationWriter> instanceAnnotationWriter;
    private readonly SimpleLazy<JsonLightODataAnnotationWriter> odataAnnotationWriter;
    private bool allowRelativeUri;

    internal ODataJsonLightSerializer(
      ODataJsonLightOutputContext jsonLightOutputContext,
      bool initContextUriBuilder = false)
      : base((ODataOutputContext) jsonLightOutputContext)
    {
      ODataJsonLightSerializer jsonLightSerializer = this;
      this.jsonLightOutputContext = jsonLightOutputContext;
      this.instanceAnnotationWriter = new SimpleLazy<JsonLightInstanceAnnotationWriter>((Func<JsonLightInstanceAnnotationWriter>) (() => new JsonLightInstanceAnnotationWriter(new ODataJsonLightValueSerializer(jsonLightOutputContext), jsonLightOutputContext.TypeNameOracle)));
      this.odataAnnotationWriter = new SimpleLazy<JsonLightODataAnnotationWriter>((Func<JsonLightODataAnnotationWriter>) (() => new JsonLightODataAnnotationWriter(jsonLightOutputContext.JsonWriter, jsonLightSerializer.JsonLightOutputContext.OmitODataPrefix, jsonLightSerializer.MessageWriterSettings.Version)));
      if (!initContextUriBuilder)
        return;
      this.ContextUriBuilder = ODataContextUriBuilder.Create(this.jsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri, this.jsonLightOutputContext.WritingResponse && !(this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel));
    }

    internal ODataJsonLightOutputContext JsonLightOutputContext => this.jsonLightOutputContext;

    internal IJsonWriter JsonWriter => this.jsonLightOutputContext.JsonWriter;

    internal JsonLightInstanceAnnotationWriter InstanceAnnotationWriter => this.instanceAnnotationWriter.Value;

    internal JsonLightODataAnnotationWriter ODataAnnotationWriter => this.odataAnnotationWriter.Value;

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method for consistency with other formats.")]
    internal void WritePayloadStart() => ODataJsonWriterUtils.StartJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);

    [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "This method is an instance method for consistency with other formats.")]
    internal void WritePayloadEnd() => ODataJsonWriterUtils.EndJsonPaddingIfRequired(this.JsonWriter, this.MessageWriterSettings);

    internal ODataContextUrlInfo WriteContextUriProperty(
      ODataPayloadKind payloadKind,
      Func<ODataContextUrlInfo> contextUrlInfoGen = null,
      ODataContextUrlInfo parentContextUrlInfo = null,
      string propertyName = null)
    {
      if (this.jsonLightOutputContext.MetadataLevel is JsonNoMetadataLevel)
        return (ODataContextUrlInfo) null;
      ODataContextUrlInfo contextInfo = (ODataContextUrlInfo) null;
      if (contextUrlInfoGen != null)
        contextInfo = contextUrlInfoGen();
      if (contextInfo != null && contextInfo.IsHiddenBy(parentContextUrlInfo))
        return (ODataContextUrlInfo) null;
      Uri uri = this.ContextUriBuilder.BuildContextUri(payloadKind, contextInfo);
      if (!(uri != (Uri) null))
        return (ODataContextUrlInfo) null;
      if (string.IsNullOrEmpty(propertyName))
        this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.context");
      else
        this.ODataAnnotationWriter.WritePropertyAnnotationName(propertyName, "odata.context");
      this.JsonWriter.WritePrimitiveValue(uri.IsAbsoluteUri ? (object) uri.AbsoluteUri : (object) uri.OriginalString);
      this.allowRelativeUri = true;
      return contextInfo;
    }

    internal void WriteTopLevelPayload(Action payloadWriterAction)
    {
      this.WritePayloadStart();
      payloadWriterAction();
      this.WritePayloadEnd();
    }

    internal void WriteTopLevelError(ODataError error, bool includeDebugInformation) => this.WriteTopLevelPayload((Action) (() => ODataJsonWriterUtils.WriteError(this.JsonLightOutputContext.JsonWriter, new Action<IEnumerable<ODataInstanceAnnotation>>(this.InstanceAnnotationWriter.WriteInstanceAnnotationsForError), error, includeDebugInformation, this.MessageWriterSettings.MessageQuotas.MaxNestingDepth, true)));

    internal string UriToString(Uri uri)
    {
      Uri metadataDocumentUri = this.jsonLightOutputContext.MessageWriterSettings.MetadataDocumentUri;
      if (this.jsonLightOutputContext.PayloadUriConverter != null)
      {
        Uri uri1 = this.jsonLightOutputContext.PayloadUriConverter.ConvertPayloadUri(metadataDocumentUri, uri);
        if (uri1 != (Uri) null)
          return UriUtils.UriToString(uri1);
      }
      Uri uri2 = uri;
      if (!uri2.IsAbsoluteUri)
      {
        if (!this.allowRelativeUri)
          uri2 = !(metadataDocumentUri == (Uri) null) ? UriUtils.UriToAbsoluteUri(metadataDocumentUri, uri) : throw new ODataException(Strings.ODataJsonLightSerializer_RelativeUriUsedWithoutMetadataDocumentUriOrMetadata((object) UriUtils.UriToString(uri2)));
        else
          uri2 = UriUtils.EnsureEscapedRelativeUri(uri2);
      }
      return UriUtils.UriToString(uri2);
    }
  }
}
