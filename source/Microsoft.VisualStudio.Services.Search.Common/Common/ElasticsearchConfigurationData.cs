// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ElasticsearchConfigurationData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ElasticsearchConfigurationData
  {
    public string ConfigurationScriptPath { get; set; }

    public OperationType Operation { get; set; }

    public string ElasticsearchInstallPath { get; set; }

    public string ElasticsearchIndexPath { get; set; }

    public bool RemoveElasticsearchData { get; set; }

    public string ElasticsearchServiceName { get; set; }

    public int ElasticsearchPort { get; set; }

    public string ClusterName { get; set; }

    public string ElasticsearchUser { get; set; }

    public string ElasticsearchPassword { get; set; }
  }
}
