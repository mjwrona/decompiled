// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Client.Internal.GetNuspecsInternalResponseNuspec
// Assembly: Microsoft.VisualStudio.Services.NuGet.Client.Internal, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E63C245C-898F-41A7-9916-45B2DC75C1BE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Client.Internal.dll

using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.NuGet.Client.Internal
{
  [DataContract]
  public class GetNuspecsInternalResponseNuspec
  {
    [DataMember]
    public string? DisplayVersion { get; set; }

    [DataMember]
    public string? Content { get; set; }

    [DataMember]
    public bool AreBytesCompressed { get; set; }
  }
}
