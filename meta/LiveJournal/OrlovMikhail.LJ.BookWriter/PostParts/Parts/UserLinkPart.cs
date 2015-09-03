using System.Diagnostics;

namespace OrlovMikhail.LJ.BookWriter
{
    [DebuggerDisplay("{Username}")]
    public class UserLinkPart : PostPartBase
    {
        public string Username { get; set; }
        public bool IsCommunity { get; set; }

        public UserLinkPart(string username, bool isCommunity = false)
        {
            Username = username;
            IsCommunity = isCommunity;
        }
    }
}