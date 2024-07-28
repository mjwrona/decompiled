// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Teams.Service.TeamSecurityException
// Assembly: Microsoft.Azure.Devops.Teams.Service, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 643BB522-27FA-4CE7-8A1E-31D95F8AC409
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Teams.Service.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Devops.Teams.Service
{
  [Serializable]
  public class TeamSecurityException : TeamFoundationServiceException
  {
    public TeamSecurityException()
    {
    }

    public TeamSecurityException(string message)
      : base(message)
    {
    }

    public TeamSecurityException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected TeamSecurityException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public static TeamSecurityException CreateNoReadPermissionException() => new TeamSecurityException(Resources.TeamPermission_ReadError());

    public static TeamSecurityException CreateGenericWritePermissionException() => new TeamSecurityException(Resources.TeamPermission_WriteError());

    public static TeamSecurityException CreateAdminOperationsBlockedException() => new TeamSecurityException(Resources.TeamServiceSafeguardBlockCreateTeamAndAdminOperations_Error());
  }
}
