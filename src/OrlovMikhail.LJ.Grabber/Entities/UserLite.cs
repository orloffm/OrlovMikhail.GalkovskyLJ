using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace OrlovMikhail.LJ.Grabber
{
    [Serializable]
    public sealed class UserLite
    {
        public UserLite()
        {
            UserType = UserLiteType.P;
        }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlIgnore]
        public bool NameSpecified { get { return !String.IsNullOrWhiteSpace(Name); } }

        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlIgnore]
        public bool UsernameSpecified { get { return !String.IsNullOrWhiteSpace(Username); } }

        //P (personal), C (community), Y (syndicated), S (shared), I (external identity)
        [XmlAttribute("type")]
        [DefaultValue(UserLiteType.P)]
        public UserLiteType UserType { get; set; }

        [XmlIgnore]
        public bool UserTypeSpecified { get { return UserType != UserLiteType.P; } }

        public override string ToString()
        {
            return Username;
        }
    }
}