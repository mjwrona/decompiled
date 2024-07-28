// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.ContentValidationIdentity
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [DataContract]
  [ClientIgnore]
  public class ContentValidationIdentity
  {
    [JsonConstructor]
    public ContentValidationIdentity()
    {
    }

    public ContentValidationIdentity(Microsoft.VisualStudio.Services.Identity.Identity source)
    {
      this.DisplayName = source.ProviderDisplayName;
      this.SubjectDescriptor = source.SubjectDescriptor;
    }

    [DataMember(Order = 1, EmitDefaultValue = false)]
    public string DisplayName { get; set; }

    [DataMember(Order = 2)]
    public SubjectDescriptor SubjectDescriptor { get; set; }
  }
}
