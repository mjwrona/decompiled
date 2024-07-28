// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedRequestOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Serializer;
using System;
using System.ComponentModel;

namespace Microsoft.Azure.Cosmos
{
  public sealed class ChangeFeedRequestOptions : RequestOptions
  {
    private int? pageSizeHint;

    public int? PageSizeHint
    {
      get => this.pageSizeHint;
      set
      {
        if (value.HasValue && value.Value <= 0)
          throw new ArgumentOutOfRangeException("PageSizeHint must be a positive value.");
        this.pageSizeHint = value;
      }
    }

    internal override void PopulateRequestOptions(RequestMessage request) => base.PopulateRequestOptions(request);

    [Obsolete("IfMatchEtag is inherited from the base class but not used.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string IfMatchEtag
    {
      get => throw new NotSupportedException("ChangeFeedRequestOptions does not use the IfMatchEtag property.");
      set => throw new NotSupportedException("ChangeFeedRequestOptions does not use the IfMatchEtag property.");
    }

    [Obsolete("IfNoneMatchEtag is inherited from the base class but not used.")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new string IfNoneMatchEtag
    {
      get => throw new NotSupportedException("ChangeFeedRequestOptions does not use the IfNoneMatchEtag property.");
      set => throw new NotSupportedException("ChangeFeedRequestOptions does not use the IfNoneMatchEtag property.");
    }

    internal JsonSerializationFormatOptions JsonSerializationFormatOptions { get; set; }

    internal ChangeFeedRequestOptions Clone() => new ChangeFeedRequestOptions()
    {
      PageSizeHint = this.pageSizeHint
    };
  }
}
