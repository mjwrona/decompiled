// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ODataJsonLightDeltaWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Edm;
using System;
using System.Threading.Tasks;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ODataJsonLightDeltaWriter : 
    ODataDeltaWriter,
    IODataOutputInStreamErrorListener
  {
    private readonly ODataJsonLightOutputContext jsonLightOutputContext;
    private readonly ODataJsonLightWriter resourceWriter;
    private IEdmNavigationSource navigationSource;
    private IEdmEntityType entityType;
    private IODataOutputInStreamErrorListener inStreamErrorListener;

    public ODataJsonLightDeltaWriter(
      ODataJsonLightOutputContext jsonLightOutputContext,
      IEdmNavigationSource navigationSource,
      IEdmEntityType entityType)
    {
      this.navigationSource = navigationSource;
      this.entityType = entityType;
      this.jsonLightOutputContext = jsonLightOutputContext;
      this.resourceWriter = new ODataJsonLightWriter(jsonLightOutputContext, navigationSource, (IEdmStructuredType) entityType, true, writingDelta: true);
      this.inStreamErrorListener = (IODataOutputInStreamErrorListener) this.resourceWriter;
    }

    public IEdmNavigationSource NavigationSource
    {
      get => this.navigationSource;
      set => this.navigationSource = value;
    }

    public IEdmEntityType EntityType
    {
      get => this.entityType;
      set => this.entityType = value;
    }

    public override void WriteStart(ODataDeltaResourceSet deltaResourceSet) => this.resourceWriter.WriteStart(deltaResourceSet);

    public override Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteStart(deltaResourceSet)));

    public override void WriteEnd() => this.resourceWriter.WriteEnd();

    public override Task WriteEndAsync() => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteEnd()));

    public override void WriteStart(ODataNestedResourceInfo nestedResourceInfo) => this.resourceWriter.WriteStart(nestedResourceInfo);

    public override Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteStart(nestedResourceInfo)));

    public override void WriteStart(ODataResourceSet expandedResourceSet) => this.resourceWriter.WriteStart(expandedResourceSet);

    public override Task WriteStartAsync(ODataResourceSet expandedResourceSet) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteStart(expandedResourceSet)));

    public override void WriteStart(ODataResource deltaResource) => this.resourceWriter.WriteStart(deltaResource);

    public override Task WriteStartAsync(ODataResource deltaResource) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteStart(deltaResource)));

    public override void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry)
    {
      this.resourceWriter.WriteStart(ODataDeltaDeletedEntry.GetDeletedResource(deltaDeletedEntry));
      this.resourceWriter.WriteEnd();
    }

    public override Task WriteDeltaDeletedEntryAsync(ODataDeltaDeletedEntry deltaDeletedEntry) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteStart(ODataDeltaDeletedEntry.GetDeletedResource(deltaDeletedEntry))));

    public override void WriteDeltaLink(ODataDeltaLink deltaLink) => this.resourceWriter.WriteDeltaLink(deltaLink);

    public override Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteDeltaLink(deltaLink)));

    public override void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink) => this.resourceWriter.WriteDeltaDeletedLink(deltaDeletedLink);

    public override Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink) => TaskUtils.GetTaskForSynchronousOperation((Action) (() => this.resourceWriter.WriteDeltaDeletedLink(deltaDeletedLink)));

    public override void Flush() => this.jsonLightOutputContext.Flush();

    public override Task FlushAsync() => this.jsonLightOutputContext.FlushAsync();

    void IODataOutputInStreamErrorListener.OnInStreamError() => this.inStreamErrorListener.OnInStreamError();
  }
}
