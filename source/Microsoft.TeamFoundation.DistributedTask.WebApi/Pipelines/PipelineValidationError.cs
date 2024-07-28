// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineValidationError
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineValidationError
  {
    public PipelineValidationError()
    {
    }

    public PipelineValidationError(string message)
      : this((string) null, message)
    {
    }

    public PipelineValidationError(string code, string message)
    {
      this.Code = code;
      this.Message = message;
    }

    [DataMember(EmitDefaultValue = false)]
    public string Code { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Message { get; set; }

    public static IEnumerable<PipelineValidationError> Create(Exception exception)
    {
      for (int i = 0; i < 50; ++i)
      {
        yield return new PipelineValidationError(exception.Message);
        if (exception.InnerException == null)
          break;
        exception = exception.InnerException;
      }
    }
  }
}
