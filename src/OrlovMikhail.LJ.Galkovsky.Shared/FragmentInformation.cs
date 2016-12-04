namespace OrlovMikhail.LJ.Galkovsky
{
    public class FragmentInformation
    {
        /// <summary>Relative path to the fragment file from the root directory. May be null.</summary>
        public string RelativeFragmentPath { get; set; }

        /// <summary>The entry key used by the author (part of the subject before the dot).</summary>
        public string GalkovskyEntryKey { get; set; }
    }
}