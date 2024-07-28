// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Policy.Server.PolicyBaseComponent
// Assembly: Microsoft.TeamFoundation.Policy.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C7B03386-B27B-4823-BB4F-89F7D7E42DDD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Policy.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Policy.Server
{
  internal class PolicyBaseComponent : TeamFoundationSqlResourceComponent
  {
    private static readonly Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static PolicyBaseComponent()
    {
      PolicyBaseComponent.s_sqlExceptionFactories.Add(1560001, new SqlExceptionFactory(typeof (PolicyConfigurationNotFoundException)));
      PolicyBaseComponent.s_sqlExceptionFactories.Add(1560002, new SqlExceptionFactory(typeof (PolicyOperationFailedException)));
      PolicyBaseComponent.s_sqlExceptionFactories.Add(1560003, new SqlExceptionFactory(typeof (PolicyTypeCannotBeChangedException)));
      PolicyBaseComponent.s_sqlExceptionFactories.Add(1560005, new SqlExceptionFactory(typeof (PolicyConfigurationUpdatedByAnotherRequestException)));
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) PolicyBaseComponent.s_sqlExceptionFactories;
  }
}
