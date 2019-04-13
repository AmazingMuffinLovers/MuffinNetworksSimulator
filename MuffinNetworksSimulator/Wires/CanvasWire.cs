using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace MuffinNetworksSimulator.Wires
{
    /// <summary>
    /// Связующий между классом провода и его графического представления
    /// </summary>
    class CanvasWire
    {
        public Path CanvasObject;
        public CanvasDevice Device1;
        public CanvasDevice Device2;

        /// <summary>
        /// Конструктор для создания графического провода CvsWorkspace
        /// </summary>
        /// <param name="path">Графическое представление провода</param>
        /// <param name="device1">Первое устройство</param>
        /// <param name="device2">Второе устройство</param>
        public CanvasWire(Path path, CanvasDevice device1, CanvasDevice device2)
        {
            this.CanvasObject = path;
            this.Device1 = device1;
            this.Device2 = device2;
        }
    }
}
