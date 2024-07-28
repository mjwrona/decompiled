// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingOperationHandlerNotFoundException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class ServicingOperationHandlerNotFoundException : TeamFoundationServerException
  {
    public ServicingOperationHandlerNotFoundException()
    {
    }

    public ServicingOperationHandlerNotFoundException(string message)
      : base(message)
    {
    }

    public ServicingOperationHandlerNotFoundException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected ServicingOperationHandlerNotFoundException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
