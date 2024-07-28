// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.AgentArtifactDefinition
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class AgentArtifactDefinition
  {
    public string Name { get; set; }

    public string Version { get; set; }

    public string Alias { get; set; }

    public AgentArtifactType ArtifactType { get; set; }

    public string Details { get; set; }

    public string ArtifactTypeId { get; set; }

    public int ArtifactSourceId { get; set; }

    public string ArtifactVersion { get; set; }

    public string Path { get; set; }

    public Dictionary<string, InputValue> SourceData { get; private set; }

    public AgentArtifactDefinition() => this.SourceData = new Dictionary<string, InputValue>();

    public void FillSourceData(string inputSourceData) => ServerModelUtility.FillSourceData(inputSourceData, this.SourceData);

    public string SourceDataKeys
    {
      get
      {
        bool flag = true;
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string key in this.SourceData.Keys)
        {
          if (flag)
            flag = false;
          else
            stringBuilder.Append(",");
          stringBuilder.Append(key);
        }
        return stringBuilder.ToString();
      }
    }

    public ArtifactSource ToArtifactSource() => new ArtifactSource()
    {
      Alias = this.Alias,
      ArtifactTypeId = this.ArtifactTypeId,
      SourceData = this.SourceData
    };
  }
}
