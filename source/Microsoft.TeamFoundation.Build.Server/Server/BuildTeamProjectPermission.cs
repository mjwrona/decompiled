// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildTeamProjectPermission
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildTeamProjectPermission : IValidatable
  {
    private IdentityDescriptor m_descriptor;
    private List<string> m_denies = new List<string>();
    private List<string> m_allows = new List<string>();

    public BuildTeamProjectPermission()
    {
      this.BuildACE = new AccessControlEntry(new IdentityDescriptor(), 0, 0);
      this.BuildAdministrationACE = new AccessControlEntry(new IdentityDescriptor(), 0, 0);
    }

    public List<string> Allows => this.m_allows;

    public List<string> Denies => this.m_denies;

    public string IdentityName { get; set; }

    internal IdentityDescriptor Descriptor
    {
      get => this.m_descriptor;
      set
      {
        this.m_descriptor = value;
        this.BuildACE.Descriptor = value;
        this.BuildAdministrationACE.Descriptor = value;
      }
    }

    internal void ConvertToPermissionBits()
    {
      foreach (string allow in this.Allows)
        this.SetPermissionBit(allow, true);
      foreach (string deny in this.Denies)
        this.SetPermissionBit(deny, false);
    }

    private void SetPermissionBit(string permission, bool allow)
    {
      if (permission == null)
        return;
      AccessControlEntry accessControlEntry;
      int num;
      switch (permission.Length)
      {
        case 10:
          switch (permission[0])
          {
            case 'S':
              if (!(permission == "StopBuilds"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.StopBuilds;
              break;
            case 'V':
              if (!(permission == "ViewBuilds"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.ViewBuilds;
              break;
            default:
              return;
          }
          break;
        case 11:
          if (!(permission == "QueueBuilds"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.QueueBuilds;
          break;
        case 12:
          if (!(permission == "DeleteBuilds"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.DeleteBuilds;
          break;
        case 13:
          if (!(permission == "DestroyBuilds"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.DestroyBuilds;
          break;
        case 14:
          return;
        case 15:
          return;
        case 16:
          switch (permission[0])
          {
            case 'E':
              if (!(permission == "EditBuildQuality"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.EditBuildQuality;
              break;
            case 'M':
              if (!(permission == "ManageBuildQueue"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.ManageBuildQueue;
              break;
            default:
              return;
          }
          break;
        case 17:
          if (!(permission == "UseBuildResources"))
            return;
          accessControlEntry = this.BuildAdministrationACE;
          num = AdministrationPermissions.UseBuildResources;
          break;
        case 18:
          switch (permission[0])
          {
            case 'R':
              if (!(permission == "RetainIndefinitely"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.RetainIndefinitely;
              break;
            case 'V':
              if (!(permission == "ViewBuildResources"))
                return;
              accessControlEntry = this.BuildAdministrationACE;
              num = AdministrationPermissions.ViewBuildResources;
              break;
            default:
              return;
          }
          break;
        case 19:
          switch (permission[0])
          {
            case 'E':
              if (!(permission == "EditBuildDefinition"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.EditBuildDefinition;
              break;
            case 'V':
              if (!(permission == "ViewBuildDefinition"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.ViewBuildDefinition;
              break;
            default:
              return;
          }
          break;
        case 20:
          switch (permission[11])
          {
            case 'Q':
              if (!(permission == "ManageBuildQualities"))
                return;
              accessControlEntry = this.BuildACE;
              num = BuildPermissions.ManageBuildQualities;
              break;
            case 'R':
              if (!(permission == "ManageBuildResources"))
                return;
              accessControlEntry = this.BuildAdministrationACE;
              num = AdministrationPermissions.ManageBuildResources;
              break;
            default:
              return;
          }
          break;
        case 21:
          if (!(permission == "DeleteBuildDefinition"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.DeleteBuildDefinition;
          break;
        case 22:
          if (!(permission == "UpdateBuildInformation"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.UpdateBuildInformation;
          break;
        case 23:
          return;
        case 24:
          return;
        case 25:
          return;
        case 26:
          if (!(permission == "AdministerBuildPermissions"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.AdministerBuildPermissions;
          break;
        case 27:
          return;
        case 28:
          return;
        case 29:
          return;
        case 30:
          if (!(permission == "OverrideBuildCheckInValidation"))
            return;
          accessControlEntry = this.BuildACE;
          num = BuildPermissions.OverrideBuildCheckInValidation;
          break;
        case 31:
          return;
        case 32:
          return;
        case 33:
          return;
        case 34:
          if (!(permission == "AdministerBuildResourcePermissions"))
            return;
          accessControlEntry = this.BuildAdministrationACE;
          num = AdministrationPermissions.AdministerBuildResourcePermissions;
          break;
        default:
          return;
      }
      if (allow)
        accessControlEntry.Allow |= num;
      else
        accessControlEntry.Deny |= num;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildTeamProjectPermission IdentityName={0} Allows={1} Denies={2}]", (object) this.IdentityName, (object) this.Allows.ListItems<string>(), (object) this.Denies.ListItems<string>());

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.Check("IdentityName", this.IdentityName, false, (string) null);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Allows", (IList<string>) this.Allows, BuildTeamProjectPermission.\u003C\u003EO.\u003C0\u003E__Check ?? (BuildTeamProjectPermission.\u003C\u003EO.\u003C0\u003E__Check = new Validate<string>(ArgumentValidation.Check)), false, (string) null);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Denies", (IList<string>) this.Denies, BuildTeamProjectPermission.\u003C\u003EO.\u003C0\u003E__Check ?? (BuildTeamProjectPermission.\u003C\u003EO.\u003C0\u003E__Check = new Validate<string>(ArgumentValidation.Check)), false, (string) null);
    }

    internal AccessControlEntry BuildACE { get; set; }

    internal AccessControlEntry BuildAdministrationACE { get; set; }
  }
}
