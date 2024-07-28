// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.Exceptions.ReorderWorkItemsInvalidOperationsException
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Work.WebApi.Exceptions
{
  [Serializable]
  public class ReorderWorkItemsInvalidOperationsException : ArgumentException
  {
    public ReorderWorkItemsInvalidOperationsException(string message)
      : base(message)
    {
    }

    public ReorderWorkItemsInvalidOperationsException(
      string message,
      string paramName,
      Exception innerException)
      : base(message, paramName, innerException)
    {
    }

    public ReorderWorkItemsInvalidOperationsException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
