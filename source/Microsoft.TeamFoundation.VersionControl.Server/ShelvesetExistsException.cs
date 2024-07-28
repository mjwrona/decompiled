// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ShelvesetExistsException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class ShelvesetExistsException : ServerException
  {
    public ShelvesetExistsException(string shelvesetName, string ownerName)
      : base(Resources.Format(nameof (ShelvesetExistsException), (object) WorkspaceSpec.Combine(shelvesetName, ownerName)))
    {
    }

    public ShelvesetExistsException(
      IVssRequestContext requestContext,
      SqlException ex,
      SqlError sqlError)
      : this(TeamFoundationServiceException.ExtractString(sqlError, "shelveSet"), ServerException.ExtractIdentityName(requestContext, sqlError, "identityId"))
    {
    }
  }
}
