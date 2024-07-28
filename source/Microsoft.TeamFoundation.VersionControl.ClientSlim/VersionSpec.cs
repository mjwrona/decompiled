// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Client.VersionSpec
// Assembly: Microsoft.TeamFoundation.VersionControl.ClientSlim, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CF6BF9DB-38AD-4731-862B-31BA91580FFB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.ClientSlim.dll

using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Common.Internal;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Client
{
  [XmlInclude(typeof (ChangesetVersionSpec))]
  [XmlInclude(typeof (DateVersionSpec))]
  [XmlInclude(typeof (LabelVersionSpec))]
  [XmlInclude(typeof (LatestVersionSpec))]
  [XmlInclude(typeof (WorkspaceVersionSpec))]
  public abstract class VersionSpec
  {
    internal static VersionSpecFactory VersionSpecFactory = (VersionSpecFactory) new VersionSpec.ClientVersionSpecFactory();
    public static readonly char RangeSeparator = VersionSpecCommon.RangeSeparator;
    public static readonly char Separator = ';';
    public static readonly char DeletionModifier = 'X';
    private string m_userInputString;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static VersionSpec FromXml(IServiceProvider serviceProvider, XmlReader reader)
    {
      switch (reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance"))
      {
        case "ChangesetVersionSpec":
          return (VersionSpec) ChangesetVersionSpec.FromXml(serviceProvider, reader);
        case "DateVersionSpec":
          return (VersionSpec) DateVersionSpec.FromXml(serviceProvider, reader);
        case "LabelVersionSpec":
          return (VersionSpec) LabelVersionSpec.FromXml(serviceProvider, reader);
        case "LatestVersionSpec":
          return (VersionSpec) LatestVersionSpec.FromXml(serviceProvider, reader);
        case "WorkspaceVersionSpec":
          return (VersionSpec) WorkspaceVersionSpec.FromXml(serviceProvider, reader);
        default:
          return (VersionSpec) null;
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.AppendLine("VersionSpec instance " + this.GetHashCode().ToString());
      return stringBuilder.ToString();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract void ToXml(XmlWriter writer, string element);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void ToXml(XmlWriter writer, string element, VersionSpec obj) => obj.ToXml(writer, element);

    public static VersionSpec ParseSingleSpec(string versionSpec, string user) => VersionSpec.ParseSingleSpec(versionSpec, user, user);

    public static VersionSpec ParseSingleSpec(string versionSpec, string user, string userDisplay)
    {
      VersionSpec singleSpec = (VersionSpec) VersionSpecCommon.ParseSingleSpec(VersionSpec.VersionSpecFactory, versionSpec, user, userDisplay);
      singleSpec.m_userInputString = versionSpec;
      return singleSpec;
    }

    public static VersionSpec[] Parse(string versionSpec, string user) => VersionSpec.Parse(versionSpec, user, user);

    public static VersionSpec[] Parse(string versionSpec, string user, string userDisplay)
    {
      object[] sourceArray = VersionSpecCommon.Parse(VersionSpec.VersionSpecFactory, versionSpec, user, userDisplay);
      VersionSpec[] destinationArray = new VersionSpec[sourceArray.Length];
      Array.Copy((Array) sourceArray, (Array) destinationArray, sourceArray.Length);
      return destinationArray;
    }

    public static void ParseVersionedFileSpec(
      string spec,
      string user,
      out string fileName,
      out VersionSpec[] versions)
    {
      int length = !string.IsNullOrEmpty(spec) ? spec.IndexOf(VersionSpec.Separator) : throw new InvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
      if (length < 0)
      {
        fileName = spec;
        versions = Array.Empty<VersionSpec>();
      }
      else
      {
        fileName = spec.Substring(0, length);
        versions = VersionSpec.Parse(spec.Substring(length + 1), user);
      }
      if (string.IsNullOrEmpty(fileName))
        throw new InvalidVersionSpecException(Resources.Get("VersionSpecPathEmpty"));
    }

    public static void ParseVersionedFileSpec(
      string spec,
      string user,
      out int deletionId,
      out string fileName,
      out VersionSpec[] versions)
    {
      VersionSpec.ParseVersionedFileSpec(spec, user, user, out deletionId, out fileName, out versions);
    }

    public static void ParseVersionedFileSpec(
      string spec,
      string user,
      string userDisplay,
      out int deletionId,
      out string fileName,
      out VersionSpec[] versions)
    {
      string[] strArray = !string.IsNullOrEmpty(spec) ? spec.Split(VersionSpec.Separator) : throw new InvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
      if (string.IsNullOrEmpty(strArray[0]))
        throw new InvalidVersionSpecException(Resources.Get("VersionSpecPathEmpty"));
      for (int index = 1; index < strArray.Length; ++index)
      {
        if (string.IsNullOrEmpty(strArray[index]))
          throw new InvalidVersionSpecException(Resources.Get("VersionSpecEmpty"));
      }
      if (strArray.Length == 1)
      {
        fileName = spec;
        versions = Array.Empty<VersionSpec>();
        deletionId = 0;
      }
      else if (strArray.Length == 2)
      {
        fileName = strArray[0];
        string str = strArray[1];
        if (VersionSpec.IsDeletionSpecifier(str))
        {
          versions = Array.Empty<VersionSpec>();
          deletionId = VersionSpec.ParseDeletionId(str);
        }
        else
        {
          versions = VersionSpec.Parse(str, user, userDisplay);
          deletionId = 0;
        }
      }
      else
      {
        fileName = strArray.Length == 3 ? strArray[0] : throw new InvalidVersionSpecException(Resources.Get("VersionSpecTooManyVersionsSpecified"));
        string str1 = strArray[1];
        string str2 = strArray[2];
        bool flag1 = VersionSpec.IsDeletionSpecifier(str1);
        bool flag2 = VersionSpec.IsDeletionSpecifier(str2);
        bool flag3 = VersionSpec.IsWorkspaceSpecifier(str1);
        if (flag1 & flag2)
          throw new InvalidVersionSpecException(Resources.Get("VersionSpecSpecifyOnlyOneDeletionId"));
        if (!flag1 && !flag2)
        {
          if (!flag3)
            throw new InvalidVersionSpecException(Resources.Get("VersionSpecTooManyVersionsSpecified"));
          versions = VersionSpec.Parse(str1 + (object) VersionSpec.Separator + str2, user, userDisplay);
          deletionId = 0;
        }
        else if (flag1)
        {
          versions = VersionSpec.Parse(str2, user, userDisplay);
          deletionId = VersionSpec.ParseDeletionId(str1);
        }
        else
        {
          versions = VersionSpec.Parse(str1, user, userDisplay);
          deletionId = VersionSpec.ParseDeletionId(str2);
        }
      }
      if (string.IsNullOrEmpty(fileName))
        throw new InvalidVersionSpecException(Resources.Get("VersionSpecPathEmpty"));
    }

    private static bool IsDeletionSpecifier(string spec) => (int) char.ToUpperInvariant(spec[0]) == (int) VersionSpec.DeletionModifier;

    private static bool IsWorkspaceSpecifier(string spec) => (int) char.ToUpperInvariant(spec[0]) == (int) WorkspaceVersionSpec.Identifier;

    private static int ParseDeletionId(string spec)
    {
      string s = spec.Substring(1);
      int result;
      return int.TryParse(s, out result) && result > 0 ? result : throw new InvalidVersionSpecException(Resources.Format("VersionSpecInvalidDeletionId", (object) s));
    }

    public string Format(string path)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      return path + (object) VersionSpec.Separator + this.ComputeVersionString();
    }

    public static string FormatRange(string path, VersionSpec from, VersionSpec to)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      ArgumentUtility.CheckForNull<VersionSpec>(from, nameof (from));
      ArgumentUtility.CheckForNull<VersionSpec>(to, nameof (to));
      return path + (object) VersionSpec.Separator + from.ComputeVersionString() + (object) VersionSpec.RangeSeparator + to.ComputeVersionString();
    }

    public static string AddDeletionModifierIfNecessary(string path, int deletionId)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      if (deletionId == 0)
        return path;
      return path + (object) VersionSpec.Separator + (object) VersionSpec.DeletionModifier + (object) deletionId;
    }

    public static bool ReorderVersionSpecs(ref VersionSpec start, ref VersionSpec end)
    {
      if (start is LabelVersionSpec || end is LabelVersionSpec || start is WorkspaceVersionSpec || end is WorkspaceVersionSpec)
        return false;
      if (start is ChangesetVersionSpec && end is ChangesetVersionSpec && ((ChangesetVersionSpec) start).ChangesetId > ((ChangesetVersionSpec) end).ChangesetId)
      {
        VersionSpec.Swap<VersionSpec>(ref start, ref end);
        return true;
      }
      if (start is DateVersionSpec && end is DateVersionSpec && ((DateVersionSpec) start).Date > ((DateVersionSpec) end).Date)
      {
        VersionSpec.Swap<VersionSpec>(ref start, ref end);
        return true;
      }
      if (!(start is LatestVersionSpec))
        return false;
      VersionSpec.Swap<VersionSpec>(ref start, ref end);
      return true;
    }

    private static void Swap<T>(ref T x, ref T y)
    {
      T obj = x;
      x = y;
      y = obj;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract string ComputeVersionString();

    public string DisplayString => this.m_userInputString != null ? this.m_userInputString : this.ComputeVersionString();

    public static VersionSpec Latest => (VersionSpec) LatestVersionSpec.Instance;

    private class ClientVersionSpecFactory : VersionSpecFactory
    {
      public override object CreateVersionSpec(
        VersionSpecType type,
        string originalInput,
        params object[] parameters)
      {
        VersionSpec versionSpec;
        switch (type)
        {
          case VersionSpecType.Changeset:
            versionSpec = (VersionSpec) new ChangesetVersionSpec((string) parameters[0]);
            break;
          case VersionSpecType.Date:
            versionSpec = (VersionSpec) new DateVersionSpec((DateTime) parameters[0], (string) parameters[1]);
            break;
          case VersionSpecType.Label:
            versionSpec = (VersionSpec) new LabelVersionSpec((string) parameters[0], (string) parameters[1]);
            break;
          case VersionSpecType.Latest:
            versionSpec = VersionSpec.Latest;
            break;
          case VersionSpecType.Workspace:
            if (parameters.Length == 2)
            {
              versionSpec = (VersionSpec) new WorkspaceVersionSpec((string) parameters[0])
              {
                OwnerDisplayName = (string) parameters[1]
              };
              break;
            }
            versionSpec = (VersionSpec) new WorkspaceVersionSpec((string) parameters[0], (string) parameters[1], (string) parameters[2]);
            break;
          default:
            versionSpec = (VersionSpec) null;
            break;
        }
        if (VersionSpec.Latest != versionSpec)
          versionSpec.m_userInputString = originalInput;
        return (object) versionSpec;
      }

      public override void ThrowInvalidVersionSpecException(string message) => throw new InvalidVersionSpecException(message);
    }
  }
}
