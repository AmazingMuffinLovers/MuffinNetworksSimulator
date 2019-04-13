using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// Класс описывающий коммутатор
/// </summary>
namespace MuffinNetworksSimulator
{
    class Switch : Device
    {
        public Switch(int id, DeviceType type) : base(id, type)
        {
            this.DataPorts = new Port[8];
            for (int i = 0; i < 8; i++) DataPorts[i] = new Port();
        }
    }   
}
