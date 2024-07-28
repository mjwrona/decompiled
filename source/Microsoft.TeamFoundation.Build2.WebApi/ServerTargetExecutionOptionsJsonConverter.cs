// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.ServerTargetExecutionOptionsJsonConverter
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  internal sealed class ServerTargetExecutionOptionsJsonConverter : 
    TypePropertyJsonConverter<ServerTargetExecutionOptions>
  {
    protected override ServerTargetExecutionOptions GetInstance(Type objectType)
    {
      if (objectType == typeof (ServerTargetExecutionType))
        return new ServerTargetExecutionOptions();
      return objectType == typeof (VariableMultipliersServerExecutionOptions) ? (ServerTargetExecutionOptions) new VariableMultipliersServerExecutionOptions() : base.GetInstance(objectType);
    }

    protected override ServerTargetExecutionOptions GetInstance(int targetType)
    {
      if (targetType == 0)
        return new ServerTargetExecutionOptions();
      return targetType == 1 ? (ServerTargetExecutionOptions) new VariableMultipliersServerExecutionOptions() : (ServerTargetExecutionOptions) null;
    }
  }
}
