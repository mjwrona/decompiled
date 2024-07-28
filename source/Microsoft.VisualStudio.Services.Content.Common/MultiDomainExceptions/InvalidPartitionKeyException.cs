// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions.InvalidPartitionKeyException
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common.MultiDomainExceptions
{
  [Serializable]
  public class InvalidPartitionKeyException : VssServiceException
  {
    public InvalidPartitionKeyException(string message)
      : base(message)
    {
    }

    public InvalidPartitionKeyException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
