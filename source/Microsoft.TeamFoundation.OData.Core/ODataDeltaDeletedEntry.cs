// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataDeltaDeletedEntry
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;

namespace Microsoft.OData
{
  public sealed class ODataDeltaDeletedEntry : ODataItem
  {
    private ODataDeltaSerializationInfo serializationInfo;

    public ODataDeltaDeletedEntry(string id, DeltaDeletedEntryReason reason)
    {
      this.Id = id;
      this.Reason = new DeltaDeletedEntryReason?(reason);
    }

    public string Id { get; set; }

    public DeltaDeletedEntryReason? Reason { get; set; }

    internal ODataDeltaSerializationInfo SerializationInfo
    {
      get => this.serializationInfo;
      set => this.serializationInfo = ODataDeltaSerializationInfo.Validate(value);
    }

    internal static ODataDeltaDeletedEntry GetDeltaDeletedEntry(ODataDeletedResource entry)
    {
      ODataDeltaDeletedEntry deltaDeletedEntry = new ODataDeltaDeletedEntry(entry.Id.OriginalString, (DeltaDeletedEntryReason) ((int) entry.Reason ?? 0));
      if (entry.SerializationInfo != null)
        deltaDeletedEntry.SetSerializationInfo(entry.SerializationInfo);
      return deltaDeletedEntry;
    }

    internal static ODataDeletedResource GetDeletedResource(ODataDeltaDeletedEntry entry)
    {
      Uri uri = UriUtils.StringToUri(entry.Id);
      ODataDeletedResource odataDeletedResource = new ODataDeletedResource();
      odataDeletedResource.Id = uri;
      odataDeletedResource.Reason = entry.Reason;
      ODataDeletedResource deletedResource = odataDeletedResource;
      if (entry.SerializationInfo != null)
        deletedResource.SerializationInfo = new ODataResourceSerializationInfo()
        {
          NavigationSourceName = entry.SerializationInfo == null ? (string) null : entry.SerializationInfo.NavigationSourceName
        };
      return deletedResource;
    }
  }
}
