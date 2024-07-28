// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.AccessDeniedException
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [Serializable]
  public class AccessDeniedException : TeamFoundationServerException
  {
    public AccessDeniedException()
    {
    }

    public AccessDeniedException(string message)
      : base(message)
    {
    }

    public AccessDeniedException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected AccessDeniedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
