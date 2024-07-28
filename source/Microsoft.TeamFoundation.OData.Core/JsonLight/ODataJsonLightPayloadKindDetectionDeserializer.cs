// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightPayloadKindDetectionDeserializer
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightPayloadKindDetectionDeserializer : 
    ODataJsonLightPropertyAndValueDeserializer
  {
    internal ODataJsonLightPayloadKindDetectionDeserializer(
      ODataJsonLightInputContext jsonLightInputContext)
      : base(jsonLightInputContext)
    {
    }

    internal IEnumerable<ODataPayloadKind> DetectPayloadKind(
      ODataPayloadKindDetectionInfo detectionInfo)
    {
      this.JsonReader.DisableInStreamErrorDetection = true;
      try
      {
        this.ReadPayloadStart(ODataPayloadKind.Unsupported, (PropertyAndAnnotationCollector) null, false, false);
        return this.DetectPayloadKindImplementation(detectionInfo);
      }
      catch (ODataException ex)
      {
        return Enumerable.Empty<ODataPayloadKind>();
      }
      finally
      {
        this.JsonReader.DisableInStreamErrorDetection = false;
      }
    }

    internal Task<IEnumerable<ODataPayloadKind>> DetectPayloadKindAsync(
      ODataPayloadKindDetectionInfo detectionInfo)
    {
      this.JsonReader.DisableInStreamErrorDetection = true;
      return this.ReadPayloadStartAsync(ODataPayloadKind.Unsupported, (PropertyAndAnnotationCollector) null, false, false).FollowOnSuccessWith<IEnumerable<ODataPayloadKind>>((Func<Task, IEnumerable<ODataPayloadKind>>) (t => this.DetectPayloadKindImplementation(detectionInfo))).FollowOnFaultAndCatchExceptionWith<IEnumerable<ODataPayloadKind>, ODataException>((Func<ODataException, IEnumerable<ODataPayloadKind>>) (t => Enumerable.Empty<ODataPayloadKind>())).FollowAlwaysWith<IEnumerable<ODataPayloadKind>>((Action<Task<IEnumerable<ODataPayloadKind>>>) (t => this.JsonReader.DisableInStreamErrorDetection = false));
    }

    private IEnumerable<ODataPayloadKind> DetectPayloadKindImplementation(
      ODataPayloadKindDetectionInfo detectionInfo)
    {
      if (this.ContextUriParseResult != null)
        return this.ContextUriParseResult.DetectedPayloadKinds;
      ODataError error = (ODataError) null;
      while (this.JsonReader.NodeType == JsonNodeType.Property)
      {
        string str = this.JsonReader.ReadPropertyName();
        if (ODataJsonLightDeserializer.TryParsePropertyAnnotation(str, out string _, out string _))
          return Enumerable.Empty<ODataPayloadKind>();
        if (ODataJsonLightReaderUtils.IsAnnotationProperty(str))
        {
          if (str != null && str.StartsWith("@odata.", StringComparison.Ordinal))
            return Enumerable.Empty<ODataPayloadKind>();
          this.JsonReader.SkipValue();
        }
        else
        {
          if (string.CompareOrdinal("error", str) != 0)
            return Enumerable.Empty<ODataPayloadKind>();
          if (error != null || !this.JsonReader.StartBufferingAndTryToReadInStreamErrorPropertyValue(out error))
            return Enumerable.Empty<ODataPayloadKind>();
          this.JsonReader.SkipValue();
        }
      }
      if (error == null)
        return Enumerable.Empty<ODataPayloadKind>();
      return (IEnumerable<ODataPayloadKind>) new ODataPayloadKind[1]
      {
        ODataPayloadKind.Error
      };
    }
  }
}
