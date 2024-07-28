// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.ImportRepositoryValidation
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.ComponentModel;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class ImportRepositoryValidation
  {
    [DataMember]
    public GitImportGitSource GitSource { get; set; }

    [DataMember]
    public GitImportTfvcSource TfvcSource { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Username { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Password { get; set; }
  }
}
