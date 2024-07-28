// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TeamDoesNotExistWithNameException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [Serializable]
  public class TeamDoesNotExistWithNameException : TeamFoundationServiceException
  {
    public TeamDoesNotExistWithNameException()
    {
    }

    public TeamDoesNotExistWithNameException(string teamName, Exception innerException)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, WACommonResources.TeamDoesNotExistOrNoAccess, (object) teamName), innerException)
    {
    }

    public TeamDoesNotExistWithNameException(string teamName)
      : this(teamName, (Exception) null)
    {
    }

    protected TeamDoesNotExistWithNameException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
