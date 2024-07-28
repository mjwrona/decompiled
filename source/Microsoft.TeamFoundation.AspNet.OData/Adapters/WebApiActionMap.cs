// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Adapters.WebApiActionMap
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Microsoft.AspNet.OData.Interfaces;
using System.Linq;
using System.Web.Http.Controllers;

namespace Microsoft.AspNet.OData.Adapters
{
  internal class WebApiActionMap : IWebApiActionMap
  {
    private ILookup<string, HttpActionDescriptor> innerMap;

    public WebApiActionMap(ILookup<string, HttpActionDescriptor> actionMap) => this.innerMap = actionMap != null ? actionMap : throw Error.ArgumentNull(nameof (actionMap));

    public bool Contains(string name) => this.innerMap.Contains(name);
  }
}
