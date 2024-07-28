// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.WebApi.Violation
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 78BC9F0A-6262-4C96-81AF-14E523464B20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.DevSecOps.WebApi
{
  [DataContract]
  public class Violation
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Guid Id { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FileUri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ErrorCode { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string RuleName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string SubRuleId { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MatchDetails { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MatchContent { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MatchContentHash { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MatchSecretHash { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int LineNumber { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int StartColumn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public int EndColumn { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string MatchState { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string LogicalLocation { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public RuleConfidenceLevel ConfidenceLevel { get; set; }

    public bool IsValid => this.MatchState == "Valid" || this.MatchState == "NotSuppressed";

    public Violation() => this.MatchState = "Valid";

    public override string ToString() => DevSecOpsWebApiResources.ViolationText((object) this.FileUri, (object) this.LineNumber, (object) this.StartColumn, (object) this.EndColumn, (object) this.ErrorCode, (object) this.RuleName);

    public string ToJson() => "{ \"resultId\": \"" + (this.Id.ToString() ?? string.Empty) + "\", \"ruleId\": \"" + (this.ErrorCode ?? string.Empty) + "\", \"subRuleId\": \"" + (this.SubRuleId ?? string.Empty) + "\", \"ruleName\": \"" + this.RuleName + "\", \"fileUri\": \"" + (this.FileUri ?? string.Empty) + "\", \"matchState\": \"" + this.MatchState + "\", " + string.Format("\"startLine\": \"{0}\", ", (object) this.LineNumber) + string.Format("\"startColumn\": \"{0}\", ", (object) this.StartColumn) + string.Format("\"endColumn\": \"{0}\", ", (object) this.EndColumn) + "\"saltedSecretHash\": \"" + (this.MatchSecretHash ?? string.Empty) + "\", \"saltedMatchTextHash\": \"" + (this.MatchContentHash ?? string.Empty) + "\", \"logicalLocation\": \"" + (this.LogicalLocation ?? string.Empty) + "\", \"confidenceLevel\": \"" + this.ConfidenceLevel.ToString() + "\" }";
  }
}
