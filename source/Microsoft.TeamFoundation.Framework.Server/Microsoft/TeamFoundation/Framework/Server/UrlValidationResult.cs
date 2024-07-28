// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlValidationResult
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class UrlValidationResult
  {
    private UrlValidationResult()
    {
    }

    public bool IsValid { get; private set; }

    public string Error { get; private set; }

    public UrlValidationFailureReason Reason { get; private set; }

    public static UrlValidationResult Success() => new UrlValidationResult()
    {
      IsValid = true,
      Error = string.Empty,
      Reason = UrlValidationFailureReason.None
    };

    public static UrlValidationResult Failure(
      UrlValidationFailureReason reason,
      string errorMessage)
    {
      return new UrlValidationResult()
      {
        IsValid = false,
        Error = errorMessage,
        Reason = reason
      };
    }
  }
}
