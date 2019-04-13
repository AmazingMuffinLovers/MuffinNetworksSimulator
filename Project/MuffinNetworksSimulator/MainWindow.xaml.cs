using System;
using MuffinNetworksSimulator.Wires;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MuffinNetworksSimulator
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        /// <summary>
        /// Перечисление всех режимов работы
        /// </summary>
        enum ToolMode
        {
            Cursor = 0,
            SelectZone,
            MarkZone,
            Delete
        }
        /// <summary>
        /// Перечисление всех возможных выбранных устройств
        /// </summary>
        enum DeviceSelected
        {
            Computer = 0,
            Router,
            Switch,
            Wire,
            Nothing
        }
        /// <summary>
        /// Перечисление отображающие процесс добавления витой пары
        /// </summary>
        enum AddWire
        {
            StartPoint = 0,
            LastPoint
        }

        /// <summary>
        /// Инициализация таймера реального времени
        /// </summary>
        static TimerCallback tm = new TimerCallback(RealTime);
        Timer timer = new Timer(tm, 0, 0, 1000);

        /*-----------------------------------------------------------------------------------------------------------------------------*/
        /*--------------------------------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------------------------------------------*/

        List<CanvasDevice> CanvasDeviceList = new List<CanvasDevice>();     //Лист хранящий в себе информацию обо всех объектах находящихся на канвасе
        List<CanvasWire> CanvasWireList = new List<CanvasWire>();           //Лист хранящий в себе информацию обо всех проводах находящихся на канвасе

        static ToolMode CurrentMode;                                        //Переменная отображающая, какой режим выбран на данный момент
        static DeviceSelected CurrentDeviceSelected;                        //Переменная отображающая, какое устройство сейчас выбранно на добавление
        static AddWire AddWireState;                                        //Переменная отображающая, в каком состоянии находится добавлени витой пары

        public int SelectedCanvasObjectId;                                  //Хранится id выделенного объекта канваса
        public int LastId = 0;                                              //Хранится самый последний введенный id
        public bool IsMoving = false;                                       //Хранится информация двигается объект или нет

        /// <summary>
        /// Переменные для кэширования
        /// </summary>

        public Path CashWire;                                               //Хранится графическая часть провода, которые в режиме подключения
        public object CashCanvasDevice;                                     //Записываются данные графического представления устройства при нажатии при добавлении провода
        public Point CashStartPoint;                                        //Хранит начальную координату провода
        public object CashDeciceFisrt;                                      //Заполнение портов устройствами
        public object CashPort;                                             //Хранит в себе кэш порта

        /// <summary>
        /// Старые переменные
        /// </summary>

        public int CashWireId;                                              //Хранится id провода при соединении к другому объекту
        public int CashPortIndex;                                           //Хранится index порта, для удаления при отмене соединения
        public int CashIdCanvasObject;                                      //Хранится кэш id объекта канваса
        public bool IsClicked = false;                                      //Хранится информация выделен объект или нет
        //public bool AddWire = false;                                      //Режим добавления провода      
        public bool AddWireAccess = false;                                  //Уже не помню для чего        
        public double StartLocationX, StartLocationY;                       //Хранят начальные координаты передвижения объекта канваса

        /*-----------------------------------------------------------------------------------------------------------------------------*/
        /*--------------------------------------------------------------СОБЫТИЯ--------------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Конструктор главного окна, срабатывает при запуске программы
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Инициализация логики окна
            LbTools.SelectedIndex = 0;
            CurrentMode = ToolMode.Cursor;
            CurrentDeviceSelected = DeviceSelected.Nothing;
            AddWireState = AddWire.StartPoint;
        }

        /// <summary>
        /// Замена мышки на крестик, если выбран объект LbObjects, при наведении на канвас
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing)) this.Cursor = Cursors.Cross;            
        }

        /// <summary>
        /// Изменение в CurrentDeviceSelected в зависимости от выбранного объекта в LbObjects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LbObjects.SelectedIndex)
            {
                case -1: CurrentDeviceSelected = DeviceSelected.Nothing; break;
                case 0: CurrentDeviceSelected = DeviceSelected.Computer; break;
                case 1: CurrentDeviceSelected = DeviceSelected.Router; break;
                case 2: CurrentDeviceSelected = DeviceSelected.Switch; break;
                case 3: CurrentDeviceSelected = DeviceSelected.Wire; break;
            }
        }

        /// <summary>
        /// Изменение ToolMode в зависимости от выбранного объекта в LbTools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LbTools.SelectedIndex)
            {
                case 0:
                    {
                        this.Cursor = Cursors.Arrow;
                        CurrentMode = ToolMode.Cursor;
                        LbObjects.IsEnabled = true;
                        break;
                    }
                case 3:
                    {
                        this.Cursor = Cursors.No;
                        CurrentMode = ToolMode.Delete;

                        LbObjects.IsEnabled = false;
                        LbObjects.SelectedIndex = -1;

                        CurrentDeviceSelected = DeviceSelected.Nothing;
                        break;
                    }
            }
        }

        /// <summary>
        /// Установка Cursur.Cross при отведения курсора с CvsWorkspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseLeave(object sender, MouseEventArgs e)
        {
            //Установка курсора если не выбран объект в LbObjects и выбрана ToolMode.Cursor
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing) && CurrentMode.Equals(ToolMode.Cursor)) this.Cursor = Cursors.Arrow;
            //Отменяет перетягивание объекта
            if (IsMoving) IsMoving = false;
        }

        /// <summary>
        /// Добавление объекта в CvsWorkspace, 
        /// Cнятие выделения при нажатии за пределами объекта CvsWorkspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Если выбран какой либо объект в LbObject(Кроме wire) и выбран объект Cursor в LbTools
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing) && !CurrentDeviceSelected.Equals(DeviceSelected.Wire) && !CurrentMode.Equals(ToolMode.Delete))
            {
                CvsWorkspace_AddDevice(e);              
            }
            //else CanvasObject_EmptyFieldClick(sender);
        }      

        /// <summary>
        /// Отменить добавление на канвас
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing))
            {
                LbObjects.SelectedIndex = -1;
                CurrentDeviceSelected = DeviceSelected.Nothing;
                this.Cursor = Cursors.Arrow;
            }

            if (AddWireState.Equals(AddWire.LastPoint))
            {
                CvsWorkspace.Children.Remove(CashWire);
                AddWireState = AddWire.StartPoint;
                AddWireAccess = false;
            }

            foreach (var CvsObj in CanvasDeviceList)
            {
                 CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            }

            SelectedCanvasObjectId = -1;
        }

        /// <summary>
        /// Выделение объекта на CvsWorkspace в зависимости от режима работа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasObject_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (CurrentMode)
            {
                case ToolMode.Cursor: Select_CanvasDevice(sender); break;
                case ToolMode.Delete: Delete_CanvasDevice(sender); break;
            }            
            /*
            foreach (var CvsObj in CanvasObjectList.ToArray())
            {
                if (CvsObj.CvsObject == sender && CvsObj.CvsWireObject == null)
                {
                    CvsObj.CvsObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#b886d1");
                    SelectedCanvasObjectId = CvsObj.DeviceObject.Id;
                    IsClicked = true;
                    //Для удаления
                    if (LbTools.SelectedIndex == 3)
                    {
                        foreach (var Port in CvsObj.DeviceObject.PortList.ToArray())
                        {
                            foreach (var CvsObjj in CanvasObjectList.ToArray())
                            {
                                if (Port.wire != null)
                                {
                                    if (Port.wire.Id == CvsObjj.DeviceObject.Id)
                                    {
                                        foreach (var Portt in Port.wire.EndCvsObject.DeviceObject.PortList)
                                        {
                                            if (Portt.wire == Port.wire)
                                            {
                                                Port.wire = null;
                                                Portt.wire = null;
                                            }
                                            /*{
                                                MessageBox.Show(Portt.wire.EndCvsObject.DeviceObject.Id.ToString());
                                                MessageBox.Show(Portt.wire.StartCvsObject.DeviceObject.Id.ToString());
                                                Portt.wire.EndCvsObject = null;
                                                Portt.wire.StartCvsObject = null;
                                            }*/
                                       /* }
                                        CvsWorkspace.Children.Remove(CvsObjj.CvsWireObject);
                                        //Port.wire = null;
                                        //Port.IsStartPoint = false;
                                    }
                                }
                            }
                        }

                        if (CvsObj.CvsObject == sender)
                        {
                            CanvasObjectList.Remove(CvsObj);
                            CvsWorkspace.Children.Remove(CvsObj.CvsObject);
                        }
                    }     
                }
                else if (CvsObj.CvsWireObject == null)
                {
                    CvsObj.CvsObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
                }
            }   */       
        }
    
        /// <summary>
        /// Вводит в режим перемещения объекта при зажатии мышки по нему
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasObject_MouseDown(object sender, MouseEventArgs e)
        {
            //Если не выбран тип устройства провод
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Wire))
            {
                IsMoving = true;
                StartLocationX = e.GetPosition(CvsWorkspace).X;
                StartLocationY = e.GetPosition(CvsWorkspace).Y;
            }
            //Иначе если выбрано устройство провод
            else if(CurrentDeviceSelected.Equals(DeviceSelected.Wire))
            {
                switch (AddWireState)
                {
                    case AddWire.StartPoint:
                        {
                            CvsWorkspace_AddWire_StartPoint(sender, e);
                            break;
                        }
                    case AddWire.LastPoint:
                        {
                            CvsWorkspace_AddWire_LastPoint(sender, e);
                            break;
                        }
                }              
            }
        }

        /// <summary>
        /// Выводит их режима перемещения объекта при отпускании мышки 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasObject_MouseUp(object sender, MouseEventArgs e)
        {
            IsMoving = false;
        }

        /// <summary>
        /// Перемещение объекта мышкой и провода при добавлении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseMove(object sender, MouseEventArgs e)
        {
            //Если выделен какой-либо объект и CanvasDeviceList.Count не пуст, осуществляется передвижение и текущий выбранный объект не провод
            if (SelectedCanvasObjectId != -1 && CanvasDeviceList.Count != 0 && IsMoving && !CurrentDeviceSelected.Equals(DeviceSelected.Wire))
            {
                Grid CvsObject = new Grid();
                foreach (var CvsObj in CanvasDeviceList) if(CvsObj.DeviceObject.Id == SelectedCanvasObjectId) CvsObject = CvsObj.CanvasObject;           
                //List<Port> PortList = CanvasDeviceList[SelectedCanvasObjectId].DeviceObject.PortList;

                double CashX = Canvas.GetLeft(CvsObject);
                double CashY = Canvas.GetTop(CvsObject);

                CashX += e.GetPosition(CvsWorkspace).X - StartLocationX;
                CashY += e.GetPosition(CvsWorkspace).Y - StartLocationY;

                Canvas.SetLeft(CvsObject, e.GetPosition(CvsWorkspace).X - 25);
                Canvas.SetTop(CvsObject, e.GetPosition(CvsWorkspace).Y - 25);

                //Отвечает за все подключенные провода к перемещаемуму устройства
                /*Point CashPoint = new Point();
                foreach (var port in PortList)
                {
                    foreach(var CvssOject in CanvasObjectList)
                    {
                        if(port.wire != null)
                        {
                            if (port.wire.Id == CvssOject.DeviceObject.Id)
                            {
                                LineGeometry lineGeometry = (LineGeometry)CvssOject.CvsWireObject.Data;
                                if (port.IsStartPoint)
                                {
                                    CashPoint.X = e.GetPosition(CvsWorkspace).X;
                                    CashPoint.Y = e.GetPosition(CvsWorkspace).Y;
                                    lineGeometry.StartPoint = CashPoint;
                                }
                                else
                                {
                                    CashPoint.X = e.GetPosition(CvsWorkspace).X;
                                    CashPoint.Y = e.GetPosition(CvsWorkspace).Y;
                                    lineGeometry.EndPoint = CashPoint;
                                }
                            }
                        }                        
                    }
                }*/
            }
            else if (AddWireState.Equals(AddWire.LastPoint))
            {
                double PointX = Canvas.GetLeft((UIElement)CashCanvasDevice) + 25;
                double PointY = Canvas.GetTop((UIElement)CashCanvasDevice) + 25;
                Point StartPoint = new Point(PointX, PointY);
                CashStartPoint = StartPoint;
                PointX = e.GetPosition(CvsWorkspace).X;
                PointY = e.GetPosition(CvsWorkspace).Y;
                Point EndPoint = new Point(PointX, PointY);

                LineGeometry lineGeometry = new LineGeometry(StartPoint, EndPoint);
                CashWire.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                CashWire.StrokeThickness = 3;
                CashWire.Data = lineGeometry;             
            }
        }

        /*-----------------------------------------------------------------------------------------------------------------------------*/
        /*--------------------------------------------------------------ПРОЦЕДУРЫ------------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Добавление графического представление устройства на CvsWorkspace
        /// </summary>
        /// <param name="e">Параметры нажатия мышки</param>
        private void CvsWorkspace_AddDevice(MouseButtonEventArgs e)
        {        
            //Возвращает в переменную созданный канвас
            CanvasDevice canvasDevice = CanvasDevice.CreateObject(LastId++, (DeviceType)CurrentDeviceSelected);

            //Присваение событий
            canvasDevice.CanvasObject.MouseLeftButtonDown += CanvasObject_LeftMouseDown;
            canvasDevice.CanvasObject.MouseDown += CanvasObject_MouseDown;
            canvasDevice.CanvasObject.MouseUp += CanvasObject_MouseUp;

            //Добавление на рабочую область
            CvsWorkspace.Children.Add(canvasDevice.CanvasObject);
            Canvas.SetLeft(canvasDevice.CanvasObject, e.GetPosition(CvsWorkspace).X - 25);
            Canvas.SetTop(canvasDevice.CanvasObject, e.GetPosition(CvsWorkspace).Y - 25);
            Canvas.SetZIndex(canvasDevice.CanvasObject, 1);

            //Добавление в логику программы
            CanvasDeviceList.Add(canvasDevice);
            SelectedCanvasObjectId = canvasDevice.DeviceObject.Id;

            //Возвращение настроек интерфейса
            LbObjects.SelectedIndex = -1;
            CurrentDeviceSelected = DeviceSelected.Nothing;
            this.Cursor = Cursors.Arrow;

            //Установка прозрачного фона для объекта канваса
            foreach (var CvsObjj in CanvasDeviceList)
            {
                if (CvsObjj.DeviceObject.Id != SelectedCanvasObjectId) CvsObjj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            }
        }

        /// <summary>
        /// Срабатывает при попытке выделения пустой области
        /// </summary>
        private void CanvasObject_EmptyFieldClick(object sender)
        {
            foreach (var CvsObj in CanvasDeviceList) if (!CvsObj.CanvasObject.Equals(sender)) CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            SelectedCanvasObjectId = -1;
        }

        /// <summary>
        /// Выделение объекта
        /// </summary>
        /// <param name="sender">Объект устройства, которое нужно выделить</param>
        private void Select_CanvasDevice(object sender)
        {
            foreach (var CvsObj in CanvasDeviceList)
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#b886d1");
                    SelectedCanvasObjectId = CvsObj.DeviceObject.Id;
                }
                else CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            }
        }

        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="sender">Объект устройства, которое нужно удалить</param>
        private void Delete_CanvasDevice(object sender)
        {
            Select_CanvasDevice(sender);
            foreach (var CvsObj in CanvasDeviceList.ToArray())
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    CanvasDeviceList.Remove(CvsObj);
                    CvsWorkspace.Children.Remove(CvsObj.CanvasObject);
                }
            }
        }

        /// <summary>
        /// Начало добавления провода на канвас и в логику программы
        /// </summary>
        /// <param name="sender"></param>
        private void CvsWorkspace_AddWire_StartPoint(object sender, MouseEventArgs e)
        {
            foreach(var CvsObj in CanvasDeviceList)
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    foreach(var Port in CvsObj.DeviceObject.DataPorts)
                    {
                        if(Port.Device == null)
                        {
                            AddWireAccess = true;
                            break;
                        }
                    }
                    if (AddWireAccess)
                    {
                        foreach (var Port in CvsObj.DeviceObject.DataPorts)
                        {
                            if (Port.Device == null)
                            {
                                Path Wire = new Path();
                                Wire.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                                Wire.StrokeThickness = 3;
                                double PointX = Canvas.GetLeft((UIElement)sender) + 25;
                                double PointY = Canvas.GetTop((UIElement)sender) + 25;
                                Point StartPoint = new Point(PointX, PointY);

                                PointX = e.GetPosition(CvsWorkspace).X;
                                PointY = e.GetPosition(CvsWorkspace).Y;
                                Point EndPoint = new Point(PointX, PointY);

                                LineGeometry lineGeometry = new LineGeometry(StartPoint, EndPoint);
                                Wire.Data = lineGeometry;
                                CvsWorkspace.Children.Add(Wire);

                                CashWire = Wire;
                                CashCanvasDevice = sender;
                                CashStartPoint = StartPoint;
                                CashPort = Port;

                                CashDeciceFisrt = CvsObj;

                                AddWireState = AddWire.LastPoint;
                                AddWireAccess = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Отсутствуют свободные порты!");
                        AddWireState = AddWire.StartPoint;
                    }
                    
                }
            }                  
        }

        /// <summary>
        /// Завершение добавления провода на канвас и в логику программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_AddWire_LastPoint(object sender, MouseEventArgs e)
        {
            foreach(var CvsObj in CanvasDeviceList)
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    foreach (var Port in CvsObj.DeviceObject.DataPorts)
                    {
                        if (Port.Device == null)
                        {
                            AddWireAccess = true;
                            break;
                        }
                    }
                    if (AddWireAccess)
                    {
                        foreach (var Port in CvsObj.DeviceObject.DataPorts)
                        {
                            if (Port.Device == null)
                            {
                                double PointX = Canvas.GetLeft((UIElement)sender) + 25;
                                double PointY = Canvas.GetTop((UIElement)sender) + 25;
                                Point EndPoint = new Point(PointX, PointY);
                                LineGeometry lineGeometry = new LineGeometry(CashStartPoint, EndPoint);
                                CashWire.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                                CashWire.StrokeThickness = 3;
                                CashWire.Data = lineGeometry;

                                CanvasWireList.Add(new CanvasWire(CashWire, (CanvasDevice)CashDeciceFisrt, CvsObj));
                                CanvasDevice CashDeciceFisrt1 = (CanvasDevice)CashDeciceFisrt;
                                Port.Device = CashDeciceFisrt1;
                                Port Port1 = (Port)CashPort;
                                Port1.Device = CvsObj;


                                this.Cursor = Cursors.Arrow;
                                LbObjects.SelectedIndex = -1;
                                CurrentDeviceSelected = DeviceSelected.Nothing;
                                AddWireState = AddWire.StartPoint;
                                AddWireAccess = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        CvsWorkspace.Children.Remove(CashWire);
                        AddWireState = AddWire.StartPoint;
                        AddWireAccess = false;
                        MessageBox.Show("Отсутствуют свободные порты!");
                    }
                }
            }                          
        }

        /// <summary>
        /// Срабатывает, каждый интервал срабатывания таймера
        /// </summary>
        /// <param name="obj">Просто, какой объект</param>
        private static void RealTime(object obj)
        {
            
        }
    }
}
