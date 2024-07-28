// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules.RuleValueValidationResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessMetadata.Rules
{
  public class RuleValueValidationResult
  {
    public RuleValueValidationResult(
      RuleValueValidationStatus status,
      string transformedValue,
      string errorMessage)
    {
      this.Status = status;
      this.TransformedValue = transformedValue;
      this.ErrorMessage = errorMessage;
    }

    public RuleValueValidationStatus Status { get; private set; }

    public string TransformedValue { get; private set; }

    public string ErrorMessage { get; private set; }
  }
}
