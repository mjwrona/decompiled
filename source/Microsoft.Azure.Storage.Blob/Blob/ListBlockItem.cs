// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Blob.ListBlockItem
// Assembly: Microsoft.Azure.Storage.Blob, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: A04A3512-352A-442F-A95B-BC1B94EF8840
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Blob.dll

namespace Microsoft.Azure.Storage.Blob
{
  public sealed class ListBlockItem
  {
    public string Name { get; internal set; }

    public long Length { get; internal set; }

    public bool Committed { get; internal set; }
  }
}
