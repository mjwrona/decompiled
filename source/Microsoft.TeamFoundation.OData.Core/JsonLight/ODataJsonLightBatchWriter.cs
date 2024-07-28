// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightBatchWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightBatchWriter : ODataBatchWriter
  {
    private const string PropertyId = "id";
    private const string PropertyAtomicityGroup = "atomicityGroup";
    private const string PropertyHeaders = "headers";
    private const string PropertyBody = "body";
    private const string PropertyRequests = "requests";
    private const string PropertyDependsOn = "dependsOn";
    private const string PropertyMethod = "method";
    private const string PropertyUrl = "url";
    private const string PropertyResponses = "responses";
    private const string PropertyStatus = "status";
    private readonly IJsonWriter jsonWriter;
    private string atomicityGroupId;
    private Dictionary<string, string> requestIdToAtomicGroupId = new Dictionary<string, string>();
    private Dictionary<string, IList<string>> atomicityGroupIdToRequestId = new Dictionary<string, IList<string>>();

    internal ODataJsonLightBatchWriter(ODataJsonLightOutputContext jsonLightOutputContext)
      : base((ODataOutputContext) jsonLightOutputContext)
    {
      this.jsonWriter = this.JsonLightOutputContext.JsonWriter;
    }

    private ODataJsonLightOutputContext JsonLightOutputContext => this.OutputContext as ODataJsonLightOutputContext;

    private ODataBatchOperationMessage CurrentOperationMessage
    {
      get
      {
        if (this.CurrentOperationRequestMessage != null)
          return this.CurrentOperationRequestMessage.OperationMessage;
        return this.CurrentOperationResponseMessage != null ? this.CurrentOperationResponseMessage.OperationMessage : (ODataBatchOperationMessage) null;
      }
    }

    public override void StreamRequested()
    {
      this.StartBatchOperationContent();
      this.JsonLightOutputContext.FlushBuffers();
      this.SetState(ODataBatchWriter.BatchWriterState.OperationStreamRequested);
    }

    public override Task StreamRequestedAsync()
    {
      this.StartBatchOperationContent();
      return this.JsonLightOutputContext.FlushBuffersAsync().FollowOnSuccessWith((Action<Task>) (task => this.SetState(ODataBatchWriter.BatchWriterState.OperationStreamRequested)));
    }

    public override void StreamDisposed()
    {
      this.SetState(ODataBatchWriter.BatchWriterState.OperationStreamDisposed);
      this.CurrentOperationRequestMessage = (ODataBatchOperationRequestMessage) null;
      this.CurrentOperationResponseMessage = (ODataBatchOperationResponseMessage) null;
      this.EnsurePrecedingMessageIsClosed();
    }

    public override void OnInStreamError()
    {
      this.JsonLightOutputContext.VerifyNotDisposed();
      this.SetState(ODataBatchWriter.BatchWriterState.Error);
      this.JsonLightOutputContext.JsonWriter.Flush();
      throw new ODataException(Strings.ODataBatchWriter_CannotWriteInStreamErrorForBatch);
    }

    protected override void FlushSynchronously() => this.JsonLightOutputContext.Flush();

    protected override Task FlushAsynchronously() => this.JsonLightOutputContext.FlushAsync();

    protected override void WriteStartBatchImplementation()
    {
      this.WriteBatchEnvelope();
      this.SetState(ODataBatchWriter.BatchWriterState.BatchStarted);
    }

    protected override IEnumerable<string> GetDependsOnRequestIds(IEnumerable<string> dependsOnIds)
    {
      List<string> dependsOnRequestIds = new List<string>();
      foreach (string dependsOnId in dependsOnIds)
      {
        if (this.atomicityGroupIdToRequestId.ContainsKey(dependsOnId))
          dependsOnRequestIds.AddRange((IEnumerable<string>) this.atomicityGroupIdToRequestId[dependsOnId]);
        else
          dependsOnRequestIds.Add(dependsOnId);
      }
      return (IEnumerable<string>) dependsOnRequestIds;
    }

    protected override void WriteEndBatchImplementation()
    {
      this.WritePendingMessageData(true);
      this.SetState(ODataBatchWriter.BatchWriterState.BatchCompleted);
      this.jsonWriter.EndArrayScope();
      this.jsonWriter.EndObjectScope();
    }

    protected override void WriteStartChangesetImplementation(string groupId)
    {
      this.WritePendingMessageData(true);
      this.SetState(ODataBatchWriter.BatchWriterState.ChangesetStarted);
      this.atomicityGroupId = groupId;
    }

    protected override void WriteEndChangesetImplementation()
    {
      this.WritePendingMessageData(true);
      this.SetState(ODataBatchWriter.BatchWriterState.ChangesetCompleted);
      this.atomicityGroupId = (string) null;
    }

    protected override ODataBatchOperationRequestMessage CreateOperationRequestMessageImplementation(
      string method,
      Uri uri,
      string contentId,
      BatchPayloadUriOption payloadUriOption,
      IEnumerable<string> dependsOnIds)
    {
      this.WritePendingMessageData(true);
      if (contentId == null)
        contentId = Guid.NewGuid().ToString();
      this.AddGroupIdLookup(contentId);
      this.CurrentOperationRequestMessage = this.BuildOperationRequestMessage(this.JsonLightOutputContext.GetOutputStream(), method, uri, contentId, this.atomicityGroupId, dependsOnIds ?? Enumerable.Empty<string>());
      this.SetState(ODataBatchWriter.BatchWriterState.OperationCreated);
      this.WriteStartBoundaryForOperation();
      this.jsonWriter.WriteName("id");
      this.jsonWriter.WriteValue(contentId);
      if (this.atomicityGroupId != null)
      {
        this.jsonWriter.WriteName("atomicityGroup");
        this.jsonWriter.WriteValue(this.atomicityGroupId);
      }
      if (this.CurrentOperationRequestMessage.DependsOnIds != null && this.CurrentOperationRequestMessage.DependsOnIds.Any<string>())
      {
        this.jsonWriter.WriteName("dependsOn");
        this.jsonWriter.StartArrayScope();
        foreach (string dependsOnId in this.CurrentOperationRequestMessage.DependsOnIds)
        {
          this.ValidateDependsOnId(contentId, dependsOnId);
          this.jsonWriter.WriteValue(dependsOnId);
        }
        this.jsonWriter.EndArrayScope();
      }
      this.jsonWriter.WriteName(nameof (method));
      this.jsonWriter.WriteValue(method);
      this.jsonWriter.WriteName("url");
      this.jsonWriter.WriteValue(UriUtils.UriToString(uri));
      return this.CurrentOperationRequestMessage;
    }

    protected override ODataBatchOperationResponseMessage CreateOperationResponseMessageImplementation(
      string contentId)
    {
      this.WritePendingMessageData(true);
      this.CurrentOperationResponseMessage = this.BuildOperationResponseMessage(this.JsonLightOutputContext.GetOutputStream(), contentId, this.atomicityGroupId);
      this.SetState(ODataBatchWriter.BatchWriterState.OperationCreated);
      this.WriteStartBoundaryForOperation();
      return this.CurrentOperationResponseMessage;
    }

    protected override void VerifyNotDisposed() => this.JsonLightOutputContext.VerifyNotDisposed();

    private void ValidateDependsOnId(string requestId, string dependsOnId)
    {
      if (this.atomicityGroupIdToRequestId.ContainsKey(dependsOnId))
      {
        string str;
        this.requestIdToAtomicGroupId.TryGetValue(requestId, out str);
        if (dependsOnId.Equals(str))
          throw new ODataException(Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed((object) requestId, (object) dependsOnId));
      }
      else
      {
        string p1 = (string) null;
        if (!this.requestIdToAtomicGroupId.TryGetValue(dependsOnId, out p1))
          throw new ODataException(Strings.ODataBatchReader_DependsOnIdNotFound((object) dependsOnId, (object) requestId));
        if (p1 == null)
          return;
        string str;
        this.requestIdToAtomicGroupId.TryGetValue(requestId, out str);
        if (!p1.Equals(str))
          throw new ODataException(Strings.ODataBatchReader_DependsOnRequestIdIsPartOfAtomicityGroupNotAllowed((object) requestId, (object) p1));
      }
    }

    private void AddGroupIdLookup(string contentId)
    {
      try
      {
        this.requestIdToAtomicGroupId.Add(contentId, this.atomicityGroupId);
      }
      catch (ArgumentException ex)
      {
        throw new ODataException(Strings.ODataBatchWriter_DuplicateContentIDsNotAllowed((object) contentId), (Exception) ex);
      }
      if (this.atomicityGroupId == null)
        return;
      if (!this.atomicityGroupIdToRequestId.ContainsKey(this.atomicityGroupId))
        this.atomicityGroupIdToRequestId.Add(this.atomicityGroupId, (IList<string>) new List<string>());
      this.atomicityGroupIdToRequestId[this.atomicityGroupId].Add(contentId);
    }

    private void WriteStartBoundaryForOperation() => this.jsonWriter.StartObjectScope();

    private void StartBatchOperationContent()
    {
      this.WritePendingMessageData(false);
      this.jsonWriter.WriteRawValue(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} \"{1}\" {2}", new object[3]
      {
        (object) ",",
        (object) "body",
        (object) ":"
      }));
      this.JsonLightOutputContext.JsonWriter.Flush();
    }

    private void WritePendingMessageData(bool reportMessageCompleted)
    {
      if (this.CurrentOperationMessage == null)
        return;
      if (this.CurrentOperationRequestMessage != null)
        this.WritePendingRequestMessageData();
      else
        this.WritePendingResponseMessageData();
      if (!reportMessageCompleted)
        return;
      this.CurrentOperationMessage.PartHeaderProcessingCompleted();
      this.CurrentOperationRequestMessage = (ODataBatchOperationRequestMessage) null;
      this.CurrentOperationResponseMessage = (ODataBatchOperationResponseMessage) null;
      this.EnsurePrecedingMessageIsClosed();
    }

    private void EnsurePrecedingMessageIsClosed() => this.jsonWriter.EndObjectScope();

    private void WriteBatchEnvelope()
    {
      this.jsonWriter.StartObjectScope();
      this.jsonWriter.WriteName(this.JsonLightOutputContext.WritingResponse ? "responses" : "requests");
      this.jsonWriter.StartArrayScope();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "need to use lower characters for header key")]
    private void WritePendingRequestMessageData()
    {
      this.jsonWriter.WriteName("headers");
      this.jsonWriter.StartObjectScope();
      IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationRequestMessage.Headers;
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in headers)
        {
          this.jsonWriter.WriteName(keyValuePair.Key.ToLowerInvariant());
          this.jsonWriter.WriteValue(keyValuePair.Value);
        }
      }
      this.jsonWriter.EndObjectScope();
    }

    [SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Justification = "need to use lower characters for header key")]
    private void WritePendingResponseMessageData()
    {
      this.jsonWriter.WriteName("id");
      this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.ContentId);
      if (this.atomicityGroupId != null)
      {
        this.jsonWriter.WriteName("atomicityGroup");
        this.jsonWriter.WriteValue(this.atomicityGroupId);
      }
      this.jsonWriter.WriteName("status");
      this.jsonWriter.WriteValue(this.CurrentOperationResponseMessage.StatusCode);
      this.jsonWriter.WriteName("headers");
      this.jsonWriter.StartObjectScope();
      IEnumerable<KeyValuePair<string, string>> headers = this.CurrentOperationMessage.Headers;
      if (headers != null)
      {
        foreach (KeyValuePair<string, string> keyValuePair in headers)
        {
          this.jsonWriter.WriteName(keyValuePair.Key.ToLowerInvariant());
          this.jsonWriter.WriteValue(keyValuePair.Value);
        }
      }
      this.jsonWriter.EndObjectScope();
    }
  }
}
