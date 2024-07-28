// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.ELeadException
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Common
{
  [Obsolete("Please use a strongly typed exception.")]
  [ExceptionMapping("0.0", "3.0", "ELeadException", "Microsoft.TeamFoundation.Common.ELeadException, Microsoft.TeamFoundation.Common, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class ELeadException : ApplicationException
  {
    public ELeadException()
    {
    }

    public ELeadException(string message)
      : base(message)
    {
    }

    public ELeadException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ELeadException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
