// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.ComputedSamplingTypeExpression
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  public sealed class ComputedSamplingTypeExpression : IComputedSamplingTypeExpression, IExpression
  {
    private string name;
    private string expression;

    public ComputedSamplingTypeExpression(string name, string expression, string unit)
    {
      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentNullException(nameof (name));
      if (string.IsNullOrWhiteSpace(expression))
        throw new ArgumentNullException(nameof (expression));
      this.Name = name;
      this.Expression = expression;
      this.IsBuiltIn = false;
      this.Unit = unit;
    }

    [JsonConstructor]
    internal ComputedSamplingTypeExpression(
      string name,
      string expression,
      bool isBuiltIn,
      string unit)
    {
      this.name = name;
      this.expression = expression;
      this.IsBuiltIn = isBuiltIn;
      this.Unit = unit;
    }

    public string Name
    {
      get => this.name;
      set => this.name = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof (value));
    }

    public string Expression
    {
      get => this.expression;
      set => this.expression = !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentNullException(nameof (value));
    }

    public bool IsBuiltIn { get; internal set; }

    public string Unit { get; set; }
  }
}
