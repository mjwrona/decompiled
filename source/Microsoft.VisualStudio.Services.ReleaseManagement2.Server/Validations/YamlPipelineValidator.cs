// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.YamlPipelineValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class YamlPipelineValidator
  {
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We need to store all exception.")]
    internal static void Validate(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlPipelineProcess pipeline)
    {
      if (pipeline == null)
        throw new ArgumentNullException(nameof (pipeline));
      pipeline.Errors.Clear();
      try
      {
        Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource source = YamlPipelineValidator.FromWebApiPipelineFileSource(pipeline.FileSource);
        YamlPipelineValidator.ValidatePipelineFile(pipeline.Filename, source);
      }
      catch (Exception ex)
      {
        pipeline.Errors.Add(ex.Message);
        if (ex.InnerException == null)
          return;
        pipeline.Errors.Add(ex.InnerException.Message);
      }
    }

    private static void ValidatePipelineFile(string filePath, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource source)
    {
      if (string.IsNullOrEmpty(filePath))
        throw new ArgumentException(filePath);
      if (source.Type != YamlFileSourceTypes.TFSGit)
        throw new InvalidRequestException(Resources.PipelineResourceNotValid);
    }

    private static Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource FromWebApiPipelineFileSource(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.YamlFileSource fileSource)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource yamlFileSource = fileSource != null ? new Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.YamlFileSource((YamlFileSourceTypes) fileSource.Type) : throw new ArgumentNullException(nameof (fileSource));
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
  }
}
