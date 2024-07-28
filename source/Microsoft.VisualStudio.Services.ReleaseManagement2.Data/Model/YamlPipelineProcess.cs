// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public sealed class YamlPipelineProcess : PipelineProcess
  {
    private List<string> errors;

    public YamlPipelineProcess(string fileName, YamlFileSource fileSource)
      : base(PipelineProcessTypes.Yaml)
    {
      this.FileName = fileName;
      this.FileSource = fileSource;
    }

    [JsonConstructor]
    public YamlPipelineProcess()
      : base(PipelineProcessTypes.Yaml)
    {
    }

    public string FileName { get; set; }

    public YamlFileSource FileSource { get; set; }

    public YamlPipelineProcessResource Resources { get; set; }

    public IList<string> Errors
    {
      get
      {
        if (this.errors == null)
          this.errors = new List<string>();
        return (IList<string>) this.errors;
      }
    }
  }
}
