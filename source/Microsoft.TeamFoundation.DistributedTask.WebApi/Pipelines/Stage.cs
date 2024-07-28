// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Stage
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines.Runtime;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class Stage : IGraphNode
  {
    [DataMember(Name = "Variables", EmitDefaultValue = false)]
    private List<IVariable> m_variables;
    [DataMember(Name = "Phases", EmitDefaultValue = false)]
    private List<PhaseNode> m_phases;
    [DataMember(Name = "DependsOn", EmitDefaultValue = false)]
    private HashSet<string> m_dependsOn;

    public Stage()
    {
    }

    public Stage(string name, IList<PhaseNode> phases, bool skip = false)
    {
      this.Name = name;
      this.Skip = skip;
      if (phases == null || phases.Count <= 0)
        return;
      this.m_phases = new List<PhaseNode>((IEnumerable<PhaseNode>) phases);
    }

    [DataMember(EmitDefaultValue = false)]
    public string Group { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsKnownName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Condition { get; set; }

    [DataMember]
    public bool Skip { get; set; }

    [DataMember]
    public ExclusiveLockType LockBehavior { get; set; }

    public IList<IVariable> Variables
    {
      get
      {
        if (this.m_variables == null)
          this.m_variables = new List<IVariable>();
        return (IList<IVariable>) this.m_variables;
      }
    }

    public IList<PhaseNode> Phases
    {
      get
      {
        if (this.m_phases == null)
          this.m_phases = new List<PhaseNode>();
        return (IList<PhaseNode>) this.m_phases;
      }
    }

    public ISet<string> DependsOn
    {
      get
      {
        if (this.m_dependsOn == null)
          this.m_dependsOn = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return (ISet<string>) this.m_dependsOn;
      }
    }

    void IGraphNode.Validate(PipelineBuildContext context, ValidationResult result)
    {
      if (string.IsNullOrEmpty(this.Condition))
      {
        this.Condition = GraphCondition<StageInstance>.Default;
      }
      else
      {
        StageCondition stageCondition = new StageCondition(this.Condition);
      }
      bool flag;
      if (context.FeatureFlags.TryGetValue("Pipelines.PersistedStages.EnableAutoCreation", out flag) & flag)
      {
        string group = this.Group;
        if (!PathValidation.IsValid(ref group))
        {
          result.Errors.Add(new PipelineValidationError(PipelineStrings.StageGroupInvalid((object) this.Name, (object) PathValidation.MaxGroupPathLength.ToString())));
        }
        else
        {
          this.Group = group;
          int definitionId;
          int buildId;
          if (this.ShouldResolvePersistedStage(context, out definitionId, out buildId))
          {
            IPersistedStageStore persistedStages = context.ResourceStore.PersistedStages;
            if (persistedStages == null)
              throw new ArgumentException("persistedStageStore");
            PersistedStageReference stageRef = new PersistedStageReference();
            stageRef.Name = (ExpressionValue<string>) this.Name;
            stageRef.GroupPath = this.Group;
            stageRef.DefinitionId = definitionId;
            stageRef.BuildId = buildId;
            PersistedStage persistedStage = persistedStages.ResolvePersistedStage(stageRef);
            if (persistedStage != null)
              result.ReferencedResources.PersistedStages.Add(persistedStage.GetPersistedStageReference());
          }
        }
      }
      List<IVariable> variables = this.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) > 0 ? 1 : 0) : 0) != 0)
      {
        List<IVariable> collection = new List<IVariable>();
        foreach (IVariable variable1 in (IEnumerable<IVariable>) this.Variables)
        {
          switch (variable1)
          {
            case Variable variable2:
              if (!PhaseNode.s_nonOverridableVariables.Contains(variable2.Name))
                break;
              continue;
            case VariableGroupReference queue:
              if (context.EnvironmentVersion < 2)
              {
                result.Errors.Add(new PipelineValidationError(PipelineStrings.StageVariableGroupNotSupported((object) this.Name, (object) queue)));
                continue;
              }
              result.ReferencedResources.VariableGroups.Add(queue);
              if (context.BuildOptions.ValidateResources && context.ResourceStore.VariableGroups.Get(queue) == null)
              {
                result.UnauthorizedResources.VariableGroups.Add(queue);
                result.Errors.Add(new PipelineValidationError(PipelineStrings.VariableGroupNotFoundForStage((object) this.Name, (object) queue)));
                break;
              }
              break;
          }
          collection.Add(variable1);
        }
        this.m_variables.Clear();
        this.m_variables.AddRange((IEnumerable<IVariable>) collection);
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      GraphValidator.Validate<PhaseNode>(context, result, Stage.\u003C\u003EO.\u003C0\u003E__JobNameWhenNoNameIsProvided ?? (Stage.\u003C\u003EO.\u003C0\u003E__JobNameWhenNoNameIsProvided = new Func<object, string>(PipelineStrings.JobNameWhenNoNameIsProvided)), this.Name, this.Phases, Stage.\u003C\u003EO.\u003C1\u003E__GetErrorMessage ?? (Stage.\u003C\u003EO.\u003C1\u003E__GetErrorMessage = new GraphValidator.ErrorFormatter(Phase.GetErrorMessage)));
    }

    private bool ShouldResolvePersistedStage(
      PipelineBuildContext context,
      out int definitionId,
      out int buildId)
    {
      definitionId = 0;
      buildId = 0;
      VariableValue variableValue1;
      VariableValue variableValue2;
      return context.BuildOptions.ResolvePersistedStages && this.IsKnownName && context.Variables.TryGetValue(WellKnownDistributedTaskVariables.DefinitionId, out variableValue1) && variableValue1 != null && int.TryParse(variableValue1.Value, out definitionId) && definitionId > 0 && context.Variables.TryGetValue(WellKnownDistributedTaskVariables.BuildId, out variableValue2) && variableValue2 != null && int.TryParse(variableValue2.Value, out buildId);
    }

    internal static string GetErrorMessage(string code, params object[] values)
    {
      switch (code)
      {
        case "NameInvalid":
          return PipelineStrings.StageNameInvalid(values[1]);
        case "NameNotUnique":
          return PipelineStrings.StageNamesMustBeUnique(values[1]);
        case "StartingPointNotFound":
          return PipelineStrings.PipelineNotValidNoStartingStage();
        case "DependencyNotFound":
          return PipelineStrings.StageDependencyNotFound(values[1], values[2]);
        case "GraphContainsCycle":
          return PipelineStrings.StageGraphCycleDetected(values[1], values[2]);
        default:
          throw new NotSupportedException();
      }
    }

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context)
    {
      HashSet<string> dependsOn = this.m_dependsOn;
      // ISSUE: explicit non-virtual call
      if ((dependsOn != null ? (__nonvirtual (dependsOn.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_dependsOn = (HashSet<string>) null;
      List<PhaseNode> phases = this.m_phases;
      // ISSUE: explicit non-virtual call
      if ((phases != null ? (__nonvirtual (phases.Count) == 0 ? 1 : 0) : 0) != 0)
        this.m_phases = (List<PhaseNode>) null;
      List<IVariable> variables = this.m_variables;
      // ISSUE: explicit non-virtual call
      if ((variables != null ? (__nonvirtual (variables.Count) == 0 ? 1 : 0) : 0) == 0)
        return;
      this.m_variables = (List<IVariable>) null;
    }

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context)
    {
      if (this.m_dependsOn == null)
        return;
      this.m_dependsOn = new HashSet<string>((IEnumerable<string>) this.m_dependsOn, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }
  }
}
