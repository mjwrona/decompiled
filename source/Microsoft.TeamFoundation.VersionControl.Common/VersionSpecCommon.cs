// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Common.VersionSpecCommon
// Assembly: Microsoft.TeamFoundation.VersionControl.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 156CCB01-0A1F-468C-A332-06DB9F9B179E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Common.dll

using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using System;
using System.ComponentModel;
using System.Globalization;

namespace Microsoft.TeamFoundation.VersionControl.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class VersionSpecCommon
  {
    public static readonly char RangeSeparator = '~';
    private static readonly char[] s_versionSpecRangeDelims = new char[1]
    {
      VersionSpecCommon.RangeSeparator
    };
    public static readonly char ChangesetIdentifier = 'C';
    public static readonly char DateIdentifier = 'D';
    public static readonly char LabelIdentifier = 'L';
    public static readonly char LatestIdentifier = 'T';
    public static readonly char WorkspaceIdentifier = 'W';

    public static object ParseSingleSpec(
      VersionSpecFactory factory,
      string versionSpec,
      string user,
      string userDisplay)
    {
      if (string.IsNullOrEmpty(versionSpec))
        factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
      if (versionSpec.IndexOf(VersionSpecCommon.RangeSeparator) != -1)
        factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecRangeNotPermitted"));
      char upperInvariant = char.ToUpperInvariant(versionSpec[0]);
      if ((int) upperInvariant == (int) VersionSpecCommon.LabelIdentifier)
      {
        if (versionSpec.Length == 1)
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
        string labelName;
        string labelScope;
        LabelSpec.Parse(versionSpec.Substring(1), (string) null, false, out labelName, out labelScope);
        return factory.CreateVersionSpec(VersionSpecType.Label, versionSpec, (object) labelName, (object) labelScope);
      }
      if ((int) upperInvariant == (int) VersionSpecCommon.DateIdentifier)
      {
        if (versionSpec.Length == 1)
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
        string s = versionSpec.Substring(1);
        DateTime result;
        if (!DateTime.TryParse(s, (IFormatProvider) null, DateTimeStyles.AssumeLocal, out result))
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecInvalidDateTime"));
        return factory.CreateVersionSpec(VersionSpecType.Date, versionSpec, (object) result, (object) s);
      }
      if ((int) upperInvariant == (int) VersionSpecCommon.WorkspaceIdentifier)
      {
        string workspaceName;
        string workspaceOwner;
        string workspaceOwnerDisplay;
        WorkspaceSpec.Parse(versionSpec.Substring(1), user, userDisplay, out workspaceName, out workspaceOwner, out workspaceOwnerDisplay);
        return !string.IsNullOrEmpty(workspaceName) ? factory.CreateVersionSpec(VersionSpecType.Workspace, versionSpec, (object) workspaceName, (object) workspaceOwner, (object) workspaceOwnerDisplay) : factory.CreateVersionSpec(VersionSpecType.Workspace, versionSpec, (object) workspaceOwner, (object) workspaceOwnerDisplay);
      }
      if ((int) upperInvariant == (int) VersionSpecCommon.LatestIdentifier)
      {
        if (versionSpec.Length > 1)
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecInvalidTip"));
        return factory.CreateVersionSpec(VersionSpecType.Latest, versionSpec);
      }
      int startIndex = 0;
      if ((int) upperInvariant == (int) VersionSpecCommon.ChangesetIdentifier)
      {
        if (versionSpec.Length == 1)
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
        startIndex = 1;
      }
      else if (char.IsLetter(upperInvariant))
        factory.ThrowInvalidVersionSpecException(Resources.Format("VersionSpecIndicator", (object) upperInvariant));
      return factory.CreateVersionSpec(VersionSpecType.Changeset, versionSpec, (object) versionSpec.Substring(startIndex));
    }

    public static object[] Parse(
      VersionSpecFactory factory,
      string versionSpec,
      string user,
      string userDisplay)
    {
      if (string.IsNullOrEmpty(versionSpec))
        factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
      object[] objArray1 = (object[]) null;
      string[] strArray = versionSpec.Split(VersionSpecCommon.s_versionSpecRangeDelims);
      if (strArray.Length != 0)
      {
        if (string.IsNullOrEmpty(strArray[0]) && (strArray.Length == 1 || string.IsNullOrEmpty(strArray[1])))
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
        if (strArray.Length > 2)
          factory.ThrowInvalidVersionSpecException(Resources.Get("VersionSpecOnlyTwoValid"));
        char ch;
        if (strArray.Length > 1)
        {
          objArray1 = new object[2];
          if (strArray[1].Length > 0)
          {
            objArray1[1] = VersionSpecCommon.ParseSingleSpec(factory, strArray[1], user, userDisplay);
          }
          else
          {
            object[] objArray2 = objArray1;
            VersionSpecFactory versionSpecFactory = factory;
            ch = VersionSpecCommon.LatestIdentifier;
            string originalInput = ch.ToString();
            object[] objArray3 = Array.Empty<object>();
            object versionSpec1 = versionSpecFactory.CreateVersionSpec(VersionSpecType.Latest, originalInput, objArray3);
            objArray2[1] = versionSpec1;
          }
        }
        else
          objArray1 = new object[1];
        if (strArray[0].Length > 0)
        {
          objArray1[0] = VersionSpecCommon.ParseSingleSpec(factory, strArray[0], user, userDisplay);
        }
        else
        {
          object[] objArray4 = objArray1;
          VersionSpecFactory versionSpecFactory = factory;
          ch = VersionSpecCommon.ChangesetIdentifier;
          string originalInput = ch.ToString() + "1";
          object[] objArray5 = new object[1]{ (object) "1" };
          object versionSpec2 = versionSpecFactory.CreateVersionSpec(VersionSpecType.Changeset, originalInput, objArray5);
          objArray4[0] = versionSpec2;
        }
      }
      if (objArray1 == null)
        objArray1 = Array.Empty<object>();
      return objArray1;
    }

    public static int ParseChangesetNumber(VersionSpecFactory factory, string changesetId)
    {
      if (string.IsNullOrEmpty(changesetId))
        factory.ThrowInvalidVersionSpecException(Resources.Get("ChangesetEmpty"));
      int result;
      if (!int.TryParse(changesetId, out result))
        factory.ThrowInvalidVersionSpecException(Resources.Format("ChangesetInvalid", (object) changesetId));
      VersionSpecCommon.ValidateNumber(factory, result);
      return result;
    }

    public static void ValidateNumber(VersionSpecFactory factory, int changesetId)
    {
      if (changesetId > 0)
        return;
      factory.ThrowInvalidVersionSpecException(Resources.Format("ChangesetInvalid", (object) changesetId.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
    }
  }
}
