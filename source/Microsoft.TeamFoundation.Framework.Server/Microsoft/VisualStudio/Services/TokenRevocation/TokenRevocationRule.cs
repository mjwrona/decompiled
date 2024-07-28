// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.TokenRevocationRule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  [DataContract]
  public class TokenRevocationRule
  {
    private string scopes;
    private string[] scopesList;

    [DataMember]
    public Guid RuleId { get; private set; }

    [DataMember]
    public Guid? IdentityId { get; private set; }

    [DataMember]
    public string Scopes
    {
      get => this.scopes;
      private set
      {
        this.scopes = this.scopes == null ? value : throw new Exception("Scopes cannot be changed");
        if (string.IsNullOrWhiteSpace(this.scopes))
          return;
        this.scopesList = this.scopes.Split(',');
      }
    }

    [DataMember]
    public TokenRevocationRuleType RuleType { get; set; }

    [DataMember]
    public DateTime? CreatedBefore { get; private set; }

    [DataMember]
    public Guid? HostId { get; set; }

    [DataMember]
    public DateTime ExpirationDate { get; protected internal set; }

    [DataMember]
    public Guid OwnerId { get; private set; }

    [DataMember]
    public DateTime CreationDate { get; private set; }

    public TokenRevocationRule(
      TokenRevocationRuleType ruleType = TokenRevocationRuleType.TokenRevocation,
      Guid? ruleId = null,
      Guid? identityId = null,
      string scopes = null,
      DateTime? createdBefore = null,
      Guid? hostId = null,
      DateTime? expirationDate = null,
      Guid? ownerId = null,
      DateTime? creationDate = null)
    {
      this.RuleType = ruleType;
      this.RuleId = ruleId.HasValue ? ruleId.Value : Guid.Empty;
      this.IdentityId = identityId;
      this.Scopes = scopes;
      this.CreatedBefore = createdBefore;
      this.HostId = hostId;
      this.ExpirationDate = expirationDate.HasValue ? expirationDate.Value : DateTime.MinValue;
      this.OwnerId = ownerId.HasValue ? ownerId.Value : Guid.Empty;
      this.CreationDate = creationDate.HasValue ? creationDate.Value : DateTime.MinValue;
    }

    public string[] GetScopes() => this.scopesList;
  }
}
