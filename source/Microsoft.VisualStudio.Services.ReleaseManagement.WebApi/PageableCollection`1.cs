// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PageableCollection`1
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi
{
  public class PageableCollection<T> : 
    List<T>,
    IPagedCollection<T>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
  {
    public PageableCollection(List<T> list, string continuationToken)
      : base((IEnumerable<T>) list)
    {
      this.ContinuationToken = continuationToken;
    }

    public string ContinuationToken { get; private set; }
  }
}
