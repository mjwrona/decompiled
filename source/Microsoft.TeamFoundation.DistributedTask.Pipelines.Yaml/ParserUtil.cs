// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ParserUtil
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal static class ParserUtil
  {
    public static IDictionary<string, string> GetPublicSystemVariables(TemplateContext context) => context.State["publicSystemVariables"] as IDictionary<string, string>;

    public static IDictionary<string, VariableValue> GetSystemVariables(TemplateContext context) => context.State["systemVariables"] as IDictionary<string, VariableValue>;

    public static UserVariables GetUserVariables(TemplateContext context) => context.State["userVariables"] as UserVariables;

    public static void SetPublicSystemVariables(
      TemplateContext context,
      IDictionary<string, string> publicSystemVariables)
    {
      context.State[nameof (publicSystemVariables)] = (object) publicSystemVariables;
    }

    public static void SetSystemVariables(
      TemplateContext context,
      IDictionary<string, VariableValue> systemVariables)
    {
      context.State[nameof (systemVariables)] = (object) systemVariables;
    }

    public static void SetUserVariables(TemplateContext context, UserVariables userVariables) => context.State[nameof (userVariables)] = (object) userVariables;
  }
}
