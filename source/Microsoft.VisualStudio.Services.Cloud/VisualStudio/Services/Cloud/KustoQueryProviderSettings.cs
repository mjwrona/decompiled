// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.KustoQueryProviderSettings
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Kusto.Data.Common;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class KustoQueryProviderSettings
  {
    private readonly ClientRequestProperties m_queryProperties = new ClientRequestProperties();

    public int MaxQueryRetryCount { get; set; }

    public int SlowQueryThreshold { get; set; }

    public ClientRequestProperties QueryProperties => this.m_queryProperties;
  }
}
