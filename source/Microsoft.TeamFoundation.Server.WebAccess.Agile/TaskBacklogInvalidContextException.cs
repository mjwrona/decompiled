// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.TaskBacklogInvalidContextException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile
{
  [Serializable]
  internal class TaskBacklogInvalidContextException : BacklogInvalidContextException
  {
    public TaskBacklogInvalidContextException()
    {
    }

    protected TaskBacklogInvalidContextException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
