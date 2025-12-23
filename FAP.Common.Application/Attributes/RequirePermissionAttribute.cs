using System;

namespace FAP.Common.Application.Attributes
{
    /// <summary>
    /// Attribute đánh dấu quyền cần thiết cho Command/Query.
    /// - Mỗi attribute là một nhóm OR (các code cách nhau bởi dấu phẩy).
    /// - Nhiều attribute chồng lên nhau tương đương AND giữa các nhóm.
    ///   Ví dụ:
    ///   [FapPermission("User.View,Admin.View")]    // OR
    ///   [FapPermission("User.View")]               // AND
    ///   [FapPermission("Admin.View")]              // AND
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class FapPermissionAttribute : Attribute
    {
        /// <summary>Các permission code trong nhóm OR.</summary>
        public string[] Codes { get; }

        public FapPermissionAttribute(string codesCsv)
        {
            if (string.IsNullOrWhiteSpace(codesCsv))
                throw new ArgumentNullException(nameof(codesCsv));

            Codes = codesCsv
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .ToArray();

            if (Codes.Length == 0)
                throw new ArgumentException("Không có permission hợp lệ", nameof(codesCsv));
        }
    }
}

