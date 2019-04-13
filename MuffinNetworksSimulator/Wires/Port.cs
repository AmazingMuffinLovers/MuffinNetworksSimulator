using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Физический порт устройства
    /// </summary>
    class Port
    {
        // Устройство подключенное по другую сторону витой пары
        public CanvasDevice Device;

        public Port() { }
    }
}
