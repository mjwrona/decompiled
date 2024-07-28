// Decompiled with JetBrains decompiler
// Type: Nest.ApiKeyRole
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class ApiKeyRole : IApiKeyRole
  {
    public IEnumerable<string> Cluster { get; set; }

    public IEnumerable<IApiKeyPrivileges> Index { get; set; }
  }
}
