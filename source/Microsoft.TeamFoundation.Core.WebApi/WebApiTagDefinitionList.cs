// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.WebApiTagDefinitionList
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  public class WebApiTagDefinitionList : List<WebApiTagDefinition>
  {
    public WebApiTagDefinitionList(IEnumerable<WebApiTagDefinition> collection)
      : base(collection)
    {
    }
  }
}
