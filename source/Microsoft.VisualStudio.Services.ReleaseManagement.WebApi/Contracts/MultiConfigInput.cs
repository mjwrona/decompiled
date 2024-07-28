// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.MultiConfigInput
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AE7F604E-30D7-44A7-BE7B-AB7FB5A67B31
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.dll

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts
{
  [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi", Justification = "Well known word in VSTS")]
  [DataContract]
  public class MultiConfigInput : ParallelExecutionInputBase
  {
    private readonly char[] valueSeparator = new char[1]
    {
      ','
    };

    [DataMember]
    public string Multipliers { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed")]
    public MultiConfigInput(string value, int maxNumberOfAgents = 1, bool continueOnError = true)
      : base(ParallelExecutionTypes.MultiConfiguration, maxNumberOfAgents, continueOnError)
    {
      this.Multipliers = value;
    }

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Optional")]
    public ICollection<string> GetMultiplierVariables() => string.IsNullOrEmpty(this.Multipliers) ? (ICollection<string>) Array.Empty<string>() : (ICollection<string>) ((IEnumerable<string>) Regex.Replace(this.Multipliers, "\\$\\((.*?)\\)", "$1", RegexOptions.IgnoreCase).Split(this.valueSeparator, StringSplitOptions.RemoveEmptyEntries)).Select<string, string>((Func<string, string>) (value => value.Trim())).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).ToList<string>();

    public override bool Equals(ExecutionInput other) => other is MultiConfigInput multiConfigInput && string.Equals(this.Multipliers, multiConfigInput.Multipliers, StringComparison.OrdinalIgnoreCase) && base.Equals(other);

    public override object Clone() => (object) new MultiConfigInput(this.Multipliers, this.MaxNumberOfAgents, this.ContinueOnError);
  }
}
