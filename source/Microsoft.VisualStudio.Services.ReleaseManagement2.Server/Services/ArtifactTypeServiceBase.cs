// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ArtifactTypeServiceBase
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.FormInput;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Artifacts;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services
{
  public class ArtifactTypeServiceBase : ReleaseManagement2ServiceBase
  {
    private readonly Func<IVssRequestContext, IReadOnlyList<ArtifactTypeBase>> artifactExtensionRetriever;
    private IReadOnlyList<ArtifactTypeBase> artifactTypeList;
    private IReadOnlyDictionary<string, ArtifactTypeBase> artifactTypeById;

    protected ArtifactTypeServiceBase()
      : this(ArtifactTypeServiceBase.\u003C\u003EO.\u003C0\u003E__GetArtifactTypes ?? (ArtifactTypeServiceBase.\u003C\u003EO.\u003C0\u003E__GetArtifactTypes = new Func<IVssRequestContext, IReadOnlyList<ArtifactTypeBase>>(ArtifactTypesRetriever.GetArtifactTypes)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is the intended design")]
    protected ArtifactTypeServiceBase(
      Func<IVssRequestContext, IReadOnlyList<ArtifactTypeBase>> extensionRetriever)
    {
      this.artifactExtensionRetriever = extensionRetriever;
    }

    protected IReadOnlyList<ArtifactTypeBase> GetArtifactTypes(IVssRequestContext context)
    {
      this.LoadArtifactTypeDefinition(context);
      return this.artifactTypeList;
    }

    public override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.LoadArtifactTypeDefinition(systemRequestContext);
    }

    public virtual ArtifactTypeBase GetArtifactType(
      IVssRequestContext requestContext,
      string typeId)
    {
      return this.GetArtifactType(requestContext, typeId, true);
    }

    public virtual ArtifactTypeBase GetArtifactType(
      IVssRequestContext requestContext,
      string typeId,
      bool throwOnMissing)
    {
      ArtifactTypeBase artifactType = ArtifactTypesRetriever.GetInternalArtifactTypes(requestContext).FirstOrDefault<ArtifactTypeBase>((Func<ArtifactTypeBase, bool>) (a => a.Name.Equals(typeId, StringComparison.OrdinalIgnoreCase)));
      if (artifactType == null)
      {
        this.LoadArtifactTypeDefinition(requestContext);
        if (((this.artifactTypeById.TryGetValue(typeId, out artifactType) ? 0 : (artifactType == null ? 1 : 0)) & (throwOnMissing ? 1 : 0)) != 0)
          throw new ReleaseManagementObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactTypeNotFound, (object) typeId));
      }
      return artifactType;
    }

    private static void ValidateInputDescriptors(IEnumerable<InputDescriptor> inputDescriptors)
    {
      foreach (InputDescriptor inputDescriptor in inputDescriptors)
        ArgumentUtility.CheckStringForNullOrWhiteSpace(inputDescriptor.Name, "InputDescriptor.Name");
    }

    private static void ThrowDuplicateIdentifierException(
      IEnumerable<IGrouping<string, ArtifactTypeBase>> typeDefinitions)
    {
      string enumerable = string.Empty;
      foreach (IGrouping<string, ArtifactTypeBase> typeDefinition in typeDefinitions)
        enumerable = !enumerable.IsNullOrEmpty<char>() ? string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0},{1}", (object) enumerable, (object) typeDefinition.Key) : typeDefinition.Key;
      throw new ReleaseManagementServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.ArtifactTypeDuplicateIdentifier, (object) enumerable));
    }

    private void LoadArtifactTypeDefinition(IVssRequestContext systemRequestContext)
    {
      this.artifactTypeList = this.artifactExtensionRetriever(systemRequestContext);
      List<IGrouping<string, ArtifactTypeBase>> list = this.artifactTypeList.GroupBy<ArtifactTypeBase, string>((Func<ArtifactTypeBase, string>) (c => c.Name)).Where<IGrouping<string, ArtifactTypeBase>>((Func<IGrouping<string, ArtifactTypeBase>, bool>) (g => g.Count<ArtifactTypeBase>() > 1)).ToList<IGrouping<string, ArtifactTypeBase>>();
      if (list.Any<IGrouping<string, ArtifactTypeBase>>())
        ArtifactTypeServiceBase.ThrowDuplicateIdentifierException((IEnumerable<IGrouping<string, ArtifactTypeBase>>) list);
      Dictionary<string, ArtifactTypeBase> dictionary = new Dictionary<string, ArtifactTypeBase>(this.artifactTypeList.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (ArtifactTypeBase artifactType in (IEnumerable<ArtifactTypeBase>) this.artifactTypeList)
      {
        ArtifactTypeServiceBase.ValidateInputDescriptors((IEnumerable<InputDescriptor>) artifactType.InputDescriptors);
        dictionary[artifactType.Name] = artifactType;
      }
      this.artifactTypeById = (IReadOnlyDictionary<string, ArtifactTypeBase>) dictionary;
    }
  }
}
