// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AttachCollectionException
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class AttachCollectionException : TeamFoundationServiceException
  {
    public AttachCollectionException()
    {
    }

    public AttachCollectionException(string message)
      : base(message)
    {
    }

    public AttachCollectionException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AttachCollectionException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
