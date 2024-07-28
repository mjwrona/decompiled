// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.Kqlm.KqlMdmQueryRequest
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query.Kqlm
{
  internal sealed class KqlMdmQueryRequest
  {
    public DateTime StartTimeUtc { get; set; }

    public DateTime EndTimeUtc { get; set; }

    public string MetricNamespace { get; set; }

    public string QueryStatement { get; set; }

    public IReadOnlyDictionary<string, string> QueryParameters { get; set; } = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>();
  }
}
