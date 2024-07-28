// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DirectoryAddMemberValidator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories
{
  public class DirectoryAddMemberValidator
  {
    public static readonly DirectoryAddMemberValidator Instance = new DirectoryAddMemberValidator();
    private static readonly HashSet<string> ValidProfileStates = new HashSet<string>()
    {
      (string) null,
      "Compliant"
    };
    private static readonly HashSet<string> ValidLicenseTypes = new HashSet<string>()
    {
      (string) null,
      "Advanced",
      "Basic",
      "Msdn",
      "None",
      "Optimal",
      "Stakeholder"
    };

    public void ThrowIfInvalid(string profile, string license, IEnumerable<string> localGroups)
    {
      if (!DirectoryAddMemberValidator.ValidProfileStates.Contains(profile))
        throw new DirectoryParameterException(string.Format("Invalid profile state => {0}", (object) profile));
      if (!DirectoryAddMemberValidator.ValidLicenseTypes.Contains(license))
        throw new DirectoryParameterException(string.Format("Invalid license type => {0}", (object) license));
    }

    public void ThrowIfInvalid(
      DirectoryEntityResult result,
      string member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      if (result == null)
        throw new InvalidDirectoryEntityResultException(IdentityTracing.FormatWithSafeSerialization("Expected nonnull result but received => {0} for inputs => member: {1}, profile: {2},, license: {3}, localGroups: {4}, propertiesToReturn: {5}", (object) result, (object) member, (object) profile, (object) license, (object) localGroups, (object) propertiesToReturn));
    }

    public void ThrowIfInvalid(
      DirectoryEntityResult result,
      IDirectoryEntityDescriptor member,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      if (result == null)
        throw new InvalidDirectoryEntityResultException(IdentityTracing.FormatWithSafeSerialization("Expected nonnull result but received => {0} for inputs => member: {1}, profile: {2},, license: {3}, localGroups: {4}, propertiesToReturn: {5}", (object) result, (object) member, (object) profile, (object) license, (object) localGroups, (object) propertiesToReturn));
    }

    public void ThrowIfInvalid(
      IReadOnlyList<DirectoryEntityResult> results,
      IReadOnlyList<IDirectoryEntityDescriptor> members,
      string profile = null,
      string license = null,
      IEnumerable<string> localGroups = null,
      IEnumerable<string> propertiesToReturn = null)
    {
      if (results == null || !results.All<DirectoryEntityResult>((Func<DirectoryEntityResult, bool>) (r => r != null)))
        throw new InvalidDirectoryEntityResultException(IdentityTracing.FormatWithSafeSerialization("Expected nonnull results but received => {0} for inputs => members: {1}, profile: {2},, license: {3}, localGroups: {4}, propertiesToReturn: {5}", (object) results, (object) members, (object) profile, (object) license, (object) localGroups, (object) propertiesToReturn));
    }
  }
}
