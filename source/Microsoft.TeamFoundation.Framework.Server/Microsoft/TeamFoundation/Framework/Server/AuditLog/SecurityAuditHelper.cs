// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AuditLog.SecurityAuditHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.Framework.Server.AuditLog
{
  public static class SecurityAuditHelper
  {
    private static readonly int c_bitsInInt = 32;

    public static Dictionary<string, object> SecurityData(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription namespaceDescription,
      string token,
      IEnumerable<IAccessControlEntry> permissions)
    {
      ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(namespaceDescription, nameof (namespaceDescription));
      return new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription.NamespaceId
        },
        {
          "NamespaceName",
          (object) namespaceDescription.Name
        },
        {
          "Token",
          (object) token
        },
        {
          "Permissions",
          (object) permissions
        },
        {
          "EventSummary",
          (object) SecurityAuditHelper.GetChangedPermissionsFromACEs(namespaceDescription, permissions)
        },
        {
          "EventSummaryType",
          (object) "ChangedPermission"
        }
      };
    }

    public static Dictionary<string, object> SecurityData(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription namespaceDescription,
      IEnumerable<IAccessControlList> accessControlLists)
    {
      ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(namespaceDescription, nameof (namespaceDescription));
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription.NamespaceId
        },
        {
          "NamespaceName",
          (object) namespaceDescription.Name
        },
        {
          "AccessControlLists",
          (object) accessControlLists
        }
      };
      List<ChangedPermission> changedPermissionList = new List<ChangedPermission>();
      foreach (IAccessControlList accessControlList in accessControlLists)
      {
        List<ChangedPermission> permissionsFromAcEs = SecurityAuditHelper.GetChangedPermissionsFromACEs(namespaceDescription, accessControlList.AccessControlEntries);
        changedPermissionList.AddRange((IEnumerable<ChangedPermission>) permissionsFromAcEs);
      }
      dictionary.Add("EventSummary", (object) changedPermissionList);
      dictionary.Add("EventSummaryType", (object) "ChangedPermission");
      return dictionary;
    }

    public static Dictionary<string, object> SecurityData(
      SecurityNamespaceDescription namespaceDescription,
      string token,
      IEnumerable<Guid> identityIds)
    {
      return new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription.NamespaceId
        },
        {
          "NamespaceName",
          (object) namespaceDescription.Name
        },
        {
          "Token",
          (object) token
        },
        {
          "Identities",
          (object) identityIds
        }
      };
    }

    public static Dictionary<string, object> SecurityData(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription namespaceDescription,
      string token,
      IEnumerable<IdentityDescriptor> identityDescriptors,
      int permissions = 0)
    {
      Dictionary<string, object> dictionary = new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription.NamespaceId
        },
        {
          "NamespaceName",
          (object) namespaceDescription.Name
        },
        {
          "Token",
          (object) token
        },
        {
          "Identities",
          (object) identityDescriptors
        },
        {
          "Permissions",
          (object) permissions
        }
      };
      dictionary["Permissions"] = (object) SecurityAuditHelper.GetPermissionsList(namespaceDescription, permissions);
      return dictionary;
    }

    public static Dictionary<string, object> SecurityData(
      SecurityNamespaceDescription namespaceDescription,
      IEnumerable<Guid> identityIds,
      List<RemovedAccessControlEntry> removed)
    {
      return new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription.NamespaceId
        },
        {
          "NamespaceName",
          (object) namespaceDescription.Name
        },
        {
          "Identities",
          (object) identityIds
        },
        {
          "ACEs",
          (object) removed
        }
      };
    }

    public static Dictionary<string, object> SecurityData(
      SecurityNamespaceDescription namespaceDescription,
      IEnumerable<string> tokens,
      bool recurse)
    {
      return new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription.NamespaceId
        },
        {
          "NamespaceName",
          (object) namespaceDescription.Name
        },
        {
          "Tokens",
          (object) tokens
        },
        {
          "Recurse",
          (object) recurse
        }
      };
    }

    public static Dictionary<string, object> SecurityData(
      SecurityNamespaceDescription namespaceDescription)
    {
      return new Dictionary<string, object>()
      {
        {
          "NamespaceId",
          (object) namespaceDescription
        },
        {
          "NamespaceName",
          (object) namespaceDescription
        }
      };
    }

    internal static List<ChangedPermission> GetChangedPermissionsFromACEs(
      SecurityNamespaceDescription namespaceDescription,
      IEnumerable<IAccessControlEntry> permissions)
    {
      List<ChangedPermission> permissionsFromAcEs = new List<ChangedPermission>();
      IDictionary<int, string> dictionary = (IDictionary<int, string>) namespaceDescription.Actions.ToDictionary<ActionDefinition, int, string>((Func<ActionDefinition, int>) (p => p.Bit), (Func<ActionDefinition, string>) (p => p.DisplayName));
      using (IEnumerator<IAccessControlEntry> enumerator = permissions.GetEnumerator())
      {
label_11:
        while (enumerator.MoveNext())
        {
          IAccessControlEntry current = enumerator.Current;
          if (current.IsEmpty || current.Allow == 0 && current.Deny == 0)
          {
            ChangedPermission changedPermission = new ChangedPermission()
            {
              PermissionNames = FrameworkResources.AuditSecurityResetPermissionNames(),
              Change = FrameworkResources.AuditSecurityReset(),
              SubjectDescriptor = current.Descriptor
            };
            permissionsFromAcEs.Add(changedPermission);
          }
          else
          {
            bool flag = current.Allow > 0;
            int key1 = flag ? current.Allow : current.Deny;
            string empty = string.Empty;
            string str1;
            if (dictionary.TryGetValue(key1, out str1))
            {
              string str2 = str1;
              ChangedPermission changedPermission = new ChangedPermission()
              {
                PermissionNames = str2,
                Change = flag ? FrameworkResources.AuditSecurityAllow() : FrameworkResources.AuditSecurityDeny(),
                SubjectDescriptor = current.Descriptor
              };
              permissionsFromAcEs.Add(changedPermission);
            }
            else
            {
              StringBuilder stringBuilder = new StringBuilder();
              int num = 0;
              int key2 = 1;
              while (true)
              {
                if (num < SecurityAuditHelper.c_bitsInInt && num < dictionary.Count)
                {
                  string str3;
                  if ((key1 & key2) == key2 && dictionary.TryGetValue(key2, out str3))
                  {
                    ChangedPermission changedPermission = new ChangedPermission()
                    {
                      PermissionNames = str3,
                      Change = flag ? FrameworkResources.AuditSecurityAllow() : FrameworkResources.AuditSecurityDeny(),
                      SubjectDescriptor = current.Descriptor
                    };
                    permissionsFromAcEs.Add(changedPermission);
                  }
                  ++num;
                  key2 <<= 1;
                }
                else
                  goto label_11;
              }
            }
          }
        }
      }
      return permissionsFromAcEs;
    }

    private static string GetPermissionsList(
      SecurityNamespaceDescription namespaceDescription,
      int permissions)
    {
      List<string> values = new List<string>();
      IDictionary<int, string> dictionary = (IDictionary<int, string>) namespaceDescription.Actions.ToDictionary<ActionDefinition, int, string>((Func<ActionDefinition, int>) (p => p.Bit), (Func<ActionDefinition, string>) (p => p.DisplayName));
      int num1 = permissions;
      if (permissions > 0)
      {
        int num2 = 0;
        int key = 1;
        while (num2 < SecurityAuditHelper.c_bitsInInt && num2 < dictionary.Count)
        {
          string str;
          if ((num1 & key) == key && dictionary.TryGetValue(key, out str))
            values.Add(str);
          ++num2;
          key <<= 1;
        }
      }
      return string.Join(", ", (IEnumerable<string>) values);
    }

    public static string GetActionTypeForResetOrModify(IEnumerable<IAccessControlEntry> aceList) => !aceList.All<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (ace => ace.IsEmpty)) ? SecurityAuditLogConstants.ModifyPermission : SecurityAuditLogConstants.ResetPermission;

    public static string GetActionTypeForResetOrModify(
      IEnumerable<IAccessControlList> accessControlLists)
    {
      foreach (IAccessControlList accessControlList in accessControlLists)
      {
        if (!accessControlList.AccessControlEntries.All<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (ace => ace.IsEmpty)))
          return SecurityAuditLogConstants.ModifyAccessControlLists;
      }
      return SecurityAuditLogConstants.ResetAccessControlLists;
    }
  }
}
