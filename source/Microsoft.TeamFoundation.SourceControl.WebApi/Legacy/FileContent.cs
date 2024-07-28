// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.FileContent
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class FileContent
  {
    [DataMember(Name = "content", EmitDefaultValue = false)]
    public string Content { get; set; }

    [DataMember(Name = "contentLines", EmitDefaultValue = false)]
    public List<string> ContentLines { get; set; }

    [DataMember(Name = "contentBytes", EmitDefaultValue = false)]
    public byte[] ContentBytes { get; set; }

    [DataMember(Name = "metadata", EmitDefaultValue = false)]
    public FileContentMetadata Metadata { get; set; }

    [DataMember(Name = "exceededMaxContentLength", EmitDefaultValue = false)]
    public bool ExceededMaxContentLength { get; set; }
  }
}
