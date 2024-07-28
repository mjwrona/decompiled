// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.DirectoryMemberExtensions
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public static class DirectoryMemberExtensions
  {
    public static Microsoft.VisualStudio.Services.Aad.AadObject AadObject(
      this DirectoryMember member)
    {
      return member.Properties.GetCastedValueOrDefault<string, Microsoft.VisualStudio.Services.Aad.AadObject>(nameof (AadObject));
    }

    public static Microsoft.VisualStudio.Services.Aad.AadUser AadUser(this DirectoryMember member) => member.Properties.GetCastedValueOrDefault<string, Microsoft.VisualStudio.Services.Aad.AadUser>("AadObject");

    public static Microsoft.VisualStudio.Services.Aad.AadGroup AadGroup(this DirectoryMember member) => member.Properties.GetCastedValueOrDefault<string, Microsoft.VisualStudio.Services.Aad.AadGroup>("AadObject");

    public static Microsoft.VisualStudio.Services.Aad.AadServicePrincipal AadServicePrincipal(
      this DirectoryMember member)
    {
      return member.Properties.GetCastedValueOrDefault<string, Microsoft.VisualStudio.Services.Aad.AadServicePrincipal>("AadObject");
    }

    public static DirectoryMember SetAadObject(this DirectoryMember member, Microsoft.VisualStudio.Services.Aad.AadObject aadObject)
    {
      member.Properties.Add("AadObject", (object) aadObject);
      return member;
    }
  }
}
