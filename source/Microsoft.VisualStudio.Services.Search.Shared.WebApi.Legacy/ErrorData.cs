// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.ErrorData
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8C7C9E57-44B4-4654-9458-CC8B59C2B681
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy.dll

using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy
{
  [DataContract]
  public class ErrorData : SearchSecuredObject
  {
    [DataMember(Name = "errorCode")]
    public string ErrorCode { get; set; }

    [DataMember(Name = "errorType")]
    public ErrorType ErrorType { get; set; }

    [DataMember(Name = "errorMessage")]
    public string ErrorMessage { get; set; }

    public string ToString(int indentLevel)
    {
      StringBuilder sb = new StringBuilder();
      string indentSpacing = Extensions.GetIndentSpacing(indentLevel);
      sb.Append(indentSpacing, "Error Code: ").AppendLine(this.ErrorCode);
      sb.Append(indentSpacing, "Error Type: ").Append((object) this.ErrorType);
      if (!string.IsNullOrWhiteSpace(this.ErrorMessage))
        sb.Append(indentSpacing, "Error Message: ").Append(this.ErrorMessage);
      return sb.ToString();
    }

    public override string ToString() => this.ToString(0);
  }
}
