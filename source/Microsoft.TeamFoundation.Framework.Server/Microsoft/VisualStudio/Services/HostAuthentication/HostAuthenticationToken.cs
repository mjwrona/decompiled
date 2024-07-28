// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostAuthentication.HostAuthenticationToken
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.HostAuthentication
{
  [DataContract]
  public class HostAuthenticationToken
  {
    public HostAuthenticationToken()
    {
      this.Subjects = new List<SubjectDescriptor>();
      this.Entries = new Dictionary<Guid, HostAuthenticationEntry>();
    }

    [DataMember(Name = "subs")]
    public List<SubjectDescriptor> Subjects { get; set; }

    [DataMember(Name = "hosts")]
    public Dictionary<Guid, HostAuthenticationEntry> Entries { get; set; }

    public bool IsHostAuthenticated(Guid hostId) => this.Entries.ContainsKey(hostId);

    public void AddHostAuthentication(Guid hostId, SubjectDescriptor subject)
    {
      int num = this.Subjects.IndexOf(subject);
      if (num == -1)
      {
        num = this.Subjects.Count;
        this.Subjects.Add(subject);
      }
      this.Entries[hostId] = new HostAuthenticationEntry()
      {
        Subject = num
      };
    }
  }
}
