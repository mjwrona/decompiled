// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Integration.Server.CommandGetIdentityChanges
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Integration.Server
{
  internal class CommandGetIdentityChanges : Command
  {
    private StreamingCollection<Microsoft.TeamFoundation.Integration.Server.Identity> m_identities;
    private int m_lastSequenceId;
    private TeamFoundationDataReader m_reader;

    public CommandGetIdentityChanges(IVssRequestContext requestContext)
      : base(requestContext)
    {
    }

    public void Execute(int sequenceId)
    {
      this.m_reader = this.RequestContext.GetService<TeamFoundationIdentityService>().GetIdentityChanges(this.RequestContext, new Tuple<int, int>(sequenceId, sequenceId));
      Tuple<int, int> tuple = this.m_reader.Current<Tuple<int, int>>();
      this.m_lastSequenceId = Math.Min(tuple.Item1, tuple.Item2);
      this.m_identities = new StreamingCollection<Microsoft.TeamFoundation.Integration.Server.Identity>((Command) this, this.GetCachedSize());
      this.m_reader.MoveNext();
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
    }

    public override void ContinueExecution()
    {
      foreach (TeamFoundationIdentity foundationIdentity in this.m_reader)
      {
        Microsoft.TeamFoundation.Integration.Server.Identity identity = new Microsoft.TeamFoundation.Integration.Server.Identity();
        identity.Sid = foundationIdentity.Descriptor.Identifier;
        identity.Type = !foundationIdentity.Descriptor.IdentityType.Equals("System.Security.Principal.WindowsIdentity", StringComparison.OrdinalIgnoreCase) ? (!foundationIdentity.Descriptor.IdentityType.Equals("Microsoft.TeamFoundation.Identity", StringComparison.OrdinalIgnoreCase) ? IdentityType.Extensible : IdentityType.ApplicationGroup) : (!foundationIdentity.IsContainer ? IdentityType.WindowsUser : IdentityType.WindowsGroup);
        identity.TeamFoundationId = foundationIdentity.TeamFoundationId;
        identity.DisplayName = foundationIdentity.DisplayName;
        identity.Description = foundationIdentity.GetAttribute("Description", string.Empty);
        identity.Domain = foundationIdentity.GetAttribute("Domain", string.Empty);
        identity.AccountName = foundationIdentity.GetAttribute("Account", string.Empty);
        identity.DistinguishedName = foundationIdentity.GetAttribute("Disambiguation", string.Empty);
        identity.MailAddress = foundationIdentity.GetAttribute("Mail", string.Empty);
        identity.Deleted = !foundationIdentity.IsActive;
        string[] strArray = new string[foundationIdentity.Members.Count];
        int index = 0;
        foreach (IdentityDescriptor member in (IEnumerable<IdentityDescriptor>) foundationIdentity.Members)
        {
          strArray[index] = member.Identifier;
          ++index;
        }
        identity.Members = strArray;
        if (identity.Type == IdentityType.WindowsGroup || identity.Type == IdentityType.ApplicationGroup)
          identity.SecurityGroup = true;
        this.m_identities.Enqueue(identity);
        if (this.IsCacheFull)
          return;
      }
      this.m_identities.IsComplete = true;
    }

    public StreamingCollection<Microsoft.TeamFoundation.Integration.Server.Identity> Identities => this.m_identities;

    public int LastSequenceId => this.m_lastSequenceId;

    private int GetCachedSize() => 1024;

    protected override void Dispose(bool disposing)
    {
      if (!disposing || this.m_reader == null)
        return;
      this.m_reader.Dispose();
      this.m_reader = (TeamFoundationDataReader) null;
    }
  }
}
