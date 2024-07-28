// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions.SecureTokenActionFactory
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server.SecureToken.Actions
{
  public class SecureTokenActionFactory
  {
    private Lazy<SymmetricKeySignedJwtTokenIssuer> symmetricKeySignedTokenIssuer = new Lazy<SymmetricKeySignedJwtTokenIssuer>();
    private Lazy<SymmetricKeySignedJwtTokenValidator> symmetricKeySignedTokenValidator = new Lazy<SymmetricKeySignedJwtTokenValidator>();

    public ISecureTokenIssuer DefaultSecureTokenIssuer => (ISecureTokenIssuer) this.symmetricKeySignedTokenIssuer.Value;

    public ISecureTokenValidator DefaultSecureTokenValidator => (ISecureTokenValidator) this.symmetricKeySignedTokenValidator.Value;
  }
}
