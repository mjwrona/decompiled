// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.PipelineArtifactSource
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class PipelineArtifactSource : ArtifactSource
  {
    private InputValue version;

    public int ArtifactSourceId { get; set; }

    public Guid ProjectId { get; set; }

    public string ArtifactVersion { get; set; }

    public int ReleaseId { get; set; }

    public InputValue Version
    {
      get
      {
        this.version = PipelineArtifactSource.FromString<InputValue>(this.ArtifactVersion);
        return this.version;
      }
    }

    public static T FromString<T>(string value)
    {
      if (string.IsNullOrEmpty(value))
        return default (T);
      using (StringReader reader1 = new StringReader(value))
      {
        JsonTextReader reader2 = new JsonTextReader((TextReader) reader1);
        return JsonSerializer.Create(new Lazy<JsonSerializerSettings>((Func<JsonSerializerSettings>) (() => new VssJsonMediaTypeFormatter().SerializerSettings)).Value).Deserialize<T>((JsonReader) reader2);
      }
    }

    public int VersionId
    {
      get
      {
        int result;
        return !int.TryParse(this.Version.Value, out result) ? 0 : result;
      }
    }

    public string VersionName => this.Version.DisplayValue;

    public InputValue DefinitionData
    {
      get
      {
        InputValue inputValue;
        return this.SourceData.TryGetValue("definition", out inputValue) ? inputValue : (InputValue) null;
      }
    }

    public string DefinitionId
    {
      get
      {
        InputValue inputValue;
        return this.SourceData.TryGetValue("definition", out inputValue) ? inputValue.Value : (string) null;
      }
    }

    public int ReleaseDefinitionId { get; set; }

    public DateTime CreatedOn { get; set; }
  }
}
