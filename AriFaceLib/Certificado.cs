using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AriFaceLib
{
    public class Certificado
    {
        private string serialNumber;

        public string SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }
        private string friendlyName;

        public string FriendlyName
        {
            get { return friendlyName; }
            set { friendlyName = value; }
        }
        private string expirationDateString;

        public string ExpirationDateString
        {
            get { return expirationDateString; }
            set { expirationDateString = value; }
        }
    }
}
