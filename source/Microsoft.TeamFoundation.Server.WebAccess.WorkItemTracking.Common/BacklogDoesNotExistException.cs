// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.BacklogDoesNotExistException
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Net;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  [Serializable]
  public class BacklogDoesNotExistException : TeamFoundationServiceException
  {
    public BacklogDoesNotExistException(string backlogIdOrName)
      : base(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.BacklogNotFoundException, (object) backlogIdOrName), HttpStatusCode.NotFound)
    {
    }
  }
}
