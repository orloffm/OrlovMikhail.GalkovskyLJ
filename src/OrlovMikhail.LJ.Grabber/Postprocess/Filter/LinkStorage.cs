using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrlovMikhail.LJ.Grabber
{
    public class LinkStorage<T>
    {
        Dictionary<T, HashSet<T>> _dic;

        public LinkStorage()
        {
            _dic = new Dictionary<T, HashSet<T>>();
        }

        public void AddLink(T top, T child)
        {
            HashSet<T> sub;
            if(!_dic.TryGetValue(top, out sub))
            {
                sub = new HashSet<T>();
                _dic[top] = sub;
            }
            sub.Add(child);
        }

        public IEnumerable<Tuple<T, T>> EnumerateLinks()
        {
            foreach(T top in _dic.Keys)
            {
                HashSet<T> sub = _dic[top];

                foreach(T child in sub)
                    yield return Tuple.Create(top, child);
            }
        }

        public IEnumerable<Tuple<T, T>> GetLinksWithTop(T top)
        {
            HashSet<T> sub;
            if(!_dic.TryGetValue(top, out sub))
                yield break;

            foreach(T child in sub)
                yield return Tuple.Create(top, child);
        }

        public bool RemoveLink(T top, T child)
        {
            HashSet<T> sub;
            if(!_dic.TryGetValue(top, out sub))
                return false;

            return sub.Remove(child);
        }

        public Tuple<T, T> GetLinkWithBottom(T node)
        {
            return EnumerateLinks().FirstOrDefault(z => node.Equals(z.Item2));
        }
    }
}
