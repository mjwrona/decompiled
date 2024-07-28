// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightBatchReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightBatchReader : ODataBatchReader
  {
    private readonly ODataJsonLightBatchReaderStream batchStream;
    private readonly ODataJsonLightBatchAtomicGroupCache atomicGroups = new ODataJsonLightBatchAtomicGroupCache();
    private static string PropertyNameRequests = "requests";
    private static string PropertyNameResponses = "responses";
    private ODataJsonLightBatchReader.ReaderMode mode;
    private ODataJsonLightBatchPayloadItemPropertiesCache messagePropertiesCache;

    internal ODataJsonLightBatchReader(ODataJsonLightInputContext inputContext, bool synchronous)
      : base((ODataInputContext) inputContext, synchronous)
    {
      this.batchStream = new ODataJsonLightBatchReaderStream(inputContext);
    }

    internal ODataJsonLightInputContext JsonLightInputContext => this.InputContext as ODataJsonLightInputContext;

    protected override string GetCurrentGroupIdImplementation()
    {
      string idImplementation = (string) null;
      if (this.messagePropertiesCache != null)
        idImplementation = (string) this.messagePropertiesCache.GetPropertyValue("ATOMICITYGROUP");
      return idImplementation;
    }

    protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation()
    {
      string propertyValue1 = (string) this.messagePropertiesCache.GetPropertyValue("ID");
      string propertyValue2 = (string) this.messagePropertiesCache.GetPropertyValue("ATOMICITYGROUP");
      IList<string> dependsOnRequestIds = (IList<string>) null;
      List<string> propertyValue3 = (List<string>) this.messagePropertiesCache.GetPropertyValue("DEPENDSON");
      if (propertyValue3 != null && propertyValue3.Count != 0)
      {
        this.ValidateDependsOnId((IEnumerable<string>) propertyValue3, propertyValue2, propertyValue1);
        dependsOnRequestIds = this.atomicGroups.GetFlattenedMessageIds((IList<string>) propertyValue3);
      }
      ODataBatchOperationHeaders propertyValue4 = (ODataBatchOperationHeaders) this.messagePropertiesCache.GetPropertyValue("HEADERS");
      if (propertyValue2 != null)
        propertyValue4.Add("ATOMICITYGROUP", propertyValue2);
      Stream bodyContentStream = (Stream) this.messagePropertiesCache.GetPropertyValue("BODY") ?? (Stream) new ODataJsonLightBatchBodyContentReaderStream((IODataStreamListener) this);
      string propertyValue5 = (string) this.messagePropertiesCache.GetPropertyValue("METHOD");
      ODataJsonLightBatchReader.ValidateRequiredProperty(propertyValue5, "METHOD");
      string upperInvariant = propertyValue5.ToUpperInvariant();
      string str = (string) this.messagePropertiesCache.GetPropertyValue("URL");
      ODataJsonLightBatchReader.ValidateRequiredProperty(str, "URL");
      int num1 = str.IndexOf('?');
      int num2 = str.IndexOf(':');
      if (num1 > 0 && num2 > 0 && num1 < num2)
        str = str.Substring(0, num1) + str.Substring(num1).Replace(":", "%3A");
      Uri requestUri = new Uri(str, UriKind.RelativeOrAbsolute);
      this.messagePropertiesCache = (ODataJsonLightBatchPayloadItemPropertiesCache) null;
      return this.BuildOperationRequestMessage((Func<Stream>) (() => bodyContentStream), upperInvariant, requestUri, propertyValue4, propertyValue1, propertyValue2, (IEnumerable<string>) dependsOnRequestIds, true);
    }

    protected override ODataBatchReaderState ReadAtStartImplementation()
    {
      if (this.mode == ODataJsonLightBatchReader.ReaderMode.NotDetected)
      {
        this.DetectReaderMode();
        return ODataBatchReaderState.Initial;
      }
      int num = (int) this.StartReadingBatchArray();
      this.messagePropertiesCache = new ODataJsonLightBatchPayloadItemPropertiesCache(this);
      string propertyValue = (string) this.messagePropertiesCache.GetPropertyValue("ATOMICITYGROUP");
      if (propertyValue == null)
        return ODataBatchReaderState.Operation;
      this.HandleNewAtomicGroupStart((string) this.messagePropertiesCache.GetPropertyValue("ID"), propertyValue);
      return ODataBatchReaderState.ChangesetStart;
    }

    protected override ODataBatchReaderState ReadAtChangesetStartImplementation() => ODataBatchReaderState.Operation;

    protected override ODataBatchReaderState ReadAtChangesetEndImplementation() => this.messagePropertiesCache == null && this.JsonLightInputContext.JsonReader.NodeType != JsonNodeType.StartObject ? ODataBatchReaderState.Completed : this.DetectChangesetStates(this.messagePropertiesCache);

    protected override ODataBatchReaderState ReadAtOperationImplementation()
    {
      if (this.JsonLightInputContext.JsonReader.NodeType != JsonNodeType.StartObject)
        return this.HandleMessagesEnd();
      if (this.messagePropertiesCache == null)
        this.messagePropertiesCache = new ODataJsonLightBatchPayloadItemPropertiesCache(this);
      return this.DetectChangesetStates(this.messagePropertiesCache);
    }

    protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation()
    {
      Stream bodyContentStream = (Stream) this.messagePropertiesCache.GetPropertyValue("BODY") ?? (Stream) new ODataJsonLightBatchBodyContentReaderStream((IODataStreamListener) this);
      int propertyValue1 = (int) this.messagePropertiesCache.GetPropertyValue("STATUS");
      string propertyValue2 = (string) this.messagePropertiesCache.GetPropertyValue("ID");
      string propertyValue3 = (string) this.messagePropertiesCache.GetPropertyValue("ATOMICITYGROUP");
      ODataBatchOperationHeaders propertyValue4 = (ODataBatchOperationHeaders) this.messagePropertiesCache.GetPropertyValue("HEADERS");
      this.messagePropertiesCache = (ODataJsonLightBatchPayloadItemPropertiesCache) null;
      return this.BuildOperationResponseMessage((Func<Stream>) (() => bodyContentStream), propertyValue1, propertyValue4, propertyValue2, propertyValue3);
    }

    private static void ValidateRequiredProperty(string propertyValue, string propertyName)
    {
      if (propertyValue == null)
        throw new ODataException(Strings.ODataBatchReader_RequestPropertyMissing((object) propertyName));
    }

    private void ValidateDependsOnId(
      IEnumerable<string> dependsOnIds,
      string atomicityGroupId,
      string requestId)
    {
      foreach (string dependsOnId in dependsOnIds)
      {
        if (dependsOnId.Equals(atomicityGroupId))
          throw new ODataException(Strings.ODataBatchReader_SameRequestIdAsAtomicityGroupIdNotAllowed((object) dependsOnId, (object) atomicityGroupId));
        string p1 = !dependsOnId.Equals(requestId) ? this.atomicGroups.GetGroupId(dependsOnId) : throw new ODataException(Strings.ODataBatchReader_SelfReferenceDependsOnRequestIdNotAllowed((object) dependsOnId, (object) requestId));
        if (p1 != null && !p1.Equals(this.atomicGroups.GetGroupId(requestId)))
          throw new ODataException(Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed((object) dependsOnId, (object) p1));
      }
    }

    private void DetectReaderMode()
    {
      int num = (int) this.batchStream.JsonReader.ReadNext();
      this.batchStream.JsonReader.ReadStartObject();
      string str = this.batchStream.JsonReader.ReadPropertyName();
      if (ODataJsonLightBatchReader.PropertyNameRequests.Equals(str, StringComparison.OrdinalIgnoreCase))
      {
        this.mode = ODataJsonLightBatchReader.ReaderMode.Requests;
      }
      else
      {
        if (!ODataJsonLightBatchReader.PropertyNameResponses.Equals(str, StringComparison.OrdinalIgnoreCase))
          throw new ODataException(Strings.ODataBatchReader_JsonBatchTopLevelPropertyMissing);
        this.mode = ODataJsonLightBatchReader.ReaderMode.Responses;
      }
    }

    private ODataBatchReaderState StartReadingBatchArray()
    {
      this.batchStream.JsonReader.ReadStartArray();
      return ODataBatchReaderState.Operation;
    }

    private void HandleNewAtomicGroupStart(string messageId, string groupId)
    {
      if (this.atomicGroups.IsGroupId(groupId))
        throw new ODataException(Strings.ODataBatchReader_DuplicateAtomicityGroupIDsNotAllowed((object) groupId));
      this.atomicGroups.AddMessageIdAndGroupId(messageId, groupId);
    }

    private ODataBatchReaderState HandleMessagesEnd()
    {
      ODataBatchReaderState batchReaderState;
      if (this.atomicGroups.IsWithinAtomicGroup)
      {
        this.atomicGroups.IsWithinAtomicGroup = false;
        batchReaderState = ODataBatchReaderState.ChangesetEnd;
      }
      else
      {
        this.JsonLightInputContext.JsonReader.ReadEndArray();
        this.JsonLightInputContext.JsonReader.ReadEndObject();
        batchReaderState = ODataBatchReaderState.Completed;
      }
      return batchReaderState;
    }

    private ODataBatchReaderState DetectChangesetStates(
      ODataJsonLightBatchPayloadItemPropertiesCache messagePropertiesCache)
    {
      string propertyValue1 = (string) messagePropertiesCache.GetPropertyValue("ID");
      string propertyValue2 = (string) messagePropertiesCache.GetPropertyValue("ATOMICITYGROUP");
      bool flag1 = this.atomicGroups.IsChangesetEnd(propertyValue2);
      bool flag2 = false;
      if (!flag1 && propertyValue2 != null)
        flag2 = this.atomicGroups.AddMessageIdAndGroupId(propertyValue1, propertyValue2);
      ODataBatchReaderState batchReaderState = ODataBatchReaderState.Operation;
      if (flag1)
        batchReaderState = ODataBatchReaderState.ChangesetEnd;
      else if (flag2)
        batchReaderState = ODataBatchReaderState.ChangesetStart;
      return batchReaderState;
    }

    private enum ReaderMode
    {
      NotDetected,
      Requests,
      Responses,
    }
  }
}
