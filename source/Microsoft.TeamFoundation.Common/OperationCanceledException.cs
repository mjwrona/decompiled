// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.OperationCanceledException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation
{
  [Obsolete("This class has been deprecated and will be removed from a future release. See System.OperationCanceledException instead.", false)]
  [ExceptionMapping("0.0", "3.0", "OperationCanceledException", "Microsoft.TeamFoundation.OperationCanceledException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class OperationCanceledException : Exception
  {
    public OperationCanceledException()
    {
    }

    public OperationCanceledException(string message)
      : base(message)
    {
    }

    public OperationCanceledException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected OperationCanceledException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
