// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightCollectionSerializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using Microsoft.OData.Json;
using System;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightCollectionSerializer : ODataJsonLightValueSerializer
  {
    private readonly bool writingTopLevelCollection;

    internal ODataJsonLightCollectionSerializer(
      ODataJsonLightOutputContext jsonLightOutputContext,
      bool writingTopLevelCollection)
      : base(jsonLightOutputContext, true)
    {
      this.writingTopLevelCollection = writingTopLevelCollection;
    }

    internal void WriteCollectionStart(
      ODataCollectionStart collectionStart,
      IEdmTypeReference itemTypeReference)
    {
      if (this.writingTopLevelCollection)
      {
        this.JsonWriter.StartObjectScope();
        this.WriteContextUriProperty(ODataPayloadKind.Collection, (Func<ODataContextUrlInfo>) (() => ODataContextUrlInfo.Create(collectionStart.SerializationInfo, itemTypeReference)));
        long? count = collectionStart.Count;
        if (count.HasValue)
        {
          this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.count");
          IJsonWriter jsonWriter = this.JsonWriter;
          count = collectionStart.Count;
          long num = count.Value;
          jsonWriter.WriteValue(num);
        }
        if (collectionStart.NextPageLink != (Uri) null)
        {
          this.ODataAnnotationWriter.WriteInstanceAnnotationName("odata.nextLink");
          this.JsonWriter.WriteValue(this.UriToString(collectionStart.NextPageLink));
        }
        this.JsonWriter.WriteValuePropertyName();
      }
      this.JsonWriter.StartArrayScope();
    }

    internal void WriteCollectionEnd()
    {
      this.JsonWriter.EndArrayScope();
      if (!this.writingTopLevelCollection)
        return;
      this.JsonWriter.EndObjectScope();
    }
  }
}
