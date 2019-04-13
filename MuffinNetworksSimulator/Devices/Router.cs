using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// Класс описывающий маршрутизатор
/// </summary>
namespace MuffinNetworksSimulator
{
    class Router : Device
    {
        public Router(int id, DeviceType type) : base(id, type)
        {
            this.DataPorts = new Port[3];
            for (int i = 0; i < 3; i++) DataPorts[i] = new Port();
        }
    }
}
