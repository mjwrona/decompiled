// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ReferenceAuditEventGridResultExporter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.Azure.EventGrid.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ReferenceAuditEventGridResultExporter : 
    IReferenceAuditResultExporter,
    IVssFrameworkService
  {
    private const int EventDataSizeLimit = 47616;
    private static readonly RegistryQuery RegistrySettingsQuery = (RegistryQuery) "/Configuration/BlobStore/EventGridClient/*";
    private readonly List<EventGridEvent> batchedEvents = new List<EventGridEvent>();
    private readonly Dictionary<Guid, Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData>> batchedEntries = new Dictionary<Guid, Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData>>();
    private ReferenceAuditEventGridResultExporter.EventGridSettings settings;
    private IBlobstoreEventGridClient blobstoreEventGridClient;
    private readonly SemaphoreSlim Semaphore = new SemaphoreSlim(1);

    private static EventGridEvent ConvertToEventGridEvent(
      Guid hostId,
      string referenceScope,
      IEnumerable<ReferenceAuditEventData> referenceAuditEventData)
    {
      return new EventGridEvent(Guid.NewGuid().ToString(), string.Format("/hosts/{0}/scopes/{1}", (object) hostId, (object) referenceScope), (object) referenceAuditEventData.ToArray<ReferenceAuditEventData>(), "Microsoft.VisualStudio.Services.BlobStore.ReferenceAuditEntry", DateTime.UtcNow, "1.0", (string) null, (string) null);
    }

    public static ReferenceAuditEventData CreateReferenceAuditEventData(
      ReferenceAuditEntry referenceAuditEntry)
    {
      return new ReferenceAuditEventData()
      {
        BlobId = referenceAuditEntry.BlobIdentifier.ValueString,
        DomainId = referenceAuditEntry.DomainId,
        ReferenceTimestamp = referenceAuditEntry.ReferenceTimestamp.ToString("O"),
        ReferenceId = referenceAuditEntry.Reference.Name
      };
    }

    public async Task ExportResultsAsync(
      IEnumerable<ReferenceAuditEntry> entries,
      CancellationToken cancellationToken,
      Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer)
    {
      if (entries == null)
        throw new ArgumentNullException(nameof (entries));
      await this.Semaphore.WaitAsync(cancellationToken);
      try
      {
        foreach (ReferenceAuditEntry entry in entries)
          await this.AddEntryToBatch(entry, cancellationToken);
        foreach (Guid key1 in this.batchedEntries.Keys)
        {
          Guid hostId = key1;
          foreach (string key2 in this.batchedEntries[hostId].Keys)
            await this.EnqueueEntryBatchAsync(hostId, key2, cancellationToken);
          hostId = new Guid();
        }
        await this.PublishQueuedEventsAsync(cancellationToken);
      }
      finally
      {
        this.Semaphore.Release();
      }
    }

    private async Task AddEntryToBatch(
      ReferenceAuditEntry entry,
      CancellationToken cancellationToken)
    {
      ReferenceAuditEventGridResultExporter gridResultExporter1 = this;
      Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData> dictionary1;
      if (!gridResultExporter1.batchedEntries.TryGetValue(entry.HostId, out dictionary1))
        gridResultExporter1.batchedEntries.Add(entry.HostId, dictionary1 = new Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData>());
      Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData> dictionary2 = dictionary1;
      IdBlobReference reference = entry.Reference;
      string scope1 = reference.Scope;
      ReferenceAuditEventGridResultExporter.EntryBatchEventData entryBatchEventData1;
      ref ReferenceAuditEventGridResultExporter.EntryBatchEventData local = ref entryBatchEventData1;
      if (!dictionary2.TryGetValue(scope1, out local))
      {
        Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData> dictionary3 = dictionary1;
        reference = entry.Reference;
        string scope2 = reference.Scope;
        ReferenceAuditEventGridResultExporter.EntryBatchEventData entryBatchEventData2;
        entryBatchEventData1 = entryBatchEventData2 = new ReferenceAuditEventGridResultExporter.EntryBatchEventData();
        dictionary3.Add(scope2, entryBatchEventData2);
      }
      entryBatchEventData1.AddEventData(ReferenceAuditEventGridResultExporter.CreateReferenceAuditEventData(entry));
      if (entryBatchEventData1.EstimatedSize < 47616 && entryBatchEventData1.EventCount < gridResultExporter1.settings.AuditEntriesPerEvent)
        return;
      ReferenceAuditEventGridResultExporter gridResultExporter2 = gridResultExporter1;
      Guid hostId = entry.HostId;
      reference = entry.Reference;
      string scope3 = reference.Scope;
      CancellationToken cancellationToken1 = cancellationToken;
      await gridResultExporter2.EnqueueEntryBatchAsync(hostId, scope3, cancellationToken1);
    }

    private async Task EnqueueEntryBatchAsync(
      Guid hostId,
      string scope,
      CancellationToken cancellationToken)
    {
      Dictionary<string, ReferenceAuditEventGridResultExporter.EntryBatchEventData> dictionary;
      ReferenceAuditEventGridResultExporter.EntryBatchEventData entryBatchEventData;
      if (!this.batchedEntries.TryGetValue(hostId, out dictionary) || !dictionary.TryGetValue(scope, out entryBatchEventData) || entryBatchEventData.EventCount <= 0)
        return;
      this.batchedEvents.Add(ReferenceAuditEventGridResultExporter.ConvertToEventGridEvent(hostId, scope, entryBatchEventData.Events));
      entryBatchEventData.Reset();
      if (this.batchedEvents.Count < this.settings.BatchSize)
        return;
      await this.PublishQueuedEventsAsync(cancellationToken);
    }

    private async Task PublishQueuedEventsAsync(CancellationToken cancellationToken)
    {
      if (this.batchedEvents.Count <= 0)
        return;
      await this.blobstoreEventGridClient.PublishEventsAsync((IList<EventGridEvent>) this.batchedEvents.ToList<EventGridEvent>(), cancellationToken);
      this.batchedEvents.Clear();
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.settings = (ReferenceAuditEventGridResultExporter.EventGridSettings) null;
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsUpdated), in ReferenceAuditEventGridResultExporter.RegistrySettingsQuery);
      Interlocked.CompareExchange<ReferenceAuditEventGridResultExporter.EventGridSettings>(ref this.settings, new ReferenceAuditEventGridResultExporter.EventGridSettings(systemRequestContext), (ReferenceAuditEventGridResultExporter.EventGridSettings) null);
      this.blobstoreEventGridClient = systemRequestContext.GetService<IBlobstoreEventGridClient>();
    }

    private void OnSettingsUpdated(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ReferenceAuditEventGridResultExporter.EventGridSettings>(ref this.settings, new ReferenceAuditEventGridResultExporter.EventGridSettings(requestContext));
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsUpdated));

    private class EventGridSettings
    {
      public int AuditEntriesPerEvent { get; }

      public int BatchSize { get; }

      public EventGridSettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection = requestContext.GetService<IVssRegistryService>().ReadEntriesFallThru(requestContext, in ReferenceAuditEventGridResultExporter.RegistrySettingsQuery);
        this.AuditEntriesPerEvent = registryEntryCollection.GetValueFromPath<int>(nameof (AuditEntriesPerEvent), 0);
        this.BatchSize = registryEntryCollection.GetValueFromPath<int>(nameof (BatchSize), 0);
      }
    }

    private class EntryBatchEventData
    {
      private readonly IList<ReferenceAuditEventData> events = (IList<ReferenceAuditEventData>) new List<ReferenceAuditEventData>();

      public IEnumerable<ReferenceAuditEventData> Events => (IEnumerable<ReferenceAuditEventData>) this.events;

      public int EstimatedSize { get; private set; }

      public int EventCount => this.events.Count;

      public void AddEventData(ReferenceAuditEventData eventData)
      {
        this.events.Add(eventData);
        this.EstimatedSize += eventData.EstimatedSerializedSize;
      }

      public void Reset()
      {
        this.events.Clear();
        this.EstimatedSize = 0;
      }
    }
  }
}
