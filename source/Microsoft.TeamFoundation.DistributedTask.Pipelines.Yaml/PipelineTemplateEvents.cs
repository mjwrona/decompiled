// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.PipelineTemplateEvents
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Events;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml
{
  internal static class PipelineTemplateEvents
  {
    internal static void Setup(TemplateContext context)
    {
      TemplateEvents events = context.Events;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("job", PipelineTemplateEvents.\u003C\u003EO.\u003C0\u003E__Handle_Job_OnMappingStart ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C0\u003E__Handle_Job_OnMappingStart = new OnMappingStartEventHandler(PipelineTemplateEvents.Handle_Job_OnMappingStart)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("job", PipelineTemplateEvents.\u003C\u003EO.\u003C1\u003E__Handle_Job_OnMappingEnd ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C1\u003E__Handle_Job_OnMappingEnd = new OnMappingEndEventHandler(PipelineTemplateEvents.Handle_Job_OnMappingEnd)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("phase", PipelineTemplateEvents.\u003C\u003EO.\u003C2\u003E__Handle_Phase_OnMappingStart ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C2\u003E__Handle_Phase_OnMappingStart = new OnMappingStartEventHandler(PipelineTemplateEvents.Handle_Phase_OnMappingStart)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("phase", PipelineTemplateEvents.\u003C\u003EO.\u003C3\u003E__Handle_Phase_OnMappingEnd ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C3\u003E__Handle_Phase_OnMappingEnd = new OnMappingEndEventHandler(PipelineTemplateEvents.Handle_Phase_OnMappingEnd)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("stage", PipelineTemplateEvents.\u003C\u003EO.\u003C4\u003E__Handle_Stage_OnMappingStart ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C4\u003E__Handle_Stage_OnMappingStart = new OnMappingStartEventHandler(PipelineTemplateEvents.Handle_Stage_OnMappingStart)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("stage", PipelineTemplateEvents.\u003C\u003EO.\u003C5\u003E__Handle_Stage_OnMappingEnd ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C5\u003E__Handle_Stage_OnMappingEnd = new OnMappingEndEventHandler(PipelineTemplateEvents.Handle_Stage_OnMappingEnd)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("variable", PipelineTemplateEvents.\u003C\u003EO.\u003C6\u003E__Handle_Variable_OnMappingValue ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C6\u003E__Handle_Variable_OnMappingValue = new OnMappingValueEventHandler(PipelineTemplateEvents.Handle_Variable_OnMappingValue)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("variables", PipelineTemplateEvents.\u003C\u003EO.\u003C7\u003E__Handle_Variables_OnMappingKey ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C7\u003E__Handle_Variables_OnMappingKey = new OnMappingKeyEventHandler(PipelineTemplateEvents.Handle_Variables_OnMappingKey)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("variables", PipelineTemplateEvents.\u003C\u003EO.\u003C8\u003E__Handle_Variables_OnMappingValue ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C8\u003E__Handle_Variables_OnMappingValue = new OnMappingValueEventHandler(PipelineTemplateEvents.Handle_Variables_OnMappingValue)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      events.Listen("variables", PipelineTemplateEvents.\u003C\u003EO.\u003C9\u003E__Handle_Variables_OnSequenceItem ?? (PipelineTemplateEvents.\u003C\u003EO.\u003C9\u003E__Handle_Variables_OnSequenceItem = new OnSequenceItemEventHandler(PipelineTemplateEvents.Handle_Variables_OnSequenceItem)));
    }

    private static void Handle_Job_OnMappingStart(object sender, OnMappingStartEventArgs args) => ParserUtil.GetUserVariables(args.Context).PushScope();

    private static void Handle_Job_OnMappingEnd(object sender, OnMappingEndEventArgs args) => ParserUtil.GetUserVariables(args.Context).PopScope();

    private static void Handle_Phase_OnMappingStart(object sender, OnMappingStartEventArgs args) => ParserUtil.GetUserVariables(args.Context).PushScope();

    private static void Handle_Phase_OnMappingEnd(object sender, OnMappingEndEventArgs args) => ParserUtil.GetUserVariables(args.Context).PopScope();

    private static void Handle_Stage_OnMappingStart(object sender, OnMappingStartEventArgs args) => ParserUtil.GetUserVariables(args.Context).PushScope();

    private static void Handle_Stage_OnMappingEnd(object sender, OnMappingEndEventArgs args) => ParserUtil.GetUserVariables(args.Context).PopScope();

    private static void Handle_Variable_OnMappingValue(object sender, OnMappingValueEventArgs args)
    {
      if (!string.Equals(args.Key?.Value, "name", StringComparison.OrdinalIgnoreCase) || !(args.Value is ReadOnlyLiteralToken name))
        return;
      PipelineTemplateEvents.ValidateName(args.Context, name);
    }

    private static void Handle_Variables_OnMappingKey(object sender, OnMappingKeyEventArgs args) => PipelineTemplateEvents.ValidateName(args.Context, args.Key);

    private static void Handle_Variables_OnMappingValue(object sender, OnMappingValueEventArgs args)
    {
      ReadOnlyLiteralToken key = args.Key;
      if (!PipelineTemplateEvents.IsValidName(args.Context, key, out string _) || !(args.Value is ReadOnlyLiteralToken onlyLiteralToken))
        return;
      ParserUtil.GetUserVariables(args.Context)[key.Value] = onlyLiteralToken.Value;
    }

    private static void Handle_Variables_OnSequenceItem(object sender, OnSequenceItemEventArgs args)
    {
      if (!(args.Item is ReadOnlyMappingToken source) || source.Count <= 1)
        return;
      KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken> keyValuePair1 = source[0];
      if (!string.Equals(keyValuePair1.Key?.Value, "name", StringComparison.OrdinalIgnoreCase) || !(keyValuePair1.Value is ReadOnlyLiteralToken name) || !PipelineTemplateEvents.IsValidName(args.Context, name, out string _))
        return;
      foreach (KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken> keyValuePair2 in source.Skip<KeyValuePair<ReadOnlyLiteralToken, IReadOnlyTemplateToken>>(1))
      {
        if (string.Equals(keyValuePair2.Key?.Value, "value", StringComparison.OrdinalIgnoreCase) && keyValuePair2.Value is ReadOnlyLiteralToken onlyLiteralToken)
        {
          ParserUtil.GetUserVariables(args.Context)[name.Value] = onlyLiteralToken.Value;
          break;
        }
      }
    }

    private static bool IsValidName(
      TemplateContext context,
      ReadOnlyLiteralToken name,
      out string reason)
    {
      if (string.IsNullOrEmpty(name?.Value))
      {
        reason = YamlStrings.ExpectedNonEmptyString();
        return false;
      }
      if (ParserUtil.GetSystemVariables(context).ContainsKey(name.Value))
      {
        reason = YamlStrings.CannotOverrideSystemVariable((object) name.Value);
        return false;
      }
      reason = (string) null;
      return true;
    }

    private static bool ValidateName(TemplateContext context, ReadOnlyLiteralToken name)
    {
      string reason;
      if (PipelineTemplateEvents.IsValidName(context, name, out reason))
        return true;
      context.Error((IReadOnlyTemplateToken) name, reason);
      return false;
    }
  }
}
