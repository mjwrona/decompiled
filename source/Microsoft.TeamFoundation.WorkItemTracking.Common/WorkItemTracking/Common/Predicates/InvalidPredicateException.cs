// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates.InvalidPredicateException
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common.Predicates
{
  public class InvalidPredicateException : InvalidOperationException
  {
    public InvalidPredicateException()
      : base("PredicateError")
    {
    }

    public InvalidPredicateException(string message)
      : base(message)
    {
    }

    public InvalidPredicateException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public InvalidPredicateException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
