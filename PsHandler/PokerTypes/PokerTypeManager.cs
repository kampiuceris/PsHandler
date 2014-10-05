using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace PsHandler.PokerTypes
{
    public class PokerTypeManager
    {
        public readonly List<PokerType> _pokerTypes = new List<PokerType>();
        public readonly object _lock = new object();

        public PokerType[] GetPokerTypesCopy()
        {
            return _pokerTypes.ToArray();
        }

        public void Add(PokerType pokerType)
        {
            lock (_lock)
            {
                if (!_pokerTypes.Any(o => o.Name.Equals(pokerType.Name)))
                {
                    _pokerTypes.Add(pokerType);
                }
                _pokerTypes.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public void Add(IEnumerable<PokerType> pokerTypes)
        {
            lock (_lock)
            {
                foreach (var pokerType in pokerTypes)
                {
                    if (!_pokerTypes.Any(o => o.Name.Equals(pokerType.Name)))
                    {
                        _pokerTypes.Add(pokerType);
                    }
                }
                _pokerTypes.Sort((o0, o1) => string.CompareOrdinal(o0.Name, o1.Name));
            }
        }

        public void Remove(PokerType pokerType)
        {
            lock (_lock)
            {
                _pokerTypes.Remove(pokerType);
            }
        }

        public void RemoveAll()
        {
            lock (_lock)
            {
                _pokerTypes.Clear();
            }
        }

        public void SeedDefaultValues()
        {
            if (!_pokerTypes.Any())
            {
                Add(PokerType.GetDefaultValues());
            }
        }

        public PokerType GetPokerType(string title, string className, out int errorFlags)
        {
            List<PokerType> possiblePokerTypes = new List<PokerType>();
            lock (_lock)
            {
                foreach (PokerType pokerType in _pokerTypes)
                {
                    if (pokerType.RegexWindowTitle.IsMatch(title) && pokerType.RegexWindowClass.IsMatch(className))
                    {
                        possiblePokerTypes.Add(pokerType);
                    }
                }
            }
            if (possiblePokerTypes.Count == 1)
            {
                errorFlags = 0; // OK
                return possiblePokerTypes[0];
            }
            if (possiblePokerTypes.Count == 0)
            {
                errorFlags = 1; // Not found
                return null;
            }
            if (possiblePokerTypes.Count > 1)
            {
                errorFlags = 2; // More than one PokerType found
                return null;
            }
            errorFlags = 3; // Unknown error
            return null;
        }

        public XElement ToXElement()
        {
            var xElement = new XElement("PokerTypes");
            foreach (PokerType pokerType in GetPokerTypesCopy())
            {
                xElement.Add(pokerType.ToXElement());
            }
            return xElement;
        }

        public void FromXElement(XElement xElement)
        {
            foreach (XElement xPokerType in xElement.Elements("PokerType"))
            {
                PokerType pokerType = PokerType.FromXElement(xPokerType);
                if (pokerType != null)
                {
                    Add(pokerType);
                }
            }
        }
    }
}
