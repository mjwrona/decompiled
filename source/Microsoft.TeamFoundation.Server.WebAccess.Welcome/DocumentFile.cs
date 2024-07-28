// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Welcome.DocumentFile
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Welcome, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B024A61-082C-4505-8523-CF030F6A8A5A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Welcome.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Welcome
{
  [DataContract]
  public class DocumentFile
  {
    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string name { get; set; }

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public string path { get; set; }
  }
}
