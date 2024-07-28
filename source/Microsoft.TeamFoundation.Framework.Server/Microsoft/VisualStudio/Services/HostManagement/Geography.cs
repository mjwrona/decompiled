// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.HostManagement.Geography
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.HostManagement
{
  [DataContract]
  public class Geography
  {
    [DataMember]
    public string Code { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public bool IsActive { get; set; }

    public Geography()
    {
    }

    public Geography(Geography geography)
    {
      this.Code = geography.Code;
      this.IsActive = geography.IsActive;
      this.DisplayName = geography.DisplayName;
    }

    public Geography Clone() => new Geography(this);
  }
}
