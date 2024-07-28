// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.ResourceId
// Assembly: Microsoft.Azure.Cosmos.Direct, Version=3.29.4.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: FFE3C00D-4333-4294-8947-B1C93A852E2F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Direct.dll

using Microsoft.Azure.Cosmos.Core.Trace;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.Azure.Documents
{
  internal sealed class ResourceId : IEquatable<ResourceId>
  {
    private const int EncryptionScopeIdLength = 5;
    private const int OfferIdLength = 3;
    private const int RbacResourceIdLength = 6;
    private const int SnapshotIdLength = 7;
    public static readonly ushort Length = 20;
    public static readonly ushort MaxPathFragment = 8;
    public static readonly ResourceId Empty = new ResourceId();

    private ResourceId()
    {
      this.Offer = 0U;
      this.Database = 0U;
      this.DocumentCollection = 0U;
      this.ClientEncryptionKey = 0U;
      this.StoredProcedure = 0UL;
      this.Trigger = 0UL;
      this.UserDefinedFunction = 0UL;
      this.Document = 0UL;
      this.PartitionKeyRange = 0UL;
      this.User = 0U;
      this.ClientEncryptionKey = 0U;
      this.Permission = 0UL;
      this.Attachment = 0U;
      this.Schema = 0UL;
      this.UserDefinedType = 0U;
      this.Snapshot = 0UL;
      this.RoleAssignment = 0UL;
      this.RoleDefinition = 0UL;
      this.SystemDocument = 0UL;
      this.PartitionedSystemDocument = 0UL;
      this.EncryptionScope = 0UL;
    }

    public uint Offer { get; private set; }

    public ResourceId OfferId => new ResourceId()
    {
      Offer = this.Offer
    };

    public uint Database { get; private set; }

    public ResourceId DatabaseId => new ResourceId()
    {
      Database = this.Database
    };

    public bool IsDatabaseId => this.Database != 0U && this.DocumentCollection == 0U && this.User == 0U && this.UserDefinedType == 0U && this.ClientEncryptionKey == 0U;

    public bool IsDocumentCollectionId => this.Database != 0U && this.DocumentCollection != 0U && this.Document == 0UL && this.PartitionKeyRange == 0UL && this.StoredProcedure == 0UL && this.Trigger == 0UL && this.UserDefinedFunction == 0UL && this.SystemDocument == 0UL && this.PartitionedSystemDocument == 0UL;

    public bool IsPartitionKeyRangeId => this.Database != 0U && this.DocumentCollection != 0U && this.PartitionKeyRange != 0UL && this.Document == 0UL && this.StoredProcedure == 0UL && this.Trigger == 0UL && this.UserDefinedFunction == 0UL && this.SystemDocument == 0UL && this.PartitionedSystemDocument == 0UL;

    public uint DocumentCollection { get; private set; }

    public ResourceId DocumentCollectionId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection
    };

    public bool IsClientEncryptionKeyId => this.Database != 0U && this.ClientEncryptionKey > 0U;

    public ulong UniqueDocumentCollectionId => (ulong) this.Database << 32 | (ulong) this.DocumentCollection;

    public ulong StoredProcedure { get; private set; }

    public ResourceId StoredProcedureId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      StoredProcedure = this.StoredProcedure
    };

    public ulong Trigger { get; private set; }

    public ResourceId TriggerId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      Trigger = this.Trigger
    };

    public ulong UserDefinedFunction { get; private set; }

    public ResourceId UserDefinedFunctionId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      UserDefinedFunction = this.UserDefinedFunction
    };

    public ulong Conflict { get; private set; }

    public ResourceId ConflictId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      Conflict = this.Conflict
    };

    public ulong Document { get; private set; }

    public ResourceId DocumentId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      Document = this.Document
    };

    public ulong PartitionKeyRange { get; private set; }

    public ResourceId PartitionKeyRangeId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      PartitionKeyRange = this.PartitionKeyRange
    };

    public uint User { get; private set; }

    public ResourceId UserId => new ResourceId()
    {
      Database = this.Database,
      User = this.User
    };

    public uint ClientEncryptionKey { get; private set; }

    public ResourceId ClientEncryptionKeyId => new ResourceId()
    {
      Database = this.Database,
      ClientEncryptionKey = this.ClientEncryptionKey
    };

    public uint UserDefinedType { get; private set; }

    public ResourceId UserDefinedTypeId => new ResourceId()
    {
      Database = this.Database,
      UserDefinedType = this.UserDefinedType
    };

    public ulong Permission { get; private set; }

    public ResourceId PermissionId => new ResourceId()
    {
      Database = this.Database,
      User = this.User,
      Permission = this.Permission
    };

    public uint Attachment { get; private set; }

    public ResourceId AttachmentId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      Document = this.Document,
      Attachment = this.Attachment
    };

    public ulong Schema { get; private set; }

    public ResourceId SchemaId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      Schema = this.Schema
    };

    public ulong Snapshot { get; private set; }

    public ResourceId SnapshotId => new ResourceId()
    {
      Snapshot = this.Snapshot
    };

    public bool IsSnapshotId => this.Snapshot > 0UL;

    public ulong EncryptionScope { get; private set; }

    public ResourceId EncryptionScopeId => new ResourceId()
    {
      EncryptionScope = this.EncryptionScope
    };

    public bool IsEncryptionScopeId => this.EncryptionScope > 0UL;

    public ulong RoleAssignment { get; private set; }

    public ResourceId RoleAssignmentId => new ResourceId()
    {
      RoleAssignment = this.RoleAssignment
    };

    public bool IsRoleAssignmentId => this.RoleAssignment > 0UL;

    public ulong RoleDefinition { get; private set; }

    public ResourceId RoleDefinitionId => new ResourceId()
    {
      RoleDefinition = this.RoleDefinition
    };

    public bool IsRoleDefinitionId => this.RoleDefinition > 0UL;

    public ulong AuthPolicyElement { get; private set; }

    public ResourceId AuthPolicyElementId => new ResourceId()
    {
      AuthPolicyElement = this.AuthPolicyElement
    };

    public bool IsAuthPolicyElementId => this.AuthPolicyElement > 0UL;

    public ulong SystemDocument { get; private set; }

    public ResourceId SystemDocumentId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      SystemDocument = this.SystemDocument
    };

    public ulong PartitionedSystemDocument { get; private set; }

    public ResourceId PartitionedSystemDocumentId => new ResourceId()
    {
      Database = this.Database,
      DocumentCollection = this.DocumentCollection,
      PartitionedSystemDocument = this.PartitionedSystemDocument
    };

    public ulong InteropUser { get; private set; }

    public ResourceId InteropUserId => new ResourceId()
    {
      InteropUser = this.InteropUser
    };

    public bool IsInteropUserId => this.InteropUser > 0UL;

    public byte[] Value
    {
      get
      {
        int length = 0;
        if (this.Offer > 0U)
          length += 3;
        else if (this.Snapshot > 0UL)
          length += 7;
        else if (this.EncryptionScope > 0UL)
          length += 5;
        else if (this.RoleAssignment > 0UL)
          length += 6;
        else if (this.RoleDefinition > 0UL)
          length += 6;
        else if (this.AuthPolicyElement > 0UL)
          length += 6;
        else if (this.InteropUser > 0UL)
          length += 6;
        else if (this.Database > 0U)
          length += 4;
        if (this.DocumentCollection > 0U || this.User > 0U || this.UserDefinedType > 0U || this.ClientEncryptionKey > 0U)
          length += 4;
        if (this.Document > 0UL || this.Permission > 0UL || this.StoredProcedure > 0UL || this.Trigger > 0UL || this.UserDefinedFunction > 0UL || this.Conflict > 0UL || this.PartitionKeyRange > 0UL || this.Schema > 0UL || this.UserDefinedType > 0U || this.ClientEncryptionKey > 0U || this.SystemDocument > 0UL || this.PartitionedSystemDocument > 0UL)
          length += 8;
        if (this.Attachment > 0U)
          length += 4;
        byte[] dst = new byte[length];
        if (this.Offer > 0U)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Offer), 0, dst, 0, 3);
        else if (this.Database > 0U)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Database), 0, dst, 0, 4);
        else if (this.Snapshot > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Snapshot), 0, dst, 0, 7);
        else if (this.EncryptionScope > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.EncryptionScope), 0, dst, 0, 5);
        else if (this.AuthPolicyElement > 0UL)
        {
          ResourceId.BlockCopy(BitConverter.GetBytes(this.AuthPolicyElement), 0, dst, 0, 4);
          ResourceId.BlockCopy(BitConverter.GetBytes(12288), 0, dst, 4, 2);
        }
        else if (this.RoleAssignment > 0UL)
        {
          ResourceId.BlockCopy(BitConverter.GetBytes(this.RoleAssignment), 0, dst, 0, 4);
          ResourceId.BlockCopy(BitConverter.GetBytes(4096), 0, dst, 4, 2);
        }
        else if (this.RoleDefinition > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.RoleDefinition), 0, dst, 0, 6);
        else if (this.InteropUser > 0UL)
        {
          ResourceId.BlockCopy(BitConverter.GetBytes(this.InteropUser), 0, dst, 0, 6);
          ResourceId.BlockCopy(BitConverter.GetBytes(8192), 0, dst, 4, 2);
        }
        if (this.DocumentCollection > 0U)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.DocumentCollection), 0, dst, 4, 4);
        else if (this.User > 0U)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.User), 0, dst, 4, 4);
        if (this.StoredProcedure > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.StoredProcedure), 0, dst, 8, 8);
        else if (this.Trigger > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Trigger), 0, dst, 8, 8);
        else if (this.UserDefinedFunction > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.UserDefinedFunction), 0, dst, 8, 8);
        else if (this.Conflict > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Conflict), 0, dst, 8, 8);
        else if (this.Document > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Document), 0, dst, 8, 8);
        else if (this.PartitionKeyRange > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.PartitionKeyRange), 0, dst, 8, 8);
        else if (this.Permission > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Permission), 0, dst, 8, 8);
        else if (this.Schema > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Schema), 0, dst, 8, 8);
        else if (this.SystemDocument > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.SystemDocument), 0, dst, 8, 8);
        else if (this.PartitionedSystemDocument > 0UL)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.PartitionedSystemDocument), 0, dst, 8, 8);
        else if (this.UserDefinedType > 0U)
        {
          ResourceId.BlockCopy(BitConverter.GetBytes(this.UserDefinedType), 0, dst, 8, 4);
          ResourceId.BlockCopy(BitConverter.GetBytes(1U), 0, dst, 12, 4);
        }
        else if (this.ClientEncryptionKey > 0U)
        {
          ResourceId.BlockCopy(BitConverter.GetBytes(this.ClientEncryptionKey), 0, dst, 8, 4);
          ResourceId.BlockCopy(BitConverter.GetBytes(2U), 0, dst, 12, 4);
        }
        if (this.Attachment > 0U)
          ResourceId.BlockCopy(BitConverter.GetBytes(this.Attachment), 0, dst, 16, 4);
        return dst;
      }
    }

    public static ResourceId Parse(string id)
    {
      ResourceId rid = (ResourceId) null;
      if (!ResourceId.TryParse(id, out rid))
        throw new BadRequestException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, RMResources.InvalidResourceID, (object) id));
      return rid;
    }

    public static byte[] Parse(ResourceType eResourceType, string id) => ResourceId.HasNonHierarchicalResourceId(eResourceType) ? Encoding.UTF8.GetBytes(id) : ResourceId.Parse(id).Value;

    public static ResourceId NewDatabaseId(uint dbid) => new ResourceId()
    {
      Database = dbid
    };

    public static ResourceId NewRoleDefinitionId(ulong roleDefinitionId) => new ResourceId()
    {
      RoleDefinition = roleDefinitionId
    };

    public static ResourceId NewRoleAssignmentId(ulong roleAssignmentId) => new ResourceId()
    {
      RoleAssignment = roleAssignmentId
    };

    public static ResourceId NewAuthPolicyElementId(ulong authPolicyElementId) => new ResourceId()
    {
      AuthPolicyElement = authPolicyElementId
    };

    public static ResourceId NewSnapshotId(ulong snapshotId) => new ResourceId()
    {
      Snapshot = snapshotId
    };

    public static ResourceId NewEncryptionScopeId(ulong encryptionScopeId) => new ResourceId()
    {
      EncryptionScope = encryptionScopeId
    };

    public static ResourceId NewInteropUserId(ulong interopUserId) => new ResourceId()
    {
      InteropUser = interopUserId
    };

    public static ResourceId NewDocumentCollectionId(string databaseId, uint collectionId) => ResourceId.NewDocumentCollectionId(ResourceId.Parse(databaseId).Database, collectionId);

    public static ResourceId NewDocumentCollectionId(uint databaseId, uint collectionId) => new ResourceId()
    {
      Database = databaseId,
      DocumentCollection = collectionId
    };

    public static ResourceId NewClientEncryptionKeyId(string databaseId, uint clientEncryptionKeyId) => ResourceId.NewClientEncryptionKeyId(ResourceId.Parse(databaseId).Database, clientEncryptionKeyId);

    public static ResourceId NewClientEncryptionKeyId(uint databaseId, uint clientEncryptionKeyId) => new ResourceId()
    {
      Database = databaseId,
      ClientEncryptionKey = clientEncryptionKeyId
    };

    public static ResourceId NewCollectionChildResourceId(
      string collectionId,
      ulong childId,
      ResourceType resourceType)
    {
      ResourceId resourceId1 = ResourceId.Parse(collectionId);
      if (!resourceId1.IsDocumentCollectionId)
      {
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid collection RID '{0}'.", (object) collectionId);
        DefaultTrace.TraceError(message);
        throw new ArgumentException(message);
      }
      ResourceId resourceId2 = new ResourceId();
      resourceId2.Database = resourceId1.Database;
      resourceId2.DocumentCollection = resourceId1.DocumentCollection;
      switch (resourceType)
      {
        case ResourceType.Document:
          resourceId2.Document = childId;
          return resourceId2;
        case ResourceType.StoredProcedure:
          resourceId2.StoredProcedure = childId;
          return resourceId2;
        case ResourceType.Trigger:
          resourceId2.Trigger = childId;
          return resourceId2;
        case ResourceType.UserDefinedFunction:
          resourceId2.UserDefinedFunction = childId;
          return resourceId2;
        case ResourceType.PartitionKeyRange:
          resourceId2.PartitionKeyRange = childId;
          return resourceId2;
        case ResourceType.PartitionedSystemDocument:
          resourceId2.PartitionedSystemDocument = childId;
          return resourceId2;
        case ResourceType.SystemDocument:
          resourceId2.SystemDocument = childId;
          return resourceId2;
        default:
          string message1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ResourceType '{0}'  not a child of Collection.", (object) resourceType);
          DefaultTrace.TraceError(message1);
          throw new ArgumentException(message1);
      }
    }

    public static ResourceId NewUserId(string databaseId, uint userId)
    {
      ResourceId resourceId = ResourceId.Parse(databaseId);
      return new ResourceId()
      {
        Database = resourceId.Database,
        User = userId
      };
    }

    public static ResourceId NewPermissionId(string userId, ulong permissionId)
    {
      ResourceId resourceId = ResourceId.Parse(userId);
      return new ResourceId()
      {
        Database = resourceId.Database,
        User = resourceId.User,
        Permission = permissionId
      };
    }

    public static ResourceId NewAttachmentId(string documentId, uint attachmentId)
    {
      ResourceId resourceId = ResourceId.Parse(documentId);
      return new ResourceId()
      {
        Database = resourceId.Database,
        DocumentCollection = resourceId.DocumentCollection,
        Document = resourceId.Document,
        Attachment = attachmentId
      };
    }

    public static string CreateNewCollectionChildResourceId(
      int childResourceIdIndex,
      ResourceType resourceType,
      string ownerResourceId)
    {
      byte[] numArray = new byte[8];
      switch (resourceType)
      {
        case ResourceType.Document:
          numArray[7] = (byte) 0;
          break;
        case ResourceType.StoredProcedure:
          numArray[7] = (byte) 128;
          break;
        case ResourceType.Trigger:
          numArray[7] = (byte) 112;
          break;
        case ResourceType.UserDefinedFunction:
          numArray[7] = (byte) 96;
          break;
        case ResourceType.PartitionKeyRange:
          numArray[7] = (byte) 80;
          break;
        case ResourceType.PartitionedSystemDocument:
          numArray[7] = (byte) 160;
          break;
        case ResourceType.SystemDocument:
          numArray[7] = (byte) 208;
          break;
        default:
          string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid resource for CreateNewCollectionChildResourceId: '{0}'.", (object) resourceType);
          DefaultTrace.TraceError(message);
          throw new ArgumentException(message);
      }
      byte[] bytes = BitConverter.GetBytes(childResourceIdIndex);
      if (bytes.Length > 6)
        throw new BadRequestException("ChildResourceIdIndex size is too big to be used as resource id.");
      for (int index = 0; index < bytes.Length; ++index)
        numArray[index] = bytes[index];
      int startIndex = 0;
      ulong uint64 = BitConverter.ToUInt64(numArray, startIndex);
      return ResourceId.NewCollectionChildResourceId(ownerResourceId, uint64, resourceType).ToString();
    }

    public static bool TryParse(string id, out ResourceId rid)
    {
      rid = (ResourceId) null;
      try
      {
        if (string.IsNullOrEmpty(id) || id.Length % 4 != 0)
          return false;
        byte[] buffer = (byte[]) null;
        if (!ResourceId.Verify(id, out buffer) || buffer.Length % 4 != 0 && buffer.Length != 3 && buffer.Length != 7 && buffer.Length != 6 && buffer.Length != 5)
          return false;
        rid = new ResourceId();
        if (buffer.Length == 3)
        {
          rid.Offer = (uint) ResourceId.ToUnsignedLong(buffer);
          return true;
        }
        if (buffer.Length == 7)
        {
          rid.Snapshot = ResourceId.ToUnsignedLong(buffer);
          return true;
        }
        if (buffer.Length == 6)
        {
          int num = (int) buffer[5];
          ulong unsignedLong = ResourceId.ToUnsignedLong(buffer, 4);
          ResourceId.RbacResourceType rbacResourceType = (ResourceId.RbacResourceType) num;
          if ((uint) rbacResourceType <= 16U)
          {
            switch (rbacResourceType)
            {
              case ResourceId.RbacResourceType.RbacResourceType_RoleDefinition:
                rid.RoleDefinition = unsignedLong;
                goto label_18;
              case ResourceId.RbacResourceType.RbacResourceType_RoleAssignment:
                rid.RoleAssignment = unsignedLong;
                goto label_18;
            }
          }
          else
          {
            switch (rbacResourceType)
            {
              case ResourceId.RbacResourceType.RbacResourceType_InteropUser:
                rid.InteropUser = unsignedLong;
                goto label_18;
              case ResourceId.RbacResourceType.RbacResourceType_AuthPolicyElement:
                rid.AuthPolicyElement = unsignedLong;
                goto label_18;
            }
          }
          return false;
label_18:
          return true;
        }
        if (buffer.Length == 5)
        {
          rid.EncryptionScope = (ulong) (uint) ResourceId.ToUnsignedLong(buffer);
          return true;
        }
        if (buffer.Length >= 4)
          rid.Database = BitConverter.ToUInt32(buffer, 0);
        if (buffer.Length >= 8)
        {
          byte[] dst1 = new byte[4];
          ResourceId.BlockCopy(buffer, 4, dst1, 0, 4);
          if ((((int) dst1[0] & 128) > 0 ? 1 : 0) != 0)
          {
            rid.DocumentCollection = BitConverter.ToUInt32(dst1, 0);
            if (buffer.Length >= 16)
            {
              byte[] dst2 = new byte[8];
              ResourceId.BlockCopy(buffer, 8, dst2, 0, 8);
              ulong uint64 = BitConverter.ToUInt64(buffer, 8);
              if ((int) dst2[7] >> 4 == 0)
              {
                rid.Document = uint64;
                if (buffer.Length == 20)
                  rid.Attachment = BitConverter.ToUInt32(buffer, 16);
              }
              else if ((int) dst2[7] >> 4 == 8)
                rid.StoredProcedure = uint64;
              else if ((int) dst2[7] >> 4 == 7)
                rid.Trigger = uint64;
              else if ((int) dst2[7] >> 4 == 6)
                rid.UserDefinedFunction = uint64;
              else if ((int) dst2[7] >> 4 == 4)
                rid.Conflict = uint64;
              else if ((int) dst2[7] >> 4 == 5)
                rid.PartitionKeyRange = uint64;
              else if ((int) dst2[7] >> 4 == 9)
                rid.Schema = uint64;
              else if ((int) dst2[7] >> 4 == 13)
              {
                rid.SystemDocument = uint64;
              }
              else
              {
                if ((int) dst2[7] >> 4 != 10)
                  return false;
                rid.PartitionedSystemDocument = uint64;
              }
            }
            else if (buffer.Length != 8)
              return false;
          }
          else
          {
            rid.User = BitConverter.ToUInt32(dst1, 0);
            if (buffer.Length == 16)
            {
              if (rid.User > 0U)
              {
                rid.Permission = BitConverter.ToUInt64(buffer, 8);
              }
              else
              {
                uint uint32 = BitConverter.ToUInt32(buffer, 8);
                switch ((ResourceId.ExtendedDatabaseChildResourceType) BitConverter.ToUInt32(buffer, 12))
                {
                  case ResourceId.ExtendedDatabaseChildResourceType.UserDefinedType:
                    rid.UserDefinedType = uint32;
                    break;
                  case ResourceId.ExtendedDatabaseChildResourceType.ClientEncryptionKey:
                    rid.ClientEncryptionKey = uint32;
                    break;
                  default:
                    return false;
                }
              }
            }
            else if (buffer.Length != 8)
              return false;
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        return false;
      }
    }

    public static bool Verify(string id, out byte[] buffer)
    {
      if (string.IsNullOrEmpty(id))
        throw new ArgumentNullException(nameof (id));
      if (ResourceId.TryDecodeFromBase64String(id, out buffer) && buffer.Length <= (int) ResourceId.Length)
        return true;
      buffer = (byte[]) null;
      return false;
    }

    public static bool Verify(string id)
    {
      byte[] buffer = (byte[]) null;
      return ResourceId.Verify(id, out buffer);
    }

    public override string ToString() => ResourceId.ToBase64String(this.Value);

    public bool Equals(ResourceId other) => other != null && ((IEnumerable<byte>) this.Value).SequenceEqual<byte>((IEnumerable<byte>) other.Value);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return obj is ResourceId && this.Equals((ResourceId) obj);
    }

    public override int GetHashCode() => throw new NotImplementedException();

    public static bool TryDecodeFromBase64String(string s, out byte[] bytes) => ResourceIdBase64Decoder.TryDecode(s.Replace('-', '/'), out bytes);

    public static ulong ToUnsignedLong(byte[] buffer) => ResourceId.ToUnsignedLong(buffer, buffer.Length);

    public static ulong ToUnsignedLong(byte[] buffer, int length)
    {
      ulong unsignedLong = 0;
      for (int index = 0; index < length; ++index)
        unsignedLong |= (ulong) ((uint) buffer[index] << index * 8);
      return unsignedLong;
    }

    public static string ToBase64String(byte[] buffer) => ResourceId.ToBase64String(buffer, 0, buffer.Length);

    public static string ToBase64String(byte[] buffer, int offset, int length) => Convert.ToBase64String(buffer, offset, length).Replace('/', '-');

    private static ResourceId NewDocumentId(uint dbId, uint collId) => new ResourceId()
    {
      Database = dbId,
      DocumentCollection = collId,
      Document = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0)
    };

    private static ResourceId NewDocumentCollectionId(uint dbId)
    {
      ResourceId resourceId = new ResourceId();
      resourceId.Database = dbId;
      byte[] dst = new byte[4];
      byte[] byteArray = Guid.NewGuid().ToByteArray();
      byteArray[0] |= (byte) 128;
      ResourceId.BlockCopy(byteArray, 0, dst, 0, 4);
      resourceId.DocumentCollection = BitConverter.ToUInt32(dst, 0);
      resourceId.Document = 0UL;
      resourceId.User = 0U;
      resourceId.Permission = 0UL;
      return resourceId;
    }

    private static ResourceId NewDatabaseId() => new ResourceId()
    {
      Database = BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0),
      DocumentCollection = 0,
      Document = 0,
      User = 0,
      Permission = 0
    };

    public static void BlockCopy(byte[] src, int srcOffset, byte[] dst, int dstOffset, int count)
    {
      int num = srcOffset + count;
      for (int index = srcOffset; index < num; ++index)
        dst[dstOffset++] = src[index];
    }

    private static bool HasNonHierarchicalResourceId(ResourceType eResourceType) => false;

    private enum CollectionChildResourceType : byte
    {
      Document = 0,
      Conflict = 4,
      PartitionKeyRange = 5,
      UserDefinedFunction = 6,
      Trigger = 7,
      StoredProcedure = 8,
      Schema = 9,
      PartitionedSystemDocument = 10, // 0x0A
      SystemDocument = 13, // 0x0D
    }

    private enum ExtendedDatabaseChildResourceType
    {
      UserDefinedType = 1,
      ClientEncryptionKey = 2,
    }

    internal enum RbacResourceType : byte
    {
      RbacResourceType_RoleDefinition = 0,
      RbacResourceType_RoleAssignment = 16, // 0x10
      RbacResourceType_InteropUser = 32, // 0x20
      RbacResourceType_AuthPolicyElement = 48, // 0x30
    }
  }
}
