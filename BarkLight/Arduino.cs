using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarkLight
{
    class Arduino
    {
        SerialPort mySerialPort = new SerialPort("COM6", 9600);

        internal void SendToPort(string portal)
        {
            if(mySerialPort != null){
                mySerialPort.Open();
                mySerialPort.Write(portal);
                mySerialPort.Close();
            }
        }
    }
}
