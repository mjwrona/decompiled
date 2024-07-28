// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.GraphQLJsonTextReader
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

using Newtonsoft.Json;
using System.IO;

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi
{
  internal class GraphQLJsonTextReader : JsonTextReader
  {
    public GraphQLJsonTextReader(TextReader reader)
      : base(reader)
    {
    }

    public override bool Read()
    {
      int num = base.Read() ? 1 : 0;
      if (num == 0)
        return num != 0;
      if (this.TokenType != JsonToken.PropertyName)
        return num != 0;
      if (this.Value == null)
        return num != 0;
      if (!this.Value.Equals((object) "__typename"))
        return num != 0;
      this.SetToken(JsonToken.PropertyName, (object) "$type");
      return num != 0;
    }
  }
}
