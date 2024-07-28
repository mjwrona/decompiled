// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters.ReleasePipelineConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Converters
{
  public static class ReleasePipelineConverter
  {
    internal static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PipelineProcess FromWebApiPipeline(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess process)
    {
      if (process == null)
        throw new ArgumentNullException(nameof (process));
      if (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.PipelineProcessTypes.Yaml != process.Type)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PipelineProcess) new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DesignerPipelineProcess();
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcess yamlPipelineProcess1 = process as Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcess;
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource fileSource = ReleasePipelineConverter.FromWebApiPipelineFileSource(yamlPipelineProcess1.FileSource);
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess yamlPipelineProcess2 = new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess(yamlPipelineProcess1.Filename, fileSource);
      ReleasePipelineConverter.CopyErrors(yamlPipelineProcess2.Errors, yamlPipelineProcess1.Errors);
      return (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PipelineProcess) yamlPipelineProcess2;
    }

    internal static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess ToWebApiPipeline(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PipelineProcess pipeline)
    {
      if (pipeline == null)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) null;
      if (Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.PipelineProcessTypes.Yaml != pipeline.Type)
        return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DesignerPipelineProcess();
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess yamlPipelineProcess = pipeline as Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlPipelineProcess;
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcess webApiPipeline = new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcess();
      webApiPipeline.Filename = yamlPipelineProcess.FileName;
      webApiPipeline.FileSource = ReleasePipelineConverter.ToWebApiPipelineFileSource(yamlPipelineProcess.FileSource);
      ReleasePipelineConverter.CopyErrors(webApiPipeline.Errors, yamlPipelineProcess.Errors);
      return (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.PipelineProcess) webApiPipeline;
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource FromWebApiPipelineFileSource(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlFileSource fileSource)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource yamlFileSource = fileSource != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource((Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations.YamlFileSourceTypes) fileSource.Type) : throw new ArgumentNullException(nameof (fileSource));
      foreach (KeyValuePair<string, YamlSourceReference> keyValuePair in (IEnumerable<KeyValuePair<string, YamlSourceReference>>) fileSource.SourceReference)
      {
        YamlSourceReference yamlSourceReference = keyValuePair.Value;
        if (yamlSourceReference != null)
        {
          InputValue inputValue = new InputValue()
          {
            Value = yamlSourceReference.Id,
            DisplayValue = yamlSourceReference.Name
          };
          yamlFileSource.SourceData.Add(keyValuePair.Key, inputValue);
        }
      }
      return yamlFileSource;
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlFileSource ToWebApiPipelineFileSource(
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource pipelineFileSource)
    {
      return new Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlFileSource((Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.YamlFileSourceTypes) pipelineFileSource.Type)
      {
        SourceReference = (IDictionary<string, YamlSourceReference>) pipelineFileSource.SourceData.ToDictionary<KeyValuePair<string, InputValue>, string, YamlSourceReference>((Func<KeyValuePair<string, InputValue>, string>) (data => data.Key), (Func<KeyValuePair<string, InputValue>, YamlSourceReference>) (data => ReleasePipelineConverter.ToArtifactSourceReference(data.Value)))
      };
    }

    private static YamlSourceReference ToArtifactSourceReference(InputValue data) => new YamlSourceReference()
    {
      Id = data.Value,
      Name = data.DisplayValue
    };

    private static void CopyErrors(IList<string> dest, IList<string> source)
    {
      if (dest == null)
        dest = (IList<string>) new List<string>();
      foreach (string str in (IEnumerable<string>) source)
        dest.Add(str);
    }
  }
}
