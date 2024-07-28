// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.Container
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public sealed class Container
  {
    public IDictionary<string, string> Environment { get; set; }

    public bool MapDockerSocket { get; set; }

    public string Image { get; set; }

    public string Options { get; set; }

    public IList<string> Volumes { get; set; }

    public IList<string> Ports { get; set; }
  }
}
