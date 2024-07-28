// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphUserCreationContextJsonConverter
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
  public class GraphUserCreationContextJsonConverter : 
    VssJsonCreationConverter<GraphUserCreationContext>
  {
    protected override GraphUserCreationContext Create(Type objectType, JObject jsonObject)
    {
      bool flag1 = jsonObject["originId"] != null;
      bool flag2 = jsonObject["principalName"] != null;
      bool flag3 = jsonObject["mailAddress"] != null;
      if (((IEnumerable<bool>) new bool[3]
      {
        flag1,
        flag2,
        flag3
      }).Count<bool>((Func<bool, bool>) (b => b)) != 1)
        throw new ArgumentException(WebApiResources.GraphUserMissingRequiredFields());
      if (flag1)
        return (GraphUserCreationContext) new GraphUserOriginIdCreationContext();
      if (flag2)
        return (GraphUserCreationContext) new GraphUserPrincipalNameCreationContext();
      if (flag3)
        return (GraphUserCreationContext) new GraphUserMailAddressCreationContext();
      throw new ArgumentException(WebApiResources.GraphUserMissingRequiredFields());
    }
  }
}
