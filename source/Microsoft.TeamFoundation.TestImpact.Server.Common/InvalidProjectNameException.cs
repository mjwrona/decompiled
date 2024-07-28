// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestImpact.Server.Common.InvalidProjectNameException
// Assembly: Microsoft.TeamFoundation.TestImpact.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 079E4AEE-0642-4BDD-8B76-CECF38EBB798
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestImpact.Server.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestImpact.Server.Common
{
  [CLSCompliant(false)]
  [Serializable]
  public class InvalidProjectNameException : TeamFoundationServerException
  {
    public InvalidProjectNameException()
    {
    }

    public InvalidProjectNameException(string message)
      : base(message)
    {
    }

    public InvalidProjectNameException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected InvalidProjectNameException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
