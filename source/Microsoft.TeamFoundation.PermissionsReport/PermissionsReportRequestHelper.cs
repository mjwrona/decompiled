// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.PermissionsReport.PermissionsReportRequestHelper
// Assembly: Microsoft.TeamFoundation.PermissionsReport, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 5F2A073E-CE9C-4394-ABB1-512D7FB702F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.PermissionsReport.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.PermissionsReport.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;

namespace Microsoft.TeamFoundation.PermissionsReport
{
  public class PermissionsReportRequestHelper
  {
    private static readonly string s_area = "PermissionsReport";
    private static readonly string s_layer = nameof (PermissionsReportRequestHelper);

    public static void ValidateJobData(GeneratePermissionsReportJobData jobData) => ArgumentUtility.CheckForEmptyGuid(jobData.ReportId, "ReportId");

    public static void ValidateDescriptors(IVssRequestContext context, string[] descriptors)
    {
      foreach (string descriptor in descriptors)
      {
        IdentityDescriptor identityDescriptor = (IdentityDescriptor) null;
        SubjectDescriptor subjectDescriptor = SubjectDescriptor.FromString(descriptor);
        if (subjectDescriptor != new SubjectDescriptor() && !subjectDescriptor.IsUnknownSubjectType())
          identityDescriptor = subjectDescriptor.ToIdentityDescriptor(context);
        if (identityDescriptor == (IdentityDescriptor) null)
          throw new PermissionsReportRequestInvalidException("Invalid identity descriptor: " + descriptor);
      }
    }

    public static void ValidateResources(
      IVssRequestContext context,
      PermissionsReportResource[] resources)
    {
      try
      {
        foreach (PermissionsReportResource resource in resources)
        {
          if (string.IsNullOrWhiteSpace(PermissionsReportHelper.GetPermissionSetToken(context, resource)))
            throw new ArgumentException(string.Format("Unable to retrieve token for the given resource type: {0} and resource id: {1}", (object) resource.ResourceType, (object) resource.ResourceId));
        }
      }
      catch (Exception ex)
      {
        context.TraceException(34003611, PermissionsReportRequestHelper.s_area, PermissionsReportRequestHelper.s_layer, ex);
        switch (ex)
        {
          case ProjectDoesNotExistWithNameException _:
          case ProjectDoesNotExistException _:
          case GitRepositoryNotFoundException _:
            throw new PermissionsReportRequestInvalidException("Invalid request: " + ex.Message, ex);
          default:
            throw;
        }
      }
    }
  }
}
