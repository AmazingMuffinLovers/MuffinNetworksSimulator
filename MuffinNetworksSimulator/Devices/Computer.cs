using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// Класс описывающий комьютер
/// </summary>
namespace MuffinNetworksSimulator
{
    class Computer : Device
    {
        public Computer(int id, DeviceType type) : base(id, type)
        {
            this.DataPorts = new Port[1];
            DataPorts[0] = new Port();
        }
    }
}
