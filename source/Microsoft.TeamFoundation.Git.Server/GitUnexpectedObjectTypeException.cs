// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitUnexpectedObjectTypeException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Serializable]
  public class GitUnexpectedObjectTypeException : TeamFoundationServiceException
  {
    public GitUnexpectedObjectTypeException(
      GitObjectType expectedType,
      Sha1Id objectId,
      GitObjectType resolvedType)
      : base(Resources.Format("UnexpectedObjectType", (object) expectedType, (object) objectId, (object) resolvedType))
    {
    }
  }
}
