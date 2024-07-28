// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.PublicKeyModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using System;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  public class PublicKeyModel
  {
    private IPublicKeyUtility PublicKeyUtility { get; set; }

    public PublicKeyModel(
      SessionToken sessionToken,
      IPublicKeyUtility keyUtility = null,
      string datePattern = "MM/dd/yyyy")
    {
      this.PublicKeyUtility = keyUtility ?? (IPublicKeyUtility) new DefaultPublicKeyUtility();
      this.AuthorizationId = new Guid?(sessionToken.AuthorizationId);
      this.CreatedTime = sessionToken.ValidFrom;
      this.Data = sessionToken.PublicData;
      this.IsValid = sessionToken.IsValid;
      this.Description = HttpUtility.HtmlDecode(sessionToken.DisplayName);
      this.Fingerprint = this.PublicKeyUtility.CalculatePublicKeyFingerprint(this.Data);
      this.FormattedCreatedTime = this.CreatedTime.ToString(datePattern);
    }

    public PublicKeyModel()
    {
    }

    public Guid? AuthorizationId { get; set; }

    public string Description { get; set; }

    public string Data { get; set; }

    public string Fingerprint { get; set; }

    public DateTime CreatedTime { get; set; }

    public string FormattedCreatedTime { get; set; }

    public bool IsValid { get; set; }
  }
}
