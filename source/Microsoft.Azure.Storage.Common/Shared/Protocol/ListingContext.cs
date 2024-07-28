// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Storage.Shared.Protocol.ListingContext
// Assembly: Microsoft.Azure.Storage.Common, Version=11.2.3.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 0978DA65-6954-4A99-9ACB-2EF3D979A5D5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Storage.Common.dll

using Microsoft.Azure.Storage.Core.Util;

namespace Microsoft.Azure.Storage.Shared.Protocol
{
  public class ListingContext
  {
    private int? maxResults;

    public ListingContext(string prefix, int? maxResults)
    {
      this.Prefix = prefix;
      this.MaxResults = maxResults;
      this.Marker = (string) null;
    }

    public string Prefix { get; set; }

    public int? MaxResults
    {
      get => this.maxResults;
      set
      {
        if (value.HasValue)
          CommonUtility.AssertInBounds<int>("maxResults", value.Value, 1);
        this.maxResults = value;
      }
    }

    public string Marker { get; set; }
  }
}
