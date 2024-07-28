// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ApiLicensesController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  [SupportedRouteArea("Api", NavigationContextLevels.Application)]
  [OutputCache(CacheProfile = "NoCache")]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiLicensesController : AdminAreaController
  {
    private readonly string c_csvDelimiter = ",";

    [HttpGet]
    public ActionResult Display(Guid licenseTypeId)
    {
      this.CheckManageLicensesPermission();
      ArgumentUtility.CheckForEmptyGuid(licenseTypeId, nameof (licenseTypeId));
      return (ActionResult) this.View((object) new LicenseModel(this.TfsRequestContext, licenseTypeId));
    }

    [HttpPost]
    public ActionResult SetDefaultLicenseType(Guid licenseTypeId)
    {
      this.CheckManageLicensesPermission();
      this.TfsRequestContext.GetService<LicensePackageService>().SetDefaultLicenseType(this.TfsRequestContext, licenseTypeId);
      return (ActionResult) new EmptyResult();
    }

    [HttpGet]
    public ActionResult Trace(Guid teamFoundationId)
    {
      TeamFoundationIdentityService service1 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationOnPremLicensingService service2 = this.TfsRequestContext.GetService<TeamFoundationOnPremLicensingService>();
      TeamFoundationIdentity readIdentity = service1.ReadIdentities(this.TfsRequestContext, new Guid[1]
      {
        teamFoundationId
      }, MembershipQuery.Expanded, ReadIdentityOptions.None, (IEnumerable<string>) null)[0];
      if (readIdentity == null)
        throw new ArgumentException(AdminServerResources.TraceLicenseCouldNotFindUser, nameof (teamFoundationId));
      ILicenseType[] licensesForUser = service2.GetLicensesForUser(this.TfsRequestContext, readIdentity.Descriptor);
      TraceLicenseModel model = new TraceLicenseModel();
      model.Identity = readIdentity;
      model.Licenses = licensesForUser;
      model.AffectingIdentities = new List<KeyValuePair<TeamFoundationIdentity, ILicenseType>>();
      TeamFoundationIdentity[] licenseGroups = service2.GetLicenseGroups(this.TfsRequestContext, ((IEnumerable<ILicenseType>) licensesForUser).Select<ILicenseType, Guid>((Func<ILicenseType, Guid>) (x => x.Id)).ToArray<Guid>(), MembershipQuery.Direct);
      List<IdentityDescriptor> identityDescriptorList = new List<IdentityDescriptor>();
      int[] numArray = new int[licenseGroups.Length];
      for (int index = 0; index < licenseGroups.Length; ++index)
      {
        TeamFoundationIdentity foundationIdentity = licenseGroups[index];
        IEnumerable<IdentityDescriptor> identityDescriptors = readIdentity.MemberOf.Concat<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          readIdentity.Descriptor
        }).Intersect<IdentityDescriptor>((IEnumerable<IdentityDescriptor>) foundationIdentity.Members, (IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        identityDescriptorList.AddRange(identityDescriptors);
        numArray[index] = identityDescriptors.Count<IdentityDescriptor>();
      }
      if (identityDescriptorList.Count > 0)
      {
        TeamFoundationIdentity[] foundationIdentityArray = service1.ReadIdentities(this.TfsRequestContext, identityDescriptorList.ToArray());
        int index1 = 0;
        for (int index2 = 0; index2 < numArray.Length; ++index2)
        {
          for (int index3 = 0; index3 < numArray[index2]; ++index3)
          {
            model.AffectingIdentities.Add(new KeyValuePair<TeamFoundationIdentity, ILicenseType>(foundationIdentityArray[index1], licensesForUser[index2]));
            ++index1;
          }
        }
      }
      else
        model.IsDefault = true;
      return (ActionResult) this.View((object) model);
    }

    [HttpGet]
    public ActionResult Export(string exportName = "data")
    {
      this.CheckManageLicensesPermission();
      TeamFoundationIdentityService service1 = this.TfsRequestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationOnPremLicensingService service2 = this.TfsRequestContext.GetService<TeamFoundationOnPremLicensingService>();
      TeamFoundationIdentity[] foundationIdentityArray = service1.ReadIdentities(this.TfsRequestContext, service1.ReadIdentity(this.TfsRequestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, MembershipQuery.Expanded, ReadIdentityOptions.None).Members.ToArray<IdentityDescriptor>(), MembershipQuery.None, ReadIdentityOptions.ExtendedProperties, (IEnumerable<string>) new string[1]
      {
        "LastAccessedTime"
      });
      ILicenseType[] licenseTypes;
      Dictionary<IdentityDescriptor, int> dictionary = service2.ExportUserLicenses(this.TfsRequestContext, out licenseTypes);
      StringBuilder stringBuilder = new StringBuilder();
      this.EscapeForCSV(stringBuilder, (object) AdminServerResources.AuditLogDisplayNameHeader, false);
      this.EscapeForCSV(stringBuilder, (object) AdminServerResources.AuditLogUniqueNameHeader, false);
      this.EscapeForCSV(stringBuilder, (object) AdminServerResources.AuditLogServerNameHeader, false);
      this.EscapeForCSV(stringBuilder, (object) AdminServerResources.AuditLogLastAccessedHeader, false);
      this.EscapeForCSV(stringBuilder, (object) AdminServerResources.AuditLogIsGroupHeader, false);
      foreach (ILicenseType licenseType in licenseTypes)
        this.EscapeForCSV(stringBuilder, (object) licenseType.Name, false);
      this.EscapeForCSV(stringBuilder, (object) AdminServerResources.AuditLogIsDefaultHeader, true);
      foreach (TeamFoundationIdentity foundationIdentity in foundationIdentityArray)
      {
        this.EscapeForCSV(stringBuilder, (object) foundationIdentity.DisplayName, false);
        this.EscapeForCSV(stringBuilder, (object) foundationIdentity.UniqueName, false);
        this.EscapeForCSV(stringBuilder, (object) Environment.MachineName, false);
        object obj;
        if (foundationIdentity.TryGetProperty("LastAccessedTime", out obj))
          this.EscapeForCSV(stringBuilder, (object) obj.ToString(), false);
        else
          this.EscapeForCSV(stringBuilder, (object) string.Empty, false);
        this.EscapeForCSV(stringBuilder, (object) foundationIdentity.IsContainer, false);
        int num = 0;
        bool flag = !dictionary.TryGetValue(foundationIdentity.Descriptor, out num);
        for (int index = 0; index < licenseTypes.Length; ++index)
          this.EscapeForCSV(stringBuilder, (object) ((num >> index & 1) > 0), false);
        this.EscapeForCSV(stringBuilder, (object) flag, true);
      }
      string s = stringBuilder.ToString();
      string fileDownloadName = Path.ChangeExtension(exportName, "csv");
      return (ActionResult) this.File(((IEnumerable<byte>) Encoding.UTF8.GetPreamble()).Concat<byte>((IEnumerable<byte>) Encoding.UTF8.GetBytes(s)).ToArray<byte>(), "text/csv", fileDownloadName);
    }

    private void EscapeForCSV(StringBuilder stringBuilder, object obj, bool isEndLine)
    {
      if (obj is bool flag)
        stringBuilder.Append(flag ? 1 : 0);
      else if (!string.IsNullOrEmpty(obj.ToString()))
        stringBuilder.AppendFormat("\"{0}\"", (object) obj.ToString().Replace("\"", "\"\""));
      if (isEndLine)
        stringBuilder.AppendLine();
      else
        stringBuilder.Append(this.c_csvDelimiter);
    }
  }
}
