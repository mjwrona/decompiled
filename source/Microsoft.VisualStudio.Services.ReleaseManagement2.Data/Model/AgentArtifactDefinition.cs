// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.AgentArtifactDefinition
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.VisualStudio.Services.FormInput;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class AgentArtifactDefinition
  {
    public string ArtifactTypeId { get; set; }

    public int ArtifactSourceId { get; set; }

    public string ArtifactVersion { get; set; }

    public string Name { get; set; }

    public string Alias { get; set; }

    public string Path { get; set; }

    public Dictionary<string, InputValue> SourceData { get; private set; }

    public AgentArtifactDefinition() => this.SourceData = new Dictionary<string, InputValue>();

    public void FillSourceData(string inputSourceData) => Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ServerModelUtility.FillSourceData(inputSourceData, this.SourceData);

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
