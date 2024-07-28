// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IVariableGroupSecretsHelper
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public interface IVariableGroupSecretsHelper
  {
    void StoreSecrets(IVssRequestContext context, VariableGroup group);

    void UpdateSecrets(IVssRequestContext context, VariableGroup group);

    void ReadSecrets(IVssRequestContext context, Guid projectId, VariableGroup group);

    void DeleteSecrets(IVssRequestContext context, Guid projectId, int groupId);

    void DeleteSecrets(IVssRequestContext context, int groupId);

    void MigrateSecretsFromProjectToCollectionLevel(
      IVssRequestContext context,
      Guid projectId,
      int groupId);
  }
}
