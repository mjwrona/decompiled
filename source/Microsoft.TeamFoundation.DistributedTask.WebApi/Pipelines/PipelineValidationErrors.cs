// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.PipelineValidationErrors
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class PipelineValidationErrors : IEnumerable<PipelineValidationError>, IEnumerable
  {
    private readonly List<PipelineValidationError> m_errors = new List<PipelineValidationError>();
    private readonly int m_maxErrors;
    private readonly int m_maxMessageLength;

    public PipelineValidationErrors()
    {
    }

    public PipelineValidationErrors(int maxErrors, int maxMessageLength)
    {
      this.m_maxErrors = maxErrors;
      this.m_maxMessageLength = maxMessageLength;
    }

    public int Count => this.m_errors.Count;

    public int MaxErrors => this.m_maxErrors;

    public int MaxMessageLength => this.m_maxMessageLength;

    public void Add(string message) => this.Add(new PipelineValidationError(message));

    public void Add(Exception ex) => this.Add((string) null, ex);

    public void Add(string messagePrefix, Exception ex)
    {
      for (int index = 0; index < 50; ++index)
      {
        this.Add(new PipelineValidationError(!string.IsNullOrEmpty(messagePrefix) ? messagePrefix + " " + ex.Message : ex.Message));
        if (ex.InnerException == null)
          break;
        ex = ex.InnerException;
      }
    }

    public void Add(string messagePrefix, PipelineValidationError error)
    {
      string message = !string.IsNullOrEmpty(messagePrefix) ? messagePrefix + " " + error.Message : error.Message;
      this.Add(new PipelineValidationError(error.Code, message));
    }

    public void Add(IEnumerable<PipelineValidationError> errors)
    {
      foreach (PipelineValidationError error in errors)
        this.Add(error);
    }

    public void Add(PipelineValidationError error)
    {
      if (this.m_maxErrors > 0 && this.m_errors.Count >= this.m_maxErrors)
        return;
      if (this.m_maxMessageLength > 0)
      {
        int? length = error.Message?.Length;
        int maxMessageLength = this.m_maxMessageLength;
        if (length.GetValueOrDefault() > maxMessageLength & length.HasValue)
          error = new PipelineValidationError(error.Code, error.Message.Substring(0, this.m_maxMessageLength) + "[...]");
      }
      this.m_errors.Add(error);
    }

    public void Clear() => this.m_errors.Clear();

    public IEnumerator<PipelineValidationError> GetEnumerator() => ((IEnumerable<PipelineValidationError>) this.m_errors).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable) this.m_errors).GetEnumerator();
  }
}
