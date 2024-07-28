// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRefUpdateRejectedByPluginException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  [Serializable]
  public class GitRefUpdateRejectedByPluginException : TeamFoundationServiceException
  {
    public GitRefUpdateRejectedByPluginException(string pluginMessage)
    {
      string message;
      if (!string.IsNullOrEmpty(pluginMessage))
        message = string.Format("{0} {1}", (object) Resources.Get("RefRejectedByPlugin"), (object) Resources.Format("CustomMessage", (object) pluginMessage));
      else
        message = string.Format("{0}.", (object) Resources.Get("RefRejectedByPlugin"));
      // ISSUE: explicit constructor call
      base.\u002Ector(message);
    }
  }
}
