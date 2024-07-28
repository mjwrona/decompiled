// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphSystemSubject
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphSystemSubject : GraphSubject
  {
    public override string SubjectKind => "systemSubject";

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphSystemSubject(
      string origin,
      string originId,
      SubjectDescriptor descriptor,
      IdentityDescriptor legacyDescriptor,
      string displayName,
      ReferenceLinks links,
      string url)
      : base(origin, originId, descriptor, legacyDescriptor, displayName, links, url)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal GraphSystemSubject(
      GraphSystemSubject systemSubject,
      string origin = null,
      string originId = null,
      SubjectDescriptor? descriptor = null,
      IdentityDescriptor legacyDescriptor = null,
      string displayName = null,
      ReferenceLinks links = null,
      string url = null)
    {
      string origin1 = origin ?? systemSubject?.Origin;
      string originId1 = originId ?? systemSubject?.OriginId;
      SubjectDescriptor descriptor1 = descriptor ?? (systemSubject != null ? systemSubject.Descriptor : new SubjectDescriptor());
      IdentityDescriptor legacyDescriptor1 = legacyDescriptor;
      if ((object) legacyDescriptor1 == null)
        legacyDescriptor1 = systemSubject?.LegacyDescriptor ?? (IdentityDescriptor) null;
      string displayName1 = displayName ?? systemSubject?.DisplayName;
      ReferenceLinks links1 = links ?? systemSubject?.Links;
      string url1 = url ?? systemSubject?.Url;
      // ISSUE: explicit constructor call
      this.\u002Ector(origin1, originId1, descriptor1, legacyDescriptor1, displayName1, links1, url1);
    }

    protected GraphSystemSubject()
    {
    }
  }
}
