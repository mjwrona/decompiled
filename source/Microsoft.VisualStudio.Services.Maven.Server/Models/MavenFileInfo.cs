// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenFileInfo
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenFileInfo
  {
    [DataMember(EmitDefaultValue = false)]
    public string StorageId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long Size { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime DateAdded { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Content { get; set; }
  }
}
