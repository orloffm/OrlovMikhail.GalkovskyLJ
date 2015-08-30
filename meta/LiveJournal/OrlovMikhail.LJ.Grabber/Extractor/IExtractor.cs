using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrlovMikhail.LJ.Grabber
{
    /// <summary>Скачивает один пост as is, но разворачивая комментарии.</summary>
    public interface IExtractor
    {
        /// <summary>Клиент, используемый данным экстрактором.
        /// Устанавливается в конструкторе.</summary>
        ILJClient Client { get; }

        EntryPage GetFrom(LiveJournalTarget url, ILJClientData clientData);

        /// <summary>Заполняет объект поста.</summary>
        bool AbsorbAllData(EntryPage freshSource, ILJClientData clientData, ref EntryPage dumpData);
    }
}
