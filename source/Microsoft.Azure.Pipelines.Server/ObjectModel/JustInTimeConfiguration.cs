// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.JustInTimeConfiguration
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  public class JustInTimeConfiguration
  {
    public string MainFilePath { get; set; }

    public StatusCallbackInfo Callbacks { get; set; }

    public IList<JustInTimeConfigurationFile> Files { get; } = (IList<JustInTimeConfigurationFile>) new List<JustInTimeConfigurationFile>();

    public IDictionary<string, string> Properties { get; } = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  }
}
