// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.RequiresHostedException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [Serializable]
  public class RequiresHostedException : TeamFoundationServerException
  {
    public RequiresHostedException(string message)
      : base(message)
    {
    }

    public RequiresHostedException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected RequiresHostedException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
