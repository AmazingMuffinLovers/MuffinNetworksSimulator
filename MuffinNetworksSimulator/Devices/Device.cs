using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// Класс описывающий общие свойства всех устройств
/// </summary>
namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Перечисление типов устройств
    /// </summary>
    enum DeviceType
    {
        Computer = 0,
        Router,
        Switch
    }
    
    /// <summary>
    /// Абстрактный класс для всех устройств
    /// </summary>
    abstract class Device
    {
        public DeviceType Type;                         // Индекс типа устройства
        public int Id;                                  // Id устройства
        public Port[] DataPorts;                        // Массив портов


        /// <summary>
        /// Конструктор устройства
        /// </summary>
        /// <param name="id">Уникальный идентификатор</param>
        /// <param name="type">Тип устройства</param>
        public Device(int id, DeviceType type)
        {
            this.Id = id;
            this.Type = type;
        }
    }
}
