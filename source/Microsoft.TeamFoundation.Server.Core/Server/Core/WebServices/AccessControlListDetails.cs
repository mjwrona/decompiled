// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.WebServices.AccessControlListDetails
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.WebServices
{
  public class AccessControlListDetails
  {
    public AccessControlListDetails()
    {
    }

    public AccessControlListDetails(IAccessControlList acl)
    {
      this.Token = acl.Token;
      this.InheritPermissions = acl.InheritPermissions;
      this.Entries = acl.AccessControlEntries.Select<IAccessControlEntry, AccessControlEntryDetails>((Func<IAccessControlEntry, AccessControlEntryDetails>) (s => new AccessControlEntryDetails(acl.Token, s))).ToList<AccessControlEntryDetails>();
      this.IncludeExtendedInfo = acl.AccessControlEntries.Any<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (s => s.IncludesExtendedInfo));
    }

    [XmlAttribute]
    public bool InheritPermissions { get; set; }

    [XmlAttribute]
    public string Token { get; set; }

    [XmlAttribute]
    public bool IncludeExtendedInfo { get; set; }

    [XmlElement("AccessControlEntries", typeof (List<AccessControlEntryDetails>))]
    public List<AccessControlEntryDetails> Entries { get; set; }

    public IAccessControlList ToAccessControlList() => (IAccessControlList) new AccessControlList(this.Token, this.InheritPermissions, this.Entries.Select<AccessControlEntryDetails, IAccessControlEntry>((Func<AccessControlEntryDetails, IAccessControlEntry>) (s => s.ToAccessControlEntry())));
  }
}
