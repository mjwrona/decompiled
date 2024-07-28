// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.JustInTimeContext
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class JustInTimeContext
  {
    public string ContextName { get; set; }

    public string Ref { get; set; }

    public string RepositoryId { get; set; }

    public string Sha { get; set; }

    public string Token { get; set; }

    public string TokenType { get; set; }

    public Dictionary<string, JToken> ExtendedContext { get; } = new Dictionary<string, JToken>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
