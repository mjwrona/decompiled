// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.DefaultTeamNotFoundException
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ExceptionMapping("0.0", "3.0", "DefaultTeamNotFoundException", "Microsoft.TeamFoundation.Core.WebApi.DefaultTeamNotFoundException, Microsoft.TeamFoundation.Core.WebApi, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class DefaultTeamNotFoundException : VssServiceException
  {
    public DefaultTeamNotFoundException(string message)
      : base(message)
    {
    }
  }
}
