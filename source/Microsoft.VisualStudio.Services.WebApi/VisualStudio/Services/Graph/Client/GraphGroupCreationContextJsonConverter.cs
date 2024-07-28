// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphGroupCreationContextJsonConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  public class GraphGroupCreationContextJsonConverter : 
    VssJsonCreationConverter<GraphGroupCreationContext>
  {
    protected override GraphGroupCreationContext Create(Type objectType, JObject jsonObject)
    {
      bool flag1 = jsonObject["originId"] != null;
      bool flag2 = jsonObject["mailAddress"] != null;
      bool flag3 = jsonObject["displayName"] != null;
      if (((IEnumerable<bool>) new bool[3]
      {
        flag1,
        flag3,
        flag2
      }).Count<bool>((Func<bool, bool>) (b => b)) > 1)
        throw new ArgumentNullException(WebApiResources.GraphGroupMissingRequiredFields());
      if (flag1)
        return (GraphGroupCreationContext) new GraphGroupOriginIdCreationContext();
      if (flag2)
        return (GraphGroupCreationContext) new GraphGroupMailAddressCreationContext();
      if (flag3)
        return (GraphGroupCreationContext) new GraphGroupVstsCreationContext();
      throw new ArgumentException(WebApiResources.GraphGroupMissingRequiredFields());
    }
  }
}
