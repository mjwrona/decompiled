// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineValidationException
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class PipelineValidationException : PipelineException
  {
    private List<PipelineValidationError> m_errors;

    public PipelineValidationException()
      : this(PipelineStrings.PipelineNotValid())
    {
    }

    public PipelineValidationException(IEnumerable<PipelineValidationError> errors)
      : this(PipelineStrings.PipelineNotValidWithErrors((object) string.Join(" ", (errors ?? Enumerable.Empty<PipelineValidationError>()).Take<PipelineValidationError>(2).Select<PipelineValidationError, string>((Func<PipelineValidationError, string>) (e => e.Message)))))
    {
      this.m_errors = new List<PipelineValidationError>(errors ?? Enumerable.Empty<PipelineValidationError>());
    }

    public PipelineValidationException(string message)
      : base(message)
    {
    }

    public PipelineValidationException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public IList<PipelineValidationError> Errors
    {
      get
      {
        if (this.m_errors == null)
          this.m_errors = new List<PipelineValidationError>();
        return (IList<PipelineValidationError>) this.m_errors;
      }
    }

    protected PipelineValidationException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
