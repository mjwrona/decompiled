// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UnexpectedDatabaseResultException
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Serializable]
  public class UnexpectedDatabaseResultException : TeamFoundationServiceException
  {
    public UnexpectedDatabaseResultException(string storedProcedure)
      : base(FrameworkResources.UnexpectedDatabaseResultException((object) storedProcedure))
    {
      this.LogException = true;
      this.EventId = TeamFoundationEventId.UnexpectedDatabaseResultException;
    }

    protected UnexpectedDatabaseResultException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.EventId = TeamFoundationEventId.UnexpectedDatabaseResultException;
    }
  }
}
