// PsHandler - poker software helping tool.
// Copyright (C) 2014-2015  kampiuceris

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsHandler
{
    public class TimeZone
    {
        public string Code;
        public string Name;
        public TimeSpan TimeDifference;

        public bool IsEqual(TimeZone o)
        {
            return Code.Equals(o.Code);
        }
    }

    public class TimeZones
    {
        public static TimeZone[] AllTimeZones = new TimeZone[]
        {
            new UTC(),
            new HST(),
            new AKT(),
            new PT(),
            new MT(),
            new CT(),
            new ET(),
            new AT(),
            new NT(),
            new ART(),
            new BRT(),
            new WET(),
            new CET(),
            new EET(),
            new MSK(),
            new IST(),
            new CCT(),
            new JST(),
            new AWST(),
            new ACST(),
            new AEST(),
            new NZT(),
        };

        public class UTC : TimeZone
        {
            public UTC()
            {
                Code = "UTC";
                Name = "Coordinated Universal Time";
                TimeDifference = new TimeSpan(0, 0, 0);
            }
        }
        public class HST : TimeZone
        {
            public HST()
            {
                Code = "HST";
                Name = "Hawaii Standard Time";
                TimeDifference = new TimeSpan(-10, 0, 0);
            }
        }
        public class AKT : TimeZone
        {
            public AKT()
            {
                Code = "AKT";
                Name = "Alaska Time";
                TimeDifference = new TimeSpan(-8, 0, 0);
            }
        }
        public class PT : TimeZone
        {
            public PT()
            {
                Code = "PT";
                Name = "Pacific Time";
                TimeDifference = new TimeSpan(-7, 0, 0);
            }
        }
        public class MT : TimeZone
        {
            public MT()
            {
                Code = "MT";
                Name = "Mountain Time";
                TimeDifference = new TimeSpan(-6, 0, 0);
            }
        }
        public class CT : TimeZone
        {
            public CT()
            {
                Code = "CT";
                Name = "Central Time";
                TimeDifference = new TimeSpan(-5, 0, 0);
            }
        }
        public class ET : TimeZone
        {
            public ET()
            {
                Code = "ET";
                Name = "Eastern Time";
                TimeDifference = new TimeSpan(-4, 0, 0);
            }
        }
        public class AT : TimeZone
        {
            public AT()
            {
                Code = "AT";
                Name = "Atlantic Time";
                TimeDifference = new TimeSpan(-3, 0, 0);
            }
        }
        public class NT : TimeZone
        {
            public NT()
            {
                Code = "NT";
                Name = "Newfoundland Time";
                TimeDifference = new TimeSpan(-2, -30, 0);
            }
        }
        public class ART : TimeZone
        {
            public ART()
            {
                Code = "ART";
                Name = "Argentina Time";
                TimeDifference = new TimeSpan(-3, 0, 0);
            }
        }
        public class BRT : TimeZone
        {
            public BRT()
            {
                Code = "BRT";
                Name = "Brasilia Time";
                TimeDifference = new TimeSpan(-3, 0, 0);
            }
        }
        public class WET : TimeZone
        {
            public WET()
            {
                Code = "WET";
                Name = "Western European Time";
                TimeDifference = new TimeSpan(1, 0, 0);
            }
        }
        public class CET : TimeZone
        {
            public CET()
            {
                Code = "CET";
                Name = "Central European Time";
                TimeDifference = new TimeSpan(2, 0, 0);
            }
        }
        public class EET : TimeZone
        {
            public EET()
            {
                Code = "EET";
                Name = "Eastern European Time";
                TimeDifference = new TimeSpan(3, 0, 0);
            }
        }
        public class MSK : TimeZone
        {
            public MSK()
            {
                Code = "MSK";
                Name = "Moscow Standard Time";
                TimeDifference = new TimeSpan(4, 0, 0);
            }
        }
        public class IST : TimeZone
        {
            public IST()
            {
                Code = "IST";
                Name = "India Standard Time";
                TimeDifference = new TimeSpan(5, 30, 0);
            }
        }
        public class CCT : TimeZone
        {
            public CCT()
            {
                Code = "CCT";
                Name = "China Coast Time";
                TimeDifference = new TimeSpan(8, 0, 0);
            }
        }
        public class JST : TimeZone
        {
            public JST()
            {
                Code = "JST";
                Name = "Japan Standard Time";
                TimeDifference = new TimeSpan(9, 0, 0);
            }
        }
        public class AWST : TimeZone
        {
            public AWST()
            {
                Code = "AWST";
                Name = "Australian Western Standard Time";
                TimeDifference = new TimeSpan(8, 0, 0);
            }
        }
        public class ACST : TimeZone
        {
            public ACST()
            {
                Code = "ACST";
                Name = "Australian Central Standard Time";
                TimeDifference = new TimeSpan(10, 30, 0);
            }
        }
        public class AEST : TimeZone
        {
            public AEST()
            {
                Code = "AEST";
                Name = "Australian Eastern Standard Time";
                TimeDifference = new TimeSpan(11, 0, 0);
            }
        }
        public class NZT : TimeZone
        {
            public NZT()
            {
                Code = "NZT";
                Name = "New Zealand Time";
                TimeDifference = new TimeSpan(13, 0, 0);
            }
        }
    }
}

