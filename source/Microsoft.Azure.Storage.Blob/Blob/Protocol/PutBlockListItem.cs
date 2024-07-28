// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.Protocol.PutBlockListItem
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

namespace Microsoft.Azure.Storage.Blob.Protocol
{
  public sealed class PutBlockListItem
  {
    public PutBlockListItem(string id, BlockSearchMode searchMode)
    {
      this.Id = id;
      this.SearchMode = searchMode;
    }

    public string Id { get; private set; }

    public BlockSearchMode SearchMode { get; private set; }
  }
}
