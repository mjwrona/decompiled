// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Server.ObjectModel.DesignerJsonConfiguration
// Assembly: Microsoft.Azure.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC20940E-746B-4985-9936-F8ACD7ADA1DB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Server.dll

using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Pipelines.Server.ObjectModel
{
  [DataContract]
  public class DesignerJsonConfiguration : PipelineConfiguration
  {
    private static readonly int CurrentVersion = 1;

    public DesignerJsonConfiguration()
      : base(ConfigurationType.DesignerJson, DesignerJsonConfiguration.CurrentVersion)
    {
    }

    [DataMember]
    public JObject DesignerJson { get; set; }
  }
}
