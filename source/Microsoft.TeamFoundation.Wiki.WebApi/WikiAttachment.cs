// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.WebApi.WikiAttachment
// Assembly: Microsoft.TeamFoundation.Wiki.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4A8C8A50-70A8-447A-B2AD-300BEAACF074
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Wiki.WebApi
{
  [DataContract]
  public class WikiAttachment
  {
    public WikiAttachment(string name, string path)
    {
      this.Name = name;
      this.Path = path;
    }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; private set; }

    [DataMember(Name = "path", EmitDefaultValue = false)]
    public string Path { get; private set; }
  }
}
