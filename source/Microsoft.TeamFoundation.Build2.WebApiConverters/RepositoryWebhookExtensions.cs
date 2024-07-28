// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.RepositoryWebhookExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class RepositoryWebhookExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.RepositoryWebhook ToWebApiRepositoryWebhook(
      this Microsoft.TeamFoundation.Build2.Server.RepositoryWebhook srvRepositoryWebhook)
    {
      if (srvRepositoryWebhook == null)
        return (Microsoft.TeamFoundation.Build.WebApi.RepositoryWebhook) null;
      return new Microsoft.TeamFoundation.Build.WebApi.RepositoryWebhook()
      {
        Name = srvRepositoryWebhook.Name,
        Types = srvRepositoryWebhook.Types.Select<Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType, Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType>((Func<Microsoft.TeamFoundation.Build2.Server.DefinitionTriggerType, Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType>) (tt => tt.ToWebApiDefinitionTriggerType())).ToList<Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType>(),
        Url = srvRepositoryWebhook.Url
      };
    }
  }
}
