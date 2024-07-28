// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.AggregateTraversalResult
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public abstract class AggregateTraversalResult : TraversalResultBase
  {
    private List<string> m_details;
    private List<Exception> m_exceptions;

    [DataMember(Name = "dedupVisitCount", EmitDefaultValue = true)]
    public long DedupVisitCount { get; protected internal set; }

    [DataMember(Name = "failureCount", EmitDefaultValue = true)]
    public int FailureCount
    {
      get
      {
        List<string> details = this.m_details;
        return details == null ? 0 : __nonvirtual (details.Count);
      }
    }

    [DataMember(Name = "failures", EmitDefaultValue = false)]
    public string[] FailureDetails
    {
      get
      {
        lock (this)
          return this.m_details == null ? Array.Empty<string>() : this.m_details.ToArray();
      }
      private set
      {
        lock (this)
          this.m_details = ((IEnumerable<string>) value).ToList<string>();
      }
    }

    public Exception[] FailureExceptions
    {
      get
      {
        lock (this)
          return this.m_exceptions == null ? new Exception[this.FailureDetails.Length] : this.m_exceptions.ToArray();
      }
    }

    public TeamFoundationJobExecutionResult AsJobResult
    {
      get
      {
        if (this.Exception != null)
          return TeamFoundationJobExecutionResult.Failed;
        return this.FailureDetails.Length != 0 ? TeamFoundationJobExecutionResult.PartiallySucceeded : TeamFoundationJobExecutionResult.Succeeded;
      }
    }

    public override string StatusMessage
    {
      get
      {
        StringBuilder stringBuilder = new StringBuilder("[" + this.TraversalReason + "]");
        if (!string.IsNullOrWhiteSpace(this.ExtraInfo))
        {
          stringBuilder.Append("[");
          stringBuilder.Append(this.ExtraInfo);
          stringBuilder.Append("]");
        }
        stringBuilder.Append(" ");
        if (this.Exception != null)
        {
          stringBuilder.Append("Error: ");
          stringBuilder.Append(this.Exception.Message);
        }
        else if (this.FailureCount > 0)
        {
          lock (this)
          {
            if (this.m_details != null)
            {
              int count = this.m_details.Count;
              int num = Math.Min(10, count) - 1;
              for (int index = 0; index <= num; ++index)
              {
                stringBuilder.Append(string.Format("[{0}] ", (object) index));
                stringBuilder.Append(this.m_details[index]);
                if (index != num)
                  stringBuilder.Append(", ");
              }
              if (count > 10)
                stringBuilder.Append(string.Format("... ({0} in total)", (object) (this.FailureCount - count)));
            }
          }
        }
        else
          stringBuilder.Append(string.Format("{0} roots successfully visited.", (object) this.VisitCount));
        return stringBuilder.ToString();
      }
    }

    public AggregateTraversalResult(string extraInfo) => this.ExtraInfo = extraInfo;

    public void SetFailure(Exception ex) => this.Exception = ex;

    public int RecordFailure(ITraversalResult dedupResult) => this.RecordDedupResult(dedupResult, true);

    public void RecordSuccess(ITraversalResult dedupResult) => this.RecordDedupResult(dedupResult, false);

    private int RecordDedupResult(ITraversalResult dedupResult, bool addError)
    {
      lock (this)
      {
        if (addError)
        {
          if (this.m_details == null)
            this.m_details = new List<string>();
          this.m_details.Add(dedupResult.StatusMessage);
          if (this.m_exceptions == null)
            this.m_exceptions = new List<Exception>();
          this.m_exceptions.Add(dedupResult.Exception);
        }
        ++this.VisitCount;
        this.DedupVisitCount += dedupResult.VisitCount;
        return this.FailureCount;
      }
    }

    protected abstract string TraversalReason { get; }
  }
}
