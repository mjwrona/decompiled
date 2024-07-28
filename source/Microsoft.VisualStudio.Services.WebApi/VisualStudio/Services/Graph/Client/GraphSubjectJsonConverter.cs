// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphSubjectJsonConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  public class GraphSubjectJsonConverter : VssJsonCreationConverter<GraphSubject>
  {
    protected override GraphSubject Create(Type objectType, JObject jsonObject)
    {
      JToken jtoken = jsonObject.GetValue("SubjectKind", StringComparison.OrdinalIgnoreCase);
      string str = jtoken != null ? jtoken.ToString() : throw new ArgumentException(WebApiResources.UnknownEntityType((object) jtoken));
      switch (str)
      {
        case "group":
          return (GraphSubject) new GraphGroup((GraphGroup) null);
        case "scope":
          return (GraphSubject) new GraphScope((GraphScope) null);
        case "user":
          return (GraphSubject) new GraphUser((GraphUser) null);
        case "systemSubject":
          return (GraphSubject) new GraphSystemSubject((GraphSystemSubject) null);
        case "servicePrincipal":
          return (GraphSubject) new GraphServicePrincipal((GraphServicePrincipal) null);
        default:
          throw new ArgumentException(WebApiResources.UnknownEntityType((object) str));
      }
    }
  }
}
