// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.WebApi.TcmAttachment
// Assembly: Microsoft.VisualStudio.Services.Tcm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DCD48481-6B90-4012-9254-BC9E7077DAC8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.WebApi.dll

using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Tcm.WebApi
{
  [DataContract]
  public class TcmAttachment
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Stream Stream { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string FileName { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string CompressionType { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public long ContentLength { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ContentType { get; set; }
  }
}
