using System;
using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("{Username}")]
    public class UserLinkPart : PostPartBase, IEquatable<UserLinkPart>
    {
        public string Username { get; set; }
        public bool IsCommunity { get; set; }

        public UserLinkPart(string username, bool isCommunity = false)
        {
            Username = username;
            IsCommunity = isCommunity;
        }

        #region equality
        public override int GetHashCode()
        {
            return Username.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return AreEqual(this, obj as UserLinkPart);
        }

        public static bool AreEqual(UserLinkPart p1, UserLinkPart p2)
        {
            if(ReferenceEquals(p1, p2))
                return true;
            else if(ReferenceEquals(p1, null) || ReferenceEquals(p2, null))
                return false;

            return String.Equals(p1.Username, p2.Username, StringComparison.OrdinalIgnoreCase) && p1.IsCommunity ==p2.IsCommunity;
        }

        public bool Equals(UserLinkPart other)
        {
            return AreEqual(this, other);
        }
        #endregion
    }
}