// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.DefinitionEnvironmentData
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.Enumerations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class DefinitionEnvironmentData
  {
    public DefinitionEnvironmentData() => this.Steps = (IList<DefinitionEnvironmentStepData>) new List<DefinitionEnvironmentStepData>();

    public int Id { get; set; }

    public int Rank { get; set; }

    public string Name { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Setter needed for deserialization")]
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
    public IList<string> EnvironmentTriggers { get; set; }

    public IList<DefinitionEnvironmentStepData> Steps { get; private set; }

    public DefinitionEnvironmentData DeepClone()
    {
      DefinitionEnvironmentData definitionEnvironmentData = this.ShallowClone();
      foreach (DefinitionEnvironmentStepData step in (IEnumerable<DefinitionEnvironmentStepData>) this.Steps)
        definitionEnvironmentData.Steps.Add(step.DeepClone());
      return definitionEnvironmentData;
    }

    public DefinitionEnvironmentData ShallowClone() => new DefinitionEnvironmentData()
    {
      Id = this.Id,
      Rank = this.Rank,
      Name = this.Name,
      EnvironmentTriggers = this.EnvironmentTriggers == null || !this.EnvironmentTriggers.Any<string>() ? (IList<string>) null : (IList<string>) new List<string>((IEnumerable<string>) this.EnvironmentTriggers)
    };

    public IEnumerable<DefinitionEnvironmentStepData> GetDefinitionEnvironmentSteps(
      EnvironmentStepType stepType)
    {
      return this.Steps != null ? this.Steps.Where<DefinitionEnvironmentStepData>((Func<DefinitionEnvironmentStepData, bool>) (step => step.StepType == stepType)) : (IEnumerable<DefinitionEnvironmentStepData>) null;
    }
  }
}
