using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    public class RelatedDataSaver : IRelatedDataSaver
    {
        private readonly IEntryBaseHelper _eph;
        private readonly ILJClient _ljc;
        private readonly IUserpicStorageFactory _ups;
        private readonly IFileStorageFactory _fsf;
        private readonly ISuitableCommentsPicker _scp;

        public RelatedDataSaver(IEntryBaseHelper eph, ILJClient ljc,
            IUserpicStorageFactory ups, IFileStorageFactory fsf,
            ISuitableCommentsPicker scp)
        {
            _eph = eph;
            _ljc = ljc;
            _ups = ups;
            _fsf = fsf;
            _scp = scp;
        }

        /// <param name="bookRoot">Root of the project.</param>
        /// <param name="dumpLocation">It's location. Where the file will be stored.</param>
        public void EnsureAllIsSaved(IEnumerable<EntryBase> allData, string bookRoot, string dumpLocation)
        {
            // Files. We store them in a local path.
            Uri[] urls = _eph.GetFiles(allData);
            using(IFileStorage fstorage = _fsf.CreateOn(dumpLocation))
            {
                foreach(Uri u in urls)
                    DownloadAndStoreIfNotAvailable(fstorage, u);
            }

            // Userpics. We store them in the book's root subfolder.
            Tuple<string, Userpic>[] userpics = _eph.GetUserpics(allData);
            using(IUserpicStorage ustorage = _ups.CreateOn(bookRoot))
            {
                foreach(var tuple in userpics)
                {
                    Userpic userpic = tuple.Item2;
                    Uri u = userpic.GetUri();

                    DownloadAndStoreIfNotAvailable(ustorage, u);
                }
            }
        }

        private void DownloadAndStoreIfNotAvailable(IStorage storage, Uri u)
        {
            FileInfoBase existing = storage.TryGet(u);
            if(existing == null)
            {
                // OK, download and save!
                byte[] data = _ljc.DownloadFile(u);
                if(data != null)
                    storage.EnsureStored(u, data);
            }
        }
    }
}
