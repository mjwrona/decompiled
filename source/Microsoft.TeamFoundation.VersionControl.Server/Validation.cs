// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Validation
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Globalization;
using System.IO;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  internal class Validation
  {
    private static ItemSpec[] m_rootRecursive = new ItemSpec[1]
    {
      new ItemSpec("$/", RecursionType.Full)
    };
    private VersionControlRequestContext m_versionControlRequestContext;

    public Validation(
      VersionControlRequestContext versionControlRequestContext)
    {
      this.m_versionControlRequestContext = versionControlRequestContext;
    }

    internal void check(IValidatable[] array, string parameterName, bool allowEmpty)
    {
      if (array == null || array.Length == 0)
      {
        if (!allowEmpty)
          throw new ArgumentNullException(parameterName);
      }
      else
      {
        for (int index = 0; index < array.Length; ++index)
          this.check(array[index], parameterName, false);
      }
    }

    internal void check(IValidatable obj, string parameterName, bool allowNull)
    {
      if (obj == null)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else
        obj.Validate(this.m_versionControlRequestContext, parameterName);
    }

    internal void checkComment(string comment, string parameterName, bool allowNull) => this.checkComment(comment, parameterName, allowNull, 1073741823);

    internal void checkChangesetComment(string comment, string parameterName, bool allowNull) => this.checkComment(comment, parameterName, allowNull, 1073741823);

    internal void checkComment(
      string comment,
      string parameterName,
      bool allowNull,
      int maxLength)
    {
      if (comment == null)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (!VersionControlUtil.IsLegalComment(comment, maxLength))
        throw new IllegalCommentException(comment, maxLength);
    }

    internal void checkComputerName(string computerName, string parameterName, bool allowNull)
    {
      if (computerName == null || computerName.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (!TFCommonUtil.IsLegalComputerName(computerName))
        throw new IllegalComputerNameException(computerName);
    }

    internal void checkFieldName(string fieldName, string parameterName, bool allowNullOrEmpty)
    {
      if (!allowNullOrEmpty && string.IsNullOrEmpty(fieldName))
        throw new ArgumentNullException(parameterName);
      if (fieldName.Length > 64)
        throw new IllegalFieldNameException(fieldName);
    }

    internal void checkIdentity(ref string identityName, string parameterName, bool allowNull)
    {
      if (string.IsNullOrEmpty(identityName))
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else
      {
        if (!(identityName == "."))
          return;
        identityName = this.m_versionControlRequestContext.RequestContext.GetRequestingUserUniqueName();
      }
    }

    internal void checkIdentities(string[] identityNames, string parameterName, bool allowNull)
    {
      if (identityNames == null || identityNames.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else
      {
        for (int index = 0; index < identityNames.Length; ++index)
          this.checkIdentity(ref identityNames[index], parameterName, false);
      }
    }

    internal void checkItem(
      ref string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters,
      PathLength maxPathLength)
    {
      ItemValidationError error = VersionControlUtil.CheckItem(ref item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters, maxPathLength);
      Validation.HandleItemValidationError(item, parameterName, error, maxPathLength);
    }

    internal void checkLabelName(
      string labelName,
      string parameterName,
      bool allowNull,
      bool allowWildcards)
    {
      if (labelName == null)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (!LabelSpec.IsLegalName(labelName, allowWildcards))
        throw new IllegalLabelNameException(labelName);
    }

    internal void checkLocalItem(
      string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters)
    {
      ItemValidationError error = VersionControlUtil.CheckLocalItem(item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters);
      Validation.HandleItemValidationError(item, parameterName, error, PathLength.Length259);
    }

    private static void HandleItemValidationError(
      string item,
      string parameterName,
      ItemValidationError error,
      PathLength maxPathLength)
    {
      switch (error)
      {
        case ItemValidationError.WildcardNotAllowed:
          throw new WildcardNotAllowedException("WildcardNotAllowedException", new object[1]
          {
            (object) parameterName
          });
        case ItemValidationError.RepositoryPathTooLong:
          throw new RepositoryPathTooLongException(item, (int) maxPathLength);
        case ItemValidationError.LocalItemRequired:
          throw new LocalItemRequiredException(item);
      }
    }

    internal void checkLockLevel(LockLevel lockLevel, string parameterName)
    {
      if (lockLevel < LockLevel.None || lockLevel > LockLevel.Unchanged)
        throw new ArgumentOutOfRangeException(parameterName);
    }

    internal void checkNotNull(object value, string parameterName)
    {
      if (value == null)
        throw new ArgumentNullException(parameterName);
    }

    internal void checkNotNullOrEmpty(string value, string parameterName)
    {
      if (string.IsNullOrEmpty(value))
        throw new ArgumentNullException(parameterName);
    }

    internal static int ValidatePathLength(int value, string parameterName) => value >= 0 && (value > 399 || Enum.IsDefined(typeof (PathLength), (object) value)) ? Math.Min(399, value) : throw new ArgumentOutOfRangeException(parameterName);

    internal static void checkPendChangesOptions(
      PendChangesOptions options,
      SupportedFeatures supportedFeatures,
      string parameterName)
    {
      bool flag = (supportedFeatures & SupportedFeatures.GetLatestOnCheckout) != 0;
      if ((options & PendChangesOptions.GetLatestOnCheckout) != 0 && !flag)
        throw new ArgumentException(Resources.Format("OptionRequestedButNotSupported", (object) "GetLatestOnCheckout"), parameterName);
    }

    internal void checkPolicyName(string policyName, string parameterName, bool allowNullOrEmpty)
    {
      if (!allowNullOrEmpty && string.IsNullOrEmpty(policyName))
        throw new ArgumentNullException(parameterName);
      if (policyName.Length > 256)
        throw new IllegalPolicyNameException(policyName);
    }

    internal void checkServerItem(
      ref string item,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allow8Dot3Paths,
      bool checkReservedCharacters,
      PathLength maxPathLength)
    {
      ItemValidationError error = VersionControlUtil.CheckServerItem(ref item, parameterName, allowNull, allowWildcards, allow8Dot3Paths, checkReservedCharacters, maxPathLength);
      Validation.HandleItemValidationError(item, parameterName, error, maxPathLength);
    }

    internal void checkServerItems(
      string[] serverItems,
      string parameterName,
      bool allowNull,
      bool allowWildcards,
      bool allowDuplicates,
      bool allow8Dot3Paths,
      bool allowNoItems,
      bool checkReservedCharacters,
      PathLength maxPathLength)
    {
      if (serverItems == null || serverItems.Length == 0)
      {
        if (serverItems == null)
        {
          if (!allowNull)
            throw new ArgumentNullException(parameterName);
        }
        else if (!allowNoItems)
          throw new ArgumentNullException(parameterName);
      }
      else
      {
        for (int index = 0; index < serverItems.Length; ++index)
          this.checkServerItem(ref serverItems[index], parameterName, false, allowWildcards, allow8Dot3Paths, checkReservedCharacters, maxPathLength);
        if (allowDuplicates)
          return;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        Array.Sort<string>(serverItems, Validation.\u003C\u003EO.\u003C0\u003E__Compare ?? (Validation.\u003C\u003EO.\u003C0\u003E__Compare = new Comparison<string>(VersionControlPath.Compare)));
        for (int index = 0; index < serverItems.Length - 1; ++index)
        {
          if (VersionControlPath.Equals(serverItems[index], serverItems[index + 1]))
            throw new DuplicateServerItemException(serverItems[index]);
        }
      }
    }

    internal void checkProxyUrl(string url, string parameterName, bool allowNull)
    {
      if (url == null || url.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (url.Length > 256)
        throw new IllegalUrlException();
    }

    internal void checkUrl(string url, string parameterName, bool allowNull)
    {
      if (url == null || url.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (url.Length > 2048)
        throw new IllegalUrlException();
    }

    internal void checkShelvesetName(string shelvesetName, string parameterName, bool allowNull)
    {
      if (shelvesetName == null || shelvesetName.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (!WorkspaceSpec.IsLegalName(shelvesetName))
        throw new IllegalShelvesetException(shelvesetName);
    }

    internal void checkStream(Stream stream, string parameterName, bool requireSeek)
    {
      if (stream == null)
        throw new ArgumentNullException(parameterName);
      if (requireSeek && !stream.CanSeek)
        throw new ArgumentException(Resources.Format("StreamMustSupportSeek", (object) parameterName));
    }

    internal void checkWorkspaceName(string workspaceName, string parameterName, bool allowNull)
    {
      if (workspaceName == null || workspaceName.Length == 0)
      {
        if (!allowNull)
          throw new ArgumentNullException(parameterName);
      }
      else if (!WorkspaceSpec.IsLegalName(workspaceName))
        throw new IllegalWorkspaceException(workspaceName);
    }

    internal void checkVersionSpec(VersionSpec spec, string parameterName, bool allowNull) => this.checkVersionSpec(spec, parameterName, allowNull, false);

    internal void checkVersionSpec(
      VersionSpec spec,
      string parameterName,
      bool allowNull,
      bool allowDateBeforeFirstChangeset)
    {
      if (!allowNull && spec == null)
        throw new ArgumentNullException(parameterName);
      if (spec == null)
        return;
      spec.Validate(this.m_versionControlRequestContext, parameterName);
      if (allowDateBeforeFirstChangeset || !(spec is DateVersionSpec))
        return;
      ((DateVersionSpec) spec).ValidateBeforeFirstChangeset(this.m_versionControlRequestContext);
    }

    internal void checkVersionNumber(int version, string parameterName)
    {
      if (version < 0)
        throw new ArgumentOutOfRangeException(parameterName, Microsoft.TeamFoundation.VersionControl.Common.Internal.Resources.Format("ChangesetInvalid", (object) version));
    }

    internal static void checkGetOptions(bool force, int maxResults)
    {
      if (force && maxResults > 0)
        throw new ArgumentException("InvalidGetOptionMaxResults");
    }

    internal void checkRequestSize(IValidatable[] requestInputs, string message, int maxInputs)
    {
      if (requestInputs != null && requestInputs.Length > maxInputs)
        throw new ArgumentException(Resources.Format("MaxInputsExceeded", (object) maxInputs), message);
    }

    internal void checkDeferredCheckinOptions(
      string[] serverItems,
      bool deferCheckin,
      int checkinTicket)
    {
      if (checkinTicket < 0)
        throw new ArgumentOutOfRangeException(nameof (checkinTicket));
      if (serverItems == null && deferCheckin)
        throw new ArgumentException(Resources.Get("EmptyCheckInWithDeferNotAllowed"));
    }

    internal void checkLabelItemSpecs(
      LabelItemSpec[] labelItemSpecs,
      string parameterName,
      out bool hasLocalItems,
      bool allowNull)
    {
      this.check((IValidatable[]) labelItemSpecs, "labelSpecs", allowNull);
      hasLocalItems = false;
      if (labelItemSpecs == null)
        return;
      for (int index = 0; index < labelItemSpecs.Length; ++index)
      {
        this.check((IValidatable) labelItemSpecs[index].ItemSpec, string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}].ItemSpec", (object) parameterName, (object) index), false);
        hasLocalItems = hasLocalItems || !labelItemSpecs[index].ItemSpec.isServerItem;
      }
    }

    internal void emptyToNull(ref string s)
    {
      if (s == null || s.Length != 0)
        return;
      s = (string) null;
    }

    internal void emptyToNull(ref string[] array)
    {
      if (array == null || array.Length != 0)
        return;
      array = (string[]) null;
    }

    internal void emptyToRoot(ref ItemSpec[] items)
    {
      if (items != null && items.Length != 0)
        return;
      items = Validation.m_rootRecursive;
    }

    internal void emptyToRoot(ref ItemSpec itemSpec)
    {
      if (itemSpec != null)
        return;
      itemSpec = new ItemSpec("$/", RecursionType.Full);
    }

    internal void nullToEmpty(ref string s)
    {
      if (s != null)
        return;
      s = string.Empty;
    }
  }
}
