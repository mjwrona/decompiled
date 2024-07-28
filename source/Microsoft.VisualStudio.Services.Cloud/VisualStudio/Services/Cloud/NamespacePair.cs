// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.NamespacePair
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class NamespacePair
  {
    public NamespacePair()
    {
      this.Primary = "";
      this.Secondary = "";
    }

    public NamespacePair(string primary, string secondary)
    {
      this.Primary = primary;
      this.Secondary = secondary;
    }

    public NamespacePair(NamespacePair pair)
    {
      this.Primary = pair.Primary;
      this.Secondary = pair.Secondary;
    }

    [DataMember]
    public string Primary { get; set; }

    [DataMember]
    public string Secondary { get; set; }
  }
}
