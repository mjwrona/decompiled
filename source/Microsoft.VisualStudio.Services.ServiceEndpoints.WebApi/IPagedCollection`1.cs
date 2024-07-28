// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.IPagedCollection`1
// Assembly: Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52155B17-64DE-4C30-B15E-F2E70DBED717
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi
{
  public interface IPagedCollection<T> : 
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable,
    IPagedList<T>,
    IPagedList
  {
  }
}
