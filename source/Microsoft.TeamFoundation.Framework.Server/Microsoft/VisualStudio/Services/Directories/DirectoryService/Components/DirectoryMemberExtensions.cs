// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryService.Components.DirectoryMemberExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DirectoryService.Components
{
  public static class DirectoryMemberExtensions
  {
    public static bool TryGetTargetIdForCreateOrUpdate(
      this DirectoryMember candidateMember,
      string expectedDomain,
      string expectedOriginId,
      ITraceRequest tracer,
      out Guid targetId)
    {
      object obj;
      if (candidateMember.Properties.TryGetValue("TargetIdForCreateOrUpdate", out obj))
      {
        if (obj is Guid guid)
        {
          targetId = guid;
          return true;
        }
        tracer.TraceSerializedConditionally(10012461, TraceLevel.Error, "DirectoryService", nameof (TryGetTargetIdForCreateOrUpdate), "Found cached target ID which was not a GUID: {0}", obj);
      }
      targetId = Guid.Empty;
      bool isTargetIdValid = true;
      IReadOnlyList<VsdIdentityResult> missesWhereAllPropertiesMatch = (IReadOnlyList<VsdIdentityResult>) null;
      IReadOnlyList<VsdIdentityResult> missesWhereUnchangeablePropertiesMismatch = (IReadOnlyList<VsdIdentityResult>) null;
      IReadOnlyDictionary<Guid, VsdIdentityResult> readOnlyDictionary = (IReadOnlyDictionary<Guid, VsdIdentityResult>) null;
      IReadOnlyList<VsdIdentityResult> missesRequiringNewIdentityId = (IReadOnlyList<VsdIdentityResult>) null;
      try
      {
        targetId = candidateMember.LocalIdAsGuid ?? Guid.Empty;
        if (candidateMember.VsdIdentity != null)
          targetId = candidateMember.VsdIdentity.Id;
        if (candidateMember.NearMissIdentityResults.Any<VsdIdentityResult>())
        {
          missesWhereAllPropertiesMatch = (IReadOnlyList<VsdIdentityResult>) candidateMember.NearMissIdentityResults.Where<VsdIdentityResult>((Func<VsdIdentityResult, bool>) (r => r.DomainAndOriginIdAreEmptyOrMatch(expectedDomain, expectedOriginId) && r.HasMatchingUnchangeableProperties && r.HasMatchingChangeableProperties)).ToList<VsdIdentityResult>();
          missesWhereUnchangeablePropertiesMismatch = (IReadOnlyList<VsdIdentityResult>) candidateMember.NearMissIdentityResults.Where<VsdIdentityResult>((Func<VsdIdentityResult, bool>) (r => r.DomainAndOriginIdAreEmptyOrMatch(expectedDomain, expectedOriginId) && !r.HasMatchingUnchangeableProperties)).ToList<VsdIdentityResult>();
          readOnlyDictionary = (IReadOnlyDictionary<Guid, VsdIdentityResult>) candidateMember.NearMissIdentityResults.Where<VsdIdentityResult>((Func<VsdIdentityResult, bool>) (r => r.DomainAndOriginIdAreEmptyOrMatch(expectedDomain, expectedOriginId) && r.HasMatchingUnchangeableProperties && !r.HasMatchingChangeableProperties)).ToDedupedDictionary<VsdIdentityResult, Guid, VsdIdentityResult>((Func<VsdIdentityResult, Guid>) (r => r.Identity.MasterId), (Func<VsdIdentityResult, VsdIdentityResult>) (r => r));
          missesRequiringNewIdentityId = (IReadOnlyList<VsdIdentityResult>) candidateMember.NearMissIdentityResults.Where<VsdIdentityResult>((Func<VsdIdentityResult, bool>) (r => !r.DomainAndOriginIdAreEmptyOrMatch(expectedDomain, expectedOriginId) && r.HasMatchingLocalId)).ToList<VsdIdentityResult>();
          if (missesWhereAllPropertiesMatch.Count != 0 || missesWhereUnchangeablePropertiesMismatch.Count > 0 || readOnlyDictionary.Count > 1)
          {
            candidateMember.Status = "IdConflict";
            isTargetIdValid = false;
            tracer.TraceSerializedConditionally(10012462, TraceLevel.Error, "DirectoryService", nameof (TryGetTargetIdForCreateOrUpdate), "TryGetTargetIdForCreateOrUpdate encountered error caused due to values in the following lists: allPropertiesMatchList: {0}, unchangeablePropertiesMismatchList: {1}, changeablePropertiesMismatchList: {2}", (object) missesWhereAllPropertiesMatch, (object) missesWhereUnchangeablePropertiesMismatch, (object) readOnlyDictionary);
          }
          else if (readOnlyDictionary.Count == 1)
            targetId = readOnlyDictionary.Single<KeyValuePair<Guid, VsdIdentityResult>>().Key;
          else if (missesRequiringNewIdentityId.Count >= 1)
          {
            targetId = Guid.NewGuid();
          }
          else
          {
            candidateMember.Status = "IdConflict";
            isTargetIdValid = false;
            tracer.TraceSerializedConditionally(10012463, TraceLevel.Error, "DirectoryService", nameof (TryGetTargetIdForCreateOrUpdate), "TryGetTargetIdForCreateOrUpdate encountered error caused due to values in the following lists: missesRequiringNewIdentityId: {0}, missesWhereChangeablePropertiesMismatch: {1}", (object) missesRequiringNewIdentityId, (object) readOnlyDictionary);
          }
        }
        candidateMember.Properties["TargetIdForCreateOrUpdate"] = (object) targetId;
      }
      catch (Exception ex)
      {
        candidateMember.Status = "IdConflict";
        candidateMember.Exception = ex;
        isTargetIdValid = false;
      }
      finally
      {
        TeamFoundationTracingService.TraceDirectoryMemberTargetIdForCreateOrUpdate(nameof (TryGetTargetIdForCreateOrUpdate), Guid.Empty, candidateMember, targetId, isTargetIdValid, expectedDomain, expectedOriginId, missesWhereAllPropertiesMatch, missesWhereUnchangeablePropertiesMismatch, readOnlyDictionary, missesRequiringNewIdentityId);
      }
      return isTargetIdValid;
    }

    private static class PropertyNames
    {
      public const string TargetIdForCreateOrUpdate = "TargetIdForCreateOrUpdate";
    }
  }
}
