// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataDeltaWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Threading.Tasks;

namespace Microsoft.OData
{
  public abstract class ODataDeltaWriter
  {
    public abstract void WriteStart(ODataDeltaResourceSet deltaResourceSet);

    public abstract Task WriteStartAsync(ODataDeltaResourceSet deltaResourceSet);

    public abstract void WriteEnd();

    public abstract Task WriteEndAsync();

    public abstract void WriteStart(ODataNestedResourceInfo nestedResourceInfo);

    public abstract Task WriteStartAsync(ODataNestedResourceInfo nestedResourceInfo);

    public abstract void WriteStart(ODataResourceSet expandedResourceSet);

    public abstract Task WriteStartAsync(ODataResourceSet expandedResourceSet);

    public abstract void WriteStart(ODataResource deltaResource);

    public abstract Task WriteStartAsync(ODataResource deltaResource);

    public abstract void WriteDeltaDeletedEntry(ODataDeltaDeletedEntry deltaDeletedEntry);

    public abstract Task WriteDeltaDeletedEntryAsync(ODataDeltaDeletedEntry deltaDeletedEntry);

    public abstract void WriteDeltaLink(ODataDeltaLink deltaLink);

    public abstract Task WriteDeltaLinkAsync(ODataDeltaLink deltaLink);

    public abstract void WriteDeltaDeletedLink(ODataDeltaDeletedLink deltaDeletedLink);

    public abstract Task WriteDeltaDeletedLinkAsync(ODataDeltaDeletedLink deltaDeletedLink);

    public abstract void Flush();

    public abstract Task FlushAsync();
  }
}
