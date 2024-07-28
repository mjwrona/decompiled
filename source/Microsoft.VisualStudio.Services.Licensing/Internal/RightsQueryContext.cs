// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.Internal.RightsQueryContext
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Licensing.Internal
{
  public class RightsQueryContext : IRightsQueryContext
  {
    private const string s_area = "VisualStudio.Services.LicensingService";
    private const string s_layer = "BusinessLogic";

    public Guid AccountId { get; private set; }

    public string MachineId { get; private set; }

    public Version ProductVersion { get; private set; }

    public string ProductVersionBuildLab { get; private set; }

    public ReleaseType ReleaseType { get; private set; }

    public LicensingRequestType RequestType { get; private set; }

    public string SingleRightName { get; private set; }

    public Microsoft.VisualStudio.Services.Identity.Identity UserIdentity { get; private set; }

    public VisualStudioFamily VisualStudioFamily { get; private set; }

    public VisualStudioEdition VisualStudioEdition { get; private set; }

    public static IRightsQueryContext CreateUsageRightsContext(
      IVssRequestContext requestContext,
      string rightName)
    {
      RightsQueryContext usageRightsContext = new RightsQueryContext()
      {
        RequestType = LicensingRequestType.Service
      };
      if (rightName != null)
        usageRightsContext.SingleRightName = rightName;
      return (IRightsQueryContext) usageRightsContext;
    }

    public static IRightsQueryContext CreateClientRightsContext(
      IVssRequestContext requestContext,
      ClientRightsQueryContext clientRightsQueryContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      RightsQueryContext queryContext = new RightsQueryContext()
      {
        RequestType = LicensingRequestType.Client,
        SingleRightName = clientRightsQueryContext.ProductFamily,
        UserIdentity = userIdentity
      };
      RightsQueryContext.ParseProductVersion(requestContext, clientRightsQueryContext.ProductVersion, queryContext);
      RightsQueryContext.ParseProductFamily(requestContext, clientRightsQueryContext, queryContext);
      RightsQueryContext.ParseProductEdition(requestContext, clientRightsQueryContext, queryContext);
      RightsQueryContext.ParseReleaseType(requestContext, clientRightsQueryContext.ReleaseType, queryContext);
      RightsQueryContext.ParseMachineId(requestContext, clientRightsQueryContext.MachineId, queryContext);
      return (IRightsQueryContext) queryContext;
    }

    public static IRightsQueryContext CreateMaxClientRightsContext(
      IVssRequestContext requestContext,
      ClientRightsQueryContext clientRightsQueryContext,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      RightsQueryContext queryContext = new RightsQueryContext()
      {
        RequestType = LicensingRequestType.Client,
        SingleRightName = clientRightsQueryContext.ProductFamily,
        UserIdentity = userIdentity
      };
      RightsQueryContext.ParseProductVersion(requestContext, clientRightsQueryContext.ProductVersion, queryContext);
      RightsQueryContext.ParseProductFamily(requestContext, clientRightsQueryContext, queryContext);
      RightsQueryContext.ParseReleaseType(requestContext, clientRightsQueryContext.ReleaseType, queryContext);
      return (IRightsQueryContext) queryContext;
    }

    public static IRightsQueryContext CreateServiceRightsContext(
      IVssRequestContext requestContext,
      string rightName,
      Guid accountId,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity)
    {
      ArgumentUtility.CheckForEmptyGuid(accountId, nameof (accountId));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(userIdentity, nameof (userIdentity));
      RightsQueryContext serviceRightsContext = new RightsQueryContext()
      {
        RequestType = LicensingRequestType.Service,
        AccountId = accountId,
        UserIdentity = userIdentity
      };
      if (rightName != null)
        serviceRightsContext.SingleRightName = rightName;
      return (IRightsQueryContext) serviceRightsContext;
    }

    internal static IRightsQueryContext CreateQueryContextForUnitTests(
      LicensingRequestType requestType = LicensingRequestType.Client,
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = null,
      Guid accountId = default (Guid),
      string singleRightName = null,
      Version productVersion = null,
      string productVersionBuildLab = null,
      string machineId = null,
      ReleaseType releaseType = ReleaseType.Release,
      VisualStudioEdition visualStudioEdition = VisualStudioEdition.Unspecified,
      VisualStudioFamily visualStudioFamily = VisualStudioFamily.Invalid)
    {
      return (IRightsQueryContext) new RightsQueryContext()
      {
        RequestType = requestType,
        UserIdentity = userIdentity,
        AccountId = accountId,
        SingleRightName = singleRightName,
        ProductVersion = productVersion,
        ProductVersionBuildLab = productVersionBuildLab,
        MachineId = machineId,
        ReleaseType = releaseType,
        VisualStudioEdition = visualStudioEdition,
        VisualStudioFamily = visualStudioFamily
      };
    }

    private RightsQueryContext()
    {
    }

    private static void ParseProductVersion(
      IVssRequestContext requestContext,
      string versionText,
      RightsQueryContext queryContext)
    {
      if (string.IsNullOrWhiteSpace(versionText))
      {
        requestContext.Trace(1031400, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Invalid product version: '{0}'.", (object) (versionText ?? string.Empty));
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
      string[] strArray = versionText.Split(new char[1]
      {
        ' '
      }, StringSplitOptions.RemoveEmptyEntries);
      if (strArray.Length < 1 || strArray.Length > 2)
      {
        requestContext.Trace(1031401, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Invalid product version: '{0}'.", (object) versionText);
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
      Version result = (Version) null;
      if (Version.TryParse(strArray[0], out result))
      {
        queryContext.ProductVersion = result;
        if (strArray.Length <= 1 || string.IsNullOrEmpty(strArray[1]))
          return;
        queryContext.ProductVersionBuildLab = strArray[1];
      }
      else
      {
        requestContext.Trace(1031402, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Invalid product version: '{0}'.", (object) versionText);
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductVersion());
      }
    }

    private static void ParseProductFamily(
      IVssRequestContext requestContext,
      ClientRightsQueryContext clientRightsQueryContext,
      RightsQueryContext queryContext)
    {
      VisualStudioFamily family = VisualStudioFamily.Invalid;
      if (!string.IsNullOrEmpty(clientRightsQueryContext.ProductFamily) && ParsingUtilities.TryParseProductFamily(clientRightsQueryContext.ProductFamily, out family))
      {
        queryContext.VisualStudioFamily = family;
      }
      else
      {
        requestContext.Trace(1031430, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unrecognized product family: '{0}'.", (object) (clientRightsQueryContext.ProductFamily ?? string.Empty));
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductFamily());
      }
    }

    private static void ParseProductEdition(
      IVssRequestContext requestContext,
      ClientRightsQueryContext clientRightsQueryContext,
      RightsQueryContext queryContext)
    {
      VisualStudioEdition edition = VisualStudioEdition.Unspecified;
      if (!string.IsNullOrEmpty(clientRightsQueryContext.ProductEdition) && ParsingUtilities.TryParseProductEdition(clientRightsQueryContext.ProductEdition, out edition))
        queryContext.VisualStudioEdition = edition;
      else if (LicensingComparers.RightNameComparer.Compare(clientRightsQueryContext.ProductFamily, "VisualStudio") == 0)
      {
        requestContext.Trace(1031410, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unrecognized product edition: '{0}'.", (object) (clientRightsQueryContext.ProductEdition ?? string.Empty));
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextProductEdition());
      }
    }

    private static void ParseReleaseType(
      IVssRequestContext requestContext,
      string releaseTypeText,
      RightsQueryContext queryContext)
    {
      ReleaseType releaseType = ReleaseType.Release;
      if (!string.IsNullOrEmpty(releaseTypeText) && ParsingUtilities.TryParseReleaseType(releaseTypeText, out releaseType))
      {
        queryContext.ReleaseType = releaseType;
      }
      else
      {
        requestContext.Trace(1031420, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Unrecognized release type: '{0}'.", (object) (releaseTypeText ?? string.Empty));
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextReleaseType());
      }
    }

    private static void ParseMachineId(
      IVssRequestContext requestContext,
      string machineId,
      RightsQueryContext queryContext)
    {
      if (string.IsNullOrWhiteSpace(machineId))
      {
        requestContext.Trace(1031440, TraceLevel.Error, "VisualStudio.Services.LicensingService", "BusinessLogic", "Invalid machine id: '{0}'.", (object) (machineId ?? string.Empty));
        throw new InvalidClientRightsQueryContextException(LicensingResources.InvalidClientRightsQueryContextMachineId());
      }
      queryContext.MachineId = machineId;
    }
  }
}
