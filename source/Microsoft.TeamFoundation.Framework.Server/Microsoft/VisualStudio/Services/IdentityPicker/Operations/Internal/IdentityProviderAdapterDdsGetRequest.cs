// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal.IdentityProviderAdapterDdsGetRequest
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.IdentityPicker.Operations.Internal
{
  [JsonObject(MemberSerialization.OptOut)]
  internal class IdentityProviderAdapterDdsGetRequest : IdentityProviderAdapterGetRequest
  {
    public IList<string> RequestProperties { get; set; }

    public Dictionary<string, object> PresetProperties { get; set; }

    public string Prefix { get; set; }

    public IList<string> Emails { get; set; }

    public IList<string> DomainSamAccountNames { get; set; }

    public IList<string> EntityIds { get; set; }

    public IList<SubjectDescriptor> SubjectDescriptors { get; set; }

    public IList<Guid> DirectoryIds { get; set; }

    public string DdsPagingToken { get; set; }

    public IList<string> AccountNames { get; set; }

    public IEnumerable<string> FilterByAncestorEntityIds { get; set; }

    public IEnumerable<string> FilterByEntityIds { get; set; }
  }
}
