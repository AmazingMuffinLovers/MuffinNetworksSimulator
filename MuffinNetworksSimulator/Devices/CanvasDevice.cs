using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Связующий между классом устройства и объекта на канвасе
    /// </summary>
    class CanvasDevice
    {
        public Device DeviceObject;         //Логическое представление устройства
        public Grid CanvasObject;           //Графическое представление устройства

        private Device DeviceObj;           //Промежуточная переменная

        /// <summary>
        /// Конструктор для создания графического объекта CvsWorkspace
        /// </summary>
        /// <param name="Id">Уникальный идентификатор устройства</param>
        /// <param name="CanvasObj">Графическое представление утройства</param>
        /// <param name="type">Тип устройства</param>
        public CanvasDevice(int Id, DeviceType type, Grid CanvasObj)
        {
            switch (type)
            {
                case DeviceType.Computer:
                    {
                        DeviceObj = new Computer(Id, type);
                        break;
                    }
                case DeviceType.Router:
                    {
                        DeviceObj = new Router(Id, type);
                        break;
                    }
                case DeviceType.Switch:
                    {
                        DeviceObj = new Switch(Id, type);
                        break;
                    }
            }

            this.DeviceObject = DeviceObj;
            this.CanvasObject = CanvasObj;
        }

        /// <summary>
        /// Создание устройства канваса
        /// </summary>
        /// <param name="Id">Уникальный идентификатор</param>
        /// <param name="type">Тип устройства</param>
        /// <returns>Устройство</returns>
        public static CanvasDevice CreateObject(int Id, DeviceType type)
        {
            
            Uri uriImageSource = null;
            Grid CanvasObject = new Grid();
            CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#b886d1");
            CanvasObject.Width = 50;
            CanvasObject.Height = 50;
            Image image = new Image();

            switch (type)
            {
                case DeviceType.Computer:
                    {
                        uriImageSource = new Uri(@"/MuffinNetworksSimulator;component/icon/computer.png", UriKind.RelativeOrAbsolute);
                        break;
                    }
                case DeviceType.Router:
                    {
                        uriImageSource = new Uri(@"/MuffinNetworksSimulator;component/icon/router.png", UriKind.RelativeOrAbsolute);
                        break;
                    }
                case DeviceType.Switch:
                    {
                        uriImageSource = new Uri(@"/MuffinNetworksSimulator;component/icon/switch.png", UriKind.RelativeOrAbsolute);
                        break;
                    }
            }

            image.Source = new BitmapImage(uriImageSource);
            CanvasObject.Children.Add(image);

            CanvasDevice canvasDevice = new CanvasDevice(Id, type, CanvasObject);

            return canvasDevice;
            
        }
    }
}
