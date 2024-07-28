// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.DocumentClientEventSource
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Documents;
using System;
using System.Diagnostics.Tracing;
using System.Net.Http.Headers;

namespace Microsoft.Azure.Cosmos
{
  [EventSource(Name = "DocumentDBClient", Guid = "f832a342-0a53-5bab-b57b-d5bc65319768")]
  internal class DocumentClientEventSource : EventSource, ICommunicationEventSource
  {
    private static Lazy<DocumentClientEventSource> documentClientEventSourceInstance = new Lazy<DocumentClientEventSource>((Func<DocumentClientEventSource>) (() => new DocumentClientEventSource()));

    public static DocumentClientEventSource Instance => DocumentClientEventSource.documentClientEventSourceInstance.Value;

    internal DocumentClientEventSource()
    {
    }

    [NonEvent]
    private unsafe void WriteEventCoreWithActivityId(
      Guid activityId,
      int eventId,
      int eventDataCount,
      EventSource.EventData* dataDesc)
    {
      CustomTypeExtensions.SetActivityId(ref activityId);
      this.WriteEventCore(eventId, eventDataCount, dataDesc);
    }

    [Event(1, Message = "HttpRequest to URI '{2}' with resourceType '{3}' and request headers: accept '{4}', authorization '{5}', consistencyLevel '{6}', contentType '{7}', contentEncoding '{8}', contentLength '{9}', contentLocation '{10}', continuation '{11}', emitVerboseTracesInQuery '{12}', enableScanInQuery '{13}', eTag '{14}', httpDate '{15}', ifMatch '{16}', ifNoneMatch '{17}', indexingDirective '{18}', keepAlive '{19}', offerType '{20}', pageSize '{21}', preTriggerExclude '{22}', preTriggerInclude '{23}', postTriggerExclude '{24}', postTriggerInclude '{25}', profileRequest '{26}', resourceTokenExpiry '{27}', sessionToken '{28}', setCookie '{29}', slug '{30}', userAgent '{31}', xDate'{32}'. ActivityId {0}, localId {1}", Keywords = (EventKeywords) 1, Level = EventLevel.Verbose)]
    private unsafe void Request(
      Guid activityId,
      Guid localId,
      string uri,
      string resourceType,
      string accept,
      string authorization,
      string consistencyLevel,
      string contentType,
      string contentEncoding,
      string contentLength,
      string contentLocation,
      string continuation,
      string emitVerboseTracesInQuery,
      string enableScanInQuery,
      string eTag,
      string httpDate,
      string ifMatch,
      string ifNoneMatch,
      string indexingDirective,
      string keepAlive,
      string offerType,
      string pageSize,
      string preTriggerExclude,
      string preTriggerInclude,
      string postTriggerExclude,
      string postTriggerInclude,
      string profileRequest,
      string resourceTokenExpiry,
      string sessionToken,
      string setCookie,
      string slug,
      string userAgent,
      string xDate)
    {
      if (uri == null)
        throw new ArgumentException(nameof (uri));
      if (resourceType == null)
        throw new ArgumentException(nameof (resourceType));
      if (accept == null)
        throw new ArgumentException(nameof (accept));
      if (authorization == null)
        throw new ArgumentException(nameof (authorization));
      if (consistencyLevel == null)
        throw new ArgumentException(nameof (consistencyLevel));
      if (contentType == null)
        throw new ArgumentException(nameof (contentType));
      if (contentEncoding == null)
        throw new ArgumentException(nameof (contentEncoding));
      if (contentLength == null)
        throw new ArgumentException(nameof (contentLength));
      if (contentLocation == null)
        throw new ArgumentException(nameof (contentLocation));
      if (continuation == null)
        throw new ArgumentException(nameof (continuation));
      if (emitVerboseTracesInQuery == null)
        throw new ArgumentException(nameof (emitVerboseTracesInQuery));
      if (enableScanInQuery == null)
        throw new ArgumentException(nameof (enableScanInQuery));
      if (eTag == null)
        throw new ArgumentException(nameof (eTag));
      if (httpDate == null)
        throw new ArgumentException(nameof (httpDate));
      if (ifMatch == null)
        throw new ArgumentException(nameof (ifMatch));
      if (ifNoneMatch == null)
        throw new ArgumentException(nameof (ifNoneMatch));
      if (indexingDirective == null)
        throw new ArgumentException(nameof (indexingDirective));
      if (keepAlive == null)
        throw new ArgumentException(nameof (keepAlive));
      if (offerType == null)
        throw new ArgumentException(nameof (offerType));
      if (pageSize == null)
        throw new ArgumentException(nameof (pageSize));
      if (preTriggerExclude == null)
        throw new ArgumentException(nameof (preTriggerExclude));
      if (preTriggerInclude == null)
        throw new ArgumentException(nameof (preTriggerInclude));
      if (postTriggerExclude == null)
        throw new ArgumentException(nameof (postTriggerExclude));
      if (postTriggerInclude == null)
        throw new ArgumentException(nameof (postTriggerInclude));
      if (profileRequest == null)
        throw new ArgumentException(nameof (profileRequest));
      if (resourceTokenExpiry == null)
        throw new ArgumentException(nameof (resourceTokenExpiry));
      if (sessionToken == null)
        throw new ArgumentException(nameof (sessionToken));
      if (setCookie == null)
        throw new ArgumentException(nameof (setCookie));
      if (slug == null)
        throw new ArgumentException(nameof (slug));
      if (userAgent == null)
        throw new ArgumentException(nameof (userAgent));
      if (xDate == null)
        throw new ArgumentException(nameof (xDate));
      byte[] byteArray1 = activityId.ToByteArray();
      byte[] byteArray2 = localId.ToByteArray();
      fixed (byte* numPtr1 = byteArray1)
        fixed (byte* numPtr2 = byteArray2)
          fixed (char* chPtr1 = uri)
            fixed (char* chPtr2 = resourceType)
              fixed (char* chPtr3 = accept)
                fixed (char* chPtr4 = authorization)
                  fixed (char* chPtr5 = consistencyLevel)
                    fixed (char* chPtr6 = contentType)
                      fixed (char* chPtr7 = contentEncoding)
                        fixed (char* chPtr8 = contentLength)
                          fixed (char* chPtr9 = contentLocation)
                            fixed (char* chPtr10 = continuation)
                              fixed (char* chPtr11 = emitVerboseTracesInQuery)
                                fixed (char* chPtr12 = enableScanInQuery)
                                  fixed (char* chPtr13 = eTag)
                                    fixed (char* chPtr14 = httpDate)
                                      fixed (char* chPtr15 = ifMatch)
                                        fixed (char* chPtr16 = ifNoneMatch)
                                          fixed (char* chPtr17 = indexingDirective)
                                            fixed (char* chPtr18 = keepAlive)
                                              fixed (char* chPtr19 = offerType)
                                                fixed (char* chPtr20 = pageSize)
                                                  fixed (char* chPtr21 = preTriggerExclude)
                                                    fixed (char* chPtr22 = preTriggerInclude)
                                                      fixed (char* chPtr23 = postTriggerExclude)
                                                        fixed (char* chPtr24 = postTriggerInclude)
                                                          fixed (char* chPtr25 = profileRequest)
                                                            fixed (char* chPtr26 = resourceTokenExpiry)
                                                              fixed (char* chPtr27 = sessionToken)
                                                                fixed (char* chPtr28 = setCookie)
                                                                  fixed (char* chPtr29 = slug)
                                                                    fixed (char* chPtr30 = userAgent)
                                                                      fixed (char* chPtr31 = xDate)
                                                                      {
                                                                        EventSource.EventData* dataDesc = stackalloc EventSource.EventData[33];
                                                                        dataDesc->DataPointer = (IntPtr) (void*) numPtr1;
                                                                        dataDesc->Size = byteArray1.Length;
                                                                        dataDesc[1].DataPointer = (IntPtr) (void*) numPtr2;
                                                                        dataDesc[1].Size = byteArray2.Length;
                                                                        dataDesc[2].DataPointer = (IntPtr) (void*) chPtr1;
                                                                        dataDesc[2].Size = (uri.Length + 1) * 2;
                                                                        dataDesc[3].DataPointer = (IntPtr) (void*) chPtr2;
                                                                        dataDesc[3].Size = (resourceType.Length + 1) * 2;
                                                                        dataDesc[4].DataPointer = (IntPtr) (void*) chPtr3;
                                                                        dataDesc[4].Size = (accept.Length + 1) * 2;
                                                                        dataDesc[5].DataPointer = (IntPtr) (void*) chPtr4;
                                                                        dataDesc[5].Size = (authorization.Length + 1) * 2;
                                                                        dataDesc[6].DataPointer = (IntPtr) (void*) chPtr5;
                                                                        dataDesc[6].Size = (consistencyLevel.Length + 1) * 2;
                                                                        dataDesc[7].DataPointer = (IntPtr) (void*) chPtr6;
                                                                        dataDesc[7].Size = (contentType.Length + 1) * 2;
                                                                        dataDesc[8].DataPointer = (IntPtr) (void*) chPtr7;
                                                                        dataDesc[8].Size = (contentEncoding.Length + 1) * 2;
                                                                        dataDesc[9].DataPointer = (IntPtr) (void*) chPtr8;
                                                                        dataDesc[9].Size = (contentLength.Length + 1) * 2;
                                                                        dataDesc[10].DataPointer = (IntPtr) (void*) chPtr9;
                                                                        dataDesc[10].Size = (contentLocation.Length + 1) * 2;
                                                                        dataDesc[11].DataPointer = (IntPtr) (void*) chPtr10;
                                                                        dataDesc[11].Size = (continuation.Length + 1) * 2;
                                                                        dataDesc[12].DataPointer = (IntPtr) (void*) chPtr11;
                                                                        dataDesc[12].Size = (emitVerboseTracesInQuery.Length + 1) * 2;
                                                                        dataDesc[13].DataPointer = (IntPtr) (void*) chPtr12;
                                                                        dataDesc[13].Size = (enableScanInQuery.Length + 1) * 2;
                                                                        dataDesc[14].DataPointer = (IntPtr) (void*) chPtr13;
                                                                        dataDesc[14].Size = (eTag.Length + 1) * 2;
                                                                        dataDesc[15].DataPointer = (IntPtr) (void*) chPtr14;
                                                                        dataDesc[15].Size = (httpDate.Length + 1) * 2;
                                                                        dataDesc[16].DataPointer = (IntPtr) (void*) chPtr15;
                                                                        dataDesc[16].Size = (ifMatch.Length + 1) * 2;
                                                                        dataDesc[17].DataPointer = (IntPtr) (void*) chPtr16;
                                                                        dataDesc[17].Size = (ifNoneMatch.Length + 1) * 2;
                                                                        dataDesc[18].DataPointer = (IntPtr) (void*) chPtr17;
                                                                        dataDesc[18].Size = (indexingDirective.Length + 1) * 2;
                                                                        dataDesc[19].DataPointer = (IntPtr) (void*) chPtr18;
                                                                        dataDesc[19].Size = (keepAlive.Length + 1) * 2;
                                                                        dataDesc[20].DataPointer = (IntPtr) (void*) chPtr19;
                                                                        dataDesc[20].Size = (offerType.Length + 1) * 2;
                                                                        dataDesc[21].DataPointer = (IntPtr) (void*) chPtr20;
                                                                        dataDesc[21].Size = (pageSize.Length + 1) * 2;
                                                                        dataDesc[22].DataPointer = (IntPtr) (void*) chPtr21;
                                                                        dataDesc[22].Size = (preTriggerExclude.Length + 1) * 2;
                                                                        dataDesc[23].DataPointer = (IntPtr) (void*) chPtr22;
                                                                        dataDesc[23].Size = (preTriggerInclude.Length + 1) * 2;
                                                                        dataDesc[24].DataPointer = (IntPtr) (void*) chPtr23;
                                                                        dataDesc[24].Size = (postTriggerExclude.Length + 1) * 2;
                                                                        dataDesc[25].DataPointer = (IntPtr) (void*) chPtr24;
                                                                        dataDesc[25].Size = (postTriggerInclude.Length + 1) * 2;
                                                                        dataDesc[26].DataPointer = (IntPtr) (void*) chPtr25;
                                                                        dataDesc[26].Size = (profileRequest.Length + 1) * 2;
                                                                        dataDesc[27].DataPointer = (IntPtr) (void*) chPtr26;
                                                                        dataDesc[27].Size = (resourceTokenExpiry.Length + 1) * 2;
                                                                        dataDesc[28].DataPointer = (IntPtr) (void*) chPtr27;
                                                                        dataDesc[28].Size = (sessionToken.Length + 1) * 2;
                                                                        dataDesc[29].DataPointer = (IntPtr) (void*) chPtr28;
                                                                        dataDesc[29].Size = (setCookie.Length + 1) * 2;
                                                                        dataDesc[30].DataPointer = (IntPtr) (void*) chPtr29;
                                                                        dataDesc[30].Size = (slug.Length + 1) * 2;
                                                                        dataDesc[31].DataPointer = (IntPtr) (void*) chPtr30;
                                                                        dataDesc[31].Size = (userAgent.Length + 1) * 2;
                                                                        dataDesc[32].DataPointer = (IntPtr) (void*) chPtr31;
                                                                        dataDesc[32].Size = (xDate.Length + 1) * 2;
                                                                        this.WriteEventCoreWithActivityId(activityId, 1, 33, dataDesc);
                                                                      }
    }

    [NonEvent]
    public void Request(
      Guid activityId,
      Guid localId,
      string uri,
      string resourceType,
      HttpRequestHeaders requestHeaders)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      string[] keys = new string[29]
      {
        "Accept",
        "authorization",
        "x-ms-consistency-level",
        "Content-Type",
        "Content-Encoding",
        "Content-Length",
        "Content-Location",
        "x-ms-continuation",
        "x-ms-documentdb-query-emit-traces",
        "x-ms-documentdb-query-enable-scan",
        "etag",
        "date",
        "If-Match",
        "If-None-Match",
        "x-ms-indexing-directive",
        "Keep-Alive",
        "x-ms-offer-type",
        "x-ms-max-item-count",
        "x-ms-documentdb-pre-trigger-exclude",
        "x-ms-documentdb-pre-trigger-include",
        "x-ms-documentdb-post-trigger-exclude",
        "x-ms-documentdb-post-trigger-include",
        "x-ms-profile-request",
        "x-ms-documentdb-expiry-seconds",
        "x-ms-session-token",
        "Set-Cookie",
        "Slug",
        "User-Agent",
        "x-ms-date"
      };
      string[] valuesFromHttpHeaders = Helpers.ExtractValuesFromHTTPHeaders((System.Net.Http.Headers.HttpHeaders) requestHeaders, keys);
      this.Request(activityId, localId, uri, resourceType, valuesFromHttpHeaders[0], valuesFromHttpHeaders[1], valuesFromHttpHeaders[2], valuesFromHttpHeaders[3], valuesFromHttpHeaders[4], valuesFromHttpHeaders[5], valuesFromHttpHeaders[6], valuesFromHttpHeaders[7], valuesFromHttpHeaders[8], valuesFromHttpHeaders[9], valuesFromHttpHeaders[10], valuesFromHttpHeaders[11], valuesFromHttpHeaders[12], valuesFromHttpHeaders[13], valuesFromHttpHeaders[14], valuesFromHttpHeaders[15], valuesFromHttpHeaders[16], valuesFromHttpHeaders[17], valuesFromHttpHeaders[18], valuesFromHttpHeaders[19], valuesFromHttpHeaders[20], valuesFromHttpHeaders[21], valuesFromHttpHeaders[22], valuesFromHttpHeaders[23], valuesFromHttpHeaders[24], valuesFromHttpHeaders[25], valuesFromHttpHeaders[26], valuesFromHttpHeaders[27], valuesFromHttpHeaders[28]);
    }

    [Event(2, Message = "HttpResponse took {3}ms with status code {2} and response headers: contentType '{4}', contentEncoding '{5}', contentLength '{6}', contentLocation '{7}', currentMediaStorageUsageInMB '{8}', currentResourceQuotaUsage '{9}', databaseAccountConsumedDocumentStorageInMB '{10}', databaseAccountProvisionedDocumentStorageInMB '{11}', databaseAccountReservedDocumentStorageInMB '{12}', gatewayVersion '{13}', indexingDirective '{14}', itemCount '{15}', lastStateChangeUtc '{16}', maxMediaStorageUsageInMB '{17}', maxResourceQuota '{18}', newResourceId '{19}', ownerFullName '{20}', ownerId '{21}', requestCharge '{22}', requestValidationFailure '{23}', retryAfter '{24}', retryAfterInMilliseconds '{25}', serverVersion '{26}', schemaVersion '{27}', sessionToken '{28}', version '{29}'. ActivityId {0}, localId {1}", Keywords = (EventKeywords) 1, Level = EventLevel.Verbose)]
    private unsafe void Response(
      Guid activityId,
      Guid localId,
      short statusCode,
      double milliseconds,
      string contentType,
      string contentEncoding,
      string contentLength,
      string contentLocation,
      string currentMediaStorageUsageInMB,
      string currentResourceQuotaUsage,
      string databaseAccountConsumedDocumentStorageInMB,
      string databaseAccountProvisionedDocumentStorageInMB,
      string databaseAccountReservedDocumentStorageInMB,
      string gatewayVersion,
      string indexingDirective,
      string itemCount,
      string lastStateChangeUtc,
      string maxMediaStorageUsageInMB,
      string maxResourceQuota,
      string newResourceId,
      string ownerFullName,
      string ownerId,
      string requestCharge,
      string requestValidationFailure,
      string retryAfter,
      string retryAfterInMilliseconds,
      string serverVersion,
      string schemaVersion,
      string sessionToken,
      string version)
    {
      if (contentType == null)
        throw new ArgumentException(nameof (contentType));
      if (contentEncoding == null)
        throw new ArgumentException(nameof (contentEncoding));
      if (contentLength == null)
        throw new ArgumentException(nameof (contentLength));
      if (contentLocation == null)
        throw new ArgumentException(nameof (contentLocation));
      if (currentMediaStorageUsageInMB == null)
        throw new ArgumentException(nameof (currentMediaStorageUsageInMB));
      if (currentResourceQuotaUsage == null)
        throw new ArgumentException(nameof (currentResourceQuotaUsage));
      if (databaseAccountConsumedDocumentStorageInMB == null)
        throw new ArgumentException(nameof (databaseAccountConsumedDocumentStorageInMB));
      if (databaseAccountProvisionedDocumentStorageInMB == null)
        throw new ArgumentException(nameof (databaseAccountProvisionedDocumentStorageInMB));
      if (databaseAccountReservedDocumentStorageInMB == null)
        throw new ArgumentException(nameof (databaseAccountReservedDocumentStorageInMB));
      if (gatewayVersion == null)
        throw new ArgumentException(nameof (gatewayVersion));
      if (indexingDirective == null)
        throw new ArgumentException(nameof (indexingDirective));
      if (itemCount == null)
        throw new ArgumentException(nameof (itemCount));
      if (lastStateChangeUtc == null)
        throw new ArgumentException(nameof (lastStateChangeUtc));
      if (maxMediaStorageUsageInMB == null)
        throw new ArgumentException(nameof (maxMediaStorageUsageInMB));
      if (maxResourceQuota == null)
        throw new ArgumentException(nameof (maxResourceQuota));
      if (newResourceId == null)
        throw new ArgumentException(nameof (newResourceId));
      if (ownerFullName == null)
        throw new ArgumentException(nameof (ownerFullName));
      if (ownerId == null)
        throw new ArgumentException(nameof (ownerId));
      if (requestCharge == null)
        throw new ArgumentException(nameof (requestCharge));
      if (requestValidationFailure == null)
        throw new ArgumentException(nameof (requestValidationFailure));
      if (retryAfter == null)
        throw new ArgumentException(nameof (retryAfter));
      if (retryAfterInMilliseconds == null)
        throw new ArgumentException(nameof (retryAfterInMilliseconds));
      if (serverVersion == null)
        throw new ArgumentException(nameof (serverVersion));
      if (schemaVersion == null)
        throw new ArgumentException(nameof (schemaVersion));
      if (sessionToken == null)
        throw new ArgumentException(nameof (sessionToken));
      if (version == null)
        throw new ArgumentException(nameof (version));
      byte[] byteArray1 = activityId.ToByteArray();
      byte[] byteArray2 = localId.ToByteArray();
      fixed (byte* numPtr1 = byteArray1)
        fixed (byte* numPtr2 = byteArray2)
          fixed (char* chPtr1 = contentType)
            fixed (char* chPtr2 = contentEncoding)
              fixed (char* chPtr3 = contentLength)
                fixed (char* chPtr4 = contentLocation)
                  fixed (char* chPtr5 = currentMediaStorageUsageInMB)
                    fixed (char* chPtr6 = currentResourceQuotaUsage)
                      fixed (char* chPtr7 = databaseAccountConsumedDocumentStorageInMB)
                        fixed (char* chPtr8 = databaseAccountProvisionedDocumentStorageInMB)
                          fixed (char* chPtr9 = databaseAccountReservedDocumentStorageInMB)
                            fixed (char* chPtr10 = gatewayVersion)
                              fixed (char* chPtr11 = indexingDirective)
                                fixed (char* chPtr12 = itemCount)
                                  fixed (char* chPtr13 = lastStateChangeUtc)
                                    fixed (char* chPtr14 = maxMediaStorageUsageInMB)
                                      fixed (char* chPtr15 = maxResourceQuota)
                                        fixed (char* chPtr16 = newResourceId)
                                          fixed (char* chPtr17 = ownerFullName)
                                            fixed (char* chPtr18 = ownerId)
                                              fixed (char* chPtr19 = requestCharge)
                                                fixed (char* chPtr20 = requestValidationFailure)
                                                  fixed (char* chPtr21 = retryAfter)
                                                    fixed (char* chPtr22 = retryAfterInMilliseconds)
                                                      fixed (char* chPtr23 = serverVersion)
                                                        fixed (char* chPtr24 = schemaVersion)
                                                          fixed (char* chPtr25 = sessionToken)
                                                            fixed (char* chPtr26 = version)
                                                            {
                                                              EventSource.EventData* dataDesc = stackalloc EventSource.EventData[30];
                                                              dataDesc->DataPointer = (IntPtr) (void*) numPtr1;
                                                              dataDesc->Size = byteArray1.Length;
                                                              dataDesc[1].DataPointer = (IntPtr) (void*) numPtr2;
                                                              dataDesc[1].Size = byteArray2.Length;
                                                              dataDesc[2].DataPointer = (IntPtr) (void*) &statusCode;
                                                              dataDesc[2].Size = 2;
                                                              dataDesc[3].DataPointer = (IntPtr) (void*) &milliseconds;
                                                              dataDesc[3].Size = 8;
                                                              dataDesc[4].DataPointer = (IntPtr) (void*) chPtr1;
                                                              dataDesc[4].Size = (contentType.Length + 1) * 2;
                                                              dataDesc[5].DataPointer = (IntPtr) (void*) chPtr2;
                                                              dataDesc[5].Size = (contentEncoding.Length + 1) * 2;
                                                              dataDesc[6].DataPointer = (IntPtr) (void*) chPtr3;
                                                              dataDesc[6].Size = (contentLength.Length + 1) * 2;
                                                              dataDesc[7].DataPointer = (IntPtr) (void*) chPtr4;
                                                              dataDesc[7].Size = (contentLocation.Length + 1) * 2;
                                                              dataDesc[8].DataPointer = (IntPtr) (void*) chPtr5;
                                                              dataDesc[8].Size = (currentMediaStorageUsageInMB.Length + 1) * 2;
                                                              dataDesc[9].DataPointer = (IntPtr) (void*) chPtr6;
                                                              dataDesc[9].Size = (currentResourceQuotaUsage.Length + 1) * 2;
                                                              dataDesc[10].DataPointer = (IntPtr) (void*) chPtr7;
                                                              dataDesc[10].Size = (databaseAccountConsumedDocumentStorageInMB.Length + 1) * 2;
                                                              dataDesc[11].DataPointer = (IntPtr) (void*) chPtr8;
                                                              dataDesc[11].Size = (databaseAccountProvisionedDocumentStorageInMB.Length + 1) * 2;
                                                              dataDesc[12].DataPointer = (IntPtr) (void*) chPtr9;
                                                              dataDesc[12].Size = (databaseAccountReservedDocumentStorageInMB.Length + 1) * 2;
                                                              dataDesc[13].DataPointer = (IntPtr) (void*) chPtr10;
                                                              dataDesc[13].Size = (gatewayVersion.Length + 1) * 2;
                                                              dataDesc[14].DataPointer = (IntPtr) (void*) chPtr11;
                                                              dataDesc[14].Size = (indexingDirective.Length + 1) * 2;
                                                              dataDesc[15].DataPointer = (IntPtr) (void*) chPtr12;
                                                              dataDesc[15].Size = (itemCount.Length + 1) * 2;
                                                              dataDesc[16].DataPointer = (IntPtr) (void*) chPtr13;
                                                              dataDesc[16].Size = (lastStateChangeUtc.Length + 1) * 2;
                                                              dataDesc[17].DataPointer = (IntPtr) (void*) chPtr14;
                                                              dataDesc[17].Size = (maxMediaStorageUsageInMB.Length + 1) * 2;
                                                              dataDesc[18].DataPointer = (IntPtr) (void*) chPtr15;
                                                              dataDesc[18].Size = (maxResourceQuota.Length + 1) * 2;
                                                              dataDesc[19].DataPointer = (IntPtr) (void*) chPtr16;
                                                              dataDesc[19].Size = (newResourceId.Length + 1) * 2;
                                                              dataDesc[20].DataPointer = (IntPtr) (void*) chPtr17;
                                                              dataDesc[20].Size = (ownerFullName.Length + 1) * 2;
                                                              dataDesc[21].DataPointer = (IntPtr) (void*) chPtr18;
                                                              dataDesc[21].Size = (ownerId.Length + 1) * 2;
                                                              dataDesc[22].DataPointer = (IntPtr) (void*) chPtr19;
                                                              dataDesc[22].Size = (requestCharge.Length + 1) * 2;
                                                              dataDesc[23].DataPointer = (IntPtr) (void*) chPtr20;
                                                              dataDesc[23].Size = (requestValidationFailure.Length + 1) * 2;
                                                              dataDesc[24].DataPointer = (IntPtr) (void*) chPtr21;
                                                              dataDesc[24].Size = (retryAfter.Length + 1) * 2;
                                                              dataDesc[25].DataPointer = (IntPtr) (void*) chPtr22;
                                                              dataDesc[25].Size = (retryAfterInMilliseconds.Length + 1) * 2;
                                                              dataDesc[26].DataPointer = (IntPtr) (void*) chPtr23;
                                                              dataDesc[26].Size = (serverVersion.Length + 1) * 2;
                                                              dataDesc[27].DataPointer = (IntPtr) (void*) chPtr24;
                                                              dataDesc[27].Size = (schemaVersion.Length + 1) * 2;
                                                              dataDesc[28].DataPointer = (IntPtr) (void*) chPtr25;
                                                              dataDesc[28].Size = (sessionToken.Length + 1) * 2;
                                                              dataDesc[29].DataPointer = (IntPtr) (void*) chPtr26;
                                                              dataDesc[29].Size = (version.Length + 1) * 2;
                                                              this.WriteEventCoreWithActivityId(activityId, 2, 30, dataDesc);
                                                            }
    }

    [NonEvent]
    public virtual void Response(
      Guid activityId,
      Guid localId,
      short statusCode,
      double milliseconds,
      HttpResponseHeaders responseHeaders)
    {
      if (!this.IsEnabled(EventLevel.Verbose, (EventKeywords) 1))
        return;
      string[] keys = new string[26]
      {
        "Content-Type",
        "Content-Encoding",
        "Content-Length",
        "Content-Location",
        "x-ms-media-storage-usage-mb",
        "x-ms-resource-usage",
        "x-ms-databaseaccount-consumed-mb",
        "x-ms-databaseaccount-provisioned-mb",
        "x-ms-databaseaccount-reserved-mb",
        "x-ms-gatewayversion",
        "x-ms-indexing-directive",
        "x-ms-item-count",
        "x-ms-last-state-change-utc",
        "x-ms-max-media-storage-usage-mb",
        "x-ms-resource-quota",
        "x-ms-new-resource-id",
        "x-ms-alt-content-path",
        "x-ms-content-path",
        "x-ms-request-charge",
        "x-ms-request-validation-failure",
        "Retry-After",
        "x-ms-retry-after-ms",
        "x-ms-serviceversion",
        "x-ms-schemaversion",
        "x-ms-session-token",
        "x-ms-version"
      };
      string[] valuesFromHttpHeaders = Helpers.ExtractValuesFromHTTPHeaders((System.Net.Http.Headers.HttpHeaders) responseHeaders, keys);
      this.Response(activityId, localId, statusCode, milliseconds, valuesFromHttpHeaders[0], valuesFromHttpHeaders[1], valuesFromHttpHeaders[2], valuesFromHttpHeaders[3], valuesFromHttpHeaders[4], valuesFromHttpHeaders[5], valuesFromHttpHeaders[6], valuesFromHttpHeaders[7], valuesFromHttpHeaders[8], valuesFromHttpHeaders[9], valuesFromHttpHeaders[10], valuesFromHttpHeaders[11], valuesFromHttpHeaders[12], valuesFromHttpHeaders[13], valuesFromHttpHeaders[14], valuesFromHttpHeaders[15], valuesFromHttpHeaders[16], valuesFromHttpHeaders[17], valuesFromHttpHeaders[18], valuesFromHttpHeaders[19], valuesFromHttpHeaders[20], valuesFromHttpHeaders[21], valuesFromHttpHeaders[22], valuesFromHttpHeaders[23], valuesFromHttpHeaders[24], valuesFromHttpHeaders[25]);
    }

    public class Keywords
    {
      public const EventKeywords HttpRequestAndResponse = (EventKeywords) 1;
    }
  }
}
