// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.PageResult`1
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.AspNet.OData
{
  [DataContract]
  [JsonObject]
  public class PageResult<T> : PageResult, IEnumerable<T>, IEnumerable
  {
    public PageResult(IEnumerable<T> items, Uri nextPageLink, long? count)
      : base(nextPageLink, count)
    {
      this.Items = items != null ? items : throw Error.ArgumentNull("data");
    }

    [DataMember]
    public IEnumerable<T> Items { get; private set; }

    public IEnumerator<T> GetEnumerator() => this.Items.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.Items.GetEnumerator();
  }
}
