// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PageResult
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData
{
  [DataContract]
  public abstract class PageResult
  {
    private long? _count;

    protected PageResult(Uri nextPageLink, long? count)
    {
      this.NextPageLink = nextPageLink;
      this.Count = count;
    }

    [DataMember]
    public Uri NextPageLink { get; private set; }

    [DataMember]
    public long? Count
    {
      get => this._count;
      private set
      {
        if (value.HasValue && value.Value < 0L)
          throw Error.ArgumentMustBeGreaterThanOrEqualTo(nameof (value), (object) value.Value, (object) 0);
        this._count = value;
      }
    }
  }
}
